using System.Collections.Generic;
using Cinemachine;
using Log;
using UnityEngine;

public class TestPlayerCharacterObj : CharacterObj
{
    private ModelComponent mModelComp;
    private HitRecordComponent mHitRecordComp;
    private MovementComponent mMovementComp;
    public TestPlayerCharacterObj()
    : base()
    {
        //mPlayerController = new AIController();
        //mPlayerController.SetOwner(this);

        VelocityComponent VelocityComp = new VelocityComponent(this, 1);
        AddComponent(VelocityComp);
        GravityComponent GravityComp = new GravityComponent(this, 2);
        AddComponent(GravityComp);
        mModelComp = new ModelComponent(this, 4);
        AddComponent(mModelComp);
    }

    public override void Init()
    {
        base.Init();

        mMovementComp = new MovementComponent(this, 3);
        AddComponent(mMovementComp);
        CharacterController controller = mGameObject.GetComponent<CharacterController>();
        mMovementComp.SetCharacterController(controller);

        //HitRecordComponent
        mHitRecordComp = new HitRecordComponent(this, 0);
        AddComponent(mHitRecordComp);
        GameInstance.Instance.HitRecordSys.Register(mHitRecordComp);
        mActionCtrl.SetChangeActinCallback((lastAction, newAction) => {
            mHitRecordComp.Clear();
        });
        //HitRecordComponent

        ModelRoot = mGameObject.transform.Find("Model").gameObject;

        mModelComp.SetModelRoot(ModelRoot);
        List<string> prefabs = new List<string>()
        {
            "Prefabs/Character/Player/Vergil/Body/body",
            "Prefabs/Character/Player/Vergil/Hair/hair",
        };
        mModelComp.LoadModel(prefabs);
    }

    public override void BeginPlay()
    {
        base.BeginPlay();
    }

    public override void Destroy()
    {
        mModelComp     = null;
        mHitRecordComp = null;
        mMovementComp  = null;
        base.Destroy();
    }

    public override void HandleInAirAction()
    {
        if (!mMovementComp.IsGrounded && mMovementComp.GetCharacterController().isGrounded)
        {
            bool found;
            CharacterAction action = Action.GetActionById("Jump_Vertical_Landing", out found);
            if (found && Action.CanActionCancelCurrentAction(action, false, out CancelTag foundTag, out BeCanceledTag beCabceledTag))
            {
                ActionChangeInfo info = new ActionChangeInfo() {
                    changeType = ActionChangeType.ChangeToActionId,
                    param      = "Jump_Vertical_Landing",
                    priority   = 2

                };
               Action.PreorderActionByActionChangeInfo(info);
            }

            action = Action.GetActionById("Jump_Vertical_Landing_Move", out found);
            if (found && Action.CanActionCancelCurrentAction(action, false, out CancelTag foundTag1, out BeCanceledTag beCabceledTag1))
            {
                ActionChangeInfo info = new ActionChangeInfo() {
                    changeType = ActionChangeType.ChangeToActionId,
                    param      = "Jump_Vertical_Landing_Move",
                    priority   = 2

                };
                Action.PreorderActionByActionChangeInfo(info);
            }
        }
        else if (!mMovementComp.IsGrounded && !mMovementComp.GetCharacterController().isGrounded)
        {
            //string actionName = mOwner.Action.CurAction.mActionName;
            //if (actionName == "")
            CharacterAction action = Action.GetActionById("Jump_Vertical_Fly_Loop", out bool found);
            if (found && Action.CanActionCancelCurrentAction(action, false, out CancelTag foundTag, out BeCanceledTag beCabceledTag))
            {
                ActionChangeInfo info = new ActionChangeInfo() {
                    changeType = ActionChangeType.ChangeToActionId,
                    param      = "Jump_Vertical_Fly_Loop",
                    priority   = 2

                };
                Action.PreorderActionByActionChangeInfo(info);
            }
        }
    }

    public override void LoadActions()
    {
        List<string> DebugActionList = new() {
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
            "Vergil/Basic/Jump_Vertical",
            "Vergil/Basic/Jump_Vertical_Fly_Loop",
            "Vergil/Basic/Jump_Vertical_Landing",
            "Vergil/Basic/Jump_Vertical_Move",
            "Vergil/Basic/Jump_Vertical_Fly_Loop_Move",
            "Vergil/Basic/Jump_Vertical_Landing_Move",
        };

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

    public override Vector3 ThisTickMove()
    {
        if (UnderForceMove)
        {
            return ForceMove();
        }
        else
        {
            return NatureMove();
        }
    }

    Vector3 NatureMove()
    {
        if (Action.CurAction.HasRootMotion())
        {
            Vector3 RootMotionMove = Action.RootMotionMove;
            // SimpleLog.Info("RootMotionMove", RootMotionMove);
            //Transform transform = gameObject.transform;
            // MoveInputAcceptance
            if (Action.CurAction.AppilyGravityInRootMotion)
            {
                // RootMotionMove.y = -mOwner.GravityComp.Gravity / GameInstance.Instance.LogicFrameRate;
                RootMotionMove.y = -9.8f / GameInstance.Instance.LogicFrameRate;
            }
            //float MoveInputAcceptance = GetMoveInputAcceptance();
            //RootMotionMove.x += adjDir.x * 6 * MoveInputAcceptance * (1f / GameInstance.Instance.LogicFrameRate);
            //RootMotionMove.z += adjDir.z * 6 * MoveInputAcceptance * (1f / GameInstance.Instance.LogicFrameRate);
            return RootMotionMove;
        }
        else
        {
            //todo
            //float MoveInputAcceptance = GetMoveInputAcceptance();
            //float speed               = Speed;
            //speed                     = speed / GameInstance.Instance.LogicFrameRate;
            //Vector3 motion            = adjDir * speed * MoveInputAcceptance;
            Vector3 motion = new Vector3(0, 0, 0);
            motion.y                  = -9.8f / GameInstance.Instance.LogicFrameRate;
            return motion;
        }
    }

    Vector3 ForceMove()
    {
        Vector3 move = mForceMove.MoveTween(mForceMove);
        mForceMove.Update();
        return move;
    }
}