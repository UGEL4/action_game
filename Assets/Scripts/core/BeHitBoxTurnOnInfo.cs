using System;

[Serializable]
public struct BeHitBoxTurnOnInfo
{
    public FrameIndexRange FrameIndexRange;

    public string[] Tags;
    public string[] TempTurnOnBeCancelTags;
    public int Priority;

    //被攻击时的动作变化
    public ActionChangeInfo SelfActionChangeInfo;
    //攻击方的动作变化
    public ActionChangeInfo TargetActionChangeInfo;
}
