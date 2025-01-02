using System;
using UnityEngine;

[Serializable]
public struct BeHitBox
{
    public string Tag;
    public CharacterObj Owner;
    public bool Active;

    public BeHitBox(CharacterObj owner, string tag, bool active)
    {
        Owner  = owner;
        Tag    = tag;
        Active = active;
    }
}
