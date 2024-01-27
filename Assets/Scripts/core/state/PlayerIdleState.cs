using UnityEngine;

public class PlayerIdleState : BaseState<PlayerFSM.PlayerState>
{
    Character mOwner;
    public PlayerIdleState(Character owner) : base(PlayerFSM.PlayerState.Idle)
    {
        mOwner = owner;
    }
    public override void Enter(FSM<PlayerFSM.PlayerState> fsm)
    {
        //Debug.Log("PlayerIdleState.Enter");
        //mOwner.animator?.SetFloat("Speed", 0.0f);
        mOwner.animator?.CrossFade("idle", 0.1f);
    }
    public override void Update(FSM<PlayerFSM.PlayerState> fsm)
    {

    }
    public override void Exit(FSM<PlayerFSM.PlayerState> fsm)
    {
        //Debug.Log("PlayerIdleState.Exit");
    }

    public override PlayerFSM.PlayerState GetNextState()
    {
        return PlayerFSM.PlayerState.Idle;
    }
}