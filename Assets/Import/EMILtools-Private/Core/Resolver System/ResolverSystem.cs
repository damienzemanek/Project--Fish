using System;
using System.Threading.Tasks;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;

public static class ResolverSystem
{
    
    //         Debug.Log($" [===== Executing & Resolving Step {i}... =====] ");
    //        Debug.Log($" [===== Step {i} Fully Executed & Resolved =====] ");      


    public abstract class Resolver<TDelegate, TContext>
        where TDelegate : Delegate
        where TContext : class, IContext
    {
        protected abstract bool Execute(TDelegate command, TContext ctx);
        
        public static async Task<bool> ResolveContainer<TResolveType>(
        
            Resolver<TDelegate, TContext> resolver,
            ResolveContainer<TResolveType> Resolves, 
            TDelegate command, 
            TContext ctx,
            bool canShortCircuit)
    
            where TResolveType : class, IResolveContext
    
        {
            if (Resolves.beforeExecution.Length > 0 && 
                !await ResolveArray(Resolves.beforeExecution, canShortCircuit, ctx))
            {
                Debug.Log(" (!) Resolver Short Circuited (Before Execution)");
                return false;
            }

            if (resolver.Execute(command, ctx) && canShortCircuit)
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

    
    public static async Task<bool> ResolveArray(
        IResolveContext[] resolvables,
        bool isShortCircuit,
        IContext ctx = null)
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
