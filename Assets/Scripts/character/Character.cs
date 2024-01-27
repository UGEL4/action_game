using System;
using System.Collections.Generic;
using Action;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public ActionFrame mCurActionFrame;
    public List<CharacterAction> mActions;

    public ActionController actionCtrl;

    //public CharacterController CHController;

    ///
    //public InputAction Move;
    public float speed;

    public Vector3 curMovement;
    public bool isMovementPresed = false;

    private PlayerFSM fsm;

    public Animator animator {get; private set;}
    public PlayerController controller {get; private set;}
    public InputToCommand inputToCommand;

    void Start()
    {
        animator = GetComponent<Animator>();
        fsm.Start();
        //animator.SetFloat("Speed", 1.0f);

        inputToCommand.Owner = this;
        if (actionCtrl != null)
        {
            List<CharacterAction> actions = new List<CharacterAction>();
            CharacterAction idleAction    = new CharacterAction();
            idleAction.mActionName        = "Idle";
            idleAction.mAnimation         = "Idle_60fps";
            actions.Add(idleAction);
            CharacterAction walkAction = new CharacterAction();
            walkAction.mActionName     = "Walk";
            walkAction.mAnimation      = "Walk_60fps";
            actions.Add(walkAction);
            CharacterAction runAction = new CharacterAction();
            runAction.mActionName     = "Run";
            runAction.mAnimation      = "Run_60fps";
            actions.Add(runAction);
            CharacterAction sprintAction = new CharacterAction();
            sprintAction.mActionName     = "Sprint";
            sprintAction.mAnimation      = "Sprint_60fps";
            actions.Add(sprintAction);
            actionCtrl.AllActions = actions;
        }
    }
    void Update()
    {
        fsm.Update();
        //CHController.Move(curMovement * speed * Time.deltaTime);
        //MoveDir();
    }

    void Awake()
    {
        /* Move.Enable();
        Move.started += OnMovementInput;
        Move.performed += OnMovementInput;
        Move.canceled += OnMovementInput; */

        fsm = new PlayerFSM(this);
        controller = GetComponent<PlayerController>();
    }

    public bool IsMoving()
    {
        return controller.IsMoving();
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        Vector2 val = context.ReadValue<Vector2>();
        curMovement.x = val.x;
        curMovement.y = 0.0f;
        curMovement.z = val.y;
        isMovementPresed = val.x != 0.0f || val.y != 0.0f;
    }

    void MoveDir()
    {
        Vector3 posToLookat = new Vector3();
        posToLookat.x = curMovement.x;
        posToLookat.y = 0.0f;
        posToLookat.z = curMovement.z;
        Quaternion curRot = transform.rotation;
        if (isMovementPresed)
        {
            Quaternion targetRot = Quaternion.LookRotation(posToLookat);
            transform.rotation   = Quaternion.Slerp(curRot, targetRot, 1.0f);
        }
    }

    public Vector3 GetForward()
    {
        return transform.forward;
    }

    public Vector3 GetRight()
    {
        return transform.right;
    }

    public Vector3 GetCameraForward()
    {
        return controller.GetCameraForward();
    }
}