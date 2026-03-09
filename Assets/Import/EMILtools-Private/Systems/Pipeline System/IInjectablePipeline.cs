
namespace EMILtools.Systems
{
    public interface IInjectablePipeline<TContext>
        where TContext : class, IPipelineContext
    {
        public Pipeline<TContext> executionPipeline { get; set; }

        public virtual Pipeline<TContext> InjectPipeline(PipelineBuilder<TContext> builder)
            => throw new System.NotImplementedException();

        public virtual PipelineBuilder<TContext> InjectSteps(PipelineBuilder<TContext> builder)
            => throw new System.NotImplementedException();  

        public virtual PipelineStepDelegate<TContext> InjectMainStep() 
            => throw new System.NotImplementedException(); 
    
        public void Setup(bool setupWithFinalStep)
        {
            // + 1 to accomodate for the final step
            var builder = new PipelineBuilder<TContext>();
            if (setupWithFinalStep) executionPipeline = InjectPipeline(builder);
            else executionPipeline = InjectSteps(builder).InjectMainMethod(InjectMainStep());
        }
    }
}