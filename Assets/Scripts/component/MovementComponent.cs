
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
        mIsGrounded       = false;
        SetupJumpVariables();
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
        // if (adjDir.magnitude > 0.0f)
        {
            if (mOwner.Action.CurAction.HasRootMotion())
            {
                Vector3 RootMotionMove = mOwner.Action.RootMotionMove;
                //SimpleLog.Info("RootMotionMove", RootMotionMove);
                Transform transform    = mOwner.gameObject.transform;
                if (transform)
                {
                    // // 更新角色的旋转
                    // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                    transform.LookAt(transform.position + adjDir);
                }
                if (adjDir.magnitude > 0f)
                {
                    //MoveInputAcceptance
                    Vector3 xyz = new Vector3(RootMotionMove.x, 0, RootMotionMove.z);
                    mController.Move(xyz.magnitude * adjDir);
                    xyz.x = 0;
                    xyz.z = 0;
                    xyz.y = RootMotionMove.y;
                    mController.Move(xyz);
                }
                else
                {
                    mController.Move(RootMotionMove);
                }
            }
            else
            {
                float MoveInputAcceptance = mOwner.GetMoveInputAcceptance();
                float speed               = mOwner.Speed;
                speed                     = speed / GameInstance.Instance.LogicFrameRate;
                Vector3 motion            = adjDir * speed * MoveInputAcceptance;
                mController.Move(motion);
            }
        }
        // HandleGravity();
        // HandleJump();
        // mController.Move(JumpVelocity * (1f / GameInstance.Instance.LogicFrameRate));
        if (!mIsGrounded && mController.isGrounded)
        {
            //todo: fixed debug
            ActionChangeInfo info = new ActionChangeInfo()
            {
                changeType = ActionChangeType.ChangeToActionId,
                param = "Jump_Vertical_Landing",
                priority = 1

            };
            mOwner.Action.PreorderActionByActionChangeInfo(info);
        }
        mIsGrounded = mController.isGrounded;
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
}