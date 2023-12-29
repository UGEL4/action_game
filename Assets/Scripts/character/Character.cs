using System.Collections.Generic;
using Action;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    public ActionFrame mCurActionFrame;
    public List<CharacterAction> mActions;

    //public CharacterController CHController;

    ///
    //public InputAction Move;
    public float speed;

    public Vector3 curMovement;
    public bool isMovementPresed = false;

    private PlayerFSM fsm;

    public Animator animator {get; private set;}
    public PlayerController controller {get; private set;}

    void Start()
    {
        animator = GetComponent<Animator>();
        fsm.Start();
        //animator.SetFloat("Speed", 1.0f);
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
}