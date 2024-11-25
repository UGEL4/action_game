using System;
using UnityEngine;

[Serializable]
public struct BeHitBox
{
    public SimpleColliderBox ColliderBox;
    public string[] Tags;

    [HideInInspector]
    public Character Owner;

    public int BasePriority;
    public int TempPriority;
    public bool Active;
}
