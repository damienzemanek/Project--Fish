using System;
using System.Threading.Tasks;
using EMILtools.Core;
using UnityEngine;
using static EMILtools.Systems.ResolverSystem;

namespace EMILtools.Systems
{
    
    /// <summary>
    /// Frame-Agnostic Async Pipeline Executor
    /// SRP: Execution
    /// </summary>
    public static class PipelineExecutor<TContext>
        where TContext : class, IPipelineContext
    {
        static PipelineResolver Resolver = new();
        class PipelineResolver : ContextResolver<PipelineStepDelegate<TContext>, TContext>
        {
            protected override bool Execute(PipelineStepDelegate<TContext> command, in TContext ctx)
             => command(ctx);
        }

        public static async Task Execute(Pipeline<TContext> pipeline, TContext ctx)
        {
            for (int i = 0; i < pipeline.Size; i++)
            {
                var step = pipeline[i];
                var isShortCircuit = step.StepType == StepType.ShortCircuit;
                if(!await Resolver.ResolveContainer(step.Resolves, step.Execute, isShortCircuit, ctx)) return;
                
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
        public static Task TryTo(Pipeline<TContext> pipeline, in TContext ctx)
        => Execute(pipeline, ctx);

        
        
    }
}
