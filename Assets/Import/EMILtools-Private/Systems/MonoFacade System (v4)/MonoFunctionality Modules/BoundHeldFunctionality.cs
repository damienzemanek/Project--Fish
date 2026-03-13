using System;
using EMILtools.Core;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EMILtools.Systems
{

    /// <summary>
    /// Context used in MonoFacade Functionalities
    /// </summary>
    public interface IModuleUsabableContext : IPipelineContext
    {
        
    }
     
    public interface IBindable
    {
        public void Bind();
        public void Unbind();
    }
    
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
        where TFacade : class, IFacade
        where TContext : class, IModuleUsabableContext
    {
        [NonSerialized] readonly Publisher<TContext> publisher; // Lazy (Injected)
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
            TContext,
            SettableTemplate> 
            : UnboundFunctionality<TFacade, TContext>, 
        IBindable
        where TFacade : class, IFacade
        where TContext : class, IModuleUsabableContext
        where SettableTemplate : class, IDataSetter<bool>, new()
    {
        /// <summary>
        /// Alias for Settable.unnamedStoredValue1
        /// </summary>
        [ShowInInspector] protected bool isActive => Settable.data;
        protected SettableTemplate SetContext => Settable;
        [NonSerialized] [ShowInInspector] readonly SettableTemplate Settable;
        protected BoundSetFunctionality(IPublisher publisher, TFacade facade) : base(facade)
        {
            Settable = new SettableTemplate();
            Settable.Publisher = publisher;
            Debug.Log("Succesfully cached publisher : " + publisher.GetType().Name + " which is " + publisher.GetType());
           // Debug.Log($"Settable action is : " + Settable.Publisher + $" and template call is : " + Settable.Subscriber + $" for functionality : " + GetType().Name);
        }

        public void Bind()
        {
            Debug.Log("Trying to Bind (" + GetType().Name + ")");
            Settable.Publisher.Add(Settable.Subscriber);
            if(this is ON_SET onSet) Settable.OnSet.Add(onSet.OnSet);
            Debug.Log("Bound(" + GetType().Name + ")");
        }

        public void Unbind()
        {
            Settable.Publisher.Remove(Settable.Subscriber);
            if(this is ON_SET onSet) Settable.OnSet.Remove(onSet.OnSet);
        }
    }


}