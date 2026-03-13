using System;
using System.Threading.Tasks;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;

namespace EMILtools.Systems
{ 
    public static class ResolverSystem
    {
        
        
        public static class Resolver
        {
            public static async Task<bool> ResolveContainer<TContext>(
                ResolveContainer resolves,
                Func<TContext, bool> execute,
                bool canShortCircuit,
                TContext ctx)
            {
                if (!await ResolveArray(resolves.beforeExecution, canShortCircuit, ctx)) return false;

                if (execute(ctx) && canShortCircuit)
                {
                    await ResolveArray(resolves.failedExecution, true, ctx); return false;
                }

                if (!await ResolveArray(resolves.afterExecution, canShortCircuit, ctx)) return false;

                return true;
            }

            public static async Task<bool> ResolveContainer(
                ResolveContainer resolves,
                Func<bool> execute,
                bool canShortCircuit)
            {
                if (!await ResolveArray(resolves.beforeExecution, canShortCircuit))
                    return false;

                if (execute() && canShortCircuit)
                {
                    await ResolveArray(resolves.failedExecution, true); return false;
                }

                if (!await ResolveArray(resolves.afterExecution, canShortCircuit))
                    return false;

                return true;
            }
            
            static async Task<bool> ResolveArray(
                IResolvable[] resolvables,
                bool isShortCircuit)
            {
                if (resolvables == null || resolvables.Length == 0)
                    return true;

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
                }

                return true;
            }
            
            static async Task<bool> ResolveArray<TContext>(
                IResolvable[] resolvables,
                bool isShortCircuit,
                TContext ctx)
            {
                if (resolvables == null || resolvables.Length == 0)
                    return true;

                for (int i = 0; i < resolvables.Length; i++)
                {
                    var resolve = resolvables[i];
                    
                    bool result = resolve switch
                    {
                        IResolvable r when r is IContextViewImmutable ctxView => r.Resolve(ctxView),
                        IResolvable r => r.Resolve(ctx),
                        _ => resolve.Resolve()
                    };
                    

                    if (!result && isShortCircuit)
                        return false;

                    if (resolve is IResolveWaitable waitable && !waitable.waiting)
                    {
                        waitable.waiting = true;
                        await waitable.WaitUntilResolved();
                    }
                }

                return true;
            }
        }
        
        
    }
}

