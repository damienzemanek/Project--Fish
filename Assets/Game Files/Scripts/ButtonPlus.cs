using System;
using System.Collections.Generic;
using EMILtools.Systems;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.EventSystems;


public class ButtonPlus : MonoBehaviour, IPointerEnterHandler, IPointerClickHandler, IPointerDownHandler, IPointerExitHandler, IPointerUpHandler
{
    [Flags]
    public enum ButtonType
    {
        Click = 1 << 0,
        Enter = 1 << 1, 
        Exit = 1 << 2,
        Up = 1 << 3,
        Down = 1 << 4
    }
    
    public ButtonType triggerType;

    [SerializeReference][ShowIf("@triggerType.HasFlag(ButtonType.Click)")] public StateMachine<IContextViewImmutable>.StateNode onClickState;
    [SerializeReference][ShowIf("@triggerType.HasFlag(ButtonType.Enter)")] public StateMachine<IContextViewImmutable>.StateNode onEnterState;
    [SerializeReference][ShowIf("@triggerType.HasFlag(ButtonType.Exit)")] public StateMachine<IContextViewImmutable>.StateNode onExitState;
    [SerializeReference][ShowIf("@triggerType.HasFlag(ButtonType.Up)")] public StateMachine<IContextViewImmutable>.StateNode onUpState;
    [SerializeReference][ShowIf("@triggerType.HasFlag(ButtonType.Down)")] public StateMachine<IContextViewImmutable>.StateNode onDownState;
    
    [Serializable]
    public abstract class ButtonEvent : StateMachine<IContextViewImmutable>.IButtonEvent
    {
        public abstract void Execute();
        public abstract void DefaultState();
    }

    [Serializable]
    public class SetActive : ButtonEvent
    {
        public GameObject target;
        public bool swapDefaultState;
        public override void Execute() => target.SetActive(!target.activeSelf);
        public override void DefaultState() => target.SetActive(swapDefaultState);
    }

    [Serializable]
    public class ButtonUnityEvent : ButtonEvent
    {
        public UnityEngine.Events.UnityEvent eventToExecute;
        public override void Execute() => eventToExecute.Invoke();
        public override void DefaultState() { }
    }

    [Serializable]
    public class Animate : ButtonEvent
    {
        public Animator target;
        public string animationName;
        public override void Execute() => target.Play(animationName);
        public override void DefaultState() { }
    }

    private void Awake() => ResetButtonEvents();
#if UNITY_EDITOR
    private void OnValidate() => ResetButtonEvents();
    #endif

    public void ResetButtonEvents()
    {
        onClickState?.ResetButtonEvents();
        onEnterState?.ResetButtonEvents();
        onExitState?.ResetButtonEvents();
        onUpState?.ResetButtonEvents();
        onDownState?.ResetButtonEvents();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (!triggerType.HasFlag(ButtonType.Enter) || onEnterState == null) return;
        onEnterState.onEnterEvents.ForEach(x => x.Execute());
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (!triggerType.HasFlag(ButtonType.Click) || onClickState == null) return;
        onClickState.onEnterEvents.ForEach(x => x.Execute());
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        if (!triggerType.HasFlag(ButtonType.Down) || onDownState == null) return;
        onDownState.onEnterEvents.ForEach(x => x.Execute());
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (!triggerType.HasFlag(ButtonType.Exit) || onExitState == null) return;
        onExitState.onEnterEvents.ForEach(x => x.Execute());
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        if (!triggerType.HasFlag(ButtonType.Up) || onUpState == null) return;
        onUpState.onEnterEvents.ForEach(x => x.Execute());
    }
}
