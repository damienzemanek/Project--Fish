using System.Collections.Generic;

namespace EMILtools.Systems
{
    /// <summary>
    /// A builder class for constructing a pipeline of steps to be executed using a defined context.
    /// SRP: Initialization
    /// </summary>
    /// <typeparam name="TContext">
    /// The type of the context associated with the pipeline.
    /// TContext must be a structure and implement the IPipelineContext interface.
    /// </typeparam>
    public class PipelineBuilder<TContext>
        where TContext : class, IPipelineContext
    {
        // ------ Variables ---------
        List<PipelineStep<TContext>> steps;
    
        // ------ Ctor ---------
        public PipelineBuilder() => steps = new List<PipelineStep<TContext>>();
    
        // ------ Methods ---------
        PipelineBuilder<TContext> AddStep(
            StepType stepType,
            PipelineStepDelegate<TContext> @if,
            IResolveContext[] before = null,
            IResolveContext[] after = null,
            IResolveContext[] shortCircuited = null)
        {
            steps.Add(new PipelineStep<TContext>(stepType, @if, before, after, shortCircuited));
            return this;
        }
    
        // ------ API Methods ---------
        public PipelineBuilder<TContext> Add_ShortCircuit(
            PipelineStepDelegate<TContext> @if,
            IResolveContext[] before = null, IResolveContext[] after = null, IResolveContext[] shortCircuited = null)
        {
            return AddStep(StepType.ShortCircuit, @if, before, after, shortCircuited);
        }
        public PipelineBuilder<TContext> Add_Middleware(PipelineStepDelegate<TContext> method, 
            IResolveContext[] before = null, IResolveContext[] after = null)
        {
            return AddStep(StepType.Middleware, method, before, after);
        }
        
        
    
        public Pipeline<TContext> InjectMainMethod(PipelineStepDelegate<TContext> mainMethod) 
        {
            steps.Add(new PipelineStep<TContext>(mainMethod));
            return new Pipeline<TContext>(steps.ToArray());
        }
    }
}
