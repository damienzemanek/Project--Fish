using System;
using System.Collections.Generic;
using EMILtools.Core;
using Sirenix.OdinInspector;
using UnityEngine;


public interface IThresholdMutator<TEnum, TDelegator>
{
    public void AddOrReplaceDelegate(TEnum label, TDelegator cb);
}


[Serializable]
public class Threshold<TEnum, TDelegator> : IThresholdMutator<TEnum, TDelegator>
    where TDelegator : IDelegator
    where TEnum : Enum
{
    [Serializable]
    public struct Entry
    {
        public TEnum label;
        public float threshold;
        [NonSerialized] public TDelegator cb;
    }
    
    [SerializeField] Entry[] entries;
    [ShowInInspector] int index;
    
    [Button]
    public void Reset()
    {
        if (entries == null || entries.Length == 0)
        {
            index = -1;
            return;
        }

        Array.Sort(entries, (a, b) => b.threshold.CompareTo(a.threshold)); 
        index = 0; 
    }
    
    public void SetAllDelegates(TDelegator cb)
    {
        if (entries == null) throw new InvalidOperationException("Threshold entries are not initialized.");

        for (int i = 0; i < entries.Length; i++)
        {
            ref var entry = ref entries[i];
            entry.cb = cb;
        }
    }

    public void AddOrReplaceDelegate(TEnum label, TDelegator cb)
    {
        if (entries == null) throw new InvalidOperationException("Threshold entries are not initialized.");

        int indx = Array.FindIndex(entries, entry => EqualityComparer<TEnum>.Default.Equals(entry.label, label));
        if (indx == -1) throw new ArgumentException("Threshold does not contain label", nameof(label));
        ref var entry = ref entries[indx];
        entry.cb = cb;
    }

    public float GetHighestThreshold()
    {
        float highest = -1;
        foreach (var entry in entries)
        {
            if (entry.threshold > highest) highest = entry.threshold;
        }
        return highest;
    }
    
    public TEnum GetHighestThresholdLabel()
    {
        float highest = float.MinValue;
        TEnum label = default;
        foreach (var entry in entries)
        {
            if (entry.threshold > highest)
            {
                highest = entry.threshold;
                label = entry.label;
            }
        }
        return label;
    }
    
    public bool GetNearestLastThreshold(float value, out TEnum label, out TDelegator returnCallback)
    {
        if (entries == null || entries.Length == 0)
        {
            label = default;
            returnCallback = default;
            return false;
        }

        Array.Sort(entries, (a, b) => b.threshold.CompareTo(a.threshold));

        int nearest = -1;

        // "Last reached" while moving downward through thresholds.
        // Example thresholds: Alive(10), Dying(3), Dead(0)
        // value 4  -> Alive
        // value 2  -> Dying
        // value 0  -> Dead
        for (int i = 0; i < entries.Length; i++)
        {
            if (value <= entries[i].threshold)
                nearest = i;
            else
                break;
        }

        if (nearest == -1)
        {
            label = default;
            returnCallback = default;
            return false;
        }

        label = entries[nearest].label;
        returnCallback = entries[nearest].cb;
        return true;
    }
    
    public bool WasThresholdReached(float value, out TEnum label, out TDelegator returnCallback)
    {
        if (entries == null || index < 0 || index >= entries.Length)
        {
            label = default;
            returnCallback = default;
            return false;
        }

        ref var entry = ref entries[index];

        if (value <= entry.threshold)
        {
            label = entry.label;
            returnCallback = entry.cb;
            index++; 
            return true;
        }

        label = default;
        returnCallback = default;
        return false;
    }
    
    public void SyncIndexToValue(float value)
    {
        if (entries == null || entries.Length == 0)
        {
            index = -1;
            return;
        }

        Array.Sort(entries, (a, b) => b.threshold.CompareTo(a.threshold));

        int next = 0;
        while (next < entries.Length && value <= entries[next].threshold)
            next++;

        index = next; 
    }
    
    public void LogThresholds(float currentValue)
    {
        if (entries == null || entries.Length == 0)
        {
            Debug.Log("No thresholds configured.");
            return;
        }

        string log = $"[Threshold Debug] Current HP: {currentValue}\n";
        for (int i = 0; i < entries.Length; i++)
        {
            var entry = entries[i];
            string status = (i < index) ? "[REACHED]" : (i == index ? "[NEXT]" : "[PENDING]");
            log += $"{status} {entry.label}: {entry.threshold} (Index: {i})\n";
        }
        Debug.Log(log);
    }
}