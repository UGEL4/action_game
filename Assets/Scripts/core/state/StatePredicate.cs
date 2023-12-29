using System;

public class StatePredicate
{
    readonly Func<bool> func;
    public StatePredicate(Func<bool> func)
    {
        this.func = func;
    }

    public bool Evaluate()
    {
        return func.Invoke();
    }
}