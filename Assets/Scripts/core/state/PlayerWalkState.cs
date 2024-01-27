using UnityEngine;

public class PlayerWalkState : BaseState<PlayerFSM.PlayerState>
{
    Character mOwner;
    public PlayerWalkState(Character owner) : base(PlayerFSM.PlayerState.Walk)
    {
        mOwner = owner;
    }
    public override void Enter(FSM<PlayerFSM.PlayerState> fsm)
    {
        //Debug.Log("PlayerWalkState.Enter");
        //mOwner.animator?.SetFloat("Speed", 1.0f);
        mOwner.animator?.CrossFade("walking", 0.9f);
    }
    public override void Update(FSM<PlayerFSM.PlayerState> fsm)
    {

    }
    public override void Exit(FSM<PlayerFSM.PlayerState> fsm)
    {
        //Debug.Log("PlayerWalkState.Exit");
    }

    public override PlayerFSM.PlayerState GetNextState()
    {
        return PlayerFSM.PlayerState.Walk;
    }
}