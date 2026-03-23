using System;
using UnityEngine;
using EMILtools.Systems;
using Pathfinding;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "EMILtools/ScriptableObjects/Configs/Enemy")]
public class EnemyConfig : Config
{
    public enum EnemyAnims { Idle, Attack, Walk, }
    public enum EnemyBlendVars { }
    
    [field: SerializeField] public AnimHandle<EnemyAnims, EnemyBlendVars> animHandle { get; private set; }
    [field: SerializeField] public Follow follow { get; private set; }
    [field: SerializeField] public InAir inAir { get; private set; }
    [field: SerializeField] public ClampLateralMov clampLateralMove { get; private set; }
    [field: SerializeField] public Jump jump { get; private set; }
    [field: SerializeField] public ViewRange viewRange { get; private set; }
    [field: SerializeField] public TakeDmg takeDmg { get; private set; }
    
    [Serializable]
    public struct TakeDmg
    {
        [field: SerializeField] public float invulnerablePeriod { get; private set; }
    }
    
    [Serializable]
    public struct ViewRange
    {
        [field: SerializeField] public Ref<float> delayToStopChasing { get; private set; }
    }
    
    [Serializable]
    public struct Jump
    {
        [field: SerializeField] public float jumpForceScalar { get; private set; }
        [field: SerializeField] public Ref<float> jumpDelay { get; private set;}
    }
    
    [Serializable]
    public struct ClampLateralMov
    {
        [field: SerializeField] public float maxVelocity { get; private set; }
    }
    
    [Serializable]
    public struct Follow
    {
        [field: SerializeField] public float speedScalar { get; private set; }   
        [field: SerializeField] public float nextWaypointDistance { get; private set; }  
        [field: SerializeField] public float stoppingDistToTarget { get; private set; }  
        [field: SerializeField] public float minHorizAngleToFollow { get; private set; } 

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