using System;
using UnityEngine;

[Serializable]
public enum AttackHitBoxType
{
    Box = 0,
    Ray,
    Count
}

[Serializable]
public struct AttackHitBox
{
    public string Tag;
    public Character Owner;
    public bool Active;
    public AttackHitBoxType BoxType;

    public AttackHitBox(Character owner, string tag, AttackHitBoxType boxType, bool active)
    {
        Owner   = owner;
        Tag     = tag;
        BoxType = boxType;
        Active  = active;
    }
}
