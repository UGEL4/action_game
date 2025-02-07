using System;
using System.Collections.Generic;
using Log;
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

    protected IController mPlayerController;

    protected ActionController mActionCtrl;
    public ActionController Action => mActionCtrl;

    protected InputToCommand mInputToCommand;

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

    protected List<ComponentBase> mComponents = new();

    protected ForceMove mForceMove = ForceMove.NoForceMove;
    protected bool UnderForceMove => mForceMove.FrameElapsed <= mForceMove.Data.InFrame;

    public CharacterObj()
    {
        IsBeginPlayed = false;
        mInputToCommand   = new InputToCommand(this);
        mActionCtrl       = new ActionController(this, mInputToCommand);
    }

    public void AddComponent(ComponentBase component)
    {
        if (!mComponents.Contains(component))
        {
            mComponents.Add(component);
            //排序
            mComponents.Sort((a, b) => a.mPriority < b.mPriority ? -1 : 1);
        }
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
    }

    // public InputReader GetInputReader()
    // {
    //     var comp = mGameObject.GetComponent<InputReaderComponentMono>();
    //     if (comp)
    //     {
    //         return comp.Input;
    //     }
    //     return null;
    // }

    public virtual void BeginPlay()
    {
        IsBeginPlayed = true;
        mActionCtrl.BeginPlay();
        mPlayerController?.BeginPlay();
        for (int i = 0; i < mComponents.Count; i++)
        {
            mComponents[i].BeginPlay();
        }
    }

    public virtual void EndPlay()
    {
        
    }

    public void UpdateLogic(int frameIndex)
    {
        mPlayerController?.UpdateLogic();
        mInputToCommand.Tick();
        mActionCtrl.Tick();

        for (int i = 0; i < mComponents.Count; i++)
        {
            mComponents[i].UpdateLogic(frameIndex);
        }
    }

    public void UpdateRender(float deltaTime)
    {
        for (int i = 0; i < mComponents.Count; i++)
        {
            mComponents[i].UpdateRender(deltaTime);
        }
    }

    public virtual void Destroy()
    {
        for (int i = 0; i < mComponents.Count; i++)
        {
            mComponents[i].EndPlay();
        }
        mComponents.Clear();

        mPlayerController = null;
        mInputToCommand   = null;
        mActionCtrl       = null;

        GameObject.Destroy(mGameObject);
        mGameObject = null;
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
        return mPlayerController != null ? (mPlayerController as PlayerController) : null;
    }

    public float GetMoveInputAcceptance()
    {
        return mActionCtrl.MoveInputAcceptance;
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
            //AttackBoxTurnOnInfo attackBox = GetAttackBoxTurnOnInfo(pair.Key);
            AttackBoxTurnOnInfo attackBox = pair.Key;
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

    public virtual HitRecord GetHitRecord(CharacterObj target, int phase)
    {
        return null;
    }

    // <summary>
    /// 添加命中记录
    /// </summary>
    /// <param name="target">谁被命中</param>
    /// <param name="attackPhase">算是第几阶段的攻击命中的</param>
    /// <returns></returns>
    public virtual void AddHitRecord(CharacterObj target, int attackPhase)
    {
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

    public virtual void LoadActions()
    {

    }

    public virtual void PlayAnimation(string actionName, float normalizedTransitionDuration, float normalizedTimeOffset)
    {
    }

    public bool HasBegunPlay()
    {
        return IsBeginPlayed;
    }

    public virtual void HandleInAirAction()
    {
        
    }

    public virtual Vector3 ThisTickMove()
    {
        return Vector3.zero;
    }

    public virtual Quaternion ThisTickRotation()
    {
        return Quaternion.identity;
    }

    public T GetComponent<T>() where T : ComponentBase
    {
        System.Type t = typeof(T);
        for (int i = 0; i < mComponents.Count; i++)
        {
            if (t == mComponents[i].GetComponentType())
            {
                return (T)mComponents[i];
            }
        }
        return null;
    }

    public void SetForceMove(MoveInfo info)
    {
        mForceMove = ForceMove.FromData(info);
    }
}

[Serializable]
public struct ActionInfoContainer
{
    public CharacterAction data;
}
