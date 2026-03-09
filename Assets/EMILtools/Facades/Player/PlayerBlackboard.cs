using System;
using EMILtools.Systems;
using UnityEngine;

[Serializable]
public class PlayerBlackboard : Blackboard
{
    [field:NonSerialized] public Rigidbody2D rb { get; private set;}
}