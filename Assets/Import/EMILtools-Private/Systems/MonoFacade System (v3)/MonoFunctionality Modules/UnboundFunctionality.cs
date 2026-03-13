using System;
using Sirenix.OdinInspector;
using UnityEngine;
using static EMILtools.Systems.SubscriberExecutor;


namespace EMILtools.Systems
{
    public abstract class UnboundFunctionality<TFacade, TMonoStructure, TContext> : MonoFunctionalityModule<TFacade, TMonoStructure>, 
        IInjectablePipeline<TContext>
        where TFacade : class, IFacade
        where TContext : class, IModuleUsabableContext
        where TMonoStructure : IMonoStructure
    {
        // Variables
        public Pipeline<TContext> ExecutionPipeline { get; set; }
        protected readonly SubscriberCtx<Action<TContext>, ActionResolverCtx<TContext>, TContext> subscriber;
        
        // Ctor
        protected UnboundFunctionality(TFacade facade) : base(facade)
            => subscriber = 
        new SubscriberCtx<Action<TContext>, ActionResolverCtx<TContext>, TContext>(ExecuteSubscription);
    
        // API Access
        protected IInjectablePipeline<TContext> injectablePipeline => this;
    
        // Methods
        public PipelineStepDelegate<TContext> InjectMainStep() => new(ExecutionImplementation);
        
        void ExecuteSubscription(TContext ctx) => PipelineExecutor<TContext>.Execute(ExecutionPipeline, ctx);
        
        public override void SetupModule()
        {
            injectablePipeline.Setup(setupWithFinalStep: false);
            Awake();
        }
    
        // Abstract    
        /// <summary>
        /// Inject steps into the pipeline
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public abstract PipelineBuilder<TContext> InjectSteps(PipelineBuilder<TContext> builder);


        /// <summary>
        /// Execute the functionality's purpose
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract bool ExecutionImplementation(TContext ctx);
    }
}
