using System;
using Sirenix.OdinInspector;
using UnityEngine;


namespace EMILtools.Systems
{
    public abstract class UnboundFunctionality<TFacade, TMonoStructure, TContext> : MonoFunctionalityModule<TFacade, TMonoStructure>, 
        IExecuteTemplate<TContext>, 
        IInjectablePipeline<TContext>
        where TFacade : class, IFacade<TMonoStructure>
        where TContext : class, IModuleUsabableContext
        where TMonoStructure : IMonoStructure
    {
        // Variables
        public Pipeline<TContext> executionPipeline { get; set; }
    
        // Ctor
        protected UnboundFunctionality(TFacade facade) : base(facade) { }
    
        // API Access
        protected IInjectablePipeline<TContext> injectablePipeline => this;
        protected IExecuteTemplate<TContext> ExecuteTemplate => this;
    
        // Methods
        public PipelineStepDelegate<TContext> InjectMainStep() => new(ExecutionImplementation);

        [Button]
        public void Execute()
        {
            var ctx = facade.API_Structure().API_ContextData;

            //Debug.Log($"Expected: {typeof(TContext)}");
            //Debug.Log($"Actual: {ctx?.GetType()}");

            PipelineExecutor<TContext>.Execute(executionPipeline, (TContext)ctx);
        }
        
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
