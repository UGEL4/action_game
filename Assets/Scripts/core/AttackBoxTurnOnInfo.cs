using System;
using ACTTools;

[Serializable]
public struct AttackRayPointData
{
    public PositionRotationData[] RayPointTransforms; //射线点在开启时所有帧的位置和旋转
}

[Serializable]
public struct AttackRayPointGroup
{
    public AttackRayPointData[] Points; //所有射线点的数据
}

[Serializable]
public struct AttackBoxTurnOnInfo
{
    /// <summary>
    /// 开启的时间段, 第几帧到第几帧之间开启
    /// </summary>
    public FrameIndexRange[] FrameIndexRange; //每组射线的开启帧
    public AttackRayPointGroup[] RayPointGroupList; // 所有射线组的数据
    public SimpleColliderBox[] AttackBoxes; // 攻击盒在开启时所有帧的数据
    //public string[] AttackBoxTag; //碰撞盒在开启时所有帧的数据

    /// <summary>
    /// 这段攻击的逻辑数据是ActionInfo中的哪个AttackInfo
    /// </summary>
    public int AttackPhase;

    public int Priority;
}
