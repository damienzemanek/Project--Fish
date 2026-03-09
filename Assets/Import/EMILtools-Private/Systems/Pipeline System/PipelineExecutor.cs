using System.Threading.Tasks;

namespace EMILtools.Systems
{
    /// <summary>
    /// Frame-Agnostic Async Pipeline Executor
    /// SRP: Execution
    /// </summary>
    public static class PipelineExecutor
    {
        public static async Task Execute<TContext>(Pipeline<TContext> pipeline, TContext ctx)
            where TContext : class, IPipelineContext
        {
            // For loop for structs,
            // - Has no mutations to TContext
            for (int i = 0; i < pipeline.Size; i++)
            {
                var step = pipeline[i];
                var contexts = step.resolveContexts;
                var isShortCircuit = step.StepType == StepType.ShortCircuit;
                for (int j = 0; j < contexts.Length; j++)
                {
                    var resolve = contexts[j];
                    if (!resolve.Resolve(ctx) && isShortCircuit) return;
                    if (resolve is IResolveWaitable waitable && !waitable.waiting)
                    {
                        waitable.waiting = true; // Must set waiting here because Resolve() just starts the timer
                        await waitable.WaitUntilResolved();
                    }
                }
                if (step.Execute(ctx) && isShortCircuit) return;
            }
                
        }
        
        

        /// <summary>
        /// Main API for executing pipelines,
        /// FLUENT API-lite, call pipelines like your "Trying to" "Do something"
        /// ---------------------------------
        /// Usage:
        /// var jump = PipelineBuilder...
        /// TryTo(jump, jumpContext);
        /// ---------------------------------
        /// Slightly more performant option via "in" keyword
        /// SRP: API
        /// </summary>
        /// <param name="pipeline"></param>
        /// <param name="ctx"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static Task TryTo<TContext>(Pipeline<TContext> pipeline, in TContext ctx)
            where TContext : class, IPipelineContext
        => Execute(pipeline, ctx);



        /// <summary>
        /// Secondary API for executing pipelines,
        /// FLUENT API-lite, call pipelines like your "Trying to" "Do something"
        /// ---------------------------------
        /// Usage:
        /// var jump = PipelineBuilder...
        /// jumpContext.TryTo(jump);
        /// ---------------------------------
        /// Slightly less performant option via "this" (this is pass by value for value-types)
        /// SRP: API
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="pipeline"></param>
        /// <typeparam name="TContext"></typeparam>
        /// <returns></returns>
        public static Task TryTo<TContext>(this TContext ctx, Pipeline<TContext> pipeline)
            where TContext : class, IPipelineContext
            => Execute(pipeline, ctx);


        /// Commented version of Executor for Debugging
        ///
        //  public static async Task Execute<TContext>(Pipeline<TContext> pipeline, TContext ctx)
        //      where TContext : struct, IPipelineContext
        // {
        //     // For loop for structs,
        //     // - Has no mutations to TContext
        //     for (int i = 0; i < pipeline.Size; i++)
        //     {
        //         //Debug.Log($"Executing Step {i}");
        //         var step = pipeline[i];
        //         var contexts = step.resolveContexts;
        //         var isShortCircuit = step.StepType == StepType.ShortCircuit;
        //         //Debug.Log($"Setup Variables : Step: {step.GetType().Name} : Contexts {contexts.Length} : isShortCircuit {isShortCircuit}");
        //         for (int j = 0; j < contexts.Length; j++)
        //         {
        //             var resolve = contexts[j];
        //             if (!resolve.Resolve(ctx) && isShortCircuit)
        //             {
        //                 //Debug.Log("Resolver Short Circuited");
        //                 return;
        //             }
        //
        //             if (resolve is IResolveWaitable waitable && !waitable.waiting)
        //             {
        //                 //Debug.Log($"Waiting for {waitable.GetType().Name} to resolve");
        //                 waitable.waiting = true;
        //                 await waitable.WaitUntilResolved();
        //                 // Debug.Log($" {waitable.GetType().Name} Resolved");
        //             }
        //         }
        //
        //         //Debug.Log($"Executing {step.GetType().Name} : {step.Execute(ctx)}");
        //         if (step.Execute(ctx) && isShortCircuit)
        //         {
        //             //Debug.Log("Step Short Circuited");
        //             return;
        //             
        //         }
        //     }
        //         
        // }
        
    }
}
