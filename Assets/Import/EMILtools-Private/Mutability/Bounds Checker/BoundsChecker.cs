using System;
using System.Collections.Generic;
using EMILtools.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public class BoundsChecker : MonoBehaviour
{
    
    [Header("Who will receive the Message?")]
    [SerializeField] bool ThingCollidedWith;
    [SerializeField] bool SelectedReceiver;
    [SerializeField] [ShowIf("SelectedReceiver")] InterfaceReference<IBoundsCheckMsgReceiver, MonoBehaviour> selectedReceiver;

    
    [Header("Which trigger callbacks are active?")]
    [SerializeField] private bool enter = true;
    [SerializeField] private bool exit = true;
    [SerializeField] private bool stay;

    [Header("Layer filtering")]
    [SerializeField] private LayerMask layerMask = ~0;

    HashSet<IBoundsCheckMsgReceiver> collisions;

    void Awake()
    {
        this.Get<Collider>().isTrigger = true;
        if(ThingCollidedWith) collisions = new HashSet<IBoundsCheckMsgReceiver>();
        if(selectedReceiver == null) Debug.LogError("No Receiver Selected");
        if(selectedReceiver.Value == null) Debug.LogError("No Receiver Selected");
    }

    bool PassesLayerMask(GameObject go) => (layerMask.value & (1 << go.layer)) != 0;

    private void OnTriggerEnter(Collider other)
    {
        if (!enter) return;
        if (!PassesLayerMask(other.gameObject)) return;

        if (ThingCollidedWith)
        {
            if (!other.TryGetComponent(out IBoundsCheckMsgReceiver msgReceiver)) return;
            if (!collisions.Add(msgReceiver)) return;
            msgReceiver.OnEnterBounds(other, this);
        }

        if (SelectedReceiver)
        {
            selectedReceiver.Value.OnEnterBounds(other, this);
            Debug.Log("Entered");
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (!exit) return;
        if (!PassesLayerMask(other.gameObject)) return;

        if (ThingCollidedWith)
        {
            if (!other.TryGetComponent(out IBoundsCheckMsgReceiver msgReceiver)) return;
            if (!collisions.Remove(msgReceiver)) return;
            msgReceiver.OnExitBounds(other, this);
        }
        if(SelectedReceiver) selectedReceiver.Value.OnExitBounds(other, this);
    }

    private void OnTriggerStay(Collider other)
    {
        if (!stay) return;
        if (!PassesLayerMask(other.gameObject)) return;

        if (ThingCollidedWith)
        {
            if (!other.TryGetComponent(out IBoundsCheckMsgReceiver msgReceiver)) return;
            msgReceiver.OnStayBounds(other, this);
        }
        if(SelectedReceiver) selectedReceiver.Value.OnStayBounds(other, this);
    }
}