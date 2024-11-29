using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class ActionController
{
    private Character mOwner;
    private Animator animator;
    private InputToCommand command;
    public List<CharacterAction> AllActions {get; set;} = new List<CharacterAction>();
    public CharacterAction CurAction { private set; get; } 
    private List<BeCanceledTag> curBeCanceledTagList = new List<BeCanceledTag>();
    private List<PreorderActionInfo> preorderActionList = new List<PreorderActionInfo>();
    private Action<CharacterAction, CharacterAction> _onChangeAction = null;
    public List<AttackBoxTurnOnInfo> ActiveAttackBoxTurnOnInfoList { private set; get; } = new List<AttackBoxTurnOnInfo>();
    public List<string> ActiveAttackBoxTurnOnInfoTagList { private set; get; } = new List<string>();

    /// <summary>
    /// 当前动画百分比，放这里方便些，其实最好放一个函数里
    /// 但是因为要访问动画百分比的频率较高，不如就一次性了
    /// </summary>
    private float _pec = 0;

    /// <summary>
    /// 这个动作在上一个update经历了多少百分比了
    /// </summary>
    private float _wasPercentage = 0;

    private uint mLastFrameIndex = 0;
    private uint mCurrentFrameIndex = 0;

    public float MoveInputAcceptance {get; private set;}

    public void SetChangeActinCallback(Action<CharacterAction, CharacterAction> cb)
    {
        _onChangeAction = cb;
    }

    public ActionController(Character owner, InputToCommand inputToCommand)
    {
        mOwner   = owner;
        animator = mOwner.GetComponent<Animator>();
        command  = inputToCommand;
    } 

    public void SetAllAction(List<CharacterAction> actions, string defaultActionId)
    {
        AllActions.Clear();
        if (actions != null) AllActions = actions;
        ChangeAction(defaultActionId, 0.0f, 0.0f, 0);
    }

    public void Tick()
    {
        if (AllActions.Count == 0) return;

        float dt = Time.deltaTime;

        AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
        AnimatorStateInfo nextAnimatorState = animator.GetNextAnimatorStateInfo(0);
        _pec = Mathf.Clamp01(nextAnimatorState.length > 0 ? nextAnimatorState.normalizedTime : animatorState.normalizedTime);

        mCurrentFrameIndex += 1;
        if (mCurrentFrameIndex > CurAction.mTotalFrameCount)
        {
            mCurrentFrameIndex = 0;
        }

        CalculateBoxInfo(_wasPercentage, _pec);
        //计算移动接受输入
        CalculateInputAcceptance(_wasPercentage, _pec);
        
        foreach (CharacterAction ac in AllActions)
        {
            if (CanActionCancelCurrentAction(ac, _pec, true, out CancelTag foundTag, out BeCanceledTag beCanceledTag))
            {
                //Log.SimpleLog.Info("CanActionCancelCurrentAction:", CurAction.mActionName, ac.mActionName, foundTag.startFromFrameIndex);
                preorderActionList.Add(new PreorderActionInfo(ac.mActionName, ac.mPriority + foundTag.priority + beCanceledTag.priority,
                Mathf.Min(beCanceledTag.fadeOutPercentage, foundTag.fadeInPercentage), foundTag.startFromPercentage, 0, foundTag.startFromFrameIndex));
            }
        }
        //if (preorderActionList.Count == 0 ||)
        if (preorderActionList.Count == 0 && (_pec >= 1 || CurAction.mAutoTerminate))
        {
            preorderActionList.Add(new PreorderActionInfo(CurAction.mAutoNextActionName));
        }

        _wasPercentage = _pec;

        mLastFrameIndex = mCurrentFrameIndex;

        if (preorderActionList.Count > 0)
        {
            preorderActionList.Sort((action1, action2) => action1.Priority > action2.Priority ? -1 : 1);
            if (preorderActionList[0].ActionId == CurAction.mActionName && CurAction.keepPlayingAnim)
            {
                KeepAction(_pec);
            }
            else
            {
                ChangeAction(preorderActionList[0].ActionId, preorderActionList[0].TransitionNormalized, preorderActionList[0].FromNormalized, preorderActionList[0].FromFrameIndex);
            }
        }

        preorderActionList.Clear();
    }

    bool CanActionCancelCurrentAction(CharacterAction actionInfo, float curPercent, bool checkCommand, out CancelTag foundTag, out BeCanceledTag beCabceledTag)
    {
        foundTag = new CancelTag();
        beCabceledTag = new BeCanceledTag();
        foreach (BeCanceledTag bcTagInfo in curBeCanceledTagList)
        {
            bool tagFit = false;
            foreach (string bcTagName in bcTagInfo.cancelTag)
            {
                //if (!(_wasPercentage <= bcTagInfo.range.max && curPercent >= bcTagInfo.range.min)) continue;
                //Log.SimpleLog.Info("CanActionCancelCurrentAction:", mLastFrameIndex, mCurrentFrameIndex);
                if (!(mCurrentFrameIndex <= bcTagInfo.frameIndexRange.max && mCurrentFrameIndex >= bcTagInfo.frameIndexRange.min)) continue;
                foreach (CancelTag cTag in actionInfo.mCancelTagList)
                {
                    if (bcTagName == cTag.tag)
                    {
                        tagFit = true;
                        foundTag = cTag;
                        beCabceledTag = bcTagInfo;
                        break;
                    }
                }
                if (tagFit) break;
            }
            if (!tagFit) continue;

            if (checkCommand)
            {
                foreach (ActionCommand cmd in actionInfo.mCommandList)
                {
                    if (command.ActionOccur(cmd)) return true;
                }
            }
            else
            {
                return true;
            }
        }

        return false;
    }

    void KeepAction(float currentNormalized)
    {
        if (currentNormalized >= 1)
        {
            animator.CrossFade(CurAction.mAnimation, 0, 0, 0);
            if (mCurrentFrameIndex > CurAction.mTotalFrameCount)
            {
                mCurrentFrameIndex = 0;
            }
        }
    }

    void ChangeAction(string actionName, float normalizedTransitionDuration, float normalizedTimeOffset, uint fromFrameIndex)
    {
        if (GetActionById(actionName, out CharacterAction action))
        {
            Log.SimpleLog.Log("ChangeAction: ", actionName, fromFrameIndex);
            //
            _onChangeAction?.Invoke(CurAction, action);
            animator.CrossFade(action.mAnimation, normalizedTransitionDuration, 0, normalizedTimeOffset);
            //
            CurAction = action;
            curBeCanceledTagList.Clear();
            foreach (BeCanceledTag tag in CurAction.mBeCanceledTagList)
            {
                curBeCanceledTagList.Add(tag);
            }

            ActiveAttackBoxTurnOnInfoList.Clear();
            ActiveAttackBoxTurnOnInfoTagList.Clear();

            mCurrentFrameIndex = fromFrameIndex;
            mLastFrameIndex    = fromFrameIndex;
        }
        else
        {
            mCurrentFrameIndex = 0;
            mLastFrameIndex    = 0;
        }
    }

    bool GetActionById(string actionId, out CharacterAction action)
    {
        action = new CharacterAction();
        for (int i = 0; i < AllActions.Count; i++)
        {
            if (actionId == AllActions[i].mActionName)
            {
                action = AllActions[i];
                return true;
            }
        }
        return false;
    }

    public void CalculateInputAcceptance(float wasPec, float pec)
    {
        MoveInputAcceptance = 0;
        if (CurAction.moveInputAcceptance == null) return;
        foreach (MoveInputAcceptance acceptance in CurAction.moveInputAcceptance)
        {
            if (acceptance.range.min <= pec && acceptance.range.max >= wasPec &&
                (MoveInputAcceptance <= 0 || acceptance.rate < MoveInputAcceptance))
                MoveInputAcceptance = acceptance.rate;
        }
    }

    public void AddTempBeCanceledTag(TempBeCancelledTag tag)
    {
        curBeCanceledTagList.Add(BeCanceledTag.FromTemp(tag, _pec));
    }

    public void AddTempBeCanceledTag(string id)
    {
        foreach (TempBeCancelledTag temp in CurAction.mTempBeCanceledTagList)
        {
            if (id == temp.id)
            {
                AddTempBeCanceledTag(temp);
                return;
            }
        }
    }

    /// <summary>
    /// 计算当前动画帧的信息
    /// </summary>
    /// <param name="wasPec">上一帧的百分比</param>
    /// <param name="pec">百分比进度</param>
    private void CalculateBoxInfo(float wasPec, float pec)
    {
        ActiveAttackBoxTurnOnInfoList.Clear();
        ActiveAttackBoxTurnOnInfoTagList.Clear();
        foreach (AttackBoxTurnOnInfo info in CurAction.attackPhaseList)
        {
            bool open = false;
            // foreach (PercentageRange range in info.turnOnRangeList)
            // {
            //     if (pec >= range.min && wasPec <= range.max)
            //     {
            //         open = true;
            //         break;
            //     }
            // }
            // if (open)
            // {
            //     //Log.SimpleLog.Info("range: ", range.min, range.max, wasPec, pec);
            //     foreach (string tag in info.attackBoxTag)
            //     {
            //         if (!ActiveAttackBoxTurnOnInfoTagList.Contains(tag)) ActiveAttackBoxTurnOnInfoTagList.Add(tag);
            //     }
            //     ActiveAttackBoxTurnOnInfoList.Add(info);
            // }
        }
    }
}