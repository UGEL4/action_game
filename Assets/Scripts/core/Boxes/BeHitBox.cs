using System;
using UnityEngine;

[Serializable]
public struct BeHitBox
{
    public string Tag;
    public Character Owner;
    public bool Active;

    public BeHitBox(Character owner, string tag, bool active)
    {
        Owner  = owner;
        Tag    = tag;
        Active = active;
    }
}
