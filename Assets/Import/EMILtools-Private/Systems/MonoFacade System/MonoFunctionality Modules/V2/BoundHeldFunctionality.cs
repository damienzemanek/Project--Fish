using System;
using EMILtools.Core;
using Sirenix.OdinInspector;
using UnityEngine;

public interface IModuleUsabableContext : IPipelineContext { }
 
public interface IBindable
{
    public void Bind();
    public void Unbind();
}

public interface ISettableImplementer { }

/// <summary>
/// Binds a functionality to a PersistentAction
/// - Only to be used with PersistentAction (No Args)
/// - Use TContext to pass around information
/// </summary>
/// <typeparam name="TFacade"></typeparam>
/// <typeparam name="TContext"></typeparam>
public abstract class BoundFunctionality<TFacade, TContext> : 
        UnboundFunctionality<TFacade, TContext>, 
        IBindable
    where TFacade : class, IFacade<TContext>
    where TContext : struct, IModuleUsabableContext
{
    [NonSerialized] PersistentAction action = new();
    protected BoundFunctionality(PersistentAction action, TFacade facade) : base(facade)
     => this.action = action;
    
    /// <summary>
    /// Binds the EXECUTION PIPELINE to the BOUND ACTION
    /// </summary>
    public virtual void Bind() => action.Add(Execute);
    public virtual void Unbind() => action.Remove(Execute);
}



/// <summary>
/// Binds a functionality to a PersistentAction<...>
/// - Set Args with SettableTemplate
/// - Tracks: isActive
/// </summary>
/// <typeparam name="TFacade"></typeparam>
/// <typeparam name="TContext"></typeparam>
/// <typeparam name="SettableTemplate"></typeparam>
public abstract class BoundSetFunctionality<
        TFacade,
        TContext,
        SettableTemplate> 
        : UnboundFunctionality<TFacade, TContext>, 
    IBindable,
    ISettableImplementer
    where TFacade : class, IFacade<TContext>
    where TContext : struct, IModuleUsabableContext
    where SettableTemplate : class, ISettableTemplate<bool>, new()
{
    /// <summary>
    /// Alias for Settable.unnamedStoredValue1
    /// </summary>
    [ShowInInspector] protected bool isActive => Settable._unnamedStoredValue1;
    protected SettableTemplate SetContext => Settable;
    [NonSerialized] [ShowInInspector] SettableTemplate Settable;
    protected BoundSetFunctionality(IPersistentDelegate _action, TFacade facade) : base(facade)
    {
        Settable = new SettableTemplate();
        Settable.action = _action;
        Debug.Log($"Settable action is : " + Settable.action + $" and template call is : " + Settable.TemplateCall + $" for functionality : " + GetType().Name);
    }

    public void Bind()
    {
        Settable.action.Add(Settable.TemplateCall);
        if(this is ON_SET onSet) Settable.OnSet.Add(onSet.OnSet);
    }

    public void Unbind()
    {
        Settable.action.Remove(Settable.TemplateCall);
        if(this is ON_SET onSet) Settable.OnSet.Remove(onSet.OnSet);
    }
}

