using System;
using EMILtools.Core;
using UnityEngine;

[Serializable]
public abstract class ThresholdCore : ScriptableObject
{
    public abstract void Reset();
    public abstract float GetHighestThreshold();
    public abstract void SyncIndexToValue(float value);
    public abstract void LogThresholds(float currentValue);
    public abstract bool WasThresholdReached(float currentValue, out Enum state, out IDelegator cb);
    public abstract bool GetNearestLastThreshold(float value, out Enum label, out IDelegator returnCallback);

    public virtual void SetAllDelegates(IDelegator cb) { }
    public virtual void AddOrReplaceDelegate(Enum label, IDelegator cb) { }

}