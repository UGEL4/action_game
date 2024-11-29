using System;
using ACTTools;

[Serializable]
public struct AttackBoxTurnOnInfo
{
    /// <summary>
    /// 开启的时间段, 第几帧到第几帧之间开启
    /// </summary>
    public FrameIndexRange FrameIndexRange;
    public PositionRotationData[] RayPointTransforms; // 射线点在开启时所有帧的位置和旋转
    public SimpleColliderBox[] AttackBoxes; // 攻击盒在开启时所有帧的数据
    //public string[] AttackBoxTag; //碰撞盒在开启时所有帧的数据

    /// <summary>
    /// 这段攻击的逻辑数据是ActionInfo中的哪个AttackInfo
    /// </summary>
    public int AttackPhase;

    public int Priority;
}
