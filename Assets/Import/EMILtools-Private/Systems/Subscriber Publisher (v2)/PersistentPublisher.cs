using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;

namespace EMILtools.Systems
{
    public interface IPublisher<TSubType> : IDelegatorAbstract<ISubscriber>
        where TSubType : ISubscriber
    {
        List<TSubType> Subs { get; set; }
        public Task PublishImplementation(TSubType sub);
        public async Task PublishCore()
        {
            for (int i = 0; i < Subs.Count; i++)
            {
                var sub = Subs[i];

                if (!sub.isActive)
                    continue;

                 await PublishImplementation((TSubType)sub);
            }
        }
    }
    
    public class Publisher<TContext> :
        IDelegatorAbstract<ISubscriber<TContext>>,
        IPublisher<ISubscriber<TContext>>
    {
        public List<ISubscriber<TContext>> Subs { get; set; } = new();

        TContext cachedContext;

        // ---------- Generic --------------
        public ISubscriber<TContext> Add(ISubscriber<TContext> cb) 
        { ((IPublisher<ISubscriber<TContext>>)this).Subs.Add(cb); return cb; }

        public ISubscriber<TContext> Remove(ISubscriber<TContext> cb)
        { ((IPublisher<ISubscriber<TContext>>)this).Subs.Remove(cb); return cb; }

        public ISubscriber Add(ISubscriber cb) => ((IDelegatorAbstract<ISubscriber<TContext>>)this).Add((ISubscriber<TContext>)cb);
        public ISubscriber Remove(ISubscriber cb) => ((IDelegatorAbstract<ISubscriber<TContext>>)this).Remove((ISubscriber<TContext>)cb);
        public Task PublishImplementation(ISubscriber<TContext> sub) => sub.Execute(cachedContext);
        public Task Publish(TContext ctx)
        {
            cachedContext = ctx;
            return ((IPublisher<ISubscriber<TContext>>)this).PublishCore();
        }
        
        public Task Publish()
        {
            if(typeof(TContext) != typeof(VoidCtx)) throw new InvalidOperationException("Cannot publish with VoidCtx");
            cachedContext = default;
            return ((IPublisher<ISubscriber<TContext>>)this).PublishCore();
        }
    }
    
    public class Publisher<T1, T2> : 
        IDelegatorAbstract<ISubscriber<T1, T2>>,
        IPublisher<ISubscriber<T1, T2>>
    {
        public List<ISubscriber<T1, T2>> Subs { get; set; } = new();
        
        T1 cachedContext1;
        T2 cachedContext2;
        
        public ISubscriber<T1, T2> Add(ISubscriber<T1, T2> cb)
        { ((IPublisher<ISubscriber<T1, T2>>)this).Subs.Add(cb); return cb; }

        public ISubscriber<T1, T2> Remove(ISubscriber<T1, T2> cb)
        { ((IPublisher<ISubscriber<T1, T2>>)this).Subs.Remove(cb); return cb; }
        
        public ISubscriber Add(ISubscriber cb) =>
            ((IDelegatorAbstract<ISubscriber<T1, T2>>)this).Add((ISubscriber<T1, T2>)cb);

        public ISubscriber Remove(ISubscriber cb) =>
            ((IDelegatorAbstract<ISubscriber<T1, T2>>)this).Remove((ISubscriber<T1, T2>)cb);
        
        public Task PublishImplementation(ISubscriber<T1, T2> sub)
        => sub.Execute(cachedContext1, cachedContext2);
        public Task Publish(T1 ctx1, T2 ctx2)
        {
            cachedContext1 = ctx1;
            cachedContext2 = ctx2;
            return ((IPublisher<ISubscriber<T1, T2>>)this).PublishCore();
        }
    }
}
