using System;
using EMILtools.Systems;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PlayerBlackboard : Blackboard, IEntityBlackboard
{
    public enum AttackDir { Left, Right, Up, Down }
    [field: SerializeField] [field:Required] public Rigidbody2D rb { get; private set;}
    [field:SerializeField] [field:Required] public AttackingBoundsChecker[] attackingBoundsCheckers { get; private set; }
    [field:SerializeField] [field:Required] public LivingEntity livingEntity { get; set; }
    [field:SerializeField] [field:Required] public Transform facingBody { get; private set; }
    [field:SerializeField] [field:Required] public Transform attackDirIndicator { get; private set; }
    [field:SerializeField] [field:Required] public Animator animator { get; private set; }
    [field:SerializeField] [field:Required] public Bounds2D<AttackDir> attackBounds { get; private set; }
    [field:SerializeField] [field:Required] public Transform[] feetPoints { get; private set; }
    [field:SerializeField] [field:Required] public EnumTagged<AttackDir, Collider2D>[] attackColliders { get; private set; }
    [field: SerializeField] [field:Required] public Hook hook { get; private set; }
    [field: SerializeField] [field:Required] public Transform hookAimPos { get; private set; }
    [field: SerializeField] [field: Required] public DamageFlasher damageFlasher { get; private set; }
    [field: SerializeField] public CountdownTimer invulnerableTimer { get; set; }
    [field: SerializeField] public CurveValue jumpCurve { get; private set; } = new();

}
