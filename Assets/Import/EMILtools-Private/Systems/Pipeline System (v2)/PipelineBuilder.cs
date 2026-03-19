using System;
using System.Collections.Generic;
using EMILtools.Core;

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
    public class PipelineBuilder
    {
        // ------ Variables ---------
        List<PipelineStep> steps;
    
        // ------ Ctor ---------
        public PipelineBuilder() => steps = new List<PipelineStep>();
    
        // ------ API Methods ---------
        public PipelineBuilder Add_ShortCircuit(
            IPredicate @if,
            IResolvable[] before = null, IResolvable[] after = null, IResolvable[] shortCircuited = null)
        {
            var NewResolves = new ResolveContainer(before, after, shortCircuited);
            steps.Add(new PipelineStep(@if, NewResolves));
            return this;
        }
        public PipelineBuilder Add_Middleware<TViewCtx>(
            Action<TViewCtx> method, 
            IResolvable[] before = null, IResolvable[] after = null)
        where TViewCtx : IContextViewImmutable
        {
            var NewResolves = new ResolveContainer(before, after);

            Action<TViewCtx> typed = method;
            Action<object> objAction = obj =>
            {
                if (obj is TViewCtx ctx) typed(ctx);
                else throw new ArgumentException(
                        $"The method {method.Method.Name} was passed an object of type {obj.GetType().Name} " +
                        $"instead of {typeof(TViewCtx).Name}.", nameof(method) );
            };
            
            steps.Add(new PipelineStep(objAction, NewResolves));
            return this;
        }
        
        
        public Pipeline InjectMainMethod<TViewCtx>(Action<TViewCtx> mainMethod) 
            where TViewCtx : IContextViewImmutable
        {
            Action<TViewCtx> typed = mainMethod;
            Action<object> objAction = obj =>
            {
                if (obj is TViewCtx ctx) typed(ctx);
                else throw new ArgumentException(
                        $"The method {mainMethod.Method.Name} was passed an object of type {obj.GetType().Name} " +
                        $"instead of {typeof(TViewCtx).Name}.", nameof(mainMethod) );
            };
            
            steps.Add(new PipelineStep(objAction));
            return new Pipeline(steps.ToArray());
        }
    }
}
