using System;


/// <summary>
/// Represents a delegate that defines the execution logic for a step in the pipeline.
/// Used to process a specific context within the pipeline's execution flow.
/// SRP: Execution logic for pipeline steps.
/// </summary>
/// <typeparam name="TContext">
/// The type of context used in the pipeline.
/// Intent: High Throughput, so it's a CLASS
/// </typeparam>
public delegate bool PipelineStepDelegate<in TContext>(TContext context);


/// <summary>
/// Represents a step in a pipeline,
/// Defines the type of step, its execution logic, and its associated resolution contexts.
/// SRP: Storage
/// </summary>
/// <typeparam name="TContext">
/// The type of context used in the pipeline.
/// Intent: High Throughput, so it's a CLASS
/// </typeparam>
public readonly struct PipelineStep<TContext>
    where TContext : class
{
    const bool ResolveSuccessfull = true;


    // ------ Variables --------
    public readonly PipelineStepDelegate<TContext> Execute;
    public readonly IResolveContext[] resolveContextsBeforeExecution;
    public readonly IResolveContext[] resolveContextsAfterExecution;

    public readonly StepType StepType;

    
    // ------ Ctors ------
    public PipelineStep(StepType stepType, PipelineStepDelegate<TContext> execute, 
        IResolveContext[] resolveContextsBeforeExecution = null,
        IResolveContext[] resolveContextsAfterExecution = null)
    {
        this.Execute = execute;
        this.resolveContextsBeforeExecution = resolveContextsBeforeExecution ?? Array.Empty<IResolveContext>();
        this.resolveContextsAfterExecution = resolveContextsAfterExecution ?? Array.Empty<IResolveContext>();
        StepType = stepType;
    }
    public PipelineStep(PipelineStepDelegate<TContext> mainMethod)
    {
        Execute = mainMethod;
        resolveContextsBeforeExecution = Array.Empty<IResolveContext>();
        resolveContextsAfterExecution = Array.Empty<IResolveContext>();
        StepType = StepType.MainMethod;
    }
}


/// <summary>
/// Represents the type of a step within a pipeline.
/// Defines the role and behavior of a step during pipeline execution.
/// SRP: Classification of step roles.
/// </summary>
public enum StepType
{
    /// <summary>
    /// Allows execution to continue regardless of the return state
    ///
    /// Usage: Add_Middleware(ctx => { DoSomething(ctx); return false; });
    /// </summary>
    Middleware, 
    
    /// <summary>
    /// Short-circuits execution if return state is false (Early Exit)
    /// Typically used with validation steps to ensure that the main logic is only executed when certain conditions are met.
    ///
    /// Usage: Add_ShortCircuit(ctx => isSomethingValid(ctx)); 
    /// </summary>
    ShortCircuit,

    /// <summary>
    /// Represents the primary execution step in a pipeline.
    /// This step defines the main logic to be executed within the process
    /// and is typically the final operation in the pipeline sequence.
    ///
    /// Usage: Add_MainMethod(ctx => { DoSomething(ctx); });
    /// </summary>
    MainMethod
}




