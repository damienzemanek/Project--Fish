using System;
using EMILtools.Extensions;
using UnityEngine;
using EMILtools.Systems;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "EMILtools/ScriptableObjects/Configs/Player")]
public class PlayerConfig : Config
{
    [SerializeField] public Move move;
    [SerializeField] public Jump jump;
    [SerializeField] public Friction friction;
    [SerializeField] public Fall fall;

    [Serializable]
    public struct Fall
    {
        [field: SerializeField] public float checkDist;
        [field: SerializeField] public LayerMask mask;
        
        [field: SerializeField] public ForceMode2D forceMode;
        [field: SerializeField] public float scalar;
    }
    
    [Serializable]
    public struct Friction
    {
        [field: SerializeField] public float frictionScalar { get; private set; }
    }

    [Serializable]
    public struct Jump
    {
        [field: SerializeField] public float jumpCurveRate { get; private set; }
        [field: SerializeField] public int maxJumps { get; private set; }
        [field: SerializeField] public ForceMode2D forceMode;
        [field: SerializeField] public float scalar;
        [field: SerializeField] public Ref<float> coyoteInputWindow;
    }
    
    [Serializable]
    public struct Move
    {
        [field: SerializeField] public float speedScalar { get; private set; }
        [field: SerializeField] public ForceMode2D forceMode2d { get; private set; }
        [field: SerializeField] public float maxVelocity { get; private set; }
        [field: SerializeField] public float acceleration { get; private set; }
        [field: SerializeField] public float deceleration { get; private set; }

    }
}