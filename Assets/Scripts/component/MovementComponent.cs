
using System;
using Log;
using Unity.Mathematics;
using UnityEngine;

public class MovementComponent : ComponentBase
{
    private CharacterController mController;
    // jump相关
    private bool mPressedJump;
    public int MaxJumpCount;
    private int mJumpCount;
    private float mTimeToJumpApex;
    public float TimeToJumpApex
    {
        get => mTimeToJumpApex;
        set {
            if (value > 0.01f)
            {
                TimeToJumpApex = value;
            }
        }
    }
    public float Gravity;
    public Vector3 JumpVelocity;
    private float mJumpVelocity;
    private float mGroundedVelocity;
    private bool mIsFalling;
    public bool IsFalling
    {
        get => mIsFalling;
        set => mIsFalling = value;
    }

    private float mJumpHeight;
    public float JumpHeight
    {
        get => mJumpHeight;
        set {
            if (value > 0f)
            {
                mJumpHeight = value;
                SetupJumpVariables();
            }
        }
    }

    private bool mIsGrounded;
    // jump相关

    public MovementComponent(CharacterObj owner)
        : base(owner)
    {
        Gravity           = -9.8f;
        mIsFalling        = false;
        mPressedJump      = false;
        mJumpHeight       = 2f;
        MaxJumpCount      = 1;
        mJumpCount        = 0;
        mTimeToJumpApex   = 0.5f;
        mGroundedVelocity = -0.2f;
        JumpVelocity      = new Vector3(0, mGroundedVelocity, 0);
        mIsGrounded       = true;
        SetupJumpVariables();
    }

    public override void BeginPlay()
    {
        mOwner.Action.OnActionNotify += CheckGrounded;
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

    public void SetCharacterController(CharacterController controller)
    {
        mController = controller;
    }

    public CharacterController GetCharacterController()
    {
        return mController;
    }

    // render
    private float mTime = 0f;
    // render
    void HandleMovement()
    {
        GameObject model = mOwner.ModelRoot;
        if (model)
        {
            if (model.transform.position == mOwner.gameObject.transform.position)
            {
                mTime = 0f;
                return;
            }
            mTime += (float)GameInstance.Instance.LogicFrameRate / GameInstance.Instance.FrameRate;
            Vector3 pos = Vector3.Lerp(mLastPosition, mOwner.gameObject.transform.position, mTime);
            // SimpleLog.Warn("pos: ", pos);
            model.transform.position = pos;
        }
    }

    /////////////////////////////logic
    private Vector3 mLastPosition = Vector3.zero;

    public void NatureMove()
    {
        if (mOwner.ModelRoot)
        {
            mLastPosition = mOwner.ModelRoot.transform.position;
        }
        PlayerController pc = mOwner.GetPlayerController();
        var adjDir          = pc.CharacterRelativeFlatten(pc.CurrMoveDir);
        if (mOwner.Action.CurAction.HasRootMotion())
        {
            Vector3 RootMotionMove = mOwner.Action.RootMotionMove;
            // SimpleLog.Info("RootMotionMove", RootMotionMove);
            if (adjDir.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(adjDir);
                Transform transform       = mOwner.gameObject.transform;
                if (transform)
                {
                    // // 更新角色的旋转
                    // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                    // transform.LookAt(transform.position + adjDir);
                    transform.rotation = targetRotation;
                }
                // MoveInputAcceptance
                if (mOwner.Action.CurAction.AppilyGravityInRootMotion)
                {
                    RootMotionMove.y = -mOwner.GravityComp.Gravity / GameInstance.Instance.LogicFrameRate;
                }
                mController.Move(targetRotation * RootMotionMove);
            }
            else
            {
                if (mOwner.Action.CurAction.AppilyGravityInRootMotion)
                {
                    RootMotionMove.y = -mOwner.GravityComp.Gravity / GameInstance.Instance.LogicFrameRate;
                }
                mController.Move(RootMotionMove);
            }
            /*
                // 用户输入
Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

// 将输入转为 3D 方向向量
Vector3 moveDirection = new Vector3(input.x, 0, input.y).normalized;

// 可选：输入方向基于摄像机的方向
Vector3 cameraForward = Camera.main.transform.forward;
Vector3 cameraRight = Camera.main.transform.right;
cameraForward.y = 0;
cameraRight.y = 0;
cameraForward.Normalize();
cameraRight.Normalize();
moveDirection = cameraRight * input.x + cameraForward * input.y;
moveDirection = moveDirection.normalized;

// 假设动画位移是动画计算出来的每帧偏移量
Vector3 animationDelta = new Vector3(1, 0, 3);

if (moveDirection.sqrMagnitude > 0.01f) // 确保有输入
{
    // 将动画的位移朝向用户输入方向旋转
    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
    animationDelta = targetRotation * animationDelta;
}

// 使用 CharacterController 移动
CharacterController controller = GetComponent<CharacterController>();

// 最终位移增量（加上重力影响，如果有需要）
Vector3 finalMoveDelta = animationDelta * Time.deltaTime; // 保证时间步依赖
controller.Move(finalMoveDelta);
            */
        }
        else
        {
            float MoveInputAcceptance = mOwner.GetMoveInputAcceptance();
            float speed               = mOwner.Speed;
            speed                     = speed / GameInstance.Instance.LogicFrameRate;
            Vector3 motion            = adjDir * speed * MoveInputAcceptance;
            motion.y = -9.8f / GameInstance.Instance.LogicFrameRate;
            mController.Move(motion);
        }

        // HandleGravity();
        // HandleJump();
        // mController.Move(JumpVelocity * (1f / GameInstance.Instance.LogicFrameRate));
        HandleInAirAction();
        mIsGrounded = mController.isGrounded;
        //SimpleLog.Info("mIsGrounded: ", mIsGrounded);
        /*float delta = 1f / GameInstance.Instance.LogicFrameRate;
        var e = mController.Move(Velocity * delta * mJumpSpeed);
        if (mIsFalling)
        {
            Velocity.y -= mDeltaWeight * mJumpSpeed;
            mIsFalling = !mController.isGrounded;
        }
        else
        {
            OnGround();
        }*/
        // SimpleLog.Warn("height: ", mOwner.gameObject.transform.position.y);
    }

    void SetupJumpVariables()
    {
        float time    = mTimeToJumpApex * 0.5f;
        Gravity       = -2 * mJumpHeight / Mathf.Pow(time, 2);
        mJumpVelocity = 2 * mJumpHeight / time;
    }

    void HandleJump()
    {
        if (!mIsFalling && mIsGrounded && mPressedJump)
        {
            mIsFalling     = true;
            JumpVelocity.y = mJumpVelocity;
        }
        else if (!mPressedJump && mIsFalling && mIsGrounded)
        {
            mIsFalling = false;
        }
    }

    void HandleGravity()
    {
        if (mIsGrounded)
        {
            JumpVelocity.y = mGroundedVelocity;
            mJumpCount     = 0;
        }
        else
        {
            float delta = 1f / GameInstance.Instance.LogicFrameRate;
            JumpVelocity.y += Gravity * delta;
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
            // transform.position += transform.rotation * Move;
            // transform.rotation = transform.rotation * Rot;
            // transform.Translate(Move);
            mController.Move(transform.rotation * Move);
            transform.Rotate(Rot.eulerAngles);
        }
        else
        {
            mController.Move(Move);
        }
    }

    public void Jump()
    {
        if (mPressedJump || (mJumpCount >= MaxJumpCount))
        {
            return;
        }
        mPressedJump = true;
        mJumpCount++;
    }

    public void StopJumping()
    {
        mPressedJump = false;
    }
    /////////////////////////////logic
    ///

    private void HandleInAirAction()
    {
        if (!mIsGrounded && mController.isGrounded)
        {
            CharacterAction action = mOwner.Action.GetActionById("Jump_Vertical_Landing", out bool found);
            if (found && mOwner.Action.CanActionCancelCurrentAction(action, false, out CancelTag foundTag, out BeCanceledTag beCabceledTag))
            {
                ActionChangeInfo info = new ActionChangeInfo() {
                    changeType = ActionChangeType.ChangeToActionId,
                    param      = "Jump_Vertical_Landing",
                    priority   = 2

                };
                mOwner.Action.PreorderActionByActionChangeInfo(info);
            }
        }
        else if (!mIsGrounded && !mController.isGrounded)
        {
            //string actionName = mOwner.Action.CurAction.mActionName;
            //if (actionName == "")
            CharacterAction action = mOwner.Action.GetActionById("Jump_Vertical_Fly_Loop", out bool found);
            if (found && mOwner.Action.CanActionCancelCurrentAction(action, false, out CancelTag foundTag, out BeCanceledTag beCabceledTag))
            {
                ActionChangeInfo info = new ActionChangeInfo() {
                    changeType = ActionChangeType.ChangeToActionId,
                    param      = "Jump_Vertical_Fly_Loop",
                    priority   = 2

                };
                mOwner.Action.PreorderActionByActionChangeInfo(info);
            }
        }
    }

    public void CheckGrounded(string functionName, string[] args)
    {
        if (functionName != "CheckGrounded")
        {
            return;
        }
        if (mIsGrounded && mController.isGrounded)
        {
            //CharacterAction action = mOwner.Action.GetActionById("Jump_Vertical_Landing", out bool found);
            //if (found && mOwner.Action.CanActionCancelCurrentAction(action, false, out CancelTag foundTag, out BeCanceledTag beCabceledTag))
            {
                ActionChangeInfo info = new ActionChangeInfo() {
                    changeType = ActionChangeType.ChangeToActionId,
                    param      = "Jump_Vertical_Landing",
                    priority   = 2

                };
                mOwner.Action.PreorderActionByActionChangeInfo(info);
            }
        }
    }
}