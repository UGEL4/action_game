using System;
using System.Collections.Generic;
using Log;
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

    public HitRecordComponent HitRecordComponent {get; private set;}

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
        SimpleLog.Info("Start: ", gameObject.name);
        // animator = GetComponent<Animator>();
        // //fsm.Start();
        // //animator.SetFloat("Speed", 1.0f);

        // inputToCommand = new InputToCommand(this);
        // actionCtrl     = new ActionController(this, inputToCommand);
        // HitRecordComponent = new HitRecordComponent();
        // GameInstance.Instance.HitRecordSys.Register(HitRecordComponent);
        // actionCtrl.SetChangeActinCallback((lastAction, newAction)=>{
        //     HitRecordComponent.Clear();
        // });
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
            //LoadActions();
        }
    }

    public void Init()
    {
        animator = GetComponent<Animator>();
        //fsm.Start();
        //animator.SetFloat("Speed", 1.0f);

        inputToCommand = new InputToCommand(this);
        actionCtrl     = new ActionController(this, inputToCommand);
        HitRecordComponent = new HitRecordComponent();
        GameInstance.Instance.HitRecordSys.Register(HitRecordComponent);
        actionCtrl.SetChangeActinCallback((lastAction, newAction)=>{
            HitRecordComponent.Clear();
        });
    }

    void Update()
    {
        //fsm.Update();
        //CHController.Move(curMovement * speed * Time.deltaTime);
        //MoveDir();
        //inputToCommand.Tick();
    }

    // void FixedUpdate()
    // {
    //     inputToCommand.Tick();
    //     actionCtrl.Tick();
    // }

    public void UpdateLogic(ulong frameIndex)
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

    public bool CanAttackTargetNow(Character target, out AttackInfo attackPhase, out BeHitBoxTurnOnInfo defensePhase)
    {
        //attackPhase = new AttackInfo();
        // if (actionCtrl.ActiveAttackBoxTurnOnInfoList.Count > 0)
        // {
        //     int phase = actionCtrl.ActiveAttackBoxTurnOnInfoList[0].attackPhase;
        //     if (phase >= 0 && actionCtrl.CurAction.attackInfoList.Length > phase) attackPhase = actionCtrl.CurAction.attackInfoList[phase];
        //     return true;
        // }

        attackPhase  = new AttackInfo();
        defensePhase = new BeHitBoxTurnOnInfo();
        if (!target)
        {
            return false;
        }

        foreach (var pair in actionCtrl.BoxHits)
        {
            //命中的最有价值的受击框才行
            //BeHitBox best = null;
            int bestPriority = -1;
            bool foundBest = false;
            AttackBoxTurnOnInfo attackBox = GetAttackBoxTurnOnInfo(pair.Key);
            if (attackBox.FrameIndexRange.Length == 0) continue;
            for (int i = 0; i < pair.Value.Count; i++)
            {
                int thisBestPriority = pair.Value[i].Priority;
                if (thisBestPriority > bestPriority)
                {
                    bestPriority = thisBestPriority;
                    defensePhase = pair.Value[i];
                    foundBest    = true;
                }
            }
            if (!foundBest) continue;

            if (attackBox.AttackPhase >= 0 && attackBox.AttackPhase < actionCtrl.CurAction.attackInfoList.Length)
            {
                attackPhase = actionCtrl.CurAction.attackInfoList[attackBox.AttackPhase];
            }
            return true;
        }

        return false;
    }

    public HitRecord GetHitRecord(Character target, int phase)
    {
        var hitRecordList = HitRecordComponent.HitRecordList;
        int uid = target.gameObject.GetInstanceID();
        for (int i = 0; i < hitRecordList.Count; i++)
        {
            if (uid == hitRecordList[i].Id && phase == hitRecordList[i].Phase)
            {
                return hitRecordList[i];
            }
        }
        return null;
    }

    private AttackBoxTurnOnInfo GetAttackBoxTurnOnInfo(string tagName)
    {
        for (int i = 0; i < actionCtrl.CurAction.attackPhaseList.Length; i++)
        {
            for (int j = 0; i < actionCtrl.CurAction.attackPhaseList[i].RayPointGroupList.Length; j++)
            {
                if (tagName == actionCtrl.CurAction.attackPhaseList[i].RayPointGroupList[j].Tag)
                {
                    return actionCtrl.CurAction.attackPhaseList[i];
                }
            }
        }
        return new AttackBoxTurnOnInfo();
    }

    public ActionController GetActionController()
    {
        return actionCtrl;
    }

    public List<string> DebugActionList = new();
    public void LoadActions()
    {
        List<CharacterAction> actions = new List<CharacterAction>();
        for (int i = 0; i < DebugActionList.Count; i++)
        {
            TextAsset ta = Resources.Load<TextAsset>("GameData/" + DebugActionList[i]);
            if (ta)
            {
                ActionInfoContainer aic = JsonUtility.FromJson<ActionInfoContainer>(ta.text);
                //if (aic.data == null) continue;
                //foreach (CharacterAction info in aic.data)
                {
                    actions.Add(aic.data);
                    GameInstance.Instance.HitBoxDataPool.LoadHitBoxData(aic.data.mActionName);
                }
            }
        }
        if (DebugActionList.Count > 0)
        {
            actionCtrl.SetAllAction(actions, DebugActionList[0]);
        }
        SimpleLog.Info(gameObject.name, " LoadActions, ", "actions:", actions.Count);
    }

    /// <summary>
    /// 添加命中记录
    /// </summary>
    /// <param name="target">谁被命中</param>
    /// <param name="attackPhase">算是第几阶段的攻击命中的</param>
    /// <returns></returns>
    public void AddHitRecord(Character target, int attackPhase)
    {
        //int idx = action.IndexOfAttack(attackPhase);
        //if (idx < 0) return;    //没有这个伤害阶段，结束
        
        HitRecord rec = GetHitRecord(target, attackPhase);
        if (rec == null)
        {
            HitRecordComponent.AddHitRecord(new HitRecord(target, attackPhase, 0, 0));
        }
        else
        {
            rec.CooldownFrame = 0;
            rec.CanHitCount  -= 1;
        }
    }
}

[Serializable]
public struct ActionInfoContainer
{
    public CharacterAction data;
}