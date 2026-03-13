using System;
using EMILtools.Systems;
using EMILtools.Timers;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public class PlayerBlackboard : Blackboard
{
    [field: SerializeField] [field:Required] public Rigidbody2D rb { get; private set;}
    [field: SerializeField] [field:Required] public AugmentPhysEX phys { get; private set;}
    [field: SerializeField] public CurveValue jumpCurve { get; set; } = new();
}