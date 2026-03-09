using System;
using Sirenix.OdinInspector;
using UnityEngine;




/// <summary>
/// Optionally Context Dependant
/// Template Method Pattern
/// ValueType Execution Hook
/// </summary>
/// <typeparam name="TContext"></typeparam>
public interface IExecuteTemplate<TContext> 
    where TContext : struct
{
    public void Execute();
    public bool ExecutionImplementation(TContext ctx);
}






public abstract class MonoFunctionalityModule<TFacade, TContext> 
    where TFacade : class, IFacade<TContext>
    where TContext : struct, IModuleUsabableContext
{
    [Title("$Name"), PropertyOrder(-1)]
    [ShowInInspector] public string Name => "Module: " + this.GetType().Name;
    
    
    // ---------- Variables ----------
    public TFacade facade { get; set; }
    public TContext context => facade.API_Context();
    

    // ---------- Ctor ----------
    public MonoFunctionalityModule(TFacade facade) => this.facade = facade;
    
    
    // ---------- Abstracts ----------
    
    /// <summary>
    /// Set up the module, called from Monobehaviour's Awake
    /// </summary>
    protected virtual void Awake() { }
    
    /// <summary>
    /// "Template Method Pattern" For Awake
    /// </summary>
    public abstract void SetupModule();

}