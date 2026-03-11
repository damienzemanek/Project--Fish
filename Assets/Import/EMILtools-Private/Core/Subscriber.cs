using System;
using System.Threading.Tasks;
using EMILtools.Systems;
using JetBrains.Annotations;
using UnityEngine;
using static EMILtools.Systems.ResolverSystem;
using static EMILtools.Systems.SubscriberExecutor;


namespace EMILtools.Systems
{
    public class VoidCtx : IContext { }


    public interface ISubscriber
    {
        bool isActive { get; set; }
        Task Execute();
    }

    public abstract class Subscriber<TDelegate> : ISubscriber
        where TDelegate : Delegate
    {
        public static VoidCtx VOID = null;
        
        readonly TDelegate _callback;
        public TDelegate callback => _callback;

        public readonly ResolveContainer<IResolvableWithContext> Resolves;
        
        public Subscriber(TDelegate callback, ResolveContainer<IResolvableWithContext> resolves)
        {
            _callback = callback;
            Resolves = resolves;
        }

        public bool isActive { get; set; }
        public abstract Task Execute();
    }

    public class ActionSub : Subscriber<Action>
    {
        public ActionSub(Action callback, ResolveContainer<IResolvableWithContext> resolves) : base(callback, resolves) { }
        public override Task Execute() => SubscriberResolver_Action.ResolveContainer(ResolverAction, Resolves, callback, VOID, false);
    }

    public class BoolPredicateSub : Subscriber<Func<bool>>
    {
        public BoolPredicateSub(Func<bool> callback, ResolveContainer<IResolvableWithContext> resolves) : base(callback, resolves) { }

        public override Task Execute() => SubscriberResolver_BoolPredicate.ResolveContainer(ResolverBoolPredicate, Resolves, callback, VOID, false);
    }    

    public static class SubscriberExecutor
    {
        public class SubscriberResolver_BoolPredicate : Resolver<Func<bool>, VoidCtx> { protected override bool Execute(Func<bool> command, [CanBeNull] VoidCtx ctx) => command(); }
        public static readonly SubscriberResolver_BoolPredicate ResolverBoolPredicate = new SubscriberResolver_BoolPredicate();
        
        public class SubscriberResolver_Action : Resolver<Action, VoidCtx> { protected override bool Execute(Action command, [CanBeNull] VoidCtx ctx) { command(); return true; } }
        public static readonly SubscriberResolver_Action ResolverAction = new SubscriberResolver_Action();
    }
}
