using EMILtools.Core;

namespace EMILtools.Systems
{
    
    /// <summary>
    /// Settables manage generic state using Template Method Pattern
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    public interface IDataSetter<T1>
    {
        public IPublisher Publisher { get; set;  }
        public ISubscriber Subscriber { get; }
        public PersistentAction OnSet { get; }
    
        /// <summary>
        /// All SettableTMP's will have at least 1 unnamedStoredValue1, meaning it is accessible from the interface
        /// </summary>
        public T1 data { get; set; }
    }


}



