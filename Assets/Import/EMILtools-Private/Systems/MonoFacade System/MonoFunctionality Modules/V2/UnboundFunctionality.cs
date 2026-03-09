using System;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class UnboundFunctionality<TFacade, TContext> : MonoFunctionalityModule<TFacade, TContext>, 
    IExecuteTemplate<TContext>, 
    IInjectablePipeline<TContext>
    where TFacade : class, IFacade<TContext>
    where TContext : struct, IModuleUsabableContext
{
    // Variables
    public Pipeline<TContext> executionPipeline { get; set; }
    
    // Ctor
    protected UnboundFunctionality(TFacade facade) : base(facade) { }
    
    // API Access
    protected IInjectablePipeline<TContext> injectablePipeline => this;
    protected IExecuteTemplate<TContext> ExecuteTemplate => this;
    
    // Methods
    public PipelineStepDelegate<TContext> InjectMainStep() => new(ExecutionImplementation);
    [Button] public void Execute() => context.TryTo(executionPipeline);
    public override void SetupModule()
    {
        injectablePipeline.Setup(setupWithFinalStep: false);
        Awake();
    }
    
    // Abstract    
    /// <summary>
    /// Inject steps into the pipeline
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public abstract PipelineBuilder<TContext> InjectSteps(PipelineBuilder<TContext> builder);
    
    /// <summary>
    /// Execute the functionality's purpose
    /// </summary>
    /// <param name="ctx"></param>
    /// <returns></returns>
    public abstract bool ExecutionImplementation(TContext ctx);

    



    
}