using System;
using System.Collections;
using System.Collections.Generic;
using ACTTools.RootMotionData;
using Log;
using Unity.VisualScripting;
using UnityEngine;
public class ActionController
{
    private CharacterObj mOwner;
    private InputToCommand command;
    public List<CharacterAction> AllActions {get; set;} = new List<CharacterAction>();
    public CharacterAction CurAction { private set; get; } 
    private List<BeCanceledTag> curBeCanceledTagList = new List<BeCanceledTag>();
    private List<PreorderActionInfo> preorderActionList = new List<PreorderActionInfo>();
    private Action<CharacterAction, CharacterAction> _onChangeAction = null;

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
    public uint CurrentFrameIndex => mCurrentFrameIndex;

    public float MoveInputAcceptance {get; private set;}

    private Dictionary<string, List<BeHitBoxTurnOnInfo>> mBoxHits = new();
    public Dictionary<string, List<BeHitBoxTurnOnInfo>> BoxHits => mBoxHits;

    public Vector3 RootMotionMove {private set; get;} = Vector3.zero;
    public Quaternion RootMotionRotation {private set; get; } = Quaternion.identity;

    public Vector3 UnderForceMove {private set; get;} = Vector3.zero;
    public bool IsUnderForceMove {private set; get; } = false;

    public Action<string, string[]> OnActionNotify = delegate {};

    public void SetChangeActinCallback(Action<CharacterAction, CharacterAction> cb)
    {
        _onChangeAction = cb;
    }

    public ActionController(CharacterObj owner, InputToCommand inputToCommand)
    {
        mOwner   = owner;
        command  = inputToCommand;
    }

    public void BeginPlay()
    {
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

        //float dt = Time.deltaTime;

        //AnimatorStateInfo animatorState = animator.GetCurrentAnimatorStateInfo(0);
        //AnimatorStateInfo nextAnimatorState = animator.GetNextAnimatorStateInfo(0);
        //_pec = Mathf.Clamp01(nextAnimatorState.length > 0 ? nextAnimatorState.normalizedTime : animatorState.normalizedTime);

        mCurrentFrameIndex += 1;
        if (mCurrentFrameIndex >= CurAction.mTotalFrameCount)
        {
            mCurrentFrameIndex = 0;
        }

        //通知
        if (CurAction.Notifies != null && CurAction.Notifies.Length > 0)
        {
            for (int i = 0; i < CurAction.Notifies.Length; i++)
            {
                if ((int)mCurrentFrameIndex == CurAction.Notifies[i].FrameIndex)
                {
                    //触发通知
                    ActionNotify(CurAction.Notifies[i].FunctionName, CurAction.Notifies[i].Params);
                }
            }
        }

        ////CalculateBoxInfo(_wasPercentage, _pec);
        //计算移动接受输入
        CalculateInputAcceptance();
        
        foreach (CharacterAction ac in AllActions)
        {
            if (CanActionCancelCurrentAction(ac, true, out CancelTag foundTag, out BeCanceledTag beCanceledTag))
            {
                //Log.SimpleLog.Info("CanActionCancelCurrentAction:", CurAction.mActionName, ac.mActionName, foundTag.startFromFrameIndex);
                preorderActionList.Add(new PreorderActionInfo(ac.mActionName, ac.mPriority + foundTag.priority + beCanceledTag.priority,
                Mathf.Min(beCanceledTag.fadeOutPercentage, foundTag.fadeInPercentage), foundTag.startFromPercentage, 0, foundTag.startFromFrameIndex));
            }
        }
        //if (preorderActionList.Count == 0 ||)
        //if (preorderActionList.Count == 0 && (_pec >= 1 || CurAction.mAutoTerminate))
        if (preorderActionList.Count == 0 && (mCurrentFrameIndex == 0 || CurAction.mAutoTerminate))
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
                KeepAction(0);
            }
            else
            {
                ChangeAction(preorderActionList[0].ActionId, preorderActionList[0].TransitionNormalized, preorderActionList[0].FromNormalized, preorderActionList[0].FromFrameIndex);
            }
        }

        //要处理当没有rootmotion时的移动情况
        //可以有一个默认的移动速度
        //RootMotionMove = GetRootMotionData();
        //RootMotionRotation = GetRootMotionRotation((int)mCurrentFrameIndex);
        ExtractRootMotion();

        preorderActionList.Clear();
        mBoxHits.Clear();
    }

    private int ActionLoopCount = 0;
    bool CanActionCancelCurrentAction(CharacterAction actionInfo, bool checkCommand, out CancelTag foundTag, out BeCanceledTag beCabceledTag)
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
                        if (actionInfo.SelfLoopCount > 0 && ActionLoopCount > actionInfo.SelfLoopCount)
                        {
                            continue;
                        }
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
            //animator.CrossFade(CurAction.mAnimation, 0, 0, 0);
            mOwner.PlayAnimation(CurAction.mAnimation, 0, 0);
            if (mCurrentFrameIndex > CurAction.mTotalFrameCount)
            {
                mCurrentFrameIndex = 0;
            }
        }
        if (mCurrentFrameIndex == 0) mCurrentFrameIndex = 1;
    }

    void ChangeAction(string actionName, float normalizedTransitionDuration, float normalizedTimeOffset, uint fromFrameIndex)
    {
        CharacterAction action = GetActionById(actionName, out bool found);
        if (found)
        {
            SimpleLog.Info("ChangeAction: ", actionName, action.mAnimation);
            //
            _onChangeAction?.Invoke(CurAction, action);
            //animator.CrossFade(action.mAnimation, normalizedTransitionDuration, 0, normalizedTimeOffset);
            mOwner.PlayAnimation(action.mAnimation, normalizedTransitionDuration, normalizedTimeOffset);
            //
            if (CurAction.mActionName == actionName && CurAction.SelfLoopCount > 0)
            {
                ActionLoopCount++;
            }
            else
            {
                ActionLoopCount = 0;
            }
            CurAction = action;
            curBeCanceledTagList.Clear();
            foreach (BeCanceledTag tag in CurAction.mBeCanceledTagList)
            {
                curBeCanceledTagList.Add(tag);
            }

            mCurrentFrameIndex = fromFrameIndex;
            mLastFrameIndex    = fromFrameIndex;
            //mOwner.HitRecordComponent.Clear();
            GameInstance.Instance.HitBoxUpdate.OnChangeAction(mOwner, actionName);
            mOwner.ClearBeHitBoxList();
            var hitBoxDataMap = GameInstance.Instance.HitBoxDataPool.GetActionHitBoxData(actionName);
            foreach (var hitBoxData in hitBoxDataMap)
            {
                mOwner.AddBeHitBox(new BeHitBox(mOwner, hitBoxData.Key, false));
            }
            mOwner.ClearAttackHitBoxList();
            for (int i = 0; i < CurAction.attackPhaseList.Length; i++)
            {
                for (int j = 0; j < CurAction.attackPhaseList[i].RayPointGroupList.Length; j++)
                {
                    string tag = CurAction.attackPhaseList[i].RayPointGroupList[j].Tag;
                    mOwner.AddAttackHitBox(new AttackHitBox(mOwner, tag, AttackHitBoxType.Ray, false));
                }
            }
        }
        else
        {
            mCurrentFrameIndex = 0;
            mLastFrameIndex    = 0;
            ActionLoopCount = 0;
        }
        mBoxHits.Clear();
    }

    CharacterAction GetActionById(string actionId, out bool found)
    {
        found = false;
        for (int i = 0; i < AllActions.Count; i++)
        {
            if (actionId == AllActions[i].mActionName)
            {
                found = true;
                return AllActions[i];
            }
        }
        return CurAction;
    }

    public void CalculateInputAcceptance()
    {
        MoveInputAcceptance = 0;
        if (CurAction.moveInputAcceptance == null) return;
        // foreach (MoveInputAcceptance acceptance in CurAction.moveInputAcceptance)
        // {
        //     if (acceptance.range.min <= pec && acceptance.range.max >= wasPec &&
        //         (MoveInputAcceptance <= 0 || acceptance.rate < MoveInputAcceptance))
        //         MoveInputAcceptance = acceptance.rate;
        // }
        if (CurAction.moveInputAcceptance.Length > 0)
        {
            MoveInputAcceptance = CurAction.moveInputAcceptance[0].rate;
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

    public void OnAttackBoxHit(string tag, BeHitBoxTurnOnInfo target)
    {
        if (!mBoxHits.ContainsKey(tag))
        {
            mBoxHits.Add(tag, new List<BeHitBoxTurnOnInfo>());
        }
        bool found = false;
        List<BeHitBoxTurnOnInfo> list = mBoxHits[tag];
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < target.Tags.Length; j++)
            {
                for (int k = 0; k < list[i].Tags.Length; k++)
                {
                    if (target.Tags[j] == list[i].Tags[k])
                    {
                        found = true;
                        break;
                    }
                }
                if (found) break;
            }
            if (found) break;
        }
        if (! found)
        {
            list.Add(target);
        }
    }

    public void OnAttackBoxExit(string tag, BeHitBoxTurnOnInfo target)
    {
        if (!mBoxHits.ContainsKey(tag))
        {
            return;
        }
        List<BeHitBoxTurnOnInfo> list = mBoxHits[tag];
        for (int i = 0; i < list.Count; i++)
        {
            for (int j = 0; j < target.Tags.Length; j++)
            {
                for (int k = 0; k < list[i].Tags.Length; k++)
                {
                    if (target.Tags[j] == list[i].Tags[k])
                    {
                        list.RemoveAt(i);
                        return;
                    }
                }
            }
        }
    }

    public void PreorderActionByActionChangeInfo(ActionChangeInfo info)
    {
        switch (info.changeType)
        {
            case ActionChangeType.Keep: break;
            case ActionChangeType.ChangeToActionId:
            {
                CharacterAction action = GetActionById(info.param, out bool found);
                if (found)
                {
                    preorderActionList.Add(new PreorderActionInfo {
                        ActionId                  = action.mActionName,
                        Priority                  = action.mPriority + info.priority,
                        TransitionNormalized      = info.transNormalized,
                        FromNormalized            = info.fromNormalized,
                        FreezingAfterChangeAction = 0,
                        FromFrameIndex            = 0
                    });
                }
                break;
            }
        }
    }

    public float MoveSpeed = 0f;
    private Vector3 GetRootMotionData()
    {
        MoveSpeed = 0;
        IsUnderForceMove = CurAction.ForceMoce;
        if (string.IsNullOrEmpty(CurAction.mActionName))
        {
            return Vector3.zero;
        }
        Vector3 Move = Vector3.zero;
        if (mCurrentFrameIndex > 0)
        {
            Vector3 LastMove = GetRootMotionPosition((int)mCurrentFrameIndex - 1);
            Vector3 NowMove  = GetRootMotionPosition((int)mCurrentFrameIndex);
            Move             = NowMove - LastMove;
        }
        else
        {
            Move = GetRootMotionPosition((int)mCurrentFrameIndex);
        }
        MoveSpeed = Move.magnitude * GameInstance.Instance.LogicFrameRate;

        if (CurAction.ForceMoce)
        {
            UnderForceMove   = Move;
        }
        return Move;
    }

    private Vector3 GetRootMotionPosition(int index, bool lastFrame)
    {
        if (index < 0)
        {
            return Vector3.zero;
        }
        RootMotionData data = CurAction.RootMotionData;
        float x             = 0;
        float y             = 0;
        float z             = 0;
        if (data.X != null)
        {
            if (index >= data.X.Length && index > 0)
            {
                if (lastFrame)
                {
                    if (index >= data.X.Length && index > 0)
                    {
                        
                    }
                }
                x = data.X[lastFrame ? data.X.Length - 2 : data.X.Length - 1];
            }
            else
            {
                x = data.X[index];
            }
        }
        if (data.Y != null)
        {
            if (index >= data.Y.Length && index > 0)
            {
                y = data.Y[lastFrame ? data.Y.Length - 2 : data.Y.Length - 1];
            }
            else
            {
                y = data.Y[index];
            }
        }
        if (data.Z != null)
        {
            if (index >= data.Z.Length && index > 0)
            {
                z = data.Z[lastFrame ? data.Z.Length - 2 : data.Z.Length - 1];
            }
            else
            {
                z = data.Z[index];
            }
        }
        return new Vector3(x, y, z);
    }

    private Quaternion GetRootMotionRotation(int index, bool lastFrame)
    {
        if (index < 0)
        {
            //index = 0;
            return Quaternion.identity;
        }
        RootMotionData data = CurAction.RootMotionData;
        float x             = 0;
        float y             = 0;
        float z             = 0;
        if (data.RX != null)
        {
            if (index >= data.RX.Length && index > 0)
            {
                x = data.RX[lastFrame ? data.RX.Length - 2 : data.RX.Length - 1];
            }
            else
            {
                x = data.RX[index];
            }
        }
        if (data.RY != null)
        {
            if (index >= data.RY.Length && index > 0)
            {
                y = data.RY[lastFrame ? data.RY.Length - 2 : data.RY.Length - 1];
            }
            else
            {
                y = data.RY[index];
            }
        }
        if (data.RZ != null)
        {
            if (index >= data.RZ.Length && index > 0)
            {
                z = data.RZ[lastFrame ? data.RZ.Length - 2 : data.RZ.Length - 1];
            }
            else
            {
                z = data.RZ[index];
            }
        }
        return Quaternion.Euler(x, y, z);
    }

    void ExtractRootMotion()
    {
        //Vector3 currentWorldPos = mOwner.transform.position;
        //Quaternion currentWorldRot = mOwner.transform.rotation;
        int frameIndex = (int)mCurrentFrameIndex;
        //先不管循环
        Vector3 prevPos     = GetRootMotionPosition(frameIndex - 1);
        Vector3 currPos     = GetRootMotionPosition(frameIndex);
        Quaternion prevRot  = GetRootMotionRotation(frameIndex - 1);
        Quaternion currRot  = GetRootMotionRotation(frameIndex);
        Vector3 deltaPos    = currPos - prevPos;
        Quaternion deltaRot = currRot * Quaternion.Inverse(prevRot);
        RootMotionMove      = deltaPos;
        RootMotionRotation  = deltaRot;
        UnderForceMove      = deltaPos;
        IsUnderForceMove    = CurAction.ForceMoce;
    }

    void ActionNotify(string functionName, string[] param)
    {
        OnActionNotify.Invoke(functionName, param);
    }
}