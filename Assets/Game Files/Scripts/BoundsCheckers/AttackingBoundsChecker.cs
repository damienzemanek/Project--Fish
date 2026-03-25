using System;
using UnityEngine;
using static AttackingBoundsChecker;

public class AttackingBoundsChecker : BoundsChecker<AttackCtx>
{
    [Serializable]
    public struct AttackCtx
    {
        public Transform attackerTransform;
        public Rigidbody attackerRb;
        public IDamageable.DamageInfo damageInfo;
    }
}



