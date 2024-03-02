using System;

[Serializable]
public struct AttackInfo
{
    public int attackPhase;
    //攻击倍数
    public float attackPower;
    //攻击能打中同一个对象的次数
    public int canHitSameTarget;
    public float hitSameTargetDelay;
    public float hitStun;

    //攻击的推力
    public MoveInfo pushPower;

    //卡帧
    public float freeze;

    public ActionChangeInfo selfActionChangeInfo;
    public ActionChangeInfo targetActionChangeInfo;

    public string[] tmpBeCancelledTagTurnOn;
}