using System;
using EMILtools.Systems;
using UnityEngine;

[Serializable]
public class EnemyBlackboard : Blackboard
{
    [field: SerializeField] public Animator animator { get; private set; }
}