using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Serialization;
using static System.Single;


public enum NoBlends { }

[Serializable]
public struct AnimState<TAnimEnum>
    where TAnimEnum : Enum
{
    public string name;
    public TAnimEnum animEnum;
    [ReadOnly] public int hash;
    public AnimState(string name, TAnimEnum animEnum)
    {
        this.name = name;
        this.animEnum = animEnum;
        hash = Animator.StringToHash(name);
    }
    public void CalculateHash() => hash = Animator.StringToHash(name);
}
    
[Serializable]
public struct BlendTreeVariable<TAnimBlendEnum>
    where TAnimBlendEnum : Enum
{
    public string name;
    public TAnimBlendEnum blendEnum;
    [ReadOnly] public int hash;
    public BlendTreeVariable(string name, TAnimBlendEnum blendEnum)
    {
        this.name = name;
        this.blendEnum = blendEnum;
        hash = Animator.StringToHash(name);
    }
    public void CalculateHash() => hash = Animator.StringToHash(name);
}

[LabelWidth(125)]
[InlineProperty]
[Serializable]
public class AnimHandle<TAnimEnum, TAnimBlendEnum>
    where TAnimEnum : Enum
    where TAnimBlendEnum : Enum
{
    [FormerlySerializedAs("states")] [LabelWidth(150)] public AnimState<TAnimEnum>[] States;
    [FormerlySerializedAs("blendTreeVariables")] [LabelWidth(150)] public BlendTreeVariable<TAnimBlendEnum>[] BlendTreeVariables;

    Dictionary<TAnimEnum, int> states;
    Dictionary<TAnimBlendEnum, int> blendTreeVariables;
    
    [Button, PropertyOrder(-1)]
    public void RecalculateHashes()
    {
        if (States != null)
            for (int i = 0; i < States.Length; i++)
            {
                var s = States[i];
                s.CalculateHash();
                States[i] = s;
            }

        if (BlendTreeVariables != null)
            for (int i = 0; i < BlendTreeVariables.Length; i++)
            {
                var s = BlendTreeVariables[i];
                s.CalculateHash();
                BlendTreeVariables[i] = s;
            }

    }

    void Initialize()
    {
        states = new Dictionary<TAnimEnum, int>();
        blendTreeVariables = new Dictionary<TAnimBlendEnum, int>();
        foreach (var state in States) states.Add(state.animEnum, state.hash);
        foreach (var blendTreeVariable in BlendTreeVariables) blendTreeVariables.Add(blendTreeVariable.blendEnum, blendTreeVariable.hash);
    }

    int GetHash(TAnimEnum animEnum)     
    {
        if (states.TryGetValue(animEnum, out var hash)) return hash;
        return -1;
    }

    public void UpdateAnimBlendFloat(Animator animator, TAnimBlendEnum blendEnum, float value)
    {
        if (blendTreeVariables == null) Initialize();
        if (animator == null) return;
        if (blendTreeVariables == null) return;
        
        if (!blendTreeVariables.TryGetValue(blendEnum, out var hash))
        {
            Debug.LogWarning($"AnimHandle: No hash mapped for blend enum {blendEnum}.");
            return;
        }
        
        animator.SetFloat(hash, value);
    }
    
    public bool Play(
        Animator animator, 
        TAnimEnum animEnum, 
        int layer = 0, 
        float normalizedTime = NegativeInfinity)
    {
        if (states == null) Initialize();
        if (animator == null) { Debug.LogWarning("Animator Null"); return false;}
        if (States == null) { Debug.LogError("States Null"); return false;}
        if (layer < 0 || layer >= animator.layerCount)  { Debug.LogError("Layer Out of Index Range"); return false;}
        if(states == null) { Debug.LogError("States Dictionary Null"); return false;}
        if(!states.TryGetValue(animEnum, out var hash))
        {
            Debug.LogError($"AnimHandle: No state mapped for enum {animEnum}");
            return false;
        }
        if(hash == 0) { Debug.LogError($"AnimHandle: Hash for enum {animEnum} is 0, Please Recalculate Hashes"); return false;}
        animator.Play(hash, layer, normalizedTime);
        return true;
    }
    
    public bool PlayThenOnEnd(
        Animator animator,
        TAnimEnum animEnum,
        Action onEnd,
        int layer = 0,
        float normalizedTime = NegativeInfinity)
    {
        if (!Play(animator, animEnum, layer, normalizedTime))
            return false;

        PlayThenOnEndAsync(animator, animEnum, onEnd, layer).Forget("AnimHandle: PlayThenOnEnd");
        return true;

        async Task PlayThenOnEndAsync(
            Animator _animator,
            TAnimEnum _animEnum,
            Action _OnEnd,
            int _layer)
        {
            int hash = GetHash(_animEnum);
            if (hash == -1 || _animator == null || _layer < 0 || _layer >= _animator.layerCount)
                return;

            const float timeoutSeconds = 1.5f;
            float start = Time.time;

            // 1) Wait until the requested state is actually active
            while (true)
            {
                if (_animator == null) return;

                var s = _animator.GetCurrentAnimatorStateInfo(_layer);
                bool inTarget = s.shortNameHash == hash || s.fullPathHash == hash;

                if (!_animator.IsInTransition(_layer) && inTarget)
                    break;

                if (Time.time - start > timeoutSeconds)
                {
                    Debug.LogWarning($"AnimHandle: Timeout waiting to ENTER {_animEnum} on layer {_layer}.");
                    _OnEnd?.Invoke();
                    return;
                }

                await Awaitable.NextFrameAsync();
            }

            // 2) Wait until target state finishes (for non-loop clips)
            while (true)
            {
                if (_animator == null) return;

                var s = _animator.GetCurrentAnimatorStateInfo(_layer);
                bool stillTarget = s.shortNameHash == hash || s.fullPathHash == hash;

                // If we already left target state, treat as ended
                if (!stillTarget)
                    break;

                // End condition for non-looping state
                if (!s.loop && !_animator.IsInTransition(_layer) && s.normalizedTime >= 1f)
                    break;

                if (Time.time - start > timeoutSeconds)
                {
                    Debug.LogWarning($"AnimHandle: Timeout waiting to FINISH {_animEnum} on layer {_layer}.");
                    return;
                }

                await Awaitable.NextFrameAsync();
            }

            _OnEnd?.Invoke();
        }
    }
    
    public bool PlayWeightSet(
        Animator animator,
        TAnimEnum animEnum,
        float initialWeight,
        float endWeight,
        int layer = 0,
        float normalizedTime = NegativeInfinity)
    {
        Animator lclAnimator = animator;
        animator.SetLayerWeight(layer, initialWeight);
        if(!PlayThenOnEnd(animator, animEnum, SetWeightTo1, layer, normalizedTime)) return false;
        return true;
        void SetWeightTo1() => lclAnimator.SetLayerWeight(layer, endWeight);
    }
    
    public bool PlayWeightSet(
        Animator animator,
        TAnimEnum animEnum,
        float weight,
        int layer = 0,
        float normalizedTime = NegativeInfinity)
    {
        Animator lclAnimator = animator;
        animator.SetLayerWeight(layer, weight);
        if(!Play(animator, animEnum, layer, normalizedTime)) return false;
        return true;
    }
    
    public bool PlayWeightSetOnEnd(
        Animator animator,
        TAnimEnum animEnum,
        float initialWeight,
        float endWeight,
        Action onEnd,
        int layer = 0,
        float normalizedTime = NegativeInfinity)
    {
        Animator lclAnimator = animator;
        Action lclOnEnd = onEnd;
        animator.SetLayerWeight(layer, initialWeight);
        if(!PlayThenOnEnd(animator, animEnum, SetWeightTo1, layer, normalizedTime)) return false;
        return true;
        void SetWeightTo1() {lclAnimator.SetLayerWeight(layer, endWeight); lclOnEnd?.Invoke();}
    }

}
