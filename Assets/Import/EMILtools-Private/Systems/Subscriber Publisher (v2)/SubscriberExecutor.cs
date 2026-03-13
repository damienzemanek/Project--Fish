using System;
using JetBrains.Annotations;
using UnityEngine;
using static EMILtools.Systems.ResolverSystem;

namespace EMILtools.Systems
{
    public static class SubscriberExecutor
    {
        public class ActionResolver : BaseResolver<Action>
        {
            protected override bool Execute(Action command)
            {
                command();
                return true;
            }
        }
        public class ActionContextResolver<T> : ContextResolver<Action<T>, T>
        {

            protected override bool Execute(Action<T> command, in T ctx)
            {
                command(ctx);
                return true;
            }
        }
        
        
        public class PredicateResolver : BaseResolver<Func<bool>>
        {
            protected override bool Execute(Func<bool> command)
            {
                return command();
            }
        }

        public class FuncResolver<TResult> : BaseResolver<Func<TResult>>
        {
            protected override bool Execute(Func<TResult> command)
            {
                command();
                return true;
            }
        }
        
        public class PredicateContextResolver<T> : ContextResolver<Func<T, bool>, T>
        {
            protected override bool Execute(Func<T, bool> command, in T ctx)
            {
                return command(ctx);
            }
        }
    }
}