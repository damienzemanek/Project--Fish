using System;
using UnityEngine;
using EMILtools.Systems;
using Sirenix.OdinInspector;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "EMILtools/ScriptableObjects/Configs/Player")]
public class PlayerConfig : Config
{
    [SerializeField] public Move move;
    
    [Serializable]
    public struct Move
    {
        [field: ShowInInspector] public float speedScalar { get; private set; }
        [field: ShowInInspector] public ForceMode forceMode { get; private set; }
    }
}