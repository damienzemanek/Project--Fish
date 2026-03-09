using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using static InterfaceEX;


namespace EMILtools.Systems
{
    public abstract class Functionalities<TMonoFacade, TMonoStructure> : IFunctionality
        where TMonoFacade : class, IFacade<TMonoStructure>
        where TMonoStructure : IMonoStructure
    {
        /// <summary>
        /// Later: Dictionary of Type and List of Modules for multi-service subscription
        /// </summary>
        readonly Dictionary<Type, MonoFunctionalityModule<TMonoFacade, TMonoStructure>> API_Modules = new();
        [field: NonSerialized] public TMonoFacade facade { get; private set; }
        
        [ShowInInspector] List<MonoFunctionalityModule<TMonoFacade, TMonoStructure>> modules; 
        List<UPDATE> _update = new();
        List<FIXED_UPDATE> _fixed = new();
        List<LATE_UPDATE> _late = new();
        public Functionalities() => modules = new List<MonoFunctionalityModule<TMonoFacade, TMonoStructure>>();
        
        public void InjectFacadeReference(IFacade<TMonoStructure> f) => facade = f as TMonoFacade;
        public void SetupModules()
        {
            AddModulesHere();
            foreach (var t in modules)  t.SetupModule();
            Debug.Log($"{GetType().Name} Functionality modules successfully setup | API Count: " + API_Modules.Count);
        }

        public void Bind()
        {
            foreach (var t in modules)
                if(t is IBindable bindable) bindable.Bind();
        }

        public void Unbind()
        {
            foreach (var t in modules)
                if(t is IBindable bindable) bindable.Unbind();
            
        }
        public void UpdateTick() { foreach (var t in _update) t.OnUpdateTick(); }
        public void FixedTick() { foreach (var t in _fixed) { t.OnFixedTick(); } }
        public void LateTick() { foreach (var t in _late) t.OnLateTick(); }
        
        
        public void AddModule(MonoFunctionalityModule<TMonoFacade, TMonoStructure> module)
        {
            modules.Add(module);
            Debug.Log("ADDING module " + module.GetType().Name + " new count is " + modules.Count);

            if (module is UPDATE u) { _update.Add(u); }
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

        public Dictionary<Type, MonoFunctionalityModule<TMonoFacade, TMonoStructure>> APIs() => API_Modules;


        protected abstract void AddModulesHere();
    }
}