using System;
using JetBrains.Annotations;
using static EMILtools.Systems.ResolverSystem;

namespace EMILtools.Systems
{
    public static class SubscriberExecutor
    {
        public class ActionResolver : Resolver<Action, VoidCtx>
        {
            protected override bool Execute(Action command, [CanBeNull] VoidCtx ctx)
            {
                command();
                return true;
            }
        }
        public class ActionContextResolver<T> : Resolver<Action<T>, T>
        {
            protected override bool Execute(Action<T> command, [CanBeNull] T ctx)
            {
                command(ctx);
                return true;
            }
        }
        
        
        public class PredicateResolver : Resolver<Func<bool>, VoidCtx>
        {
            protected override bool Execute(Func<bool> command, [CanBeNull] VoidCtx ctx)
                => command();
        }

        public class FuncResolver<TResult> : Resolver<Func<TResult>, VoidCtx>
        {
            protected override bool Execute(Func<TResult> command, [CanBeNull] VoidCtx ctx)
            {
                command();
                return true;
            }
        }
        
        public class PredicateContextResolver<T> : Resolver<Func<T, bool>, T>
        {
            protected override bool Execute(Func<T, bool> command, [CanBeNull] T ctx)
                => command(ctx);
        }
    }
}