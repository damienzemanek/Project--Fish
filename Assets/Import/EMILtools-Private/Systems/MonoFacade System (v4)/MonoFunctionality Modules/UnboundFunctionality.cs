using System;
using Sirenix.OdinInspector;

namespace EMILtools.Systems
{
    public abstract class UnboundFunctionality<TFacade, TViewCtx> : MonoFunctionalityModule<TFacade>, 
        IPipelineInjector<TViewCtx>
        where TFacade : class, IFacade
        where TViewCtx : class, IContextViewImmutable
    {
        /// <summary>
        /// Execute the functionality's purpose
        /// </summary>
        /// <param name="ctx"></param>
        /// <returns></returns>
        protected abstract void ExecutionImplementation(TViewCtx ctx);

        protected UnboundFunctionality(TFacade facade) : base(facade)
        {
            var injectablePipeline = new InjectablePipeline<TViewCtx> ( injector: this, setupPipelineInOnePass:false);
            subscriber = new SubResolvableCtx<TViewCtx>(ctx => { injectablePipeline.Execute(ctx); return false; });
        }
        public override void SetupModule() => Awake();

        // ---------------------------------- PIPELINE SYSTEM ----------------------------------
        public Pipeline<TViewCtx> InjectPipeline(PipelineBuilder<TViewCtx> builder) => throw new NotImplementedException();
        public Action<TViewCtx> InjectMainStep() => ExecutionImplementation;
        public virtual PipelineBuilder<TViewCtx> InjectSteps(PipelineBuilder<TViewCtx> builder) => builder;
        
        

        // ---------------------------------- PUB / SUB SYSTEM ----------------------------------

        protected readonly SubResolvableCtx<TViewCtx> subscriber;    
        public override ISubscriber Subscriber => subscriber;
        
        
        // ---------------------------------- CONSUME BUFFER SYSTEM ----------------------------------
        
        [ShowInInspector] ConsumeBufferSub<TViewCtx> consumeBuffer;  
        protected void ResetConsumeBuffer() => consumeBuffer?.Reset();
        protected void UseBuffer(Func<bool> bufferPredicate, Ref<float> bufferTime, Func<bool> enableHandle = null)
            => consumeBuffer = new ConsumeBufferSub<TViewCtx>(bufferPredicate, subscriber, bufferTime, enableHandle);
        


        

        
    }
}
