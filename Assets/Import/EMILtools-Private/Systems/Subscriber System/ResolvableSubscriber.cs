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
    public interface ISubscriber<TContext>
        where TContext : class, IContext
    {
        bool isActive { get; set; }
        Task Execute(TContext ctx);
    }

    /// <summary>
    /// Use VoidCtx for no context
    /// Note: Predicates return TRUE are fore SHORTCIRCUIT
    ///       Predicates return FALSE are NOT SHORTCIRCUIT
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    /// <typeparam name="TResolver"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public sealed class Subscriber<TDelegate, TResolver, TContext> : ISubscriber<TContext>
        where TDelegate : Delegate
        where TResolver : Resolver<TDelegate, TContext>, new()
        where TContext : class, IContext
    {
        public static readonly VoidCtx VoidCtx = new VoidCtx();
        readonly TDelegate Callback;
        readonly ResolveContainer<IResolvableWithContext> ResolveContainer;
        static readonly TResolver Resolver = new();
        
        public bool canShortCircuit { get; set; }
        public bool isActive { get; set; }

        public Subscriber(
            TDelegate callback,
            ResolveContainer<IResolvableWithContext> resolveContainer,
            bool isActive = true,
            bool canShortCircuit = false)
        {
            Callback = callback;
            ResolveContainer = resolveContainer;
            this.canShortCircuit = canShortCircuit;
            this.isActive = isActive;
        }

        public Task Execute(TContext ctx)
        {
            if (!isActive) return Task.CompletedTask;
            return Resolver.ResolveContainer(
                ResolveContainer,
                Callback,
                canShortCircuit,
                ctx
            );
        }
        
        public Task Execute()
        {
            if (typeof(TContext) != typeof(VoidCtx)) throw new InvalidOperationException();
            if (!isActive) return Task.CompletedTask;
            return Resolver.ResolveContainer(
                ResolveContainer,
                Callback,
                canShortCircuit
            );
        }
    }
    

    public static class SubscriberExecutor
    {
        public class ActionResolver : Resolver<Action, VoidCtx>
        {
            protected override bool Execute(Action command, [CanBeNull] VoidCtx ctx)
            {
                command();
                return true;
            }
        }
        public class ActionContextResolver<T> : Resolver<Action<T>, T>
            where T : class, IContext
        {
            protected override bool Execute(Action<T> command, [CanBeNull] T ctx)
            {
                command(ctx);
                return true;
            }
        }
        
        public class PredicateResolver : Resolver<Func<bool>, VoidCtx>
        {
            protected override bool Execute(Func<bool> command, [CanBeNull] VoidCtx ctx)
                => command();
        }

        public class FuncResolver<TResult> : Resolver<Func<TResult>, VoidCtx>
        {
            protected override bool Execute(Func<TResult> command, [CanBeNull] VoidCtx ctx)
            {
                command();
                return true;
            }
        }
        
        public class PredicateContextResolver<T> : Resolver<Func<T, bool>, T>
            where T : class, IContext
        {
            protected override bool Execute(Func<T, bool> command, [CanBeNull] T ctx)
                => command(ctx);
        }
    }
}
