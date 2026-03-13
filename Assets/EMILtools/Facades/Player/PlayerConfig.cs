using System;
using UnityEngine;
using EMILtools.Systems;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "EMILtools/ScriptableObjects/Configs/Player")]
public class PlayerConfig : Config
{
    [SerializeField] public Move move;
    [SerializeField] public Jump jump;

    [Serializable]
    public struct Jump
    {
        [field: SerializeField] public float jumpCurveRate { get; private set; }
        [field: SerializeField] public int maxJumps { get; private set; }

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