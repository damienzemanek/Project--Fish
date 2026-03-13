using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using static InterfaceEX;


namespace EMILtools.Systems
{
    public abstract class Functionalities<TMonoFacade, TContext> : IFunctionality
        where TMonoFacade : class, IFacade
        where TContext : class, IModuleUsabableContext
    {
        /// <summary>
        /// Later: Dictionary of Type and List of Modules for multi-service subscription
        /// </summary>
        readonly Dictionary<Type, MonoFunctionalityModule<TMonoFacade, TContext>> API_Modules = new();
        [field: NonSerialized] public TMonoFacade facade { get; private set; }
        
        [ShowInInspector] List<MonoFunctionalityModule<TMonoFacade, TContext>> modules;
        Publisher<TContext> updateTick = new();
        Publisher<TContext> fixedTick = new();
        Publisher<TContext> lateTick = new();
        public Functionalities() => modules = new List<MonoFunctionalityModule<TMonoFacade, TContext>>();
        
        public void InjectFacadeReference(IFacade f) => facade = f as TMonoFacade;
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
        public void UpdateTick() => updateTick.Publish((TContext)facade.API_Context<TContext>());
        public void FixedTick() => fixedTick.Publish((TContext)facade.API_Context<TContext>());
        public void LateTick() => lateTick.Publish((TContext)facade.API_Context<TContext>());
        
        
        public void AddModule(MonoFunctionalityModule<TMonoFacade, TContext> module)
        {
            modules.Add(module);
            Debug.Log("ADDING module " + module.GetType().Name + " new count is " + modules.Count);

            if (typeof(UPDATE).IsAssignableFrom(module.GetType()))
            {
                updateTick.Add(module.);
            }
            if (module is FIXED_UPDATE f) _fixed.Add(f);
            if (module is LATE_UPDATE l) _late.Add(l);
            if (module is IAPI_Module)
            {
                foreach (var iface in GetInterfacesAssignableTo<IAPI_Module>(module.GetType()))
                {
                    if (!API_Modules.TryAdd(iface, module)) throw new InvalidOperationException($"API interface {iface.Name} already registered.");
                    else Debug.Log("Added interface to modules API dictionary, new API add is : " + iface + " new count is " + API_Modules.Count);
                }                
            }
        }

        public Dictionary<Type, MonoFunctionalityModule<TMonoFacade, TContext>> APIs() => API_Modules;


        protected abstract void AddModulesHere();
    }
}