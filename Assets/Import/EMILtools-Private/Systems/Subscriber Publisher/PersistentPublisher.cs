using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;

namespace EMILtools.Systems
{
    
    public class Publisher<TContext> : IDelegatorAbstract<ISubscriber<TContext>>
        where TContext : class, IContext
    {
        List<ISubscriber<TContext>> subscribers = new();
        public ISubscriber<TContext> Add(ISubscriber<TContext> subscriber) { subscribers.Add(subscriber); return subscriber; }
        public ISubscriber<TContext> Remove(ISubscriber<TContext> subscriber) { subscribers.Remove(subscriber); return subscriber; }

        public async Task Publish(TContext ctx)
        {
            for (int i = 0; i < subscribers.Count; i++)
            {
                var sub = subscribers[i];

                if (!sub.isActive)
                    continue;

                await sub.Execute(ctx);
            }
        }
        
        public async Task Publish()
        {
            if(typeof(TContext) != typeof(VoidCtx)) throw new InvalidOperationException();

            for (int i = 0; i < subscribers.Count; i++)
            {
                var sub = subscribers[i];

                if (!sub.isActive)
                    continue;
                
                await sub.Execute();
            }
        }
    }


}
