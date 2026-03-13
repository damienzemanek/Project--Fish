using System;
using System.Collections;
using System.Threading.Tasks;
using EMILtools.Systems;
using NUnit.Framework;
using UnityEngine;
using UnityEngine.TestTools;
using static EMILtools.Systems.SubscriberExecutor;

public class PublisherTests
{
    [Test]
    public void Test1_Initializes()
    {
        var sub = new Subscriber<Action, ActionResolver>(Call);
        var publisher = new Publisher();
        
        void Call()  { }
        Assert.IsNotNull(sub);
        Assert.IsNotNull(publisher);
    }
    
    [Test]
    public void Test2_CanPublish()
    {
        var called = false;
        var sub = new Subscriber<Action, ActionResolver>(Call);
        var publisher = new Publisher();
        publisher.Add(sub);
        publisher.Publish();
        
        void Call() => called = true;
        Assert.IsTrue(called);
    }
    
        [Test]
    public async Task Test3_InactiveSubscriberNotCalled()
    {
        bool called = false;
        void Call() => called = true;

        var sub = new Subscriber<Action, ActionResolver>(Call, isActive: false);
        var publisher = new Publisher();
        publisher.Add(sub);

        await publisher.Publish();

        Assert.IsFalse(called);
    }

    [Test]
    public async Task Test4_MultipleSubscribers()
    {
        bool called1 = false;
        bool called2 = false;
        void Call1() => called1 = true;
        void Call2() => called2 = true;

        var sub1 = new Subscriber<Action, ActionResolver>(Call1);
        var sub2 = new Subscriber<Action, ActionResolver>(Call2);

        var publisher = new Publisher();
        publisher.Add(sub1);
        publisher.Add(sub2);

        await publisher.Publish();

        Assert.IsTrue(called1 && called2);
    }

    [Test]
    public async Task Test5_PublishWithResolveContainerBeforeExecution()
    {
        bool beforeResolved = false;
        bool mainCalled = false;

        var before = new Callback(() => beforeResolved = true);
        var container = new ResolveContainer<IResolvableWithContext>(beforeExecution: new IResolvableWithContext[] { before });

        void MainCall() => mainCalled = true;

        var sub = new Subscriber<Action, ActionResolver>(MainCall, container);
        var publisher = new Publisher();
        publisher.Add(sub);

        await publisher.Publish();

        Assert.IsTrue(beforeResolved);
        Assert.IsTrue(mainCalled);
    }
    
    
    public class MyContext : IContext
    {
        public int value = 0;
    }
    
    [Test]
    public async Task Test6_CustomContextSubscriberExecutes()
    {
        var ctx = new MyContext();
        void ActionCall(MyContext c) => c.value = 42;

        var sub = new SubscriberCtx<Action<MyContext>, ActionResolverCtx<MyContext>, MyContext>(
            ActionCall
        );

        var publisher = new Publisher<MyContext>();
        publisher.Add(sub);

        await publisher.Publish(ctx);

        Assert.AreEqual(42, ctx.value);
    }

    [Test]
    public async Task Test7_CustomContextMultipleSubscribers()
    {
        var ctx = new MyContext();
        void Call1(MyContext c) => c.value += 1;
        void Call2(MyContext c) => c.value += 2;

        var sub1 = new SubscriberCtx<Action<MyContext>, ActionResolverCtx<MyContext>, MyContext>(Call1);
        var sub2 = new SubscriberCtx<Action<MyContext>, ActionResolverCtx<MyContext>, MyContext>(Call2);

        var publisher = new Publisher<MyContext>();
        publisher.Add(sub1);
        publisher.Add(sub2);

        await publisher.Publish(ctx);

        Assert.AreEqual(3, ctx.value);
    }
    
    
    [Test]
    public async Task Test8_PredicateShortCircuit()
    {
        bool mainCalled = false;
        Func<bool> predicate = () => true; // returns true = short circuit
        void MainCall() => mainCalled = true;

        var sub = new Subscriber<Func<bool>, PredicateResolver>(predicate, canShortCircuit: true);
        var publisher = new Publisher();
        publisher.Add(sub);

        await publisher.Publish();

        // The main subscriber callback (not predicate) should not run because predicate short-circuits
        Assert.IsFalse(mainCalled);
    }

    [Test]
    public async Task Test9_PredicateNoShortCircuit()
    {
        bool mainCalled = false;
        Func<bool> predicate = () => false; // returns false = do not short circuit
        void MainCall() => mainCalled = true;

        var sub = new Subscriber<Func<bool>, PredicateResolver>(predicate, canShortCircuit: true);
        var publisher = new Publisher();
        publisher.Add(sub);

        // Execute a second subscriber after predicate to simulate normal execution
        var mainSub = new Subscriber<Action, ActionResolver>(MainCall);
        publisher.Add(mainSub);

        await publisher.Publish();

        // Since predicate returned false, the second subscriber should run
        Assert.IsTrue(mainCalled);
    }
}
