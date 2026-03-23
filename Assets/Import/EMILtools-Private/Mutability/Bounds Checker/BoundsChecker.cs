using System;
using System.Collections.Generic;
using EMILtools.Extensions;
using Sirenix.OdinInspector;
using UnityEngine;

public abstract class BoundsChecker<TContext> : MonoBehaviour
{
    
    [Header("What type of Bounds Checker?")]
    public bool is2D;

    [Header("Who will receive the Message?")]
    [SerializeField] private bool ThingCollidedWith;
    [SerializeField] private bool SelectedReceiver;

    [SerializeField, ShowIf("@SelectedReceiver && !is2D")]
    private InterfaceReference<IBoundsCheckMsgReceiver<Collider, TContext>, MonoBehaviour> selectedReceiver3D;

    [SerializeField, ShowIf("@SelectedReceiver && is2D")]
    private InterfaceReference<IBoundsCheckMsgReceiver<Collider2D, TContext>, MonoBehaviour> selectedReceiver2D;

    [Header("Which trigger callbacks are active?")]
    [SerializeField] private bool enter = true;
    [SerializeField] private bool exit = true;
    [SerializeField] private bool stay;

        
    [Header("Layer filtering")]
    [SerializeField] private LayerMask layerMask = ~0;
    
    
    [BoxGroup("Context")] [Header("What Message is being sent?")]
    [BoxGroup("Context")] [ShowIf("enter")] [SerializeField] public TContext enterContext;
    [BoxGroup("Context")] [ShowIf("stay")][SerializeField] public TContext stayContext;
    [BoxGroup("Context")] [ShowIf("exit")][SerializeField] public TContext exitContext;
    

    private HashSet<IBoundsCheckMsgReceiver<Collider, TContext>> collisions3D;
    private HashSet<IBoundsCheckMsgReceiver<Collider2D, TContext>> collisions2D;

    void Awake()
    {
        if (is2D)
        {
            this.Get<Collider2D>().isTrigger = true;
            if (ThingCollidedWith)
                collisions2D = new HashSet<IBoundsCheckMsgReceiver<Collider2D, TContext>>();

            if (SelectedReceiver && selectedReceiver2D.Value == null)
                Debug.LogError("No 2D Receiver Selected");
        }
        else
        {
            this.Get<Collider>().isTrigger = true;
            if (ThingCollidedWith)
                collisions3D = new HashSet<IBoundsCheckMsgReceiver<Collider, TContext>>();

            if (SelectedReceiver && selectedReceiver3D.Value == null)
                Debug.LogError("No 3D Receiver Selected");
        }
    }

    bool PassesLayerMask(GameObject go) =>
        (layerMask.value & (1 << go.layer)) != 0;


    void OnDisable()
    {
       if(is2D) collisions2D?.Clear();
       else collisions3D?.Clear();
    }

    // -------------------- 3D --------------------

    private void OnTriggerEnter(Collider other)
    {
        if (is2D || !enter || !PassesLayerMask(other.gameObject)) return;

        if (ThingCollidedWith)
        {
            if (!other.TryGetComponent(out IBoundsCheckMsgReceiver<Collider, TContext> receiver)) return;
            if (!collisions3D.Add(receiver)) return;
            receiver.OnEnterBounds(other, this, enterContext);
        }

        if (SelectedReceiver)
            selectedReceiver3D.Value?.OnEnterBounds(other, this, enterContext);
    }

    private void OnTriggerExit(Collider other)
    {
        if (is2D || !PassesLayerMask(other.gameObject)) return;

        if (ThingCollidedWith)
        {
            if (!other.TryGetComponent(out IBoundsCheckMsgReceiver<Collider, TContext> receiver)) return;
            if (!collisions3D.Remove(receiver)) return;
            if (!exit) return;
            receiver.OnExitBounds(other, this, exitContext);
        }

        if (!exit) return;
        if (SelectedReceiver)
            selectedReceiver3D.Value?.OnExitBounds(other, this, exitContext);
    }

    private void OnTriggerStay(Collider other)
    {
        if (is2D || !stay || !PassesLayerMask(other.gameObject)) return;

        if (ThingCollidedWith)
        {
            if (!other.TryGetComponent(out IBoundsCheckMsgReceiver<Collider, TContext> receiver)) return;
            receiver.OnStayBounds(other, this, stayContext);
        }

        if (SelectedReceiver)
            selectedReceiver3D.Value?.OnStayBounds(other, this, stayContext);
    }

    // -------------------- 2D --------------------

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!is2D || !enter || !PassesLayerMask(other.gameObject)) return;

        if (ThingCollidedWith)
        {
            if (!other.TryGetComponent(out IBoundsCheckMsgReceiver<Collider2D, TContext> receiver)) return;
            if (!collisions2D.Add(receiver)) return;
            receiver.OnEnterBounds(other, this, enterContext);
        }

        if (SelectedReceiver)
            selectedReceiver2D.Value?.OnEnterBounds(other, this, enterContext);
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (!is2D || !PassesLayerMask(other.gameObject)) return;

        if (ThingCollidedWith)
        {
            if (!other.TryGetComponent(out IBoundsCheckMsgReceiver<Collider2D, TContext> receiver)) return;
            if (!collisions2D.Remove(receiver)) return;
            if (!exit) return;
            receiver.OnExitBounds(other, this, exitContext);
        }

        if (!exit) return;

        if (SelectedReceiver)
            selectedReceiver2D.Value?.OnExitBounds(other, this, exitContext);
    }

    private void OnTriggerStay2D(Collider2D other)
    {
        if (!is2D || !stay || !PassesLayerMask(other.gameObject)) return;

        if (ThingCollidedWith)
        {
            if (!other.TryGetComponent(out IBoundsCheckMsgReceiver<Collider2D, TContext> receiver)) return;
            receiver.OnStayBounds(other, this, stayContext);
        }

        if (SelectedReceiver)
            selectedReceiver2D.Value?.OnStayBounds(other, this, stayContext);
    }
}