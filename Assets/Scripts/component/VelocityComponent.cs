
using UnityEngine;

public class VelocityComponent : ComponentBase
{
    public Vector3 Velocity;
    public VelocityComponent(CharacterObj owner, int priority = 0) : base(owner, priority, typeof(VelocityComponent))
    {
        Velocity = Vector3.zero;
    }
}