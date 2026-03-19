using System;
using System.Collections.Generic;
using EMILtools.Systems;
using Sirenix.OdinInspector;
using UnityEngine;


public interface IFSM
{
    public void AddTransition(IState from, IState to, IPredicate condition);
    public void AddAnyTransition(IState to, IPredicate condition);
    public void PollTransitions();
    public IState State<TState>() where TState : IState;
}

public class StateMachine<TViewCtx> : IFSM
    where TViewCtx : IContextViewImmutable
{
    [ShowInInspector] StateNode CurrentNode;
    [ShowInInspector] List<ITransition> AnyTransitions;
    Dictionary<Type, StateNode> Nodes;

    readonly TViewCtx Context;
    [ShowInInspector, ReadOnly] List<string> states;
    
    public StateMachine(TViewCtx ctx, IState initialState)
    {
        Context = ctx;
        Nodes = new Dictionary<Type, StateNode>();
        CurrentNode = GetOrAddNode(initialState);
        CurrentNode.State?.OnEnterState(Context);
        AnyTransitions = new List<ITransition>();
    }

    public void PollTransitions()
    {
        var transition = GetTransition(out var hasTransition);
        if(hasTransition) ChangeState(transition);
    }

    /// <summary>
    /// Only use if you know the state has been added
    /// </summary>
    /// <typeparam name="TState"></typeparam>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    public IState State<TState>() where TState: IState
    {
        var type = typeof(TState);
        if(!Nodes.TryGetValue(type, out var node)) throw new Exception($"State {type} not found");
        return node.State;
    }

    public void SetState(IState state)
    {
        if(state == null) return;
        CurrentNode = GetOrAddNode(state);
        CurrentNode.State?.OnExitState(Context);
    }

    ITransition GetTransition(out bool hasTransition)
    {
        hasTransition = false;
        
        for(int i = 0; i < AnyTransitions.Count; i++)
            if (AnyTransitions[i].Condition.Evaluate())
            {
                hasTransition = true;
                return AnyTransitions[i];
            }
        
        for(int i = 0; i < CurrentNode.Transitions.Count; i++)
            if(CurrentNode.Transitions[i].Condition.Evaluate())
            {
                hasTransition = true;
                return CurrentNode.Transitions[i];
            }

        return null;
    }

    
    
    public void AddTransition(IState from, IState to, IPredicate condition) 
        => GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    
    public void AddAnyTransition(IState to, IPredicate condition)
        => AnyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));

    public void AddNode(IState state) => GetOrAddNode(state);

    StateNode GetOrAddNode(IState state)
    {
        var node = Nodes.GetValueOrDefault(state.GetType());

        if (node == null)
        {
            node = new StateNode(state);
            Nodes.Add(state.GetType(), node);
            if(states == null) states = new List<string>();
            states.Add(node.name);
        }
        return node;
    }

    void ChangeState(ITransition transition)
    {
        if (CurrentNode.State == transition.To) return;

        var prev = CurrentNode.State;
        var next = Nodes[transition.To.GetType()].State;
        
        prev?.OnExitState(Context);
        next?.OnEnterState(Context);
        
        CurrentNode = Nodes[transition.To.GetType()];
    }
    
    public class StateNode
    {
        [ShowInInspector] public readonly string name;
        [HideInInspector] public IState State;
        public List<ITransition> Transitions;
        
        public StateNode(IState state)
        {
            State = state;
            Transitions = new List<ITransition>();
            name = state.GetType().Name;
        }
        
        public void AddTransition(IState to, IPredicate condition) => Transitions.Add(new Transition(to, condition));
        
    }
}