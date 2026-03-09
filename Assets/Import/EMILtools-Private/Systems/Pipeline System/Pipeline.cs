



/// <summary>
/// Execution pipeline that organizes and executes a series of steps using a specific context.
/// This allows for a structured and sequential approach to processing data or executing tasks.
/// </summary>
/// <typeparam name="TContext">
/// The context type that is passed and operated upon by the pipeline. The context
/// type must be a value type and implement the <see cref="IPipelineContext"/> interface.
/// </typeparam>
public class Pipeline<TContext>
    where TContext : struct, IPipelineContext
{
    readonly PipelineStep<TContext>[] steps;
    public int Size => steps?.Length ?? throw new System.NullReferenceException();
    public PipelineStep<TContext> this[int index] => steps[index];
    public Pipeline(PipelineStep<TContext>[] _steps) => steps = _steps;
}