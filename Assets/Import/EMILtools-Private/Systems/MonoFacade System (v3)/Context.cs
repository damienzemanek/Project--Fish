using UnityEngine;

namespace EMILtools.Systems
{
    
    public abstract class ContextData<TContextViewImmutable> : IContextViewImmutable
        where TContextViewImmutable : IContextViewImmutable
    {
        public TContextViewImmutable ContextDataImmutableAsInterface
        {
            // Can't use "as" becauseTcontextViewImmutable is an interface, not a class
            get
            {
                Debug.Log(typeof(TContextViewImmutable).Name);
                if (this is TContextViewImmutable) return (TContextViewImmutable)(object)this;
                throw new System.InvalidCastException($"ContextData of type {GetType().Name} cannot be cast to interface {typeof(TContextViewImmutable).Name}");
            }
        }
    
        public ContextData() { }
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
        where TContextData : ContextData<TContextViewImmutable>, IContextViewImmutable, new()
        where TContextViewImmutable : IContextViewImmutable
    {
        public TContextData Data;
        public TContextViewImmutable View => Data.ContextDataImmutableAsInterface;
        public Context() => Data = new TContextData();
    
    
        /// <summary>
        /// Mainly for testing
        /// </summary>
        /// <param name="data"></param>
        public Context(TContextData data) => Data = data;
    }
}





