using System;
using System.Threading.Tasks;
using EMILtools.Systems;
using NUnit.Framework;
using UnityEngine;
using static EMILtools.Systems.SubscriberExecutor;

public class SubsctiberTests : MonoBehaviour
{

    public class TestFailResolvable : IResolvableWithContext
    {
        public bool Resolve() => false;
        public bool Resolve<TContext>(in TContext ctx) where TContext : class => Resolve();
    }
    public class TestResolvable : IResolvableWithContext
    {
        public bool wasCalled;
        public bool Resolve() => wasCalled = true;
        public bool Resolve<TContext>(in TContext ctx) where TContext : class => Resolve();
        
    }
    
    [Test]
    public async Task Test1_ActionSubscriberExecutes()
    {
        bool executed = false;

        var sub = new Subscriber<Action, ActionResolver, VoidCtx>(
            () => executed = true,
            new ResolveContainer<IResolvableWithContext>(null, null, null)
        );

        await sub.Execute();

        Assert.IsTrue(executed);
    }

    [Test]
    public async Task Test2_BeforeResolversExecute()
    {
        var before = new TestResolvable();

        var container = new ResolveContainer<IResolvableWithContext>(
            beforeExecution: new IResolvableWithContext[] { before }
        );

        var sub = new Subscriber<Action, ActionResolver, VoidCtx>(
            () => { },
            container
        );

        await sub.Execute();

        Assert.IsTrue(before.wasCalled);
    }
    
    [Test]
    public async Task Test3_AfterResolversExecute()
    {
        var after = new TestResolvable();

        var container = new ResolveContainer<IResolvableWithContext>(
            afterExecution: new IResolvableWithContext[] { after }
        );

        var sub = new Subscriber<Action, ActionResolver, VoidCtx>(
            () => { },
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

        var container = new ResolveContainer<IResolvableWithContext>(
            beforeExecution: new IResolvableWithContext[] { beforeFail }
        );

        var sub = new Subscriber<Action, ActionResolver, VoidCtx>(
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

        var container = new ResolveContainer<IResolvableWithContext>(
            failedExecution: new IResolvableWithContext[] { failed }
        );

        var sub = new Subscriber<Func<bool>, PredicateResolver, VoidCtx>(
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

        var container = new ResolveContainer<IResolvableWithContext>(
            beforeExecution: new IResolvableWithContext[] { failPointBefore },
            afterExecution: new IResolvableWithContext[] { after }
        );

        var sub = new Subscriber<Action, ActionResolver, VoidCtx>(
            () => { },
            container,
            canShortCircuit: true
        );

        await sub.Execute();

        Assert.IsTrue(!after.wasCalled);
    }
        

}
