using UnityEngine;

public class PlayerWalkState : BaseState<PlayerFSM.PlayerState>
{
    CharacterObj mOwner;
    public PlayerWalkState(CharacterObj owner) : base(PlayerFSM.PlayerState.Walk)
    {
        mOwner = owner;
    }
    public override void Enter(FSM<PlayerFSM.PlayerState> fsm)
    {
        //mOwner.animator?.SetFloat("Speed", 1.0f);
        mOwner.Animator?.CrossFade("walking", 0.9f);
    }
    public override void Update(FSM<PlayerFSM.PlayerState> fsm)
    {

    }
    public override void Exit(FSM<PlayerFSM.PlayerState> fsm)
    {
    }

    public override PlayerFSM.PlayerState GetNextState()
    {
        return PlayerFSM.PlayerState.Walk;
    }
}