
using Log;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class MovementComponent : ComponentBase
{
    public MovementComponent(CharacterObj owner)
    : base(owner)
    {
    }

    public override void UpdateLogic(int frameIndex)
    {
        if (mOwner.Action.IsUnderForceMove)
        {
            ForceMove();
        }
        else
        {
            NatureMove();
        }
    }

    public override void UpdateRender(float deltaTime)
    {
        HandleMovement();
    }

    private float mTime = 0f;
    void HandleMovement()
    {
        GameObject model = mOwner.Model;
        if (model)
        {
            if (mStartSyncModelPosition)
            {
                //model.transform.position = mLastPosition;
                //model.transform.position = Vector3.Lerp(mLastPosition, mOwner.gameObject.transform.position, Time.deltaTime);
            }
            if (model.transform.position == mOwner.gameObject.transform.position)
            {
                mTime = 0f;
                mStartSyncModelPosition = false;
                return;
            }
            mTime += (float)GameInstance.Instance.LogicFrameRate / GameInstance.Instance.FrameRate;
            Vector3 pos = Vector3.Lerp(mLastPosition, mOwner.gameObject.transform.position, mTime);
            //SimpleLog.Warn("pos: ", pos);
            model.transform.position = pos;
        }
        if (mStartSyncModelPosition)
        {
            mStartSyncModelPosition = false;
        }
    }

    /////////////////////////////logic
    private int mSpeed = 1; //每帧位移0.2
    private float mRenderUnit = 0.2f;

    private Vector3 mLastPosition = Vector3.zero;
    private bool mStartSyncModelPosition = false;

    //逻辑单位到渲染单位的转换
    public float LogicUnitToRenderUnit(int unit)
    {
        return unit * mRenderUnit;
    }

    public void NatureMove()
    {
        mLastPosition = mOwner.Model.transform.position;
        mStartSyncModelPosition = true;
        PlayerController pc = mOwner.GetPlayerController();
        var adjDir = pc.CharacterRelativeFlatten(pc.CurrMoveDir);
        if (adjDir.magnitude > 0.0f)
        {
            if (mOwner.Action.CurAction.HasRootMotion())
            {
                Vector3 RootMotionMove = mOwner.Action.RootMotionMove;
                Transform transform    = mOwner.gameObject.transform;
                if (transform)
                {
                    // // 更新角色的旋转
                    // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                    transform.LookAt(transform.position + adjDir);
                }
                pc.Move(RootMotionMove.magnitude * adjDir);
            }
            else
            {
                float MoveInputAcceptance = mOwner.GetMoveInputAcceptance();
                float speed               = mOwner.Speed;
                speed                     = speed / GameInstance.Instance.LogicFrameRate;
                Vector3 motion            = adjDir * speed * MoveInputAcceptance;
                pc.Move(motion);
            }
        }
    }

    public void ForceMove()
    {
        Vector3 Move        = mOwner.Action.UnderForceMove;
        Quaternion Rot      = mOwner.Action.RootMotionRotation;
        Transform transform = mOwner.gameObject.transform;
        if (transform)
        {
            float MoveInputAcceptance = mOwner.GetMoveInputAcceptance();
            if (MoveInputAcceptance > 0)
            {
                Move *= MoveInputAcceptance;
            }
            //transform.position += transform.rotation * Move;
            //transform.rotation = transform.rotation * Rot;
            //transform.Translate(Move);
            mOwner.GetPlayerController().Move(transform.rotation * Move);
            transform.Rotate(Rot.eulerAngles);
        }
        else
        {
            Owner.GetPlayerController().Move(Move);
        }
    }
    /////////////////////////////logic
}