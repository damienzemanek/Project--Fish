using System.Threading.Tasks;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;

public static class Resolver
{
    
    //         Debug.Log($" [===== Executing & Resolving Step {i}... =====] ");
    //        Debug.Log($" [===== Step {i} Fully Executed & Resolved =====] ");      

    
    public static async Task<bool> ResolveContainer<TResolveType, TContext>(
        
        this ResolveContainer<TResolveType> Resolves, 
        ICommand<TContext, bool> command, 
        TContext ctx,
        bool canShortCircuit)
    
        where TContext : class, IPipelineContext
        where TResolveType : class, IResolveContext
    {
        if (Resolves.beforeExecution.Length > 0 && 
            !await Resolver.ResolveArray(Resolves.beforeExecution, canShortCircuit, ctx))
        {
            Debug.Log(" (!) Resolver Short Circuited (Before Execution)");
            return false;
        }

        if (command.Execute(ctx) && canShortCircuit)
        {
            Debug.Log(" (!) Short Circuit Triggered (Execution)");
            if(Resolves.failedExecution.Length > 0)
                await Resolver.ResolveArray(Resolves.failedExecution, true, ctx);
            return false;
        }
        Debug.Log($"    >> Step Executed << ");

        if (Resolves.afterExecution.Length > 0 && 
            !await Resolver.ResolveArray(Resolves.afterExecution, canShortCircuit, ctx))
        {
            Debug.Log(" (!) Resolver Short Circuited (After Execution)");
            return false;
        }

        return true;
    }
    
    
    public static async Task<bool> ResolveArray<TContext>(
        IResolveContext[] resolvables,
        bool isShortCircuit,
        TContext ctx = null)
        where TContext : class
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
