using System.Threading.Tasks;
using EMILtools.Systems;
using NUnit.Framework;
using UnityEngine;

public class SubsctiberTests : MonoBehaviour
{

    public class TestFailResolvable : IResolvable
    {
        public bool Resolve<TContext>(TContext ctx) => false;
        public bool Resolve(object ctx) => false;
    }
    public class TestResolvable : IResolvable
    {
        public bool wasCalled;
        public bool Resolve(object ctx) => wasCalled = true;
    }
    
    [Test]
    public async Task Test1_ActionSubscriberExecutes()
    {
        bool executed = false;

        var sub = new SubResolvable(
            () => executed = true,
            new ResolveContainer(null, null, null)
        );

        await sub.Execute();

        Assert.IsTrue(executed);
    }

    [Test]
    public async Task Test2_BeforeResolversExecute()
    {
        var before = new TestResolvable();

        var container = new ResolveContainer(
            beforeExecution: new IResolvable[] { before }
        );

        var sub = new SubResolvable(
            () => false,
            container
        );

        await sub.Execute();

        Assert.IsTrue(before.wasCalled);
    }
    
    [Test]
    public async Task Test3_AfterResolversExecute()
    {
        var after = new TestResolvable();

        var container = new ResolveContainer(
            afterExecution: new IResolvable[] { after }
        );

        var sub = new SubResolvable(
            () => false,
            container
        );

        await sub.Execute();

        Assert.IsTrue(after.wasCalled);
    }
    
    [Test]
    public async Task Test4_ShortCircuitStopsExecution()
    {
        bool executed = false;

        var beforeFail = new TestFailResolvable();

        var container = new ResolveContainer(
            beforeExecution: new IResolvable[] { beforeFail }
        );

        var sub = new SubResolvable(
            () => executed = true,
            container,
            canShortCircuit: true
        );

        await sub.Execute();

        Assert.IsFalse(executed);
    }
    
    [Test]
    public async Task Test5_PredicateShortCircuitTriggersFailedExecution()
    {
        bool failedResolved = false;

        var failed = new TestResolvable();
        bool shouldStop = true;

        var container = new ResolveContainer(
            failedExecution: new IResolvable[] { failed }
        );

        var sub = new SubResolvable(
            () => shouldStop,
            container,
            canShortCircuit: true
        );

        await sub.Execute();

        failedResolved = failed.wasCalled;

        Assert.IsTrue(failedResolved);
    }
    
    [Test]
    public async Task Test6_BeforeFailureStopsAllExecution()
    {
        var failPointBefore = new TestFailResolvable();
        var after = new TestResolvable();

        var container = new ResolveContainer(
            beforeExecution: new IResolvable[] { failPointBefore },
            afterExecution: new IResolvable[] { after }
        );

        var sub = new SubResolvable(
            () => false,
            container,
            canShortCircuit: true
        );

        await sub.Execute();

        Assert.IsTrue(!after.wasCalled);
    }
        

}
