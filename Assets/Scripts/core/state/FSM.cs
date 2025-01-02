using System;
using System.Collections.Generic;
using UnityEngine;

public class FSM<EState>  where EState : Enum
{
    protected StateNode mCurState;
    protected Dictionary<EState, StateNode> mStateNodes = new Dictionary<EState, StateNode>();
    protected HashSet<StateTransition<EState>> mAnyTransitions = new HashSet<StateTransition<EState>>();
    protected bool mIsChangingState = false;

    public CharacterObj owner;

    public void Update()
    {
        StateTransition<EState> transition = GetTransition();
        if (transition != null) ChangeState(transition.To);

        mCurState.State?.Update(this);
    }

    public void ChangeState(BaseState<EState> state)
    {
        mIsChangingState = true;
        if (state == mCurState.State)
        {
            mIsChangingState = false;
            return;
        }

        var prev  = mCurState.State;
        mCurState = mStateNodes[state.StateKey];
        prev?.Exit(this);
        mCurState.State?.Enter(this);
        mIsChangingState = false;
    }

    public void SetState(EState state)
    {
        mCurState = mStateNodes[state];
        if (mCurState != null)
        {
            mCurState.State?.Enter(this);
        }
    }

    public void AddTransition(BaseState<EState> from, BaseState<EState> to, StatePredicate condition)
    {
        GetOrAddNode(from).AddTransition(GetOrAddNode(to).State, condition);
    }

    public void AddAnyTransition(BaseState<EState> to, StatePredicate condition)
    {
        mAnyTransitions.Add(new StateTransition<EState>(GetOrAddNode(to).State, condition));
    }

    protected StateNode GetOrAddNode(BaseState<EState> state)
    {
        var node = mStateNodes.GetValueOrDefault(state.StateKey);
        if (node == null)
        {
            node = new StateNode(state);
            mStateNodes.Add(state.StateKey, node);
        }
        return node;
    }

    protected StateTransition<EState> GetTransition()
    {
        foreach (var transition in mAnyTransitions)
        {
            if (transition.Condition.Evaluate()) return transition;
        }

        foreach (var transition in mCurState.Transitions)
        {
            if (transition.Condition.Evaluate()) return transition;
        }
        return null;
    }
    protected class StateNode
    {
        public BaseState<EState> State { get; }
        public HashSet<StateTransition<EState>> Transitions { get; }

        public StateNode(BaseState<EState> state)
        {
            State = state;
            Transitions = new HashSet<StateTransition<EState>>();
        }

        public void AddTransition(BaseState<EState> to, StatePredicate condition)
        {
            Transitions.Add(new StateTransition<EState>(to, condition));
        }
    }
}