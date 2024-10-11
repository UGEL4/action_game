using System;
using UnityEngine;

[Serializable]
public struct PercentageRange
{
    public float min;
    public float max;

    public PercentageRange(float min, float max)
    {
        this.min = Mathf.Clamp01(min);
        this.max = Mathf.Clamp01(max);
    }
}

[Serializable]
public struct FrameIndexRange
{
    public uint min;
    public uint max;

    public FrameIndexRange(uint min, uint max)
    {
        this.min = min;
        this.max = max;
    }
}
