
using UnityEngine;

public class VelocityComponent : ComponentBase
{
    public Vector3 Velocity;
    public VelocityComponent(CharacterObj owner) : base(owner)
    {
        Velocity = Vector3.zero;
    }
}