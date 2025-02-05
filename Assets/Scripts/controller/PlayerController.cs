using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : IController
{
    private InputReader mInput;

    //方向输入
    public Vector3 mCurMoveDir  = new Vector3();
    private InputActionPhase mDirInputPhase = InputActionPhase.Disabled;

    private CharacterObj mOwner;

    public Vector3 CurrMoveDir => mCurMoveDir;

    public PlayerController()
    {

    }

    public void SetOwner(CharacterObj owner)
    {
        mOwner = owner;
    }

    public void InitializeInput()
    {
        if (mInput)
        {
            mInput.Move += OnMove;
            mInput.AttackA += OnAttackA;
            mInput.AttackB += OnAttackB;
            mInput.Jump += Jump;
            mInput.ButtonA += OnButtonA;
            mInput.ButtonB += OnButtonB;
        }
    }

    public void BeginPlay()
    {
        //mInput = mOwner.GetInputReader();
        InitializeInput();
    }

    public void UpdateLogic()
    {
        if (mDirInputPhase == InputActionPhase.Performed)
        {
            mOwner.AddInputCommand(KeyMap.DirInput);
        }
    }

    public void OnDestroy()
    {
        if (mInput)
        {
            mInput.Move -= OnMove;
            mInput.AttackA -= OnAttackA;
            mInput.AttackB -= OnAttackB;
            mInput.Jump -= Jump;
            mInput.ButtonA -= OnButtonA;
            mInput.ButtonB -= OnButtonB;
        }
    }

    public void SetInputReader(InputReader input)
    {
        mInput = input;
    }

    void OnMove(Vector2 xy, InputActionPhase phase)
    {
        mDirInputPhase = phase;
        mCurMoveDir.x = xy.x;
        mCurMoveDir.z = xy.y;
        mCurMoveDir.y = 0.0f;
        mCurMoveDir.Normalize();

        //
        ProduceInputDir();
    }

    public Vector3 GetCameraForward()
    {
        var adjDir  = Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y, Vector3.up) * mCurMoveDir;
        //return mainCamera.forward;
        return adjDir;
    }

    void ProduceInputDir()
    {
        if (mOwner == null) return;
        //Vector3 inputDir  = CameraRelativeFlatten(mCurMoveDir, Vector3.up);
        Vector3 inputDir  = CharacterRelativeFlatten(mCurMoveDir);
        float dotF        = Vector3.Dot(inputDir, mOwner.transform.forward);
        float dotR        = Vector3.Dot(inputDir, mOwner.transform.right);
        float invalidArea = 0.2f;
        bool xHasInput    = Mathf.Abs(dotR) >= invalidArea;
        bool yHasInput    = Mathf.Abs(dotF) >= invalidArea;
        /* if (yHasInput)
        {
            if (dotF > 0.0f)
            {
                owner.AddInputCommand(KeyMap.Forward);
            }
            else
            {
                owner.AddInputCommand(KeyMap.Back);
            }
        }
        if (xHasInput)
        {
            if (dotR > 0.0f)
            {
                owner.AddInputCommand(KeyMap.Right);
            }
            else
            {
                owner.AddInputCommand(KeyMap.Left);
            }
        } */
        if (!xHasInput && !yHasInput)
        {
            mOwner.AddInputCommand(KeyMap.NoDir);
        }
        else
        {
            mOwner.AddInputCommand(KeyMap.DirInput);
        }
    }

    Vector3 CameraRelativeFlatten(Vector3 input, Vector3 localUp)
    {
        // The first part creates a rotation looking into the ground, with
        // "up" matching the camera's look direction as closely as it can.
        // The second part rotates this 90 degrees, so "forward" input matches
        // the camera's look direction as closely as it can in the horizontal plane.
        Quaternion flatten = Quaternion.LookRotation(
                             -localUp,
                             Camera.main.transform.forward) *
                             Quaternion.Euler(Vector3.right * -90f);

        // Now we rotate our input vector into this frame of reference
        return flatten * input;
    }

    /// <summary>
    /// 获取xz平面上的移动方向向量
    /// </summary>
    /// <param name="input">xz平面上的输入</param>
    /// <returns>返回移动方向单位向量</returns>
    public Vector3 CharacterRelativeFlatten(Vector3 input)
    {
        Vector3 forward = Camera.main.transform.forward;
        Vector3 right   = Camera.main.transform.right;
        forward.y       = 0f;
        forward.Normalize();
        right.y = 0f;
        right.Normalize();
        Vector3 Move = forward * input.z + right * input.x;
        Move.Normalize();
        return Move;
    }

    void OnAttackA(InputActionPhase phase)
    {
        //SimpleLog.Info("OnAttackA: ", phase);
        if (phase == InputActionPhase.Started)
        {
            mOwner.AddActionCommand(KeyMap.ButtonX);
        }
    }

    void OnAttackB(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            mOwner.AddActionCommand(KeyMap.ButtonY);
        }
    }

    void OnButtonA(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            mOwner.AddActionCommand(KeyMap.ButtonA);
        }
    }

    void OnButtonB(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            mOwner.AddActionCommand(KeyMap.ButtonB);
        }
    }

    public void Jump(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            //mOwner.MovementComp.Jump();
        }
        else if (phase == InputActionPhase.Canceled)
        {
            //mOwner.MovementComp.StopJumping();
        }
    }
}
