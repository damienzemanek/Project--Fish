using System;
using Sirenix.OdinInspector;
using UnityEngine;


namespace EMILtools.Systems
{
        
    [Serializable]
    public abstract class MonoFacade<TMonoFacade, 
            TFunctionality,  // Systems and functionality
            TConfig,         // Immutable Configuration
            TMonoStructure,  // References + CQRS Context
            TActionMap>      // Internal Action bindings
        
        : MonoBehaviour, IFacade<TMonoStructure>

        where TMonoFacade    : class, IFacade<TMonoStructure>   
        where TConfig        : Config        
        where TMonoStructure : IMonoStructure, new()
        where TFunctionality : Functionalities<TMonoFacade, TMonoStructure>, new()
        where TActionMap     : class, IActionMap, new()
    {
        
        bool initialized = false;

        [field: Title("Action Mappings")]
        [field: ShowInInspector] [field:ReadOnly] [field:HideLabel] [field: NonSerialized] public TActionMap Actions { get; protected set; }
        [field: Title("Config")]
        [field:SerializeField, Required] public TConfig Config { get; private set; }
        [field: Title("Structure")]
        [SerializeField] public TMonoStructure structure;
        [field: Title("Functionality Modules")]
        [field: ShowInInspector] [field:HideLabel] [field: NonSerialized] public TFunctionality Functionality { get; private set; }

        
        public T GetFunctionality<T>() where T : class, IAPI_Module
        {
            if (Functionality.APIs().TryGetValue(typeof(T), out var module)) return module as T;
            if(module == null) Debug.LogWarning("Did not find module of type " + typeof(T));
            return null;
        }

        protected void InitializeFacade(TConfig injectConfig = null)
        {
            if (initialized) return;
            if (injectConfig != null) Config = injectConfig;
            
            structure.Init();
            Actions = new();
            Functionality = new ();
            
            Debug.Assert(Config != null, $"{name}: Config not assigned");
            Debug.Assert(Functionality != null, $"{name}: Functionality did not initialize");

            Functionality.InjectFacadeReference(this);
            Functionality.SetupModules();   // Functionality must be last because it depends on the Config and the Blackboard
            
            initialized = true;
        }
        

        protected virtual void Update()
        {
            if (!initialized) return;
            Functionality.UpdateTick();
        }
        
        protected virtual void FixedUpdate()
        {
            if (!initialized) return;
            Functionality.FixedTick();
        }
        
        protected virtual void LateUpdate()
        {
            if (!initialized) return;
            Functionality.LateTick();
        }
        

        public TMonoStructure API_Structure() => structure;

        public TBlackboardType API_Blackboard<TBlackboardType>() where TBlackboardType : IBlackboard
        {
            if(structure.API_Blackboard is TBlackboardType blackboard) return blackboard; throw new InvalidCastException();
        }

        public TConfigType API_Config<TConfigType>() where TConfigType : IConfig
        {
            if(Config is TConfigType config) return config; throw new InvalidCastException();
        }

        public TFunctionalityType API_Functionality<TFunctionalityType>() where TFunctionalityType : IFunctionality
        {
            if(Functionality is TFunctionalityType functionality) return functionality; throw new InvalidCastException();
        }
    }
}
