using System;
using UnityEngine;
using EMILtools.Systems;
using Pathfinding;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "EMILtools/ScriptableObjects/Configs/Enemy")]
public class EnemyConfig : Config
{
    public enum EnemyAnims { Idle, Attack, }
    public enum EnemyBlendVars { }
    
    [field: SerializeField] public AnimHandle<EnemyAnims, EnemyBlendVars> animHandle { get; private set; }
    [field: SerializeField] public Follow follow { get; private set; }
    [field: SerializeField] public InAir inAir { get; private set; }
    
    [Serializable]
    public struct Follow
    {
        [field: SerializeField] public float speedScalar { get; private set; }   
        [field: SerializeField] public float nextWaypointDistance { get; private set; }  
        [field: SerializeField] public float stoppingDistToTarget { get; private set; }  

    }

    [Serializable]
    public struct InAir
    {
        [field: SerializeField] public float checkDist { get; private set; } 
        [field: SerializeField] public LayerMask mask { get; private set; }
        [field: SerializeField] public float fallScalar { get; private set; }
        [field: SerializeField] public ForceMode2D forceMode { get; private set; }
    }
}