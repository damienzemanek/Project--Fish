using System;
using EMILtools.Core;
using EMILtools.Systems;


namespace EMILtools.Systems
{
    

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
        where TContext : class, IPipelineContext
    {
        
        // ------ Variables --------
        public readonly Func<TContext, bool> StepExecute;
        public readonly ResolveContainer Resolves;
        public readonly StepType StepType;

        
        // ------ Ctors ------
        public PipelineStep(StepType stepType, Func<TContext, bool> stepExecute, 
            ResolveContainer resolves = default)
        {
            StepExecute = stepExecute;
            Resolves = resolves;
            StepType = stepType;
        }
        public PipelineStep(Func<TContext, bool> mainMethod)
        {
            StepExecute = mainMethod;
            Resolves = new ResolveContainer(null, null, null);
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

}




