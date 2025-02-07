using System;
using UnityEngine;

public class ForceMove
{
    public MoveInfo Data;
    public int FrameElapsed = 0;
    public int WasElapsed   = 0;
    public Func<ForceMove, Vector3> MoveTween =
    _ => Vector3.zero;

    public static ForceMove FromData(MoveInfo data) => new ForceMove {
        Data         = data,
        FrameElapsed = 0,
        WasElapsed   = 0,
        MoveTween    = ForceMoveMethod.Methods.ContainsKey(data.tweenMethod) ? ForceMoveMethod.Methods[data.tweenMethod] :
                                                                               _ => Vector3.zero
    };

    public static ForceMove NoForceMove => new ForceMove {
        Data         = new MoveInfo(),
        WasElapsed   = int.MaxValue,
        FrameElapsed = int.MaxValue,
        MoveTween =
        _ => Vector3.zero
    };

    public void Update()
    {
        WasElapsed = FrameElapsed;
        FrameElapsed += 1;
    }
}