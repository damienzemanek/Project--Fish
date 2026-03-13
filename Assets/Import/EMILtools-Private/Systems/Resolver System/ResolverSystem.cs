using System;
using System.Threading.Tasks;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;

namespace EMILtools.Systems
{ 
    public static class ResolverSystem
    {
        //        Debug.Log($" [===== Executing & Resolving Step {i}... =====] ");
        //        Debug.Log($" [===== Step {i} Fully Executed & Resolved =====] ");      
        
        public interface IResolver
        {
            
        }
        
        public abstract class ResolverBase : IResolver
        {
            protected static async Task<bool> ResolveArray(
                IResolvable[] resolvables,
                bool isShortCircuit)
            {
                if (resolvables == null || resolvables.Length == 0)
                    return true;

                Debug.Log($" ------ Resolving {resolvables.Length} Contexts... ------");

                for (int i = 0; i < resolvables.Length; i++)
                {
                    var resolve = resolvables[i];

                    if (!resolve.Resolve() && isShortCircuit)
                        return false;

                    if (resolve is IResolveWaitable waitable && !waitable.waiting)
                    {
                        waitable.waiting = true;
                        await waitable.WaitUntilResolved();
                    }

                    Debug.Log($" (#) Context {i} Resolved!");
                }

                return true;
            }

            protected static async Task<bool> ResolveArray<TContext>(
                IResolvable[] resolvables,
                bool isShortCircuit,
                TContext ctx)
            {
                if (resolvables == null || resolvables.Length == 0)
                    return true;

                Debug.Log($" ------ Resolving {resolvables.Length} Contexts... ------");

                for (int i = 0; i < resolvables.Length; i++)
                {
                    var resolve = resolvables[i];

                    bool result;

                    if (resolve is IResolvableWithContext ctxResolve)
                        result = ctxResolve.Resolve(ctx);
                    else
                        result = resolve.Resolve();

                    if (!result && isShortCircuit)
                        return false;

                    if (resolve is IResolveWaitable waitable && !waitable.waiting)
                    {
                        waitable.waiting = true;
                        await waitable.WaitUntilResolved();
                    }

                    Debug.Log($" (#) Context {i} Resolved!");
                }

                return true;
            }
        }

        public abstract class BaseResolver<TDelegate> : ResolverBase
            where TDelegate : Delegate
        {
            protected abstract bool Execute(TDelegate command);

            public async Task<bool> ResolveContainer<TResolveType>(
                ResolveContainer<TResolveType> resolves,
                TDelegate command,
                bool canShortCircuit)
                where TResolveType : class, IResolvable
            {
                if (!await ResolveArray(resolves.beforeExecution, canShortCircuit))
                {
                    Debug.Log(" (!) Resolver Short Circuited (Before Execution)");
                    return false;
                }

                if (Execute(command) && canShortCircuit)
                {
                    Debug.Log(" (!) Short Circuit Triggered (Execution)");

                    if (resolves.failedExecution.Length > 0)
                        await ResolveArray(resolves.failedExecution, true);

                    return false;
                }

                Debug.Log(" >> Step Executed << ");

                if (!await ResolveArray(resolves.afterExecution, canShortCircuit))
                {
                    Debug.Log(" (!) Resolver Short Circuited (After Execution)");
                    return false;
                }

                return true;
            }
        }

        public abstract class ContextResolver<TDelegate, TContext> : ResolverBase
            where TDelegate : Delegate
        {
            protected abstract bool Execute(TDelegate command, in TContext ctx);

            public async Task<bool> ResolveContainer<TResolveType>(
                ResolveContainer<TResolveType> resolves,
                TDelegate command,
                bool canShortCircuit,
                TContext ctx)
                where TResolveType : class, IResolvable
            {
                if (!await ResolveArray(resolves.beforeExecution, canShortCircuit, ctx))
                {
                    Debug.Log(" (!) Resolver Short Circuited (Before Execution)");
                    return false;
                }

                if (Execute(command, ctx) && canShortCircuit)
                {
                    Debug.Log(" (!) Short Circuit Triggered (Execution)");

                    if (resolves.failedExecution.Length > 0)
                        await ResolveArray(resolves.failedExecution, true, ctx);

                    return false;
                }

                Debug.Log(" >> Step Executed << ");

                if (!await ResolveArray(resolves.afterExecution, canShortCircuit, ctx))
                {
                    Debug.Log(" (!) Resolver Short Circuited (After Execution)");
                    return false;
                }

                return true;
            }
        }
        
        public abstract class ContextResolver<TDelegate, T1, T2> : ResolverBase
            where TDelegate : Delegate
        {
            protected abstract bool Execute(TDelegate command, in T1 ctx1, in T2 ctx2);

            public async Task<bool> ResolveContainer<TResolveType>(
                ResolveContainer<TResolveType> resolves,
                TDelegate command,
                bool canShortCircuit,
                T1 ctx1,
                T2 ctx2)
                where TResolveType : class, IResolvable
            {
                if (!await ResolveArray(resolves.beforeExecution, canShortCircuit, (ctx1, ctx2)))
                {
                    Debug.Log(" (!) Resolver Short Circuited (Before Execution)");
                    return false;
                }

                if (Execute(command, ctx1, ctx2) && canShortCircuit)
                {
                    Debug.Log(" (!) Short Circuit Triggered (Execution)");

                    if (resolves.failedExecution.Length > 0)
                        await ResolveArray(resolves.failedExecution, true, (ctx1, ctx2));

                    return false;
                }

                Debug.Log(" >> Step Executed << ");

                if (!await ResolveArray(resolves.afterExecution, canShortCircuit, (ctx1, ctx2)))
                {
                    Debug.Log(" (!) Resolver Short Circuited (After Execution)");
                    return false;
                }

                return true;
            }
        }
        
    }
}

