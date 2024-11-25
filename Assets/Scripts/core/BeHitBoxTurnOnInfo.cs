using System;

[Serializable]
public struct BeHitBoxTurnOnInfo
{
    public FrameIndexRange FrameIndexRange;

    public string[] Tags;
    public string[] TempTurnOnBeCancelTags;
    public int Priority;

    public ActionChangeInfo SelfActionChangeInfo;
    public ActionChangeInfo TargetActionChangeInfo;
}
