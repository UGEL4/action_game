using System;
using Cinemachine;
using Log;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] CinemachineFreeLook freeLookCam;
    [SerializeField] InputReader input;
    // Start is called before the first frame update

    //方向输入
    public Vector3 mCurMoveDir  = new Vector3();
    private InputActionPhase mDirInputPhase = InputActionPhase.Disabled;

    private Transform mainCamera;

    [SerializeField] Character owner;

    public Vector3 CurrMoveDir => mCurMoveDir;

    void Awake()
    {
        controller  = GetComponent<CharacterController>();
        mainCamera  = Camera.main.transform;
        //freeLookCam.Follow = transform;
        //freeLookCam.LookAt = transform;
        //freeLookCam.OnTargetObjectWarped(transform, transform.position - freeLookCam.transform.position - Vector3.forward);
    }

    void InitializeInput()
    {
        input.Move += OnMove;
        input.AttackA += OnAttackA;
        input.AttackB += OnAttackB;
    }

    void Start()
    {
        InitializeInput();
    }

    void Update()
    {
        if (mDirInputPhase == InputActionPhase.Performed)
        {
            owner.AddInputCommand(KeyMap.DirInput);
        }
    }

    void OnDestroy()
    {
        input.Move -= OnMove;
        input.AttackA -= OnAttackA;
        input.AttackB -= OnAttackB;
    }

    void HandleMovement()
    {
        //var moveDir = new Vector3(input.Direction.x, 0.0f, input.Direction.y).normalized;
        var adjDir  = Quaternion.AngleAxis(mainCamera.eulerAngles.y, Vector3.up) * mCurMoveDir;
        if (adjDir.magnitude > 0.0f)
        {
            var targetRot = Quaternion.LookRotation(adjDir);
            //transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 2.0f * Time.deltaTime);
            transform.LookAt(transform.position + adjDir);
            HandleRotation();

            controller.Move(adjDir * 10.0f * Time.deltaTime);
        }
    }

    void HandleRotation()
    {
        
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

    public bool IsMoving()
    {
        return mCurMoveDir.x != 0.0f || mCurMoveDir.z != 0.0f;
    }

    public Vector3 GetCameraForward()
    {
        var adjDir  = Quaternion.AngleAxis(mainCamera.eulerAngles.y, Vector3.up) * mCurMoveDir;
        //return mainCamera.forward;
        return adjDir;
    }

    void ProduceInputDir()
    {
        if (owner == null) return;
        Vector3 inputDir  = CameraRelativeFlatten(mCurMoveDir, Vector3.up);
        float dotF        = Vector3.Dot(inputDir, transform.forward);
        float dotR        = Vector3.Dot(inputDir, transform.right);
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
            owner.AddInputCommand(KeyMap.NoDir);
        }
        else
        {
            owner.AddInputCommand(KeyMap.DirInput);
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
                             mainCamera.forward) *
                             Quaternion.Euler(Vector3.right * -90f);

        // Now we rotate our input vector into this frame of reference
        return flatten * input;
    }

    void OnAttackA(InputActionPhase phase)
    {
        SimpleLog.Info("OnAttackA: ", phase);
        if (phase == InputActionPhase.Started)
        {
            owner.AddActionCommand(KeyMap.ButtonX);
        }
    }

    void OnAttackB(InputActionPhase phase)
    {
        if (phase == InputActionPhase.Started)
        {
            owner.AddActionCommand(KeyMap.ButtonY);
        }
    }
}
