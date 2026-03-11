using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;

public class Publisher<TSubscriber> : IDelegatorAbstract<TSubscriber>
    where TSubscriber : class, ISubscriber
{
    readonly List<TSubscriber> subscribers = new();
    
    // ------------ Specific ------------
    public TSubscriber Add(TSubscriber subscriber) { subscribers.Add(subscriber); return subscriber; }
    public TSubscriber Remove(TSubscriber subscriber) { subscribers.Remove(subscriber); return subscriber; }
    
    public async Task Publish()
    {
        for (int i = 0; i < subscribers.Count; i++)
        {
            var sub = subscribers[i];

            if (!sub.isActive)
                continue;

            await sub.Execute();
        }
    }
}


public class GenericPublisher : IDelegatorAbstract<ISubscriber>
{
    public List<ISubscriber> subscribers = new();
    
    public ISubscriber Add(ISubscriber subscriber) { subscribers.Add(subscriber); return subscriber; }
    public ISubscriber Remove(ISubscriber subscriber) { subscribers.Remove(subscriber); return subscriber; }

    public async Task Publish()
    {
        for (int i = 0; i < subscribers.Count; i++)
        {
            var sub = subscribers[i];

            if (!sub.isActive)
                continue;

            await sub.Execute();
        }
    }
}

