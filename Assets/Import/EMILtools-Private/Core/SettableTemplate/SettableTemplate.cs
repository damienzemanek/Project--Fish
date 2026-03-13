using System;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;
using static EMILtools.Systems.SubscriberExecutor;




/// <summary>
/// SettableTemplates (Setters) handle what happens when values are set
/// They solve the problem of needing to do repetitive templating and work generically
///
/// Example of Repetitive Templating:
/// ---------------------------------
/// void Set(T inputVal)
/// {
///     storedVal = inputVal;
///     OnSet();
/// }
///
/// void OnSet();
/// ---------------------------------
/// 
/// Ensure you set aliases for values they are generic storage only
/// </summary>
/// <typeparam name="T1"></typeparam>
public abstract class SettableTemplate<T1> : 
    ISettableTemplate<T1>
{
    // Values
    [field: NonSerialized] T1 ISettableTemplate<T1>.data { get; set; }
    
    // Template Call 
    public Subscriber<Action<T1>, ActionContextResolver<T1>, T1> templateCallSubscriber;
    public ISubscriber Subscriber => templateCallSubscriber;
    public PersistentAction OnSet { get; set; } = new();
    void _TemplateCall(T1 val)
    {
        ((ISettableTemplate<T1>)this).data = val;
        Set(val);
        OnSet.Invoke();
    }
    
    // Ctor
    protected SettableTemplate()
        => templateCallSubscriber = new Subscriber<Action<T1>, ActionContextResolver<T1>, T1>(_TemplateCall);
    
    // Action
    [NonSerialized] Publisher<T1> _publisher;
    public IDelegatorAbstract<ISubscriber> Publisher
    {
        get => _publisher;
        set => _publisher = (Publisher<T1>)value;
    }


    // Abstract
    protected virtual void Set(T1 val) { }
    public T1 dataSlot1 => ((ISettableTemplate<T1>)this).data;

}

/// <summary>
/// Ensure you set aliases for values they are for generic storage only
/// </summary>
/// <typeparam name="T1"></typeparam>
/// <typeparam name="T2"></typeparam>
public abstract class SettableTemplate<T1, T2> : 
    ISettableTemplate<SettableTemplate<T1, T2>.Values>,
    ISettableTemplate<T1>
{
    public struct Values : IContext
    {
        public T1 dataSlot1;
        public T2 dataSlot2;
        
        public void SetSlot1(T1 val) => dataSlot1 = val;
        public void SetSlot2(T2 val) => dataSlot2 = val;
    }
    
    // Values
    public Values data { get; set; }
    
    // Template Call
    public Subscriber<Action<Values>, ActionContextResolver<Values>, Values> templateCallSubscriber;
    public ISubscriber Subscriber => templateCallSubscriber;
    void _TemplateCall(Values values)
    {
        data.SetSlot1(values.dataSlot1);
        data.SetSlot2(values.dataSlot2);
        Set(values.dataSlot1, values.dataSlot2);
        OnSet.Invoke();
    }
    
    // Ctor
    public SettableTemplate()
        => templateCallSubscriber = new Subscriber<Action<Values>, ActionContextResolver<Values>, Values>(_TemplateCall);
    public PersistentAction OnSet { get; set; } = new();

    public IDelegatorAbstract<ISubscriber> Publisher
    {
        get => _publisher; 
        set => _publisher = (Publisher<T1, T2>)value;
    }
    
    // Abstract
    protected virtual void Set(T1 val1, T2 val2) { }
    public T1 dataSlot1 => data.dataSlot1;

    // Action
    [NonSerialized] Publisher<T1, T2> _publisher;
    
    
    
    // T1 Abstracts
    T1 ISettableTemplate<T1>.data
    {
        get => data.dataSlot1;
        set => data.SetSlot1(value);
    }
    T1 _data => data.dataSlot1;

}

