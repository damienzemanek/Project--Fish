using System;
using UnityEngine;

[CreateAssetMenu(fileName = "PlayerSoundConfig", menuName = "ScriptableObjects/SoundConfigs/PlayerSounds")]
public class PlayerSoundConfig : SoundConfig
{
    public enum PlayerSounds { Walk, Attack, Hook, Pull, TakeDamage, Jump, Die  }
    [SerializeField] public SoundHandle<PlayerSounds> soundHandle;

    public override void Play(AudioSource source, string soundName, bool loop = false, float startTime = 0f)
    {
        if (Enum.TryParse<PlayerSounds>(soundName, out var result))
            soundHandle.Play(source, result, 1f, loop, startTime);
    }

    public override string[] GetSoundNames() 
        => Enum.GetNames(typeof(PlayerSounds));

    public override AudioClip GetClip(string soundName) 
        => Enum.TryParse<PlayerSounds>(soundName, out var result) ? soundHandle.GetClip(result) : null;

    public override void GenerateSounds() => soundHandle.GenerateSounds();
}
