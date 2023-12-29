using System;

public abstract class BaseState<EState> where EState : Enum
{
    public EState StateKey {get; private set;}

    public BaseState(EState key)
    {
        StateKey = key;
    }
    public abstract void Enter(FSM<EState> fsm);
    public abstract void Update(FSM<EState> fsm);
    public abstract void Exit(FSM<EState> fsm);
    public abstract EState GetNextState();
}