

using System;

namespace EMILtools.Systems
{
    public abstract class UnboundFunctionality<TFacade, TContext> : MonoFunctionalityModule<TFacade>, 
        IInjectablePipeline<TContext>
        where TFacade : class, IFacade
        where TContext : class, IModuleUsabableContext
    {
        // Variables
        public Pipeline<TContext> ExecutionPipeline { get; set; }
        protected readonly SubResolvableCtx<TContext> subscriber;
        
        // Ctor
        protected UnboundFunctionality(TFacade facade) : base(facade)
            => subscriber = 
        new SubResolvableCtx<TContext>(ExecuteSubscription);
    
        // API Access
        IInjectablePipeline<TContext> injectablePipeline => this;
        public override ISubscriber Subscriber => subscriber;
    
        // Methods
        public Func<TContext, bool> InjectMainStep() => new(ExecutionImplementation);

        bool ExecuteSubscription(TContext ctx)
        {
            PipelineExecutor<TContext>.Execute(ExecutionPipeline, ctx).Forget("Pipeline Execution");
            return false;
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
