
namespace EMILtools.Systems
{
    /// <summary>
    /// Execution pipeline that organizes and executes a series of steps using a specific context.
    /// This allows for a structured and sequential approach to processing data or executing tasks.
    /// </summary>
    /// <typeparam name="TContext">
    /// The context type that is passed and operated upon by the pipeline. The context
    /// Intent: High Throughput, so the CONTEXT a CLASS
    ///
    /// Extra Functionality Option: Since the main method returns a bool, you can use that to determine if the
    /// full pipeline execution was SUCCESSFUL dynamically
    /// </typeparam>
    public class Pipeline<TContext>
        where TContext : class, IPipelineContext
    {
        readonly PipelineStep<TContext>[] steps;
        public int Size => steps?.Length ?? throw new System.NullReferenceException();
        public PipelineStep<TContext> this[int index] => steps[index];
        public Pipeline(PipelineStep<TContext>[] _steps) => steps = _steps;
    }
}

