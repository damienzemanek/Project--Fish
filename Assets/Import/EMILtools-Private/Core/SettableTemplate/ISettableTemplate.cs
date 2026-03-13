using System;
using EMILtools.Core;
using EMILtools.Systems;
using static EMILtools.Systems.SubscriberExecutor;


/// <summary>
/// Settables manage generic state using Template Method Pattern
/// </summary>
/// <typeparam name="T1"></typeparam>
public interface ISettableTemplate<T1>
{
    public IDelegatorAbstract<ISubscriber> Publisher { get; set;  }
    public ISubscriber Subscriber { get; }
    public PersistentAction OnSet { get; }
    
    /// <summary>
    /// All SettableTMP's will have at least 1 unnamedStoredValue1, meaning it is accessible from the interface
    /// </summary>
    public T1 data { get; set; }
}

