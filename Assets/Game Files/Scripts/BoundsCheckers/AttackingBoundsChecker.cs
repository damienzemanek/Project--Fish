using System;
using EMILtools.Systems;
using UnityEngine;
using static AttackingBoundsChecker;

[Serializable]
public class AttackingBoundsChecker : BoundsChecker<AttackCtx>
{
    [Serializable]
    public struct AttackCtx
    {
        public IEntityCtx attackerEntityCtx;
        public InterfaceReference<IContextInjectible<SimpleMsg>, MonoBehaviour> hitMsgReceiver;
        public IDamageable.DamageInfo damageInfo;
    }
}



