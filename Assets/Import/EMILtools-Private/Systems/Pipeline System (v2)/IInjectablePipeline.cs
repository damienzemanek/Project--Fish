
using System;

namespace EMILtools.Systems
{
    public interface IInjectablePipeline
    {
        public Pipeline ExecutionPipeline { get; set; }

        public Pipeline InjectPipeline(PipelineBuilder builder)
            => throw new System.NotImplementedException();

        public PipelineBuilder InjectSteps(PipelineBuilder builder)
            => throw new System.NotImplementedException();  

        public Action<TViewCtx> InjectMainStep<TViewCtx>() 
            where TViewCtx : IContextViewImmutable
        => throw new System.NotImplementedException(); 
    
        public void Setup<TViewCtx>(bool setupWithFinalStep)
            where TViewCtx : IContextViewImmutable
        {
            // + 1 to accomodate for the final step
            var builder = new PipelineBuilder();
            if (setupWithFinalStep) ExecutionPipeline = InjectPipeline(builder);
            else ExecutionPipeline = InjectSteps(builder).InjectMainMethod(InjectMainStep<TViewCtx>());
        }
    }
}