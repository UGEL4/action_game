using System;

[Serializable]
public struct AttackBoxTurnOnInfo
{
    /// <summary>
    /// 开启的时间段, 第几帧到第几帧之间开启
    /// </summary>
    public FrameIndexRange FrameIndexRange;
    public string[] AttackBoxTag;

    /// <summary>
    /// 这段攻击的逻辑数据是ActionInfo中的哪个AttackInfo
    /// </summary>
    public int AttackPhase;

    public int Priority;
}
