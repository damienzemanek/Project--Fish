using System;
using System.Threading.Tasks;
using EMILtools.Systems;
using UnityEngine;
using static EMILtools.Systems.ResolverSystem;
using static EMILtools.Systems.SubscriberExecutor;


namespace EMILtools.Systems
{
    public struct VoidCtx : IContext { }

    public abstract class SubscriberBase<TDelegate, TResolver> : ISubscriber
        where TDelegate : Delegate
        where TResolver : IResolver, new()
    {
        public static readonly VoidCtx VoidCtx = new();
        protected readonly TDelegate Callback;
        protected readonly ResolveContainer<IResolvableWithContext> ResolveContainer;
        protected static readonly TResolver Resolver = new();
        
        // Mutations
        public bool canShortCircuit { get; set; }
        public bool isActive { get; set; }
        public abstract Task Execute();


        public SubscriberBase(
            TDelegate callback,
            bool isActive = true,
            bool canShortCircuit = false)
        {
            Callback = callback;
            this.canShortCircuit = canShortCircuit;
            this.isActive = isActive;
            ResolveContainer = new ResolveContainer<IResolvableWithContext>(null, null, null);
        }

        public SubscriberBase(
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
    }

    /// <summary>
    /// No context
    /// Note: Predicates return TRUE are fore SHORTCIRCUIT
    ///       Predicates return FALSE are NOT SHORTCIRCUIT
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    /// <typeparam name="TResolver"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public sealed class Subscriber<TDelegate, TResolver> 
        
        : SubscriberBase<TDelegate, TResolver>, ISubscriber
    
        where TDelegate : Delegate
        where TResolver : BaseResolver<TDelegate>, new()
    {
        public Subscriber(TDelegate callback, bool isActive = true, bool canShortCircuit = false) : base(callback, isActive, canShortCircuit) { }
        public Subscriber(TDelegate callback, ResolveContainer<IResolvableWithContext> resolveContainer, bool isActive = true, bool canShortCircuit = false) : base(callback, resolveContainer, isActive, canShortCircuit) { }
        
        public override Task Execute()
        {
            if (!isActive) return Task.CompletedTask;
            return Resolver.ResolveContainer(ResolveContainer, Callback, canShortCircuit);
        }
    }
    
    /// <summary>
    /// With Context
    /// Note: Predicates return TRUE are fore SHORTCIRCUIT
    ///       Predicates return FALSE are NOT SHORTCIRCUIT
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    /// <typeparam name="TResolver"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public sealed class SubscriberCtx<TDelegate, TResolver, TContext> 
        
        : SubscriberBase<TDelegate, TResolver>, ISubscriber<TContext>
    
        where TDelegate : Delegate
        where TResolver : ContextResolver<TDelegate, TContext>, new()
    {
        public SubscriberCtx(TDelegate callback, bool isActive = true, bool canShortCircuit = false) : base(callback, isActive, canShortCircuit) { }
        public SubscriberCtx(TDelegate callback, ResolveContainer<IResolvableWithContext> resolveContainer, bool isActive = true, bool canShortCircuit = false) : base(callback, resolveContainer, isActive, canShortCircuit) { }

        public TContext cachedCtx { get; set; }

        public Task Execute(TContext ctx)
        {
            if (!isActive) return Task.CompletedTask;
            cachedCtx = ctx;
            return Execute();
        }

        public override Task Execute()
            => Resolver.ResolveContainer( ResolveContainer, Callback, canShortCircuit, cachedCtx);
    }
    
    /// <summary>
    /// With T1, T2
    /// Note: Predicates return TRUE are fore SHORTCIRCUIT
    ///       Predicates return FALSE are NOT SHORTCIRCUIT
    /// </summary>
    /// <typeparam name="TDelegate"></typeparam>
    /// <typeparam name="TResolver"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    public sealed class SubscriberCtx<TDelegate, TResolver, T1, T2> 
        
        : SubscriberBase<TDelegate, TResolver>, ISubscriber<T1, T2>
    
        where TDelegate : Delegate
        where TResolver : ContextResolver<TDelegate, T1, T2>, new()
    {
        public SubscriberCtx(TDelegate callback, bool isActive = true, bool canShortCircuit = false) : base(callback, isActive, canShortCircuit) { }
        public SubscriberCtx(TDelegate callback, ResolveContainer<IResolvableWithContext> resolveContainer, bool isActive = true, bool canShortCircuit = false) : base(callback, resolveContainer, isActive, canShortCircuit) { }

        public T1 cachedCtx1 { get; set; }
        public T2 cachedCtx2 { get; set; }
        public Task Execute(T1 ctx1, T2 ctx2)
        {
            if (!isActive) return Task.CompletedTask;
            cachedCtx1 = ctx1;
            cachedCtx2 = ctx2;
            return Execute();
        }

        public override Task Execute()
            => Resolver.ResolveContainer( ResolveContainer, Callback, canShortCircuit, cachedCtx1, cachedCtx2 );

        
    }
}
