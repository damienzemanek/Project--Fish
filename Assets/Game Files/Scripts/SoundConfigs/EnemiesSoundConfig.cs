using System;
using UnityEngine;

[CreateAssetMenu(fileName = "EnemiesSoundConfig", menuName = "ScriptableObjects/SoundConfigs/Enemies")]
public class EnemiesSoundConfig : SoundConfig
{
    public enum EnemiesSounds
    {
        WarriorAttack, WarriorTakeDamage, WarriorYell,
        BruteAttack, BruteTakeDamage, BruteImmune, BruteYell
    }
    [SerializeField] public SoundHandle<EnemiesSounds> soundHandle;
    
    public override void Play(AudioSource source, string soundName, bool loop = false, float startTime = 0)
    {
        throw new System.NotImplementedException();
    }

    public override string[] GetSoundNames() => Enum.GetNames(typeof(EnemiesSounds));

    public override AudioClip GetClip(string soundName)
    {
        if (Enum.TryParse<EnemiesSounds>(soundName, out var result))
        {
            return soundHandle.GetClip(result);
        }
        return null;
    }

    public override void GenerateSounds() => soundHandle.GenerateSounds();
}
