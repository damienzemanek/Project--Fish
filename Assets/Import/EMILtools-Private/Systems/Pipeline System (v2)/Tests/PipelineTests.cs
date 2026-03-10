using NUnit.Framework;
using UnityEngine;
using System.Collections;
using EMILtools.Timers;
using UnityEngine.TestTools;
using EMILtools.Systems;

public class PipelineTests
{
    // Define a simple context for testing
    public class TestContextProvider : IPipelineContext
    {
        public int Value;
        public TestContextProvider(int _value = 0) => Value = _value;
    }

    
    [Test]
    public void Test1_PipelineBuilder_CreatesCorrectSize()
    {
        // Arrange
        var builder = new PipelineBuilder<TestContextProvider>();
        
        // Act
        builder.Add_ShortCircuit(ctx => true);
        var pipeline = builder.InjectMainMethod(ctx => true);

        // Assert
        Assert.AreEqual(2, pipeline.Size);
    }

    
    [Test]
    public void Test2_StepsAllPass_FinalExecutes()
    {
        // Arrange
        var myctx = new TestContextProvider(2);
        bool jumpSuccessfull = false;
        var jump = new PipelineBuilder<TestContextProvider>()
            .Add_ShortCircuit(ctx => ctx.Value == 1)
            .Add_ShortCircuit(ctx => ctx.Value == 1)
            .InjectMainMethod(MainMethod);
        Debug.Log("------- Setup Complete -------");

        
        // Act
        myctx.TryTo(jump);
        Debug.Log("------- Act Complete -------");

        
        //Assert
        Assert.AreEqual(true, jumpSuccessfull);
        Debug.Log("------- Assert Complete -------");

        bool MainMethod(TestContextProvider ctx)
        {
            Debug.Log("Main Method Being Called");
            jumpSuccessfull = true;
            return false;
        }
    }
    
    [Test]
    public void Test3_StepFail_FinalDoesNotExecute()
    {
        // Arrange
        var myctx = new TestContextProvider(2);
        bool jumpSuccessfull = false;
        var jump = new PipelineBuilder<TestContextProvider>()
            .Add_ShortCircuit(ctx => ctx.Value == 1)
            .Add_ShortCircuit(ctx => ctx.Value == 2)
            .InjectMainMethod(ctx => Jump(ctx));
        bool Jump(TestContextProvider ctx) { jumpSuccessfull = true; return true; }

        //Act
        myctx.TryTo(jump);
        
        // Assert
        Assert.AreEqual(jumpSuccessfull, false);
        
    }
    
    [Test]
    public void Test4_StepFail_FailedStepCallback()
    {
        // Arrange
        var myctx = new TestContextProvider(2);
        bool failedStepCallbackExecuted = false;
        var jump = new PipelineBuilder<TestContextProvider>()
            .Add_ShortCircuit(ctx => ctx.Value == 1)
            .Add_ShortCircuit(ctx => ctx.Value == 2,
                before: new IResolveContext[] { new Callback(() => failedStepCallbackExecuted = true) })
            .InjectMainMethod(ctx => Jump(ctx));
        bool Jump(TestContextProvider ctx) => true;

        
        // Act
        myctx.TryTo(jump);
        
        
        // Assert
        Assert.AreEqual(failedStepCallbackExecuted, true);
    }

    
    [Test]
    public void Test5_BeforeAndAfter_AllPass_ResolveContextCalled()
    {
        var myctx = new TestContextProvider(3);
        bool jumpSuccessfull = false;
        bool beforecalled = false;
        bool aftercalled = false;
        var jump = new PipelineBuilder<TestContextProvider>()
            .Add_ShortCircuit(ctx => ctx.Value == 1)
            .Add_ShortCircuit(ctx => ctx.Value == 2, 
                before: new IResolveContext[]{ new Callback(CallBefore) }, 
                after: new IResolveContext[]{ new Callback(CallAfter) })
            .InjectMainMethod(Jump);
        
        myctx.TryTo(jump);
        bool Jump(TestContextProvider ctx) { jumpSuccessfull = true; return true; }
        void CallBefore() => beforecalled = true;
        void CallAfter() => aftercalled = true;
        
        Assert.IsTrue(beforecalled, "Before callback should have been called.");
        Assert.IsTrue(aftercalled, "After callback should have been called.");
        Assert.IsTrue(jumpSuccessfull, "Jump should have been called.");

    }
    
