using System;
using EMILtools.Core;
using UnityEngine;

[Serializable]
public abstract class ThresholdCore : ScriptableObject
{
    public abstract void Reset(ref int index);
    public abstract float GetHighestThreshold();
    public abstract void SyncIndexToValue(ref int index, float value);
    public abstract void LogThresholds(ref int index, float currentValue);
    public abstract bool WasThresholdReached(ref int index, float currentValue, out Enum state);
    public abstract bool GetNearestLastThreshold(float value, out Enum label);

    public virtual void SetAllDelegates(IDelegator cb) { }
    public virtual void AddOrReplaceDelegate(Enum label, IDelegator cb) { }

}