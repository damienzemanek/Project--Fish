using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;

[Serializable]
public struct SoundState<TSoundEnum>
    where TSoundEnum : Enum
{
    public AudioClip clip;
    public TSoundEnum soundEnum;

    public SoundState(AudioClip clip, TSoundEnum soundEnum)
    {
        this.clip = clip;
        this.soundEnum = soundEnum;
    }
}

[LabelWidth(125)]
[InlineProperty]
[Serializable]
public class SoundHandle<TSoundEnum>
    where TSoundEnum : Enum
{
    [LabelWidth(150)] public SoundState<TSoundEnum>[] Sounds;

    Dictionary<TSoundEnum, AudioClip> sounds;

    void Initialize()
    {
        sounds = new Dictionary<TSoundEnum, AudioClip>();
        if (Sounds != null)
        {
            foreach (var sound in Sounds)
            {
                if (sound.soundEnum != null && !sounds.ContainsKey(sound.soundEnum))
                    sounds.Add(sound.soundEnum, sound.clip);
            }
        }
    }

    public AudioClip GetClip(TSoundEnum soundEnum)
    {
        if (sounds == null) Initialize();
        if (sounds.TryGetValue(soundEnum, out var clip)) return clip;
        return null;
    }

    public void Play(AudioSource source, TSoundEnum soundEnum, float volume = 1f, bool loop = false, float startTime = 0f)
    {
        var clip = GetClip(soundEnum);
        if (clip != null && source != null)
        {
            if (loop)
            {
                source.clip = clip;
                source.loop = true;
                source.volume = volume;
                source.time = Mathf.Clamp(startTime, 0, clip.length - 0.001f);
                source.Play();
            }
            else
            {
                source.PlayOneShot(clip, volume);
            }
        }
    }
}

[Serializable]
public abstract class SoundConfig : ScriptableObject
{
    public abstract void Play(AudioSource source, string soundName, bool loop = false, float startTime = 0f);
    public abstract string[] GetSoundNames();
    public abstract AudioClip GetClip(string soundName);
}
