using System;
using UnityEngine;

namespace EMILtools.Systems
{
    /// <summary>
    /// Override this to add data to the context
    /// </summary>
    /// <typeparam name="TBlackboard"></typeparam>
    public abstract class ContextData<TBlackboard> : IContextViewImmutable
        where TBlackboard : IBlackboard
    {
        internal TBlackboard Blackboard;
        protected ContextData() { }
    }

    /// <summary>
    /// CQRS (Command Query Responsibility Segregation) principle applied at the type level
    /// + Writes go through concrete classes
    /// + Reads go through interfaces
    ///
    /// Intended Design: High Throughput (For Pipeline)
    /// </summary>
    /// <typeparam name="TContextData"></typeparam>
    /// <typeparam name="TContextViewImmutable"></typeparam>
    public class Context<TContextData, TBlackboard, TContextViewImmutable> : IContext
        where TContextData : ContextData<TBlackboard>, TContextViewImmutable, IContextViewImmutable, new()
        where TContextViewImmutable : IContextViewImmutable
        where TBlackboard : IBlackboard
    {
        
        /// <summary>
        /// Data is mutated where it's visible (public is OK)
        /// </summary>
        public TContextData Data;

        /// <summary>
        /// This will be passed around to scripts that depend on it
        /// </summary>
        public readonly TContextViewImmutable View;

        public Context(TBlackboard blackboard)
        {
            Data = new TContextData() { Blackboard = blackboard };
            View = Data;
        }
    }
}





