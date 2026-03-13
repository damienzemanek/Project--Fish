using System;
using EMILtools.Core;


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
    public abstract class DataSetter<T1> : 
        IDataSetter<T1>
    {
        // Values
        [field: NonSerialized] T1 IDataSetter<T1>.data { get; set; }
        
        // Template Call 
        readonly SubResolvableCtx<T1> templateCallSubscriber;
        public ISubscriber Subscriber => templateCallSubscriber;
        public PersistentAction OnSet { get; set; } = new();
        bool _TemplateCall(T1 val)
        {
            ((IDataSetter<T1>)this).data = val;
            Set(val);
            OnSet.Invoke();
            return false;
        }
        
        // Ctor
        protected DataSetter()
            => templateCallSubscriber = new SubResolvableCtx<T1>(_TemplateCall);
        
        // Action
        [NonSerialized] Publisher<T1> _publisher;
        public IPublisher Publisher
        {
            get => _publisher;
            set
            {
                if (value is Publisher<T1> typedValue) _publisher = typedValue;
                else throw new ArgumentException($"Value must be of type Publisher<T1> it is currently ({value.GetType()})");
            }
        }


        // Abstract
        protected virtual void Set(T1 val) { }
        public T1 dataSlot1 => ((IDataSetter<T1>)this).data;
        
    }
        
    /// <summary>
    /// Ensure you set aliases for values they are for generic storage only
    /// </summary>
    /// <typeparam name="T1"></typeparam>
    /// <typeparam name="T2"></typeparam>
    public abstract class DataSetter<T1, T2> : 
        IDataSetter<T1>
    {
        
        // Values
        T1 data1 { get; set; }
        protected T2 data2 { get; set; }
        
        // Template Call
        readonly SubResolvableCtx<(T1, T2)> templateCallSubscriber;
        public ISubscriber Subscriber => templateCallSubscriber;
        bool _TemplateCall((T1 ctx1, T2 ctx2) data)
        {
            data1 = data.ctx1;
            data2 = data.ctx2;
            Set(data.ctx1, data.ctx2);
            OnSet.Invoke();
            //Debug.Log("SETTER TEMPLATE CALL CALLED");
            return false;
        }
        
        // Ctor
        protected DataSetter()
            => templateCallSubscriber = new SubResolvableCtx<(T1, T2)>(_TemplateCall);
        public PersistentAction OnSet { get; set; } = new();

        public IPublisher Publisher
        {
            get => _publisher;
            set
            {
                if (value is Publisher<T1, T2> typedValue) _publisher = typedValue;
                else throw new ArgumentException($"Value must be of type Publisher<T1, T2> it is currently ({value.GetType()})");
            }
        }
        
        // Abstract
        protected virtual void Set(T1 val1, T2 val2) { }
        public T1 dataSlot1 => data1;

        // Action
        [NonSerialized] Publisher<T1, T2> _publisher;
        
        
        
        // T1 Abstracts
        T1 IDataSetter<T1>.data
        {
            get => data1;
            set => data1 = value;
        }
        T1 _data => data1;

    }
}


