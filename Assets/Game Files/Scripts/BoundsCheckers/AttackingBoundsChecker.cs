using System;
using UnityEngine;
using static AttackingBoundsChecker;

public class AttackingBoundsChecker : BoundsChecker<AttackingCtx>
{
    [Serializable]
    public struct AttackingCtx
    {
        public IDamageable.DamageInfo damageInfo;
    }
}
