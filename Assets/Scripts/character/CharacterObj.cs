using System;
using System.Collections;
using System.Collections.Generic;
using Log;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class CharacterObj
{
    protected bool IsBeginPlayed;
    protected GameObject mGameObject;
    public GameObject gameObject => mGameObject;
    public Transform transform
    {
        get
        {
            if (mGameObject)
            {
                return mGameObject.transform;
            }
            return null;
        }
    }

    protected PlayerController mPlayerController;

    protected MovementComponent mMovementComponent;
    public MovementComponent MovementComp => mMovementComponent;

    protected ActionController mActionCtrl;
    public ActionController Action => mActionCtrl;

    protected InputToCommand mInputToCommand;

    public HitRecordComponent HitRecordComponent { get; private set; }

    //切换动作时会清空
    protected List<BeHitBox> mBeHitBoxList = new();
    public List<BeHitBox> BeHitBoxList => mBeHitBoxList;

    //切换动作时会清空
    protected List<AttackHitBox> mAttackHitBoxList = new();
    public List<AttackHitBox> AttackHitBoxList => mAttackHitBoxList;

    /// <summary>
    /// 速度，每秒移动多少单位
    /// </summary>
    public float Speed
    {
        get {
            if (mActionCtrl != null)
            {
                return mActionCtrl.CurAction.MoveSpeed;
            }
            return 0;
        }
    }

    public GameObject ModelRoot;
    public ModelComponent ModelComp;

    public CharacterObj()
    {
        IsBeginPlayed = false;
        mPlayerController = new PlayerController(this);
        mInputToCommand   = new InputToCommand(this);
        mActionCtrl       = new ActionController(this, mInputToCommand);
        mMovementComponent = new MovementComponent(this);
        HitRecordComponent = new HitRecordComponent();
        ModelComp          = new ModelComponent(this);
    }

    public virtual void Init()
    {
        //加载perfab
        var prefab = Resources.Load<GameObject>("Prefabs/Character/Player/Vergil/Vergil");
        mGameObject = GameObject.Instantiate(prefab);
        if (!mGameObject)
        {
            SimpleLog.Error("无法实例化", prefab.name);
            return;
        }
        mGameObject.name = prefab.name;
        mGameObject.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        mGameObject.transform.localScale = Vector3.one;
        
        CharacterController controller = mGameObject.GetComponent<CharacterController>();
        mMovementComponent.SetCharacterController(controller);
        
        //PlayerController
        var comp = mGameObject.GetComponent<InputReaderComponentMono>();
        if (comp)
        {
            mPlayerController.SetInputReader(comp.Input);
        }
        //PlayerController

        //HitRecordComponent
        GameInstance.Instance.HitRecordSys.Register(HitRecordComponent);
        mActionCtrl.SetChangeActinCallback((lastAction, newAction) => {
            HitRecordComponent.Clear();
        });
        //HitRecordComponent
    }

    public InputReader GetInputReader()
    {
        var comp = mGameObject.GetComponent<InputReaderComponentMono>();
        if (comp)
        {
            return comp.Input;
        }
        return null;
    }

    public virtual void BeginPlay()
    {
        IsBeginPlayed = true;
        mActionCtrl.BeginPlay();
        mPlayerController.BeginPlay();
        ModelComp.BeginPlay();
    }

    public virtual void EndPlay()
    {
        
    }

    public void UpdateLogic(int frameIndex)
    {
        mPlayerController.UpdateLogic();
        mInputToCommand.Tick();
        mActionCtrl.Tick();
        mMovementComponent.UpdateLogic(frameIndex);
        ModelComp.UpdateLogic(frameIndex);
    }

    public void UpdateRender(float deltaTime)
    {
        mMovementComponent.UpdateRender(deltaTime);
    }

    public virtual void Destroy()
    {
        ModelComp.EndPlay();
        GameObject.Destroy(mGameObject);
        mGameObject = null;

        mPlayerController = null;
        mInputToCommand   = null;
        mActionCtrl       = null;

        mMovementComponent.EndPlay();
        mMovementComponent = null;

        HitRecordComponent = null;
    }

    public void AddInputCommand(KeyMap key)
    {
        mInputToCommand?.AddInput(key);
    }

    public void AddActionCommand(KeyMap key)
    {
        mInputToCommand?.AddInput(key);
    }

    public PlayerController GetPlayerController()
    {
        return mPlayerController;
    }

    public float GetMoveInputAcceptance()
    {
        return mActionCtrl.MoveInputAcceptance;
    }

    void OnMovementInput(InputAction.CallbackContext context)
    {
        // Vector2 val       = context.ReadValue<Vector2>();
        // mCurrMovement.x   = val.x;
        // mCurrMovement.y   = 0.0f;
        // mCurrMovement.z   = val.y;
        // mIsMovementPresed = val.x != 0.0f || val.y != 0.0f;
    }

    void MoveDir()
    {
        // Vector3 posToLookat = new Vector3();
        // posToLookat.x       = mCurrMovement.x;
        // posToLookat.y       = 0.0f;
        // posToLookat.z       = mCurrMovement.z;
        // Quaternion curRot   = transform.rotation;
        // if (mIsMovementPresed)
        // {
        //     Quaternion targetRot = Quaternion.LookRotation(posToLookat);
        //     transform.rotation   = Quaternion.Slerp(curRot, targetRot, 1.0f);
        // }
    }

    public bool CanAttackTargetNow(CharacterObj target, out AttackInfo attackPhase, out BeHitBoxTurnOnInfo defensePhase)
    {
        attackPhase  = new AttackInfo();
        defensePhase = new BeHitBoxTurnOnInfo();
        if (target == null)
        {
            return false;
        }

        foreach (var pair in mActionCtrl.BoxHits)
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

            if (attackBox.AttackPhase >= 0 && attackBox.AttackPhase < mActionCtrl.CurAction.attackInfoList.Length)
            {
                attackPhase = mActionCtrl.CurAction.attackInfoList[attackBox.AttackPhase];
            }
            return true;
        }

        return false;
    }

    protected AttackBoxTurnOnInfo GetAttackBoxTurnOnInfo(string tagName)
    {
        for (int i = 0; i < mActionCtrl.CurAction.attackPhaseList.Length; i++)
        {
            for (int j = 0; i < mActionCtrl.CurAction.attackPhaseList[i].RayPointGroupList.Length; j++)
            {
                if (tagName == mActionCtrl.CurAction.attackPhaseList[i].RayPointGroupList[j].Tag)
                {
                    return mActionCtrl.CurAction.attackPhaseList[i];
                }
            }
        }
        return new AttackBoxTurnOnInfo();
    }

    public HitRecord GetHitRecord(CharacterObj target, int phase)
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

    // <summary>
    /// 添加命中记录
    /// </summary>
    /// <param name="target">谁被命中</param>
    /// <param name="attackPhase">算是第几阶段的攻击命中的</param>
    /// <returns></returns>
    public void AddHitRecord(CharacterObj target, int attackPhase)
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

    public void AddBeHitBox(BeHitBox box)
    {
        if (mBeHitBoxList.Contains(box))
        {
            return;
        }
        mBeHitBoxList.Add(box);
    }

    public void ClearBeHitBoxList()
    {
        mBeHitBoxList.Clear();
    }

    public void AddAttackHitBox(AttackHitBox box)
    {
        if (mAttackHitBoxList.Contains(box))
        {
            return;
        }
        mAttackHitBoxList.Add(box);
    }

    public void ClearAttackHitBoxList()
    {
        mAttackHitBoxList.Clear();
    }

    public List<string> DebugActionList = new()
    {
        "Vergil/Basic/Idle",
        "Vergil/Move/RunLoop",
        "Vergil/Yamato/ComboA_1",
        "Vergil/Yamato/ComboA_2",
        "Vergil/Yamato/ComboA_3",
        "Vergil/Yamato/ComboC_Start",
        "Vergil/Yamato/ComboC_Loop",
        "Vergil/Yamato/ComboC_Finish",
        "Vergil/Yamato/ComboB_1",
        "Vergil/Yamato/ComboB_2",
        "Vergil/Yamato/Zigenzan_Zetsu_Start",
        "Vergil/Yamato/Zigenzan_Zetsu_End",
    };
    public void LoadActions()
    {
        List<CharacterAction> actions = new List<CharacterAction>();
        for (int i = 0; i < DebugActionList.Count; i++)
        {
            TextAsset ta = Resources.Load<TextAsset>("GameData/ActionData/" + DebugActionList[i]);
            if (ta)
            {
                ActionInfoContainer aic = JsonUtility.FromJson<ActionInfoContainer>(ta.text);
                //if (aic.data == null) continue;
                //foreach (CharacterAction info in aic.data)
                {
                    aic.data.LoadRootMotion();
                    actions.Add(aic.data);
                    GameInstance.Instance.HitBoxDataPool.LoadHitBoxData(aic.data.mActionName);
                }
            }
        }
        if (actions.Count > 0)
        {
            mActionCtrl.SetAllAction(actions, actions[0].mActionName);
        }
        SimpleLog.Info(mGameObject.name, " LoadActions, ", "actions:", actions.Count);
    }

    public void PlayAnimation(string actionName, float normalizedTransitionDuration, float normalizedTimeOffset)
    {
        ModelComp.PlayAnimation(actionName);
    }

    public bool HasBegunPlay()
    {
        return IsBeginPlayed;
    }
}

[Serializable]
public struct ActionInfoContainer
{
    public CharacterAction data;
}
