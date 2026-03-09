using NUnit.Framework;

// --- Test Types ---

public interface ITestContextDataImmutable : IContextViewImmutable
{
    int SomeInt { get; }
}

public class TestContextData : ContextData<ITestContextDataImmutable>, ITestContextDataImmutable, IModuleUsabableContext
{
    public int SomeInt { get; set; }
}

public class TestBlackboard : Blackboard { }

public class TestMonoStructure : MonoStructure<TestBlackboard, TestContextData, ITestContextDataImmutable> { }


public class MonoStructureTests
{
    /// Initialization 
    [Test]
    public void Test1_MonoStructure_Init_CreatesContext()
    {
        var structure = new TestMonoStructure();
        structure.Init();

        Assert.IsNotNull(structure.Context);
        Assert.IsNotNull(structure.Context.Data);
        Assert.IsNotNull(structure.Context.View);
    }

    ///  Write-then-read — changes via .Data are reflected in .View
    [Test]
    public void Test2_MonoStructure_WriteData_ReflectedInView()
    {
        var structure = new TestMonoStructure();
        structure.Init();

        structure.Context.Data.SomeInt = 42;

        Assert.AreEqual(42, structure.Context.View.SomeInt);
    }

    /// View from external location — a "receiver" can read the immutable view
    [Test]
    public void Test3_MonoStructure_ViewAccessibleExternally()
    {
        var structure = new TestMonoStructure();
        structure.Init();

        structure.Context.Data.SomeInt = 99;

        // Simulate a module/receiver getting the view through the IMonoStructure API
        IContextViewImmutable apiContext = structure.API_Context;
        var typedView = (ITestContextDataImmutable)apiContext;
        

        Assert.AreEqual(99, typedView.SomeInt);
    }
}