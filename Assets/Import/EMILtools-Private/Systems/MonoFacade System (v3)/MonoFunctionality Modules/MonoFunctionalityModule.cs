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
    where TContext : class
{
    public void Execute();
    public bool ExecutionImplementation(TContext ctx);
}






public abstract class MonoFunctionalityModule<TFacade, TMonoStructure> 
    where TFacade : class, IFacade<TMonoStructure>
    where TMonoStructure : IMonoStructure
{
    [Title("$Name"), PropertyOrder(-1)]
    [ShowInInspector] public string Name => "Module: " + this.GetType().Name;
    
    
    // ---------- Variables ----------
    public TFacade facade { get; set; }
    

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