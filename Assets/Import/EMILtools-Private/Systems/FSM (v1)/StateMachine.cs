using System;
using System.Collections.Generic;

public class StateMachine
{
    StateNode CurrentNode;
    Dictionary<Type, StateNode> Nodes;
    List<ITransition> AnyTransitions;
    
    public StateMachine(IState initialState)
    {
        Nodes = new Dictionary<Type, StateNode>();
        CurrentNode = GetOrAddNode(initialState);
        CurrentNode.State?.OnEnter();
        AnyTransitions = new List<ITransition>();
    }

    public void Update()
    {
        var transition = GetTransition();
        if(transition != null) ChangeState(transition);
        
        CurrentNode.State?.OnUpdate();
    }

    public void FixedUpdate()
    {
        CurrentNode.State?.OnFixedUpdate();
    }

    public void SetState(IState state)
    {
        if(state == null) return;
        CurrentNode = GetOrAddNode(state);
        CurrentNode.State?.OnEnter();
    }

    ITransition GetTransition()
    {
        for(int i = 0; i < AnyTransitions.Count; i++)
            if(AnyTransitions[i].Condition.Evaluate()) return AnyTransitions[i];
        
        for(int i = 0; i < CurrentNode.Transitions.Count; i++)
            if(CurrentNode.Transitions[i].Condition.Evaluate()) return CurrentNode.Transitions[i];

        return null;
    }
    
    public void AddTransition(IState from, IState to, IPredicate condition) 
        => GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    
    public void AddAnyTransition(IState to, IPredicate condition)
        => AnyTransitions.Add(new Transition(GetOrAddNode(to).State, condition));


    StateNode GetOrAddNode(IState state)
    {
        var node = Nodes.GetValueOrDefault(state.GetType());

        if (node == null)
        {
            node = new StateNode(state);
            Nodes.Add(state.GetType(), node);
        }
        return node;
    }

    void ChangeState(ITransition transition)
    {
        if (CurrentNode.State == transition.To) return;

        var prev = CurrentNode.State;
        var next = Nodes[transition.To.GetType()].State;
        
        prev?.OnExit();
        next?.OnEnter();
        
        CurrentNode = Nodes[transition.To.GetType()];
    }
    
    class StateNode
    {
        public IState State;
        public List<ITransition> Transitions;
        
        public StateNode(IState state)
        {
            State = state;
            Transitions = new List<ITransition>();
        }
        
        public void AddTransition(IState to, IPredicate condition) => Transitions.Add(new Transition(to, condition));
        
    }
}