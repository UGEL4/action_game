using System;
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
            // List<CharacterAction> actions = new List<CharacterAction>();
            // CharacterAction idleAction    = new CharacterAction();
            // idleAction.mActionName        = "Idle";
            // idleAction.mAnimation         = "Idle_60fps";
            // idleAction.mPriority          = 100;
            // idleAction.mAutoNextActionName = "Idle";
            // idleAction.keepPlayingAnim = true;
            // idleAction.mAutoTerminate  = false;
            // {
            //     BeCanceledTag bct                = new BeCanceledTag();
            //     bct.cancelTag                    = new string[1] {"InitAction"};
            //     bct.priority                     = 0;
            //     bct.range                        = new PercentageRange(0.0f, 1.0f);
            //     idleAction.mBeCanceledTagList    = new BeCanceledTag[1];
            //     idleAction.mBeCanceledTagList[0] = bct;
            //     idleAction.mCancelTagList = new CancelTag[1];
            //     idleAction.mTempBeCanceledTagList = new TempBeCancelledTag[0];
            //     idleAction.attackInfoList = new AttackInfo[0];
            //     idleAction.attackPhaseList = new AttackBoxTurnOnInfo[0];
            // }
            // idleAction.moveInputAcceptance = new MoveInputAcceptance[1] {new MoveInputAcceptance(){range = new PercentageRange(0.0f, 1.0f), rate = 0.0f}};
            // actions.Add(idleAction);
            // CharacterAction walkAction = new CharacterAction();
            // walkAction.mActionName     = "Walk";
            // walkAction.mAnimation      = "Run_60fps";
            // walkAction.mPriority       = 100;
            // walkAction.mAutoNextActionName = "Idle";
            // walkAction.keepPlayingAnim = true;
            // walkAction.mAutoTerminate = true;
            // {
            //     CancelTag ct                 = new CancelTag();
            //     ct.tag                       = "InitAction";
            //     ct.priority                  = 0;
            //     walkAction.mCancelTagList    = new CancelTag[1];
            //     walkAction.mCancelTagList[0] = ct;

            //     BeCanceledTag bct                = new BeCanceledTag();
            //     bct.cancelTag                    = new string[1] {"InitAction"};
            //     bct.priority                     = 1;
            //     bct.range                        = new PercentageRange(0.0f, 1.0f);
            //     walkAction.mBeCanceledTagList    = new BeCanceledTag[1];
            //     walkAction.mBeCanceledTagList[0] = bct;

            //     walkAction.mTempBeCanceledTagList = new TempBeCancelledTag[0];
            //     walkAction.attackInfoList = new AttackInfo[0];
            //     walkAction.attackPhaseList = new AttackBoxTurnOnInfo[0];
            // }
            // {
            //     ActionCommand cmd       = new ActionCommand();
            //     cmd.keySequences        = new KeyMap[] { KeyMap.DirInput };
            //     cmd.validInSecond       = 0.01f;
            //     walkAction.mCommandList = new ActionCommand[] { cmd };
            // }
            // walkAction.moveInputAcceptance = new MoveInputAcceptance[1] {new MoveInputAcceptance(){range = new PercentageRange(0.0f, 1.0f), rate = 1.0f}};
            // actions.Add(walkAction);
            // CharacterAction punch01Action = new CharacterAction();
            // punch01Action.mActionName     = "Punch01";
            // punch01Action.mAnimation      = "punching_01_60fps";
            // punch01Action.mPriority       = 100;
            // punch01Action.mAutoNextActionName = "Idle";
            // punch01Action.keepPlayingAnim = false;
            // punch01Action.mAutoTerminate = false;
            // {
            //     CancelTag ct                 = new CancelTag();
            //     ct.tag                       = "InitAction";
            //     ct.priority                  = 0;
            //     punch01Action.mCancelTagList    = new CancelTag[1];
            //     punch01Action.mCancelTagList[0] = ct;

            //     /* BeCanceledTag bct                = new BeCanceledTag();
            //     bct.cancelTag                    = new string[1] {"Punching"};
            //     bct.priority                     = 1;
            //     bct.range                        = new PercentageRange(0.5f, 1.0f);
            //     punch01Action.mBeCanceledTagList    = new BeCanceledTag[1];
            //     punch01Action.mBeCanceledTagList[0] = bct; */
            //     punch01Action.mBeCanceledTagList = new BeCanceledTag[1]
            //     {
            //         new BeCanceledTag
            //         {
            //             cancelTag = new string[1] {"punch01ActionCombo"},
            //             priority = 1,
            //             range = new PercentageRange(0.5f, 1.0f)
            //         }
            //     };

            //     punch01Action.mTempBeCanceledTagList = new TempBeCancelledTag[0]
            //     /* {
            //         new TempBeCancelledTag
            //         {
            //             id = "punch01ActionCombo",
            //             percentage = 50,
            //             cancelTag = new string[1] {"punch01ActionCombo"},
            //             fadeOutPercentage = 0,
            //             increasePriority = 101
            //         }
            //     } */;
            //     punch01Action.attackInfoList = new AttackInfo[0]
            //     /* {
            //         new AttackInfo
            //         {
            //             attackPhase = 0,
            //             attackPower = 0,
            //             canHitSameTarget = 1,
            //             hitSameTargetDelay = 0,
            //             hitStun = 0,
            //             pushPower = new MoveInfo(),
            //             freeze = 0,
            //             selfActionChangeInfo = new ActionChangeInfo(),
            //             targetActionChangeInfo = new ActionChangeInfo(),
            //             tempBeCancelledTagTurnOn = new string[1] {"punch01ActionCombo"}
            //         }
            //     } */;
            //     punch01Action.attackPhaseList = new AttackBoxTurnOnInfo[0]
            //     /* {
            //         new AttackBoxTurnOnInfo
            //         {
            //             turnOnRangeList = new PercentageRange[1] { new PercentageRange(0, 1)},
            //             attackBoxTag = new string[1] {"punch01"},
            //             attackPhase = 0
            //         }
            //     } */;
            // }
            // {
            //     ActionCommand cmd       = new ActionCommand();
            //     cmd.keySequences        = new KeyMap[] { KeyMap.ButtonX };
            //     cmd.validInSecond       = 0.01f;
            //     punch01Action.mCommandList = new ActionCommand[] { cmd };
            // }
            // punch01Action.moveInputAcceptance = new MoveInputAcceptance[1] {new MoveInputAcceptance(){range = new PercentageRange(0.0f, 1.0f), rate = 0.0f}};
            // actions.Add(punch01Action);
            // CharacterAction punch02Action = new CharacterAction();
            // punch02Action.mActionName     = "Punch02";
            // punch02Action.mAnimation      = "punching_02_60fps";
            // punch02Action.mPriority       = 100;
            // punch02Action.mAutoNextActionName = "Idle";
            // punch02Action.keepPlayingAnim = false;
            // punch02Action.mAutoTerminate = false;
            // {
            //     punch02Action.mCancelTagList = new CancelTag[1]
            //     {
            //         new CancelTag
            //         {
            //             tag = "punch01ActionCombo",
            //             priority = 1
            //         }
            //     };
            //     punch02Action.mBeCanceledTagList = new BeCanceledTag[0];

            //     punch02Action.mTempBeCanceledTagList = new TempBeCancelledTag[0];
            //     punch02Action.attackInfoList = new AttackInfo[0];
            //     punch02Action.attackPhaseList = new AttackBoxTurnOnInfo[0];
            // }
            // {
            //     ActionCommand cmd       = new ActionCommand();
            //     cmd.keySequences        = new KeyMap[] { KeyMap.ButtonX };
            //     cmd.validInSecond       = 0.01f;
            //     punch02Action.mCommandList = new ActionCommand[] { cmd };
            // }
            // punch02Action.moveInputAcceptance = new MoveInputAcceptance[1] {new MoveInputAcceptance(){range = new PercentageRange(0.0f, 1.0f), rate = 0.0f}};
            // actions.Add(punch02Action);

            // CharacterAction kik01Action = new CharacterAction();
            // kik01Action.mActionName     = "Kik01";
            // kik01Action.mAnimation      = "kik_01_60fps";
            // kik01Action.mPriority       = 100;
            // kik01Action.mAutoNextActionName = "Idle";
            // kik01Action.keepPlayingAnim = false;
            // kik01Action.mAutoTerminate = false;
            // {

            //     kik01Action.mCancelTagList    = new CancelTag[2] {new CancelTag {tag = "InitAction", priority = 1}, new CancelTag {tag = "Punching", priority = 0}};

            //     kik01Action.mBeCanceledTagList    = new BeCanceledTag[0];

            //     kik01Action.mTempBeCanceledTagList = new TempBeCancelledTag[0];
            //     kik01Action.attackInfoList = new AttackInfo[0];
            //     kik01Action.attackPhaseList = new AttackBoxTurnOnInfo[0];
            // }
            // {
            //     ActionCommand cmd       = new ActionCommand();
            //     cmd.keySequences        = new KeyMap[] { KeyMap.ButtonY };
            //     cmd.validInSecond       = 0.01f;
            //     kik01Action.mCommandList = new ActionCommand[] { cmd };
            // }
            // kik01Action.moveInputAcceptance = new MoveInputAcceptance[1] {new MoveInputAcceptance(){range = new PercentageRange(0.0f, 1.0f), rate = 0.0f}};
            // actions.Add(kik01Action);
            // /* CharacterAction runAction = new CharacterAction();
            // runAction.mActionName     = "Run";
            // runAction.mAnimation      = "Run_60fps";
            // actions.Add(runAction);
            // CharacterAction sprintAction = new CharacterAction();
            // sprintAction.mActionName     = "Sprint";
            // sprintAction.mAnimation      = "Sprint_60fps";
            // actions.Add(sprintAction); */
            // actionCtrl.SetAllAction(actions, "Idle");
            LoadActions();
        }
    }
    void Update()
    {
        int a = 0;
        //fsm.Update();
        //CHController.Move(curMovement * speed * Time.deltaTime);
        //MoveDir();
        //inputToCommand.Tick();
    }

    void FixedUpdate()
    {
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

    public float GetMoveInputAcceptance()
    {
        return actionCtrl.MoveInputAcceptance;
    }

    public void AddActionCommand(KeyMap key)
    {
        inputToCommand?.AddInput(key);
    }

    public bool CanAttackTargetNow(out AttackInfo attackPhase)
    {
        attackPhase = new AttackInfo();
        if (actionCtrl.ActiveAttackBoxTurnOnInfoList.Count > 0)
        {
            int phase = actionCtrl.ActiveAttackBoxTurnOnInfoList[0].attackPhase;
            if (phase >= 0 && actionCtrl.CurAction.attackInfoList.Length > phase) attackPhase = actionCtrl.CurAction.attackInfoList[phase];
            return true;
        }

        return false;
    }

    public ActionController GetActionController()
    {
        return actionCtrl;
    }

    void LoadActions()
    {
        TextAsset ta = Resources.Load<TextAsset>("GameData/ch01_actions");
        if (ta)
        {
            List<CharacterAction> actions = new List<CharacterAction>();
            ActionInfoContainer aic = JsonUtility.FromJson<ActionInfoContainer>(ta.text);
            foreach (CharacterAction info in aic.data)
            {
                actions.Add(info);
            }

            actionCtrl.SetAllAction(actions, "Idle");
        }
    }
}

[Serializable]
public struct ActionInfoContainer
{
    public CharacterAction[] data;
}