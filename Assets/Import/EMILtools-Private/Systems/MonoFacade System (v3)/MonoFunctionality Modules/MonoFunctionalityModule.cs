using System;
using Sirenix.OdinInspector;
using UnityEngine;
using static EMILtools.Systems.SubscriberExecutor;


namespace EMILtools.Systems
{
    public abstract class MonoFunctionalityModule<TFacade, TContext> 
        where TFacade : class, IFacade
    {
        [Title("$Name"), PropertyOrder(-1)]
        [ShowInInspector] public string Name => "Module: " + this.GetType().Name;
    
    
        // ---------- Variables ----------
        public TFacade facade { get; set; }
        protected readonly SubscriberCtx<Action<TContext>, ActionResolverCtx<TContext>, TContext> subscriber;

        // ---------- Ctor ----------
        public MonoFunctionalityModule(TFacade facade) => this.facade = facade;
    
    
        // ---------- Abstracts ----------
    
        /// <summary>
        /// Set up the module, called from Monobehaviour's Awake
        /// </summary>
        protected virtual void Awake() { }
    
        /// <summary>
        /// "Template Method Pattern" For Awake
        /// </summary>
        public abstract void SetupModule();

    }
}
