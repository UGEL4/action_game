using System;

/// <summary>
/// 阶段性输入百分比
/// </summary>
[Serializable]
public struct MoveInputAcceptance
{
    /// <summary>
    /// 在百分比多少的阶段
    /// </summary>
    public PercentageRange range;
    /// <summary>
    /// 允许百分比
    /// </summary>
    public float rate;
}