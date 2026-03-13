using System;
using Sirenix.OdinInspector;
using UnityEngine;
using static EMILtools.Systems.SubscriberExecutor;


namespace EMILtools.Systems
{
    public abstract class UnboundFunctionality<TFacade, TMonoStructure, TContext> : MonoFunctionalityModule<TFacade, TMonoStructure>, 
        IInjectablePipeline<TContext>
        where TFacade : class, IFacade<TMonoStructure>
        where TContext : class, IModuleUsabableContext
        where TMonoStructure : IMonoStructure
    {
        // Variables
        public Pipeline<TContext> executionPipeline { get; set; }
        public Subscriber<Action<TContext>, ActionContextResolver<TContext>, TContext> subscriber { get; set; }
        
        // Ctor
        protected UnboundFunctionality(TFacade facade) : base(facade)
            => subscriber = 
        new Subscriber<Action<TContext>, ActionContextResolver<TContext>, TContext>(ExecuteSubscription);
    
        // API Access
        protected IInjectablePipeline<TContext> injectablePipeline => this;
    
        // Methods
        public PipelineStepDelegate<TContext> InjectMainStep() => new(ExecutionImplementation);
        
        void ExecuteSubscription(TContext ctx) => PipelineExecutor<TContext>.Execute(executionPipeline, ctx);
        
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
