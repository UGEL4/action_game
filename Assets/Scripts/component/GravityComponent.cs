using Unity.VisualScripting;

public class GravityComponent : ComponentBase
{
    public float CurrentWeight;
    public float Gravity;
    private float Ticked;
    public GravityComponent(CharacterObj owner) : base(owner)
    {
        CurrentWeight = 0f;
        Ticked        = 0f;
        Gravity       = 9.8f;
    }

    public override void UpdateLogic(int frameIndex)
    {
        float add = Gravity / GameInstance.Instance.LogicFrameRate;
        CurrentWeight += (Ticked += add);

        //更新owner.velocity
        if (mOwner.VelocityComp != null)
        {
            mOwner.VelocityComp.Velocity.y -= CurrentWeight;
        }
    }
}