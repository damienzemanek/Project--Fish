using System;
using System.Collections;
using System.Collections.Generic;
using EMILtools.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public class FinisherEvent : MonoBehaviour
{
    public bool active = false;
    public enum Traversal
    {
        Sequential,
    }

    [Serializable]
    public struct Spawns
    {
        public List<Spawn> spawns;
        public float neededActionsToComplete;
        
        public void CalculateNeddedActionsToComplete() => neededActionsToComplete = spawns.Count;
    }

    [Serializable]
    public struct Spawn
    {
        public GameObject prefab;
        public Vector3 offset;
        public float spawnDelay;
    }

    public InterfaceReference<ISignalReceiverTaggedContext<bool>, MonoBehaviour> finisherSignalReceiver;
    public List<Spawns> prefabChoices;
    public Transform spawnPoint;
    public Traversal traversal;
    
    [ReadOnly] public Ref<bool> stopEarly = new Ref<bool>(false);
    [ReadOnly] public int current = 0;
    [ReadOnly] public List<GameObject> currentlySpawned = new();


    Coroutine finisherEvent;

    void Awake()
    {
        if (prefabChoices == null || prefabChoices.Count <= 0)
            Debug.LogError("Prefab choices cannot be null or empty!");
        CalculateNeededActionsToComplete();
    }

    [Button]
    public void CalculateNeededActionsToComplete()
    {
        for (var index = 0; index < prefabChoices.Count; index++)
        {
            var spawns = prefabChoices[index];
            spawns.CalculateNeddedActionsToComplete();
            prefabChoices[index] = spawns;
        }
    }
    
    public void StartEvent() => finisherEvent = StartCoroutine(C_StartEvent());
    
    IEnumerator C_StartEvent()
    {
        active = true;
        stopEarly.val = false;
        currentlySpawned.Clear();
        Spawns spawns = prefabChoices[current];

        foreach (Spawn spawn in spawns.spawns)
        {
            if(stopEarly.val) yield break;
            yield return new WaitForSeconds(spawn.spawnDelay);
            if(stopEarly.val) yield break;
            
            var newObj = Instantiate(spawn.prefab, spawnPoint);
            newObj.transform.localPosition += spawn.offset;
            var choice = newObj.GetComponent<FinisherChoice>();
            choice.events = this;
            choice.PlayEvent();
            currentlySpawned.Add(newObj);
        }
        
        current++;
        if(current >= prefabChoices.Count) current = 0;
    }
    
    public void StopEarly()
    {
        stopEarly.val = true;
        Stop();
    }

    public void Stop()
    {
        foreach (var obj in currentlySpawned) Destroy(obj);
        currentlySpawned.Clear();
        active = false; 
    }


    public void OnExit()
    {
        
    }
}