    [Test]
    public void Test6_BeforeAndAfter_BeforePassesOnly_DueToShortCircuit_ResolveContextCalled()
    {
        var myctx = new TestContextProvider(2);
        bool jumpSuccessfull = false;
        bool beforecalled = false;
        bool aftercalled = false;
        var jump = new PipelineBuilder<TestContextProvider>()
            .Add_ShortCircuit(ctx => ctx.Value == 1)
            .Add_ShortCircuit(ctx => ctx.Value == 2, 
                before: new IResolveContext[]{ new Callback(CallBefore) }, 
                after: new IResolveContext[]{ new Callback(CallAfter) })
            .InjectMainMethod(Jump);
        
        myctx.TryTo(jump);
        bool Jump(TestContextProvider ctx) { jumpSuccessfull = true; return true; }
        void CallBefore() => beforecalled = true;
        void CallAfter() => aftercalled = true;
        
        Assert.IsTrue(beforecalled, "Before callback should have been called.");
        Assert.IsFalse(aftercalled, "After callback should have been called.");
        Assert.IsFalse(jumpSuccessfull, "Jump should have been called.");

    }

    
    
    
        
    [UnityTest]
    public IEnumerator Test7_TimedStepBlocking()
    {
        // Arrange
        var myctx = new TestContextProvider(2);
        bool jumpCalled = false;
        var jump = new PipelineBuilder<TestContextProvider>()
            .Add_ShortCircuit(ctx => ctx.Value == 0)
            .Add_ShortCircuit(ctx => ctx.Value == 1, 
                before: new IResolveContext[] { new Timed(1) })
            .InjectMainMethod(Jump);
        bool Jump(TestContextProvider ctx) => jumpCalled = true;

        
        // Act
        myctx.TryTo(jump);
        
        // Assert
        Assert.AreEqual(jumpCalled, false, "Jump should not be called immediately.");
        // Manually tick 900ms (0.9s)
        TimerUtility.TickAllFixed(0.9f);
        myctx.TryTo(jump);
        Assert.AreEqual(jumpCalled, false, "Jump should not be called after 900ms.");
        // Manually tick another 200ms (Total 1.1s)
        TimerUtility.TickAllFixed(0.2f);
        myctx.TryTo(jump);
        Assert.AreEqual(jumpCalled, true, "Jump should be called after total 1100ms.");
    
        yield return null;

    }
    
    [UnityTest]
    public IEnumerator Test8_WaitingStepBlockingAndEventuallyCompletes()
    {
        // Arrange
        var myctx = new TestContextProvider(2);
        bool jumpCalled = false;
        var jump = new PipelineBuilder<TestContextProvider>()
            .Add_ShortCircuit(ctx => ctx.Value == 0)
            .Add_ShortCircuit(ctx => ctx.Value == 1, 
                before: new IResolveContext[] { new Wait(1) })
            .InjectMainMethod(Jump);
        bool Jump(TestContextProvider ctx) => jumpCalled = true;

        
        // Act
        var task = myctx.TryTo(jump);
        
        // Assert
        Assert.AreEqual(jumpCalled, false, "Jump should not be called immediately.");
        
        TimerUtility.TickAllFixed(1.2f);
        yield return new WaitUntil(() => task.IsCompleted);
        
        Assert.AreEqual(jumpCalled, true, "Jump should be called after total 1100ms.");
    
        yield return null;
    }

    
}