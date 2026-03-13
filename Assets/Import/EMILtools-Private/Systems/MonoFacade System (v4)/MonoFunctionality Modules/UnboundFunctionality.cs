

using System;

namespace EMILtools.Systems
{
    public abstract class UnboundFunctionality<TFacade, TViewCtx> : MonoFunctionalityModule<TFacade>, 
        IInjectablePipeline<TViewCtx>
        where TFacade : class, IFacade
        where TViewCtx : class, IContextViewImmutable, IModuleUsabableContext
    {
        // Variables
        public Pipeline<TViewCtx> ExecutionPipeline { get; set; }
        protected readonly SubResolvableCtx<TViewCtx> subscriber;
        
        // Ctor
        protected UnboundFunctionality(TFacade facade) : base(facade)
            => subscriber = 
        new SubResolvableCtx<TViewCtx>(ExecuteSubscription);
    
        // API Access
        IInjectablePipeline<TViewCtx> injectablePipeline => this;
        public override ISubscriber Subscriber => subscriber;
    
        // Methods
        public Func<TViewCtx, bool> InjectMainStep() => new(ExecutionImplementation);

        bool ExecuteSubscription(TViewCtx ctx)
        {
            PipelineExecutor<TViewCtx>.Execute(ExecutionPipeline, ctx).Forget("Pipeline Execution");
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
        public abstract PipelineBuilder<TViewCtx> InjectSteps(PipelineBuilder<TViewCtx> builder);


        /// <summary>
        /// Execute the functionality's purpose
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        public abstract bool ExecutionImplementation(TViewCtx ctx);
    }
}
