using System;
using System.Collections.Generic;

public class PlayerFSM : FSM<PlayerFSM.PlayerState>
{
    public enum PlayerState
    {
        None,
        Idle,
        Walk,
        Run,
        Sprint,
        Max
    };

    Character mOwner;

    public PlayerFSM(Character owner)
    {
        mOwner = owner;

        var idleState = new PlayerIdleState(mOwner);
        var walkState = new PlayerWalkState(mOwner);
        At(idleState, walkState, new StatePredicate(()=> mOwner.IsMoving()));
        At(walkState, idleState, new StatePredicate(()=> !mOwner.IsMoving()));
    }

    public void Start()
    {
        SetState(mStateNodes.GetValueOrDefault(PlayerState.Idle).State.StateKey);
    }


    void At(BaseState<PlayerFSM.PlayerState> from, BaseState<PlayerFSM.PlayerState> to, StatePredicate condition)
    {
        AddTransition(from, to, condition);
    }
    

    void Any(BaseState<PlayerFSM.PlayerState> to, StatePredicate condition)
    {
        AddAnyTransition(to, condition);
    }
}