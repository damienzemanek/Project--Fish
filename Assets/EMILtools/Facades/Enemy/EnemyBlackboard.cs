using System;
using EMILtools.Systems;
using EMILtools.Timers;
using Pathfinding;
using Sirenix.OdinInspector;
using UnityEngine;
using static LifecycleEX;

[Serializable]
public class EnemyBlackboard : Blackboard, IEntityBlackboard
{
    IEntityBlackboard _iEntityBlackboardImplementation;
    [field: SerializeField] [field: Required] public Animator animator { get; private set; }
    [field: SerializeField] [field: Required] public Seeker seeker { get; private set; }
    [field: SerializeField] [field: Required] public Rigidbody2D rb { get; private set; }
    [field: SerializeField] [field: Required] public AttackingBoundsChecker[] attackingBoundsCheckers { get; private set; }
    [field: SerializeField] [field: Required] public DamageFlasher damageFlasher { get; private set; }
    [field: SerializeField] [field: Required] public Transform target { get; private set; }
    [field: SerializeField] [field: Required] public DelayLimitedMethod computePath { get; private set; }
    [field: SerializeField] [field: Required] public Transform[] feetPoints { get; private set; }
    [field: SerializeField] [field: Required] public CountdownTimer jumpTimer { get; set; }
    [field: SerializeField] [field: Required] public CountdownTimer invulnerableTimer { get; set; }
    [field: SerializeField] [field: Required] public CountdownTimer dyingStateTimer { get; set; }
    [field: SerializeField] [field: Required] public Transform faceDirTransform { get; private set; }
    [field: SerializeField] [field: Required] public LivingEntity livingEntity { get; private set; }
    [field: SerializeField] [field: Required] public Behaviour viewRange { get; private set; }

    
}