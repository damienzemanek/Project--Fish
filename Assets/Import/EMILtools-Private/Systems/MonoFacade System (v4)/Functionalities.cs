using System;
using System.Collections;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using static InterfaceEX;


namespace EMILtools.Systems
{
    
    public interface IFunctionality
    {
        public Dictionary<Type, IMonoFunctionalityModule> APIs { get; }
        public void InjectFacadeReference(IFacade f);
        public void SetupModules();
        public void UpdateTick();
        public void FixedTick();
        public void LateTick();
    }
    
    public abstract class Functionalities<TMonoFacade, TContext> : IFunctionality
        where TMonoFacade : class, IFacade
        where TContext : class, IModuleUsabableContext
    {
        [field: NonSerialized] protected TMonoFacade facade { get; private set; }

        
        /// <summary>
        /// Later: Dictionary of Type and List of Modules for multi-service subscription
        /// </summary>
        readonly Dictionary<Type, IMonoFunctionalityModule> API_Modules = new();
        public Dictionary<Type, IMonoFunctionalityModule> APIs => API_Modules;
        [ShowInInspector] readonly List<IMonoFunctionalityModule> modules = new();

        readonly Publisher<TContext> updateTick = new();
        readonly Publisher<TContext> fixedTick = new();
        readonly Publisher<TContext> lateTick = new();

        public void InjectFacadeReference(IFacade f)
        {
            if(f is TMonoFacade typedFacade) facade = typedFacade;
            else throw new InvalidCastException($"Facade type mismatch. Expected {typeof(TMonoFacade)}, got {f.GetType()}");
        }
        
        public void SetupModules()
        {
            AddModulesHere();
            foreach (var t in modules)  t.SetupModule();
            Debug.Log($"{GetType().Name} Functionality modules successfully setup | API Count: " + API_Modules.Count);
        }

        public void Bind()
        {
            Debug.Log("binding in progress...");
            foreach (var t in modules)
                if(t is IBindable bindable) bindable.Bind();
        }

        public void Unbind()
        {
            foreach (var t in modules)
                if(t is IBindable bindable) bindable.Unbind();
        }
        public void UpdateTick() => updateTick.Publish(facade.API_Context<TContext>()).Forget("UpdateTick");
        public void FixedTick() => fixedTick.Publish(facade.API_Context<TContext>()).Forget("FixedTick");
        public void LateTick() => lateTick.Publish(facade.API_Context<TContext>()).Forget("LateTick");
        protected void AddModule(MonoFunctionalityModule<TMonoFacade> module)
        {
            modules.Add(module);
            Debug.Log("ADDING module " + module.GetType().Name + " new count is " + modules.Count);

            if (typeof(UPDATE).IsAssignableFrom(module.GetType())) updateTick.Add(module.Subscriber);
            if (typeof(FIXED_UPDATE).IsAssignableFrom(module.GetType())) fixedTick.Add(module.Subscriber);
            if (typeof(LATE_UPDATE).IsAssignableFrom(module.GetType())) lateTick.Add(module.Subscriber);
            if (typeof(IAPI_Module).IsAssignableFrom(module.GetType()))
            {
                foreach (var iface in GetInterfacesAssignableTo<IAPI_Module>(module.GetType()))
                {
                    if (!API_Modules.TryAdd(iface, module)) throw new InvalidOperationException($"API interface {iface.Name} already registered.");
                    Debug.Log("Added interface to modules API dictionary, new API add is : " + iface + " new count is " + API_Modules.Count);
                }                
            }
        }
        
        protected abstract void AddModulesHere();

    }
}