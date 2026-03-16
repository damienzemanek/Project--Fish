using System;
using EMILtools.Systems;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;

[Serializable]
public class PlayerBlackboard : Blackboard
{
    public enum AttackDir { Left, Right, Up, Down }
    
    [field: SerializeField] [field:Required] public Rigidbody2D rb { get; private set;}
    [field:SerializeField] [field:Required] public Transform feetPoint { get; private set; }
    [field:SerializeField] [field:Required] public Transform facingBody { get; private set; }
    [field:SerializeField] [field:Required] public Transform attackDirIndicator { get; private set; }
    [field:SerializeField] [field:Required] public Animator animator { get; private set; }
    [field: SerializeField] public CurveValue jumpCurve { get; private set; } = new();
    [field:SerializeField] [field:Required] public Bounds2D<AttackDir> attackBounds { get; private set; }
}
