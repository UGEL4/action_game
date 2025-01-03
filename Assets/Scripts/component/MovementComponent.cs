
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
        // if (!RenderObj) return;

        // RenderObj.transform.position = Vector3.Lerp(RenderObj.transform.position, transform.position, GetPositionLerpT());
        // float MoveInputAcceptance = owner.GetMoveInputAcceptance();
        // if (MoveInputAcceptance <= 0.0) return;
        // //var moveDir = new Vector3(input.Direction.x, 0.0f, input.Direction.y).normalized;
        // var adjDir = Quaternion.AngleAxis(mCameraTransform.eulerAngles.y, Vector3.up) * playerController.CurrMoveDir;
        // if (adjDir.magnitude > 0.0f)
        // {
        //     var targetRot = Quaternion.LookRotation(adjDir);
        //     transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 2.0f * Time.deltaTime);
        //     transform.LookAt(transform.position + adjDir);
        //     //HandleRotation();

        //     Vector3 moveDir = adjDir * speed * Time.deltaTime * MoveInputAcceptance;
        //     controller.Move(moveDir);
        // }
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
        //mPosition = transform.position;
        //mLastPosition = mOwner.gameObject.transform.position;
        mLastPosition = mOwner.Model.transform.position;
        mStartSyncModelPosition = true;
        PlayerController pc = mOwner.GetPlayerController();
        //var adjDir          = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * pc.CurrMoveDir;
        var adjDir = pc.CharacterRelativeFlatten(pc.CurrMoveDir);
        if (adjDir.magnitude > 0.0f)
        {
            //Vector3 motion = adjDir * 6 * Time.fixedDeltaTime /** MoveInputAcceptance*/;
            //mPosition = mPosition + motion;
            SimpleLog.Warn("MoveSpeed: ", mOwner.Action.MoveSpeed);
            if (mOwner.Action.CurAction.HasRootMotion())
            {
                Vector3 RootMotionMove = mOwner.Action.RootMotionMove;
                Transform transform    = mOwner.gameObject.transform;
                if (transform)
                {
                    //var targetRot = Quaternion.LookRotation(adjDir);
                    //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 2.0f * Time.deltaTime);
                    //transform.LookAt(transform.position + adjDir);
                    // float targetAngle = Mathf.Atan2(adjDir.x, adjDir.z) * Mathf.Rad2Deg;
                    // Quaternion targetRotation = Quaternion.Euler(0, targetAngle, 0);

                    // // 更新角色的旋转
                    // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                    transform.LookAt(transform.position + adjDir);
                }
                pc.Move(RootMotionMove.magnitude * adjDir);
                //debug
                //pc.Move(6 * adjDir / 30);
            }
            else
            {
                float MoveInputAcceptance = mOwner.GetMoveInputAcceptance();
                float speed               = mOwner.Speed;
                speed                     = speed / GameInstance.Instance.LogicFrameRate;
                Vector3 motion            = adjDir * speed * MoveInputAcceptance;
                pc.Move(motion);
            }
            // if (RenderObj)
            //     RenderObj.transform.position = Vector3.Lerp(RenderObj.transform.position, transform.position, GetPositionLerpT());
        }
    }

    public void ForceMove()
    {
        Vector3 Move = mOwner.Action.UnderForceMove;
        Transform transform = mOwner.gameObject.transform;
        if (transform)
        {
            float MoveInputAcceptance = mOwner.GetMoveInputAcceptance();
            if (MoveInputAcceptance > 0)
            {
                Owner.GetPlayerController().Move(Move.magnitude * MoveInputAcceptance * transform.forward);
            }
            else
                mOwner.GetPlayerController().Move(Move.magnitude * transform.forward);
        }
        else
        {
            Owner.GetPlayerController().Move(Move);
        }
    }
    /////////////////////////////logic
}