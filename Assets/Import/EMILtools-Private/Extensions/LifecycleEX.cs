using Sirenix.OdinInspector;
using System;
using EMILtools.Core;
using UnityEngine;

public static class LifecycleEX
{
    [Serializable]
    public struct RateLimitedMethod
    {
        public float rate;
        [ReadOnly] public float interval => 1f / rate;
        [ReadOnly] public float timer;
        public PersistentAction method;

        public void UpdateTick()
        {
            timer += Time.deltaTime;

            while (timer >= interval)
            {
                timer -= interval;
                method?.Invoke();
            }
        }
    }

}
