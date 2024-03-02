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
