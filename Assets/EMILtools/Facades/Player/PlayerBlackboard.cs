using System;
using EMILtools.Systems;
using UnityEngine;

[Serializable]
public class PlayerBlackboard : Blackboard
{
    [field: SerializeField] public Rigidbody2D rb { get; private set; }
}