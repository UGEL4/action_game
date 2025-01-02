
using UnityEngine;

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

    void HandleMovement()
    {
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

    //逻辑单位到渲染单位的转换
    public float LogicUnitToRenderUnit(int unit)
    {
        return unit * mRenderUnit;
    }

    public void NatureMove()
    {
        //mPosition = transform.position;
        PlayerController pc = mOwner.GetPlayerController();
        //var adjDir          = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * pc.CurrMoveDir;
        var adjDir = pc.CharacterRelativeFlatten(pc.CurrMoveDir);
        if (adjDir.magnitude > 0.0f)
        {
            adjDir.Normalize();
            //Vector3 motion = adjDir * 6 * Time.fixedDeltaTime /** MoveInputAcceptance*/;
            //mPosition = mPosition + motion;

            if (mOwner.Action.CurAction.HasRootMotion())
            {
                Vector3 RootMotionMove = mOwner.Action.RootMotionMove;
                Transform transform = mOwner.gameObject.transform;
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
            }
            else
            {
                float MoveInputAcceptance = mOwner.GetMoveInputAcceptance();
                Vector3 motion = adjDir * LogicUnitToRenderUnit(mSpeed) * MoveInputAcceptance;
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
            mOwner.GetPlayerController().Move(Move.magnitude * transform.forward);
        }
        else
        {
            Owner.GetPlayerController().Move(Move);
        }
    }
    /////////////////////////////logic
}