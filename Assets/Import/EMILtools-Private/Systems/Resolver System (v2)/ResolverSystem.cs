using System;
using System.Threading.Tasks;
using EMILtools.Core;
using EMILtools.Systems;
using UnityEngine;

namespace EMILtools.Systems
{ 
    public static class ResolverSystem
    {
        static bool _debug = false;
        static void Log(string msg)
        {
            if (_debug) Debug.Log($"[ResolverSystem] {msg}");
        }

        public static class Resolver
        {
            public static async Task<bool> ResolveContainer<TContext>(
                ResolveContainer resolves,
                IResolvable resolveable,
                bool canShortCircuit,
                TContext ctx)
            {
                Log("=== ResolveContainer START ===");

                if (!await ResolveArray(resolves.beforeExecution, canShortCircuit, ctx))
                {
                    Log("beforeExecution FAILED (short-circuit)");
                    return false;
                }

                Log($"Main Resolve: {resolveable.GetType().Name}");
                bool mainResult = resolveable.Resolve(ctx);
                Log($"Main Result: {mainResult}");

                if (mainResult && canShortCircuit)
                {
                    Log("Short-circuit triggered by MAIN resolve");

                    await ResolveArray(resolves.failedExecution, true, ctx);

                    Log("=== ResolveContainer END (FAILED via short-circuit) ===");
                    return false;
                }

                if (!await ResolveArray(resolves.afterExecution, canShortCircuit, ctx))
                {
                    Log("afterExecution FAILED (short-circuit)");
                    return false;
                }

                Log("=== ResolveContainer END (SUCCESS) ===");
                return true;
            }
            
            
            static async Task<bool> ResolveArray<TContext>(
                IResolvable[] resolvables,
                bool isShortCircuit,
                TContext ctx)
            {
                if (resolvables == null || resolvables.Length == 0)
                {
                    Log("ResolveArray skipped (null or empty)");
                    return true;
                }

                Log($"ResolveArray START (Count: {resolvables.Length}, ShortCircuit: {isShortCircuit})");

                for (int i = 0; i < resolvables.Length; i++)
                {
                    var resolve = resolvables[i];

                    Log($"Resolving [{i}] -> {resolve.GetType().Name}");

                    var result = resolve.Resolve(ctx);

                    Log($"Result [{i}] -> {result}");

                    if (!result && isShortCircuit)
                    {
                        Log($"Short-circuit EXIT at index {i}");
                        return false;
                    }

                    if (resolve is IResolveWaitable waitable)
                    {
                        if (!waitable.waiting)
                        {
                            Log($"Wait START [{i}] -> {resolve.GetType().Name}");

                            waitable.waiting = true;
                            await waitable.WaitUntilResolved();

                            Log($"Wait END [{i}] -> {resolve.GetType().Name}");
                        }
                        else
                        {
                            Log($"Skipped wait (already waiting) [{i}]");
                        }
                    }
                }

                Log("ResolveArray END");
                return true;
            }
        }
    }
}