using System;
using Sirenix.OdinInspector;

namespace EMILtools.Systems
{
    public abstract class UnboundFunctionality<TFacade, TViewCtx> : MonoFunctionalityModule<TFacade>, 
        IInjectablePipeline
        where TFacade : class, IFacade
        where TViewCtx : class, IContextViewImmutable, IModuleUsabableContext
    {
        // ------------ Variables -----------
        

        public Pipeline ExecutionPipeline { get; set; }    // PIPELINE SYSTEM
        protected readonly SubResolvableCtx<TViewCtx> subscriber;    // PUB / SUB SYSTEM
        [ShowInInspector] ConsumeBufferSub<TViewCtx> consumeBuffer;  // INPUT BUFFER SYSTEM
        
        protected void ResetBuffer() => consumeBuffer?.Reset();
        
        // Ctor
        protected UnboundFunctionality(TFacade facade) : base(facade)
        {
            subscriber = new SubResolvableCtx<TViewCtx>(ExecuteSubscription);
        }

        // API Access
        IInjectablePipeline injectablePipeline => this;
        public override ISubscriber Subscriber => subscriber;
    
        // Methods
        public Action<APICtx> InjectMainStep<APICtx>() where APICtx : IContextViewImmutable
            => ctx => ExecutionImplementation(ctx as TViewCtx);

        bool ExecuteSubscription(TViewCtx ctx)
        {
            PipelineExecutor<TViewCtx>.Execute(ExecutionPipeline, ctx).Forget("Pipeline Execution");
            return false;
        }
        
        public override void SetupModule()
        {
            injectablePipeline.Setup<TViewCtx>(setupWithFinalStep: false);
            Awake();
        }

        // ------------ Optional Compositional Elements -----------
        
        // INPUT BUFFER SYSTEM
        protected void UseBuffer(Func<bool> bufferPredicate, Ref<float> bufferTime, Func<bool> enableHandle = null)
            => consumeBuffer = new ConsumeBufferSub<TViewCtx>(bufferPredicate, subscriber, bufferTime, enableHandle);
        
    
        // Abstract    
        /// <summary>
        /// Inject steps into the pipeline
        /// </summary>
        /// <param name="builder"></param>
        /// <returns></returns>
        public virtual PipelineBuilder InjectSteps(PipelineBuilder builder) => builder;
        
        /// <summary>
        /// Execute the functionality's purpose
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected abstract void ExecutionImplementation(TViewCtx iContextViewImmutable);
    }
}
