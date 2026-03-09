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

    [Serializable]
    public abstract class MonoStructure<TBlackboard, TContextData, TContextViewImmutable> : IMonoStructure
        where TBlackboard : IBlackboard
        where TContextData : ContextData, TContextViewImmutable, new()
        where TContextViewImmutable : IContextViewImmutable
    {
        [field: Title("Blackboard")]
        [SerializeField] public TBlackboard Blackboard;
    
        [field: Title("Context")]
        [NonSerialized] public Context<TContextData, TContextViewImmutable> Context;
    
        
        /// <summary>
        /// Default initialization
        /// </summary>
        public void Init() => Context = new Context<TContextData, TContextViewImmutable>();
    
    
        /// <summary>
        /// Mainly for testing, but also for flexibility in case we want to initialize the structure with a specific context (e.g. for pooling)
        /// </summary>
        /// <param name="_context"></param>
        public void Init(TContextData _context) => Context = new Context<TContextData, TContextViewImmutable>(_context);
    
        public IBlackboard API_Blackboard => Blackboard;
        public IContextViewImmutable API_Context => Context.View;
    }
}


