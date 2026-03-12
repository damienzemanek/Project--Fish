using System;
using System.Threading.Tasks;
using EMILtools.Systems;
using UnityEngine;
using static EMILtools.Systems.ResolverSystem;
using static EMILtools.Systems.SubscriberExecutor;


namespace EMILtools.Systems
{
    public class VoidCtx : IContext { }

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
        readonly TDelegate Callback;
        readonly ResolveContainer<IResolvableWithContext> ResolveContainer;
        static readonly TResolver Resolver = new();
        
        public bool canShortCircuit { get; set; }
        public bool isActive { get; set; }
        
        public Subscriber(
            TDelegate callback,
            bool isActive = true,
            bool canShortCircuit = false)
        {
            Callback = callback;
            this.canShortCircuit = canShortCircuit;
            this.isActive = isActive;
            ResolveContainer = new ResolveContainer<IResolvableWithContext>(null, null, null);
        }

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

            if (ResolveContainer.afterExecution == null
                && ResolveContainer.beforeExecution == null
                && ResolveContainer.failedExecution == null)
            {
                ResolveContainer = new ResolveContainer<IResolvableWithContext>(null, null, null);
            }
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
}
