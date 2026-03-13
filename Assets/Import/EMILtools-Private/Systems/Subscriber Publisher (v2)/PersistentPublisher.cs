using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;

namespace EMILtools.Systems
{
    public interface IPublisher : IDelegatorAbstract<ISubscriber>
    {
        IReadOnlyList<ISubscriber> Subs { get;}
        public Task PublishImplementation(ISubscriber sub);
        public async Task PublishCore()
        {
            for (int i = 0; i < Subs.Count; i++)
            {
                var sub = Subs[i];

                if (!sub.isActive)
                    continue;

                 await PublishImplementation(sub);
            }
        }
    }
    
    public class Publisher :
        IPublisher
    {
        public IReadOnlyList<ISubscriber> Subs => subs;

        readonly List<ISubscriber> subs = new();

        // ---------- Typed Generic --------------
        public ISubscriber Add(ISubscriber cb)
        {
            Debug.Log("cb type: " + cb.GetType().Name + "");
            Debug.Log("sub type: " + typeof(ISubscriber).Name + "");
            subs.Add(cb); 
            Debug.Log("Add complete");
            return cb;
        }
        public ISubscriber Remove(ISubscriber cb) { subs.Remove(cb); return cb; }

        public Task Publish() => ((IPublisher)this).PublishCore();
        
        public Task PublishImplementation(ISubscriber sub)
        {
            var subTyped = sub;
            return subTyped.Execute();
        }        
    }

    
    public class Publisher<TContext> :
        IDelegatorAbstract<ISubscriber<TContext>>,
        IPublisher
    {
        public IReadOnlyList<ISubscriber> Subs => subs;

        readonly List<ISubscriber<TContext>> subs = new();

        TContext cachedContext;

        // ---------- Typed Generic --------------
        public ISubscriber<TContext> Add(ISubscriber<TContext> cb)
        {
            Debug.Log("cb type: " + cb.GetType().Name + "");
            Debug.Log("sub type: " + typeof(ISubscriber<TContext>).Name + "");
            subs.Add(cb); 
            Debug.Log("Add complete");
            return cb;
        }
        public ISubscriber<TContext> Remove(ISubscriber<TContext> cb) { subs.Remove(cb); return cb; }

        public ISubscriber Add(ISubscriber cb)
        {
            Debug.Log("cb type: " + cb.GetType().Name + "");
            Debug.Log("sub type: " + typeof(ISubscriber).Name + "");
            var ret = ((IDelegatorAbstract<ISubscriber<TContext>>)this).Add((ISubscriber<TContext>)cb);
            Debug.Log("Add complete");
            return ret;
            
        }
        public ISubscriber Remove(ISubscriber cb) => ((IDelegatorAbstract<ISubscriber<TContext>>)this).Remove((ISubscriber<TContext>)cb);
        public Task PublishImplementation(ISubscriber sub)
        {
            var subTyped = sub as ISubscriber<TContext>;
            return subTyped.Execute(cachedContext);
        }        
        public Task Publish(TContext ctx)
        {
            cachedContext = ctx;
            return ((IPublisher)this).PublishCore();
        }
        
        public Task Publish()
        {
            if(typeof(TContext) != typeof(VoidCtx)) throw new InvalidOperationException("Cannot publish with VoidCtx");
            cachedContext = default;
            return ((IPublisher)this).PublishCore();
        }
        
    }
    
    public class Publisher<T1, T2> : 
        IDelegatorAbstract<ISubscriber<T1, T2>>,
        IPublisher
    {
        IReadOnlyList<ISubscriber> IPublisher.Subs => subs;
        readonly List<ISubscriber<T1, T2>> subs = new();
        
        T1 cachedContext1;
        T2 cachedContext2;

        public ISubscriber<T1, T2> Add(ISubscriber<T1, T2> cb)
        {
            Debug.Log("cb type: " + cb.GetType().Name + "");
            Debug.Log("sub type: " + typeof(ISubscriber<T1, T2>).Name + "");
            subs.Add(cb); 
            Debug.Log("Add complete");
            return cb;
        }

        public ISubscriber<T1, T2> Remove(ISubscriber<T1, T2> cb)
        { subs.Remove(cb); return cb; }

        public ISubscriber Add(ISubscriber cb)
        {
            Debug.Log("cb type: " + cb.GetType().Name + "");
            Debug.Log("sub type: " + typeof(ISubscriber).Name + "");
            ((IDelegatorAbstract<ISubscriber<T1, T2>>)this).Add((ISubscriber<T1, T2>)cb);
            Debug.Log("Add complete");
            return cb;
        }
            

        public ISubscriber Remove(ISubscriber cb) =>
            ((IDelegatorAbstract<ISubscriber<T1, T2>>)this).Remove((ISubscriber<T1, T2>)cb);
        
        public Task PublishImplementation(ISubscriber sub)
        {
            var subTyped = sub as ISubscriber<T1, T2>;
            return subTyped.Execute(cachedContext1, cachedContext2);
        }
        public Task Publish(T1 ctx1, T2 ctx2)
        {
            cachedContext1 = ctx1;
            cachedContext2 = ctx2;
            return ((IPublisher)this).PublishCore();
        }

    }
}
