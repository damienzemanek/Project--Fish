using System;
using JetBrains.Annotations;

namespace EMILtools.Systems
{
    public static class SubscriberExecutor
    {
        public class ActionResolver : ResolverSystem.Resolver<Action, VoidCtx>
        {
            protected override bool Execute(Action command, [CanBeNull] VoidCtx ctx)
            {
                command();
                return true;
            }
        }
        public class ActionContextResolver<T> : ResolverSystem.Resolver<Action<T>, T>
            where T : class, IContext
        {
            protected override bool Execute(Action<T> command, [CanBeNull] T ctx)
            {
                command(ctx);
                return true;
            }
        }
        
        public class PredicateResolver : ResolverSystem.Resolver<Func<bool>, VoidCtx>
        {
            protected override bool Execute(Func<bool> command, [CanBeNull] VoidCtx ctx)
                => command();
        }

        public class FuncResolver<TResult> : ResolverSystem.Resolver<Func<TResult>, VoidCtx>
        {
            protected override bool Execute(Func<TResult> command, [CanBeNull] VoidCtx ctx)
            {
                command();
                return true;
            }
        }
        
        public class PredicateContextResolver<T> : ResolverSystem.Resolver<Func<T, bool>, T>
            where T : class, IContext
        {
            protected override bool Execute(Func<T, bool> command, [CanBeNull] T ctx)
                => command(ctx);
        }
    }
}