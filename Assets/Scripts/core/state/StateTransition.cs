using System;

public class StateTransition<EState> where EState : Enum
{
    public BaseState<EState> To { get; }
    public StatePredicate Condition { get; }

    public StateTransition(BaseState<EState> to, StatePredicate condition)
    {
        To = to;
        Condition = condition;
    }
}