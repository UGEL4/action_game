using System;
using UnityEngine;

[Serializable]
public struct AttackHitBox
{
    //碰撞盒
    public SimpleColliderBox ColliderBox;
    public string[] Tags;

    [HideInInspector]
    public Character Owner;

    public int BasePriority;
    public int TempPriority;
    public bool Active;
}
