
using System;
using Log;
using Unity.Mathematics;
using UnityEngine;

public class MovementComponent : ComponentBase
{
    private CharacterController mController;
    //jump相关
    private bool mPressedJump;
    public int MaxJumpCount;
    private int mJumpCount;
    public float Gravity;
    public Vector3 Velocity;
    private float mJumpVelocityY;
    private bool mIsFalling;
    public bool IsFalling {
        get => mIsFalling;
        set => mIsFalling = value;
    }
    private float mDeltaWeight;

    private float mJumpSpeed;
    public float JumpSpeed {
        get => mJumpSpeed;
        set => mJumpSpeed = value;
    }

    private float mJumpHeight;
    public float JumpHeight {
        get => mJumpHeight;
        set {
            if (value > 0f)
            {
                mJumpHeight = value;
                UpdateJumpVelocitY();
            }
        }
    }
    //jump相关

    public MovementComponent(CharacterObj owner)
    : base(owner)
    {
        Velocity     = new Vector3(0, -2, 0);
        Gravity      = 9.8f;
        mIsFalling   = false;
        mDeltaWeight = Gravity / 30f;
        mJumpSpeed   = 1.5f;
        mPressedJump = false;
        mJumpHeight  = 2f;
        MaxJumpCount = 1;
        mJumpCount   = 0;
        UpdateJumpVelocitY();
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

    //render
    private float mTime = 0f;
    //render
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
            //SimpleLog.Warn("pos: ", pos);
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
                mController.Move(RootMotionMove.magnitude * adjDir);
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
        float delta = 1f / GameInstance.Instance.LogicFrameRate;
        var e = mController.Move(Velocity * delta * mJumpSpeed);
        if (mIsFalling)
        {
            Velocity.y -= mDeltaWeight * mJumpSpeed;
            mIsFalling = !mController.isGrounded;
        }
        else
        {
            OnGround();
        }
        // SimpleLog.Warn("height: ", mOwner.gameObject.transform.position.y);
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
        mIsFalling   = true;
        Velocity.y   = mJumpVelocityY;
        mJumpCount++;
    }

    public void StopJumping()
    {
        mPressedJump = false;
    }

    public void OnGround()
    {
        Velocity.y = -2f;
        mJumpCount = 0;
    }

    void UpdateJumpVelocitY()
    {
        mJumpVelocityY = Mathf.Sqrt(JumpHeight * 2 * Gravity);
    }

    void ResetJumpVelocity()
    {
        Velocity.y = -2f;
    }
    /////////////////////////////logic
}