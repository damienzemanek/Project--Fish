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

        public abstract class Resolver<TDelegate, TContext>
            where TDelegate : Delegate
        {
            /// <summary>
            /// Return TRUE if the command should SHORT CIRCUIT
            /// </summary>
            /// <param name="command"></param>
            /// <param name="ctx"></param>
            /// <returns></returns>
            protected abstract bool Execute(TDelegate command, TContext ctx);

            
            public async Task<bool> ResolveContainer<TResolveType>(
                
                ResolveContainer<TResolveType> Resolves, 
                TDelegate command,
                bool canShortCircuit,
                TContext ctx)
        
                where TResolveType : class, IResolvableWithContext

            {
                
                
                if (Resolves.beforeExecution.Length > 0)
                {
                    bool beforeSuccess = await ResolveArray(Resolves.beforeExecution, canShortCircuit, ctx);
                    if (!beforeSuccess)
                    {
                        Debug.Log(" (!) Resolver Short Circuited (Before Execution)");
                        return false;
                    }
                }

                if (Execute(command, ctx) && canShortCircuit)
                {
                    Debug.Log(" (!) Short Circuit Triggered (Execution)");
                    if(Resolves.failedExecution.Length > 0)
                        await ResolveArray(Resolves.failedExecution, true, ctx);
                    return false;
                }
                Debug.Log($"    >> Step Executed << ");

                if (Resolves.afterExecution.Length > 0 && 
                    !await ResolveArray(Resolves.afterExecution, canShortCircuit, ctx))
                {
                    Debug.Log(" (!) Resolver Short Circuited (After Execution)");
                    return false;
                }

                return true;
            } 
        }

        
        public static async Task<bool> ResolveArray<TContext>(
            IResolvableWithContext[] resolvables,
            bool isShortCircuit,
            TContext ctx)
        {
            Debug.Log($" ------ Resolving {resolvables.Length} Contexts... ------");
            for (int j = 0; j < resolvables.Length; j++)
            {
                Debug.Log($" (?) Resolving Context {j}... ({resolvables[j].GetType().Name})");
                var resolve = resolvables[j];

                if (!resolve.Resolve(ctx) && isShortCircuit)
                    return false;

                if (resolve is IResolveWaitable waitable && !waitable.waiting)
                {
                    waitable.waiting = true;
                    await waitable.WaitUntilResolved();
                }
                Debug.Log($" (#) Context {j} Resolved! ");
            }

            Debug.Log($" ------ (#) All Contexts Resolved! ------");

            return true;
        }
    }
}

