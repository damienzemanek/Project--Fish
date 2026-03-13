using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMILtools.Core;
using FlyingWormConsole3.LiteNetLibEditor.Utils;

namespace EMILtools.Systems
{
    public interface IPublisher : IDelegatorAbstract<ISubscriber>
    {
        public IReadOnlyList<ISubscriber> API_Snapshot { get; }
        public Task PublishImplementation(ISubscriber sub);
        public abstract void Snapshot();
        public async Task PublishCore()
        {
            Snapshot();
            for (int i = 0; i < API_Snapshot.Count; i++)
            {
                var sub = API_Snapshot[i];
                if (!sub.isActive) continue;
                await PublishImplementation(sub);
            }
        }
    }
    
    public class Publisher :
        IPublisher
    {
        // ISubscriber directly inherits from ISubscriber<TContext> and ISubscriber<T1, T2>

        [NonSerialized] readonly List<ISubscriber> subs = new();
        [NonSerialized] readonly List<ISubscriber> snapshot = new();
        public IReadOnlyList<ISubscriber> API_Snapshot => snapshot;
        // ---------- Typed Generic --------------
        public ISubscriber Add(ISubscriber cb) { subs.Add(cb); return cb; }
        public ISubscriber Remove(ISubscriber cb) { subs.Remove(cb); return cb; }
        public Task Publish() => ((IPublisher)this).PublishCore();
        public Task PublishImplementation(ISubscriber sub) => sub.Execute();
        
        // API
        public void Snapshot()
        {
            snapshot.Clear();
            snapshot.AddRange(subs);
        }
    }

    
    public class Publisher<TContext> :
        IDelegatorAbstract<ISubscriber<TContext>>,
        IPublisher
    {
        // ISubscriber directly inherits from ISubscriber<TContext> and ISubscriber<T1, T2>
        [NonSerialized] readonly List<ISubscriber<TContext>> subs = new();
        [NonSerialized] readonly List<ISubscriber> snapshot = new();
        public IReadOnlyList<ISubscriber> API_Snapshot => snapshot;

        [NonSerialized] TContext cachedContext;

        // ---------- Typed Generic --------------
        public ISubscriber<TContext> Add(ISubscriber<TContext> cb) { subs.Add(cb); return cb; }
        public ISubscriber<TContext> Remove(ISubscriber<TContext> cb) { subs.Remove(cb); return cb; }

        public ISubscriber Add(ISubscriber cb)
        {
            if(cb is ISubscriber<TContext> typedCB) return ((IDelegatorAbstract<ISubscriber<TContext>>)this).Add(typedCB);
            throw new InvalidTypeException($"Given subscriber type ({cb.GetType().Name}) cannot convert to and publish via {typeof(ISubscriber<TContext>).Name} on Publisher {GetType().Name}");
        }

        public ISubscriber Remove(ISubscriber cb)
        {
            if(cb is ISubscriber<TContext> typedCB) return ((IDelegatorAbstract<ISubscriber<TContext>>)this).Remove(typedCB);
            throw new InvalidTypeException($"Given subscriber type ({cb.GetType().Name}) cannot convert to and remove via {typeof(ISubscriber<TContext>).Name} on Publisher {GetType().Name}");
        }
        
        public Task PublishImplementation(ISubscriber sub)
        {
            if(sub is ISubscriber<TContext> subTyped) return subTyped.Execute(cachedContext);
            throw new InvalidTypeException($"Given subscriber type ({sub.GetType().Name}) cannot convert to and publish via {typeof(ISubscriber<TContext>).Name} on Publisher {GetType().Name}");
        }
        

        public Task Publish(TContext ctx)
        {
            cachedContext = ctx;
            return ((IPublisher)this).PublishCore();
        }
        
        public void Snapshot()
        {
            snapshot.Clear();
            snapshot.AddRange(subs);
        }
    }
    
    public class Publisher<T1, T2> : 
        IDelegatorAbstract<ISubscriber<T1, T2>>,
        IPublisher
    {
        // ISubscriber directly inherits from ISubscriber<TContext> and ISubscriber<T1, T2>
        [NonSerialized] readonly List<ISubscriber<T1, T2>> subs = new();
        [NonSerialized] readonly List<ISubscriber> snapshot = new();
        public IReadOnlyList<ISubscriber> API_Snapshot => snapshot;
        
        [NonSerialized] T1 cachedContext1;
        [NonSerialized] T2 cachedContext2;

        public ISubscriber<T1, T2> Add(ISubscriber<T1, T2> cb) { subs.Add(cb); return cb; }

        public ISubscriber<T1, T2> Remove(ISubscriber<T1, T2> cb) { subs.Remove(cb); return cb; }

        public ISubscriber Add(ISubscriber cb)
        {
            if(cb is ISubscriber<T1, T2> typedCB) ((IDelegatorAbstract<ISubscriber<T1, T2>>)this).Add(typedCB);
            else throw new InvalidTypeException($"Given subscriber type ({cb.GetType().Name}) cannot convert to/add into {typeof(ISubscriber<T1, T2>).Name} on Publisher {GetType().Name}");
            return cb;
        }


        public ISubscriber Remove(ISubscriber cb)
        {
            if(cb is ISubscriber<T1, T2> typedCB) ((IDelegatorAbstract<ISubscriber<T1, T2>>)this).Remove(typedCB);
            else throw new InvalidTypeException($"Given subscriber type ({cb.GetType().Name}) cannot convert to/remove via {typeof(ISubscriber<T1, T2>).Name} on Publisher {GetType().Name}");
            return cb;
        }
        
        public Task PublishImplementation(ISubscriber sub)
        {
            if(sub is ISubscriber<T1, T2> typedSub) return typedSub.Execute(cachedContext1, cachedContext2);
            throw new InvalidTypeException($"Given subscriber type ({sub.GetType().Name}) cannot convert to and publish via {typeof(ISubscriber<T1, T2>).Name} on Publisher {GetType().Name}");
        }
        public Task Publish(T1 ctx1, T2 ctx2)
        {
            cachedContext1 = ctx1;
            cachedContext2 = ctx2;
            return ((IPublisher)this).PublishCore();
        }
        
        public void Snapshot()
        {
            snapshot.Clear();
            snapshot.AddRange(subs);
        }

    }
}
