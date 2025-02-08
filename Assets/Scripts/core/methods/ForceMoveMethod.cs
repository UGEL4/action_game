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
                return EaseOutQuint(forceMove);
                //return EaseOutQuart(forceMove);
            }
        }
    };

    // t: 当前时间 (current time)
    // b: 起始值 (beginning value)
    // c: 变化量 (change in value = 目标值 - 起始值)
    // d: 动画持续时间 (duration)
    public static Vector3 EaseOutQuint(ForceMove forceMove)
    {
        float t     = (float)forceMove.WasElapsed / forceMove.Data.InFrame - 1;
        float t1    = (float)forceMove.FrameElapsed / forceMove.Data.InFrame - 1;
        Vector3 was = forceMove.Data.moveDistance * (t * t * t * t * t + 1);
        Vector3 cur = forceMove.Data.moveDistance * (t1 * t1 * t1 * t1 * t1 + 1);
        // SimpleLog.Info("ccc: ", y1 - y);
        return cur - was;
    }

    public static Vector3 EaseOutQuart(ForceMove forceMove)
    {
        float t     = (float)forceMove.WasElapsed / forceMove.Data.InFrame;
        float t1    = (float)forceMove.FrameElapsed / forceMove.Data.InFrame;
        t = 1 - Mathf.Pow(1 - t, 4);
        t1 = 1 - Mathf.Pow(1 - t1, 4);
        Vector3 was = forceMove.Data.moveDistance * t;
        Vector3 cur = forceMove.Data.moveDistance * t1;
        return cur - was;
    }

    //击飞：包括了xz平面的位移和垂直y的位移，类似抛物线的轨迹
    public static void KnockForce(ForceMove forceMove)
    {

    }
}