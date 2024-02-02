using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class Character : MonoBehaviour
{
    private ActionController actionCtrl;

    public float speed;

    private Vector3 curMovement;
    private bool isMovementPresed = false;

    private PlayerFSM fsm;

    public Animator animator {get; private set;}
    public PlayerController controller {get; private set;}
    private InputToCommand inputToCommand;

    void Awake()
    {
        /* Move.Enable();
        Move.started += OnMovementInput;
        Move.performed += OnMovementInput;
        Move.canceled += OnMovementInput; */

        //fsm = new PlayerFSM(this);
        controller = GetComponent<PlayerController>();
    }

    void Start()
    {
        animator = GetComponent<Animator>();
        //fsm.Start();
        //animator.SetFloat("Speed", 1.0f);

        inputToCommand = new InputToCommand(this);
        actionCtrl     = new ActionController(this, inputToCommand);
        //actionCtrl.command = inputToCommand;
        {
            List<CharacterAction> actions = new List<CharacterAction>();
            CharacterAction idleAction    = new CharacterAction();
            idleAction.mActionName        = "Idle";
            idleAction.mAnimation         = "Idle_60fps";
            idleAction.mPriority          = 100;
            idleAction.mAutoNextActionName = "Idle";
            idleAction.keepPlayingAnim = true;
            {
                BeCanceledTag bct                = new BeCanceledTag();
                bct.cancelTag                    = new string[1] {"InitAction"};
                bct.priority                     = 1;
                idleAction.mBeCanceledTagList    = new BeCanceledTag[1];
                idleAction.mBeCanceledTagList[0] = bct;
                idleAction.mCancelTagList = new CancelTag[1];
            }
            actions.Add(idleAction);
            CharacterAction walkAction = new CharacterAction();
            walkAction.mActionName     = "Walk";
            walkAction.mAnimation      = "Walk_60fps";
            walkAction.mPriority       = 100;
            walkAction.mAutoNextActionName = "Idle";
            walkAction.keepPlayingAnim = true;
            {
                CancelTag ct                 = new CancelTag();
                ct.tag                       = "InitAction";
                ct.priority                  = 1;
                walkAction.mCancelTagList    = new CancelTag[1];
                walkAction.mCancelTagList[0] = ct;

                BeCanceledTag bct                = new BeCanceledTag();
                bct.cancelTag                    = new string[1] {"InitAction"};
                bct.priority                     = 1;
                walkAction.mBeCanceledTagList    = new BeCanceledTag[1];
                walkAction.mBeCanceledTagList[0] = bct;
            }
            {
                ActionCommand cmd       = new ActionCommand();
                cmd.keySequences        = new KeyMap[] { KeyMap.DirInput };
                cmd.validInSecond       = 0.01f;
                walkAction.mCommandList = new ActionCommand[] { cmd };
            }
            actions.Add(walkAction);
            /* CharacterAction runAction = new CharacterAction();
            runAction.mActionName     = "Run";
            runAction.mAnimation      = "Run_60fps";
            actions.Add(runAction);
            CharacterAction sprintAction = new CharacterAction();
            sprintAction.mActionName     = "Sprint";
            sprintAction.mAnimation      = "Sprint_60fps";
            actions.Add(sprintAction); */
            actionCtrl.SetAllAction(actions, "Idle");
        }
    }
    void Update()
    {
        //fsm.Update();
        //CHController.Move(curMovement * speed * Time.deltaTime);
        //MoveDir();
        inputToCommand.Tick();
        actionCtrl.Tick();
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

    public void AddInputCommand(KeyMap key)
    {
        inputToCommand?.AddInput(key);
    }
}