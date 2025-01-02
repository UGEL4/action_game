using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitRecord
{
    //击中目标的id
    public int Id;

    //第几段上伤害
    public int Phase;

    //还能命中几次
    public int CanHitCount;

    //多少帧之后能再次命中
    public int CooldownFrame;

    public HitRecord(CharacterObj ch, int phase, int canHitCount, int coolDown)
    {
        Id            = ch.gameObject.GetInstanceID();
        Phase         = phase;
        CanHitCount   = canHitCount;
        CooldownFrame = coolDown;
    }

    public void Update()
    {
        if (CooldownFrame > 0)
        {
            CooldownFrame--;
        }
    }
}
