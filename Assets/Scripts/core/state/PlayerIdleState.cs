using UnityEngine;

public class PlayerIdleState : BaseState<PlayerFSM.PlayerState>
{
    CharacterObj mOwner;
    public PlayerIdleState(CharacterObj owner) : base(PlayerFSM.PlayerState.Idle)
    {
        mOwner = owner;
    }
    public override void Enter(FSM<PlayerFSM.PlayerState> fsm)
    {
        //mOwner.animator?.SetFloat("Speed", 0.0f);
        mOwner.Animator?.CrossFade("idle", 0.1f);
    }
    public override void Update(FSM<PlayerFSM.PlayerState> fsm)
    {

    }
    public override void Exit(FSM<PlayerFSM.PlayerState> fsm)
    {
    }

    public override PlayerFSM.PlayerState GetNextState()
    {
        return PlayerFSM.PlayerState.Idle;
    }
}