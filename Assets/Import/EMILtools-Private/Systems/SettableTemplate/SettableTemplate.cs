using System;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;
using static EMILtools.Systems.SubscriberExecutor;



namespace EMILtools.Systems
{
    
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
        public SubscriberCtx<Action<T1>, ActionResolverCtx<T1>, T1> templateCallSubscriber;
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
            => templateCallSubscriber = new SubscriberCtx<Action<T1>, ActionResolverCtx<T1>, T1>(_TemplateCall);
        
        // Action
        [NonSerialized] Publisher<T1> _publisher;
        public IPublisher Publisher
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
        ISettableTemplate<T1>
    {
        
        // Values
        public T1 data1 { get; set; }
        public T2 data2 { get; set; }
        
        // Template Call
        public SubscriberCtx<Action<T1, T2>, ActionResolverCtx<T1, T2>, T1, T2> templateCallSubscriber;
        public ISubscriber Subscriber => templateCallSubscriber;
        void _TemplateCall(T1 ctx1, T2 ctx2)
        {
            data1 = ctx1;
            data2 = ctx2;
            Set(ctx1, ctx2);
            OnSet.Invoke();
            //Debug.Log("SETTER TEMPLATE CALL CALLED");
        }
        
        // Ctor
        public SettableTemplate()
            => templateCallSubscriber = new SubscriberCtx<Action<T1, T2>, ActionResolverCtx<T1, T2>, T1, T2>(_TemplateCall);
        public PersistentAction OnSet { get; set; } = new();

        public IPublisher Publisher
        {
            get => _publisher; 
            set => _publisher = (Publisher<T1, T2>)value;
        }
        
        // Abstract
        protected virtual void Set(T1 val1, T2 val2) { }
        public T1 dataSlot1 => data1;

        // Action
        [NonSerialized] Publisher<T1, T2> _publisher;
        
        
        
        // T1 Abstracts
        T1 ISettableTemplate<T1>.data
        {
            get => data1;
            set => data1 = value;
        }
        T1 _data => data1;

    }
}


