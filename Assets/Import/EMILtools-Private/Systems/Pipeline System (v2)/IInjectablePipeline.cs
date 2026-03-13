
using System;

namespace EMILtools.Systems
{
    public interface IInjectablePipeline<TContext>
        where TContext : class, IPipelineContext
    {
        public Pipeline<TContext> ExecutionPipeline { get; set; }

        public virtual Pipeline<TContext> InjectPipeline(PipelineBuilder<TContext> builder)
            => throw new System.NotImplementedException();

        public virtual PipelineBuilder<TContext> InjectSteps(PipelineBuilder<TContext> builder)
            => throw new System.NotImplementedException();  

        public virtual Func<TContext, bool> InjectMainStep() 
            => throw new System.NotImplementedException(); 
    
        public void Setup(bool setupWithFinalStep)
        {
            // + 1 to accomodate for the final step
            var builder = new PipelineBuilder<TContext>();
            if (setupWithFinalStep) ExecutionPipeline = InjectPipeline(builder);
            else ExecutionPipeline = InjectSteps(builder).InjectMainMethod(InjectMainStep());
        }
    }
}