using System;
using EMILtools.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EMILtools.Systems
{
        
    /// <summary>
    /// Context used in MonoFacade Functionalities
    /// </summary>
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
    public abstract class BoundFunctionality<TFacade, TMonoStructure, TContext> : 
            UnboundFunctionality<TFacade, TMonoStructure, TContext>, 
            IBindable
        where TFacade : class, IFacade<TMonoStructure>
        where TContext : class, IModuleUsabableContext
        where TMonoStructure : IMonoStructure
    {
        [NonSerialized] Publisher<TContext> publisher; // Lazy
        protected BoundFunctionality(Publisher<TContext> publisher, TFacade facade) : base(facade)
            => this.publisher = publisher;
        
        /// <summary>
        /// Binds the EXECUTION PIPELINE to the BOUND ACTION
        /// </summary>
        public virtual void Bind() => publisher.Add(subscriber);
        public virtual void Unbind() => publisher.Remove(subscriber);
    }



    /// <summary>d
    /// Binds a functionality to a PersistentAction<...>
    /// - Set Args with SettableTemplate
    /// - Tracks: isActive
    /// </summary>
    /// <typeparam name="TFacade"></typeparam>
    /// <typeparam name="TContext"></typeparam>
    /// <typeparam name="SettableTemplate"></typeparam>
    public abstract class BoundSetFunctionality<
            TFacade,
            TMonoStructure,
            TContext,
            SettableTemplate> 
            : UnboundFunctionality<TFacade, TMonoStructure, TContext>, 
        IBindable,
        ISettableImplementer
        where TFacade : class, IFacade<TMonoStructure>
        where TMonoStructure : IMonoStructure   
        where TContext : class, IModuleUsabableContext
        where SettableTemplate : class, ISettableTemplate<bool>, new()
    {
        /// <summary>
        /// Alias for Settable.unnamedStoredValue1
        /// </summary>
        [ShowInInspector] protected bool isActive => Settable.data;
        protected SettableTemplate SetContext => Settable;
        [NonSerialized] [ShowInInspector] SettableTemplate Settable;
        protected BoundSetFunctionality(IDelegatorAbstract<ISubscriber> publisher, TFacade facade) : base(facade)
        {
            Settable = new SettableTemplate();
            Settable.Publisher = publisher;
            Debug.Log($"Settable action is : " + Settable.Publisher + $" and template call is : " + Settable.Subscriber + $" for functionality : " + GetType().Name);
        }

        public void Bind()
        {
            Debug.Log("Trying to Bind " + GetType().Name);
            Settable.Publisher.Add(Settable.Subscriber);
            if(this is ON_SET onSet) Settable.OnSet.Add(onSet.OnSet);
            Debug.Log("Bound " + GetType().Name);
        }

        public void Unbind()
        {
            Settable.Publisher.Remove(Settable.Subscriber);
            if(this is ON_SET onSet) Settable.OnSet.Remove(onSet.OnSet);
        }
    }


}