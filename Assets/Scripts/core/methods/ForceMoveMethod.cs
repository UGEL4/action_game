using System;
using System.Collections.Generic;
using Log;
using UnityEngine;

public static class ForceMoveMethod
{
    public static Dictionary<string, Func<ForceMove, Vector3>> Methods = new Dictionary<string, Func<ForceMove, Vector3>>()
    {
        {
            "Slowly",
            (forceMove) =>
            {
                if (forceMove.Data.InFrame <= 0) return Vector3.zero;
                // float wasPec = Mathf.Clamp01((float)forceMove.WasElapsed / forceMove.Data.InFrame);
                // float curPec = Mathf.Clamp01((float)forceMove.FrameElapsed / forceMove.Data.InFrame);
                // // 因为愚蠢的unity坐标系和正常游戏坐标系是反过来的，所以y负数向上，但是游戏开发的宇宙标准是y正向上
                // // 所以我们得为策划填表做个翻译，就是给他的y反个向
                // float wasRate = 1.00f - Mathf.Pow(1.00f - wasPec, 3);
                // float curRate = 1.00f - Mathf.Pow(1.00f - curPec, 3);
                // Vector3 was   = forceMove.Data.moveDistance * wasPec;
                // Vector3 cur   = forceMove.Data.moveDistance * curPec;
                // return cur - was;
                float pec = Mathf.Clamp01((float)forceMove.FrameElapsed / forceMove.Data.InFrame);
                Vector3 cur = forceMove.Data.moveDistance * pec;
                SimpleLog.Info("cur:", pec, cur);
                return cur;
            }
        }
    };
}