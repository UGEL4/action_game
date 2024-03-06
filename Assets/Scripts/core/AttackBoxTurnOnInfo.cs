using System;

[Serializable]
public struct AttackBoxTurnOnInfo
{
    public PercentageRange[] turnOnRangeList;
    public string[] attackBoxTag;
    public int attackPhase;
}
