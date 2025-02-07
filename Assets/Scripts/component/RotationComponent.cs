using UnityEngine;

public class RotationComponent : ComponentBase
{
    private Vector3 Rotation = Vector3.zero;
    public RotationComponent(CharacterObj owner, int priority = 0)
        : base(owner, priority, typeof(RotationComponent))
    {
    }

    public Vector3 GetRotation()
    {
        return Rotation;
    }
    
    public Quaternion GetRotationQ()
    {
        return Quaternion.Euler(Rotation);
    }

    public void SetRotation(Vector3 rotation)
    {
        Rotation = rotation;
    }

    public override void UpdateLogic(int frameIndex)
    {
        //玩家输入
        //rootmotion旋转和forcemove旋转覆盖玩家输入
        PlayerController pc = mOwner.GetPlayerController();
        Vector3 dir         = pc.CharacterRelativeFlatten(pc.CurrMoveDir);
        if (dir.magnitude < 0.01f) //todo
        {
            if (mOwner.Action.CurAction.HasRootMotion())
            {
                Transform transform = mOwner.gameObject.transform;
                Quaternion Rot      = transform.rotation * mOwner.Action.RootMotionRotation;
                Rotation            = Rot.eulerAngles;
            }
        }
    }
}