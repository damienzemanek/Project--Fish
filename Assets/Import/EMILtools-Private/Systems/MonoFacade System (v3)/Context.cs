using System;
using UnityEngine;

namespace EMILtools.Systems
{
    
    public abstract class ContextData : IContextViewImmutable
    {
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
    public class Context<TContextData, TContextViewImmutable> : IContext
        where TContextData : ContextData, TContextViewImmutable, IContextViewImmutable, new()
        where TContextViewImmutable : IContextViewImmutable
    {
        
        /// <summary>
        /// Data is mutated where it's visible (public is OK)
        /// </summary>
        public TContextData Data;

        /// <summary>
        /// This will be passed around to scripts that depend on it
        /// </summary>
        public TContextViewImmutable View => Data;

        public Context() => Data = new TContextData();

        public Context(TContextData data) => Data = data;
    }
}





