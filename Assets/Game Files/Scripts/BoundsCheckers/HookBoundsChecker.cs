using System;
using UnityEngine;

public class HookBoundsChecker : BoundsChecker<HookBoundsChecker.HookContext>
{
    [Serializable]
    public struct HookContext
    {
        public bool isHooked;
    }
}
