using UnityEngine;
using EMILtools.Systems;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "EMILtools/ScriptableObjects/Configs/Enemy")]
public class EnemyConfig : Config
{
    public enum EnemyAnims { Idle, Attack, }
    public enum EnemyBlendVars { }

    [field: SerializeField] public AnimHandle<EnemyAnims, EnemyBlendVars> animHandle { get; private set; }
}