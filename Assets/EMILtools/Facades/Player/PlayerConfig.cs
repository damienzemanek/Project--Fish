using System;
using UnityEngine;
using EMILtools.Systems;

[CreateAssetMenu(fileName = "PlayerConfig", menuName = "EMILtools/ScriptableObjects/Configs/Player")]
public class PlayerConfig : Config
{
    [SerializeField] public Move move;

    [Serializable]
    public struct Move
    {
        [field: SerializeField] public float speedScalar { get; private set; }
    }
}