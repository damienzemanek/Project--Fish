using NUnit.Framework;
using UnityEngine;
using System.Collections;
using EMILtools.Timers;
using UnityEngine.TestTools;
using EMILtools.Systems;

public class PipelineTests
{
    // Define a simple context for testing
    public class TestContext : IPipelineContext
    {
        public int Value;
        public TestContext(int _value = 0) => Value = _value;
    }

    
    [Test]
    public void PipelineBuilder_CreatesCorrectSize()
    {
        // Arrange
        var builder = new PipelineBuilder<TestContext>();
        
        // Act
        builder.Add_ShortCircuit(ctx => true);
        var pipeline = builder.InjectMainMethod(ctx => true);

        // Assert
        Assert.AreEqual(2, pipeline.Size);
    }


    [Test]
    public void StepsAllPass_FinalExecutes()
    {
        // Arrange
        var myctx = new TestContext(2);
        bool jumpSuccessfull = false;
        var jump = new PipelineBuilder<TestContext>()
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

        bool MainMethod(TestContext ctx)
        {
            Debug.Log("Main Method Being Called");
            jumpSuccessfull = true;
            return false;
        }
    }
    
    
    [Test]
    public void StepFail_FinalDoesNotExecute()
    {
        // Arrange
        var myctx = new TestContext(2);
        bool jumpSuccessfull = false;
        var jump = new PipelineBuilder<TestContext>()
            .Add_ShortCircuit(ctx => ctx.Value == 1)
            .Add_ShortCircuit(ctx => ctx.Value == 2)
            .InjectMainMethod(ctx => Jump(ctx));
        bool Jump(TestContext ctx) { jumpSuccessfull = true; return true; }

        //Act
        myctx.TryTo(jump);
        
        // Assert
        Assert.AreEqual(jumpSuccessfull, false);
        
    }
    
    [Test]
    public void StepFail_FailedStepCallback()
    {
        // Arrange
        var myctx = new TestContext(2);
        bool failedStepCallbackExecuted = false;
        var jump = new PipelineBuilder<TestContext>()
            .Add_ShortCircuit(ctx => ctx.Value == 1)
            .Add_ShortCircuit(ctx => ctx.Value == 2, new Callback(() => failedStepCallbackExecuted = true))
            .InjectMainMethod(ctx => Jump(ctx));
        bool Jump(TestContext ctx) => true;

        
        // Act
        myctx.TryTo(jump);
        
        
        // Assert
        Assert.AreEqual(failedStepCallbackExecuted, true);
    }
    
        
    [UnityTest]
    public IEnumerator TimedStepBlocking()
    {
        // Arrange
        var myctx = new TestContext(2);
        bool jumpCalled = false;
        var jump = new PipelineBuilder<TestContext>()
            .Add_ShortCircuit(ctx => ctx.Value == 0)
            .Add_ShortCircuit(ctx => ctx.Value == 1, new Timed(1))
            .InjectMainMethod(Jump);
        bool Jump(TestContext ctx) => jumpCalled = true;

        
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
    public IEnumerator WaitingStepBlockingAndEventuallyCompletes()
    {
        // Arrange
        var myctx = new TestContext(2);
        bool jumpCalled = false;
        var jump = new PipelineBuilder<TestContext>()
            .Add_ShortCircuit(ctx => ctx.Value == 0)
            .Add_ShortCircuit(ctx => ctx.Value == 1, new Wait(1))
            .InjectMainMethod(Jump);
        bool Jump(TestContext ctx) => jumpCalled = true;

        
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