using System;
using Sirenix.OdinInspector;
using UnityEngine;

namespace EMILtools.Systems
{
    public interface IMonoStructure
    {
        public void Init();
        public IBlackboard API_Blackboard { get; }
        public IContextViewImmutable API_Context { get; }
    }

    [HideLabel]
    [Serializable]
    public abstract class MonoStructure<TBlackboard, TContextData, TContextViewImmutable> : IMonoStructure
        where TBlackboard : class, IBlackboard
        where TContextData : ContextData, TContextViewImmutable, new()
        where TContextViewImmutable : IContextViewImmutable
    {
        [Title("Blackboard")] [SerializeField] [HideLabel]
        public TBlackboard Blackboard;
    
        [Title("Context")] [field: NonSerialized] [field: HideLabel]
        public Context<TContextData, TContextViewImmutable> Context { get; internal set; }


        /// <summary>
        /// Default initialization
        /// </summary>
        public void Init()
        {
            if(Blackboard == null) Debug.LogError($"Blackboard not assigned in {GetType().Name}");
            Context = new Context<TContextData, TContextViewImmutable>(Blackboard);
        }
        
        public IBlackboard API_Blackboard => Blackboard;
        public IContextViewImmutable API_Context => Context.View;
    }
}


