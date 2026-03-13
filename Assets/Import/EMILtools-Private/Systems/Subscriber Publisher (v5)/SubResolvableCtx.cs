using System;
using System.Threading.Tasks;
using static EMILtools.Systems.ResolverSystem;


namespace EMILtools.Systems
{
    public struct VoidCtx : IContext { }

    public abstract class ResolveSubscriberBase : ISubscriber
    {
        public static readonly VoidCtx VoidCtx = new();
        protected readonly ResolveContainer ResolveContainer;
        
        // Mutations
        public bool canShortCircuit { get; set; }
        public bool isActive { get; set; }
        public abstract Task Execute();


        public ResolveSubscriberBase(
            bool isActive = true,
            bool canShortCircuit = false)
        {
            this.canShortCircuit = canShortCircuit;
            this.isActive = isActive;
            ResolveContainer = new ResolveContainer(null, null, null);
        }

        public ResolveSubscriberBase(
            ResolveContainer resolveContainer,
            bool isActive = true,
            bool canShortCircuit = false)
        {
            ResolveContainer = resolveContainer;
            this.canShortCircuit = canShortCircuit;
            this.isActive = isActive;

            if (ResolveContainer.afterExecution == null
                && ResolveContainer.beforeExecution == null
                && ResolveContainer.failedExecution == null)
            {
                ResolveContainer = new ResolveContainer(null, null, null);
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
    public sealed class SubResolvable : ResolveSubscriberBase, ISubscriber
    {
        readonly Func<bool> Callback;
        public SubResolvable(Func<bool> callback, bool isActive = true, bool canShortCircuit = false) : base(isActive, canShortCircuit) 
            => Callback = callback;
        public SubResolvable(Func<bool> callback, ResolveContainer resolveContainer, bool isActive = true, bool canShortCircuit = false) : base(resolveContainer, isActive, canShortCircuit)
            => Callback = callback;
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
    public sealed class SubResolvableCtx<TContext> : ResolveSubscriberBase, ISubscriber<TContext>
    {
        readonly Func<TContext, bool> Callback;
        public SubResolvableCtx(Func<TContext, bool> callback, bool isActive = true, bool canShortCircuit = false) : base(isActive, canShortCircuit)
            => Callback = callback;
        public SubResolvableCtx(Func<TContext, bool> callback, ResolveContainer resolveContainer, bool isActive = true, bool canShortCircuit = false) : base(resolveContainer, isActive, canShortCircuit)
            => Callback = callback;

        public TContext cachedCtx { get; set; }

        public Task Execute(TContext ctx)
        {
            if (!isActive) return Task.CompletedTask;
            cachedCtx = ctx;
            return Execute();
        }

        public override Task Execute() => Resolver.ResolveContainer(ResolveContainer, Callback, canShortCircuit, cachedCtx);
    }
    
}
