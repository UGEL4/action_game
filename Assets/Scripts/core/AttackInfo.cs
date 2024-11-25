using System;

[Serializable]
public struct AttackInfo
{
    public int AttackPhase;
    //攻击倍数
    public float AttackPower;
    //攻击能打中同一个对象的次数
    public int CanHitSameTarget;
    public float HitSameTargetDelay;
    public float HitStun;

    //攻击的推力
    public MoveInfo PushPower;

    //卡帧
    public float Freeze;

    public ActionChangeInfo SelfActionChangeInfo;
    public ActionChangeInfo TargetActionChangeInfo;

    public string[] TempTurnOnBeCanceledTags;
}