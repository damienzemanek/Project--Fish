
using System;

namespace EMILtools.Systems
{
    public interface IPipelineInjector<TViewCtx>
        where TViewCtx : IContextViewImmutable
    {
        public Pipeline<TViewCtx> InjectPipeline(PipelineBuilder<TViewCtx> builder);

        public PipelineBuilder<TViewCtx> InjectSteps(PipelineBuilder<TViewCtx> builder);
        public Action<TViewCtx> InjectMainStep();
    }

    public class InjectablePipeline<TViewCtx>
        where TViewCtx : IContextViewImmutable
    {
        readonly Pipeline<TViewCtx> Pipeline;

        public InjectablePipeline(IPipelineInjector<TViewCtx> injector, bool setupPipelineInOnePass = true)
        {
            Pipeline = SetupInjectablePipeline(injector, setupPipelineInOnePass);
        }

        Pipeline<TViewCtx> SetupInjectablePipeline(IPipelineInjector<TViewCtx> injector, bool setupPipelineInOnePass)
        {
            var builder = new PipelineBuilder<TViewCtx>();
            return setupPipelineInOnePass 
                ? injector.InjectPipeline(builder) 
                : injector.InjectSteps(builder).InjectMainMethod(injector.InjectMainStep());
        }
        
        public static implicit operator Pipeline<TViewCtx>(InjectablePipeline<TViewCtx> pipeline) => pipeline.Pipeline;
        
        public void Execute(TViewCtx ctx) => Pipeline.Execute(ctx);
    }
}