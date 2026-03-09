using NUnit.Framework;
using EMILtools.Systems;

// --- Test Types ---

public interface ITestContextDataImmutable : IContextViewImmutable
{
    int SomeInt { get; }
}

public class TestContextData : ContextData<TestBlackboard>, ITestContextDataImmutable, IModuleUsabableContext
{
    public int SomeInt { get; set; }
}

public class TestBlackboard : Blackboard { }

public class TestMonoStructure : MonoStructure<TestBlackboard, TestContextData, ITestContextDataImmutable> { }


public class MonoStructureTests
{
    private TestMonoStructure structure;

    [SetUp]
    public void Setup()
    {
        structure = new TestMonoStructure();
        structure.Blackboard = new TestBlackboard();
        structure.Init();
    }

    /// Initialization 
    [Test]
    public void Test1_MonoStructure_Init_CreatesContext()
    {
        Assert.IsNotNull(structure.Context);
        Assert.IsNotNull(structure.Context.Data);
        Assert.IsNotNull(structure.Context.View);
    }
    
    [Test]
    public void Test2_View_IsImmutableInterface()
    {
        var view = structure.Context.View;

        Assert.IsInstanceOf<ITestContextDataImmutable>(view);
    }

    /// Write-then-read — changes via .Data are reflected in .View
    [Test]
    public void Test3_MonoStructure_WriteData_ReflectedInView()
    {
        structure.Context.Data.SomeInt = 1;

        Assert.AreEqual(1, structure.Context.View.SomeInt);
    }

    /// View from external location — a "receiver" can read the immutable view
    [Test]
    public void Test4_MonoStructure_ViewAccessibleExternally()
    {
        structure.Context.Data.SomeInt = 1;

        IContextViewImmutable apiContext = structure.API_Context;
        var typedView = (ITestContextDataImmutable)apiContext;

        Assert.AreEqual(1, typedView.SomeInt);
    }
}