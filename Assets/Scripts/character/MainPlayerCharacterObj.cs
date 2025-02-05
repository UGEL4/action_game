using System.Collections.Generic;
using Cinemachine;
using Log;
using Unity.VisualScripting;
using UnityEngine;

public class MainPlayerCharacterObj : CharacterObj
{
    public YamatoObj Y;

    private ModelComponent mModelComp;
    private HitRecordComponent mHitRecordComp;
    private MovementComponent mMovementComp;

    public MainPlayerCharacterObj()
    : base()
    {
        mPlayerController = new PlayerController();
        mPlayerController.SetOwner(this);
        Y = new YamatoObj(this);

        VelocityComponent VelocityComp = new VelocityComponent(this, 2);
        AddComponent(VelocityComp);
        GravityComponent GravityComp = new GravityComponent(this, 1);
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

        //PlayerController
        var comp = mGameObject.GetComponent<InputReaderComponentMono>();
        if (comp)
        {
            mPlayerController.SetInputReader(comp.Input);
        }
        //PlayerController

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

        mActionCtrl.OnActionNotify += Y.ActionNotify;
    }

    public override void BeginPlay()
    {
        base.BeginPlay();

        Y.BeginPlay();

        //摄像机
        var obj = GameObject.Find("CameraSystem/FreeLook Camera");
        if (obj)
        {
            CinemachineFreeLook camera = obj.GetComponent<CinemachineFreeLook>();
            if (camera)
            {
                var cameraPivot = mGameObject.transform.Find("Model/CameraPivot");
                if (cameraPivot)
                {
                    camera.Follow = cameraPivot;
                    camera.LookAt = cameraPivot;
                }
            }
        }
    }

    public override void Destroy()
    {
        mActionCtrl.OnActionNotify -= Y.ActionNotify;
        Y.EndPlay();
        Y = null;

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

    public override void PlayAnimation(string actionName, float normalizedTransitionDuration, float normalizedTimeOffset)
    {
        mModelComp?.PlayAnimation(actionName);
    }

    public override HitRecord GetHitRecord(CharacterObj target, int phase)
    {
        var hitRecordList = mHitRecordComp.HitRecordList;
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

    public override void AddHitRecord(CharacterObj target, int attackPhase)
    {
        //int idx = action.IndexOfAttack(attackPhase);
        //if (idx < 0) return;    //没有这个伤害阶段，结束
        HitRecord rec = GetHitRecord(target, attackPhase);
        if (rec == null)
        {
            mHitRecordComp.AddHitRecord(new HitRecord(target, attackPhase, 0, 0));
        }
        else
        {
            rec.CooldownFrame = 0;
            rec.CanHitCount  -= 1;
        }
    }

    public override Vector3 ThisTickMove()
    {
        if (Action.IsUnderForceMove)
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
        PlayerController pc = mPlayerController != null ? (mPlayerController as PlayerController) : null;
        if (pc == null)
        {
            return Vector3.zero;
        }
        var adjDir = pc.CharacterRelativeFlatten(pc.CurrMoveDir);
        if (Action.CurAction.HasRootMotion())
        {
            Vector3 RootMotionMove = Action.RootMotionMove;
            // SimpleLog.Info("RootMotionMove", RootMotionMove);
            if (adjDir.magnitude > 0.1f)
            {
                Quaternion targetRotation = Quaternion.LookRotation(adjDir);
                Transform transform       = gameObject.transform;
                if (transform)
                {
                    // // 更新角色的旋转
                    // transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, Time.deltaTime * 10f);
                    // transform.LookAt(transform.position + adjDir);
                    transform.rotation = targetRotation;
                }
                // MoveInputAcceptance
                if (Action.CurAction.AppilyGravityInRootMotion)
                {
                    // RootMotionMove.y = -mOwner.GravityComp.Gravity / GameInstance.Instance.LogicFrameRate;
                    RootMotionMove.y = -9.8f / GameInstance.Instance.LogicFrameRate;
                }
                RootMotionMove            = targetRotation * RootMotionMove;
                float MoveInputAcceptance = GetMoveInputAcceptance();
                RootMotionMove.x += adjDir.x * 6 * MoveInputAcceptance * (1f / GameInstance.Instance.LogicFrameRate);
                RootMotionMove.z += adjDir.z * 6 * MoveInputAcceptance * (1f / GameInstance.Instance.LogicFrameRate);
                return RootMotionMove;
            }
            else
            {
                if (Action.CurAction.AppilyGravityInRootMotion)
                {
                    // RootMotionMove.y = -mOwner.GravityComp.Gravity / GameInstance.Instance.LogicFrameRate;
                    RootMotionMove.y = -9.8f / GameInstance.Instance.LogicFrameRate;
                }
                return RootMotionMove;
            }
            /*
                // 用户输入
Vector2 input = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

// 将输入转为 3D 方向向量
Vector3 moveDirection = new Vector3(input.x, 0, input.y).normalized;

// 可选：输入方向基于摄像机的方向
Vector3 cameraForward = Camera.main.transform.forward;
Vector3 cameraRight = Camera.main.transform.right;
cameraForward.y = 0;
cameraRight.y = 0;
cameraForward.Normalize();
cameraRight.Normalize();
moveDirection = cameraRight * input.x + cameraForward * input.y;
moveDirection = moveDirection.normalized;

// 假设动画位移是动画计算出来的每帧偏移量
Vector3 animationDelta = new Vector3(1, 0, 3);

if (moveDirection.sqrMagnitude > 0.01f) // 确保有输入
{
    // 将动画的位移朝向用户输入方向旋转
    Quaternion targetRotation = Quaternion.LookRotation(moveDirection);
    animationDelta = targetRotation * animationDelta;
}

// 使用 CharacterController 移动
CharacterController controller = GetComponent<CharacterController>();

// 最终位移增量（加上重力影响，如果有需要）
Vector3 finalMoveDelta = animationDelta * Time.deltaTime; // 保证时间步依赖
controller.Move(finalMoveDelta);
            */
        }
        else
        {
            float MoveInputAcceptance = GetMoveInputAcceptance();
            float speed               = Speed;
            speed                     = speed / GameInstance.Instance.LogicFrameRate;
            Vector3 motion            = adjDir * speed * MoveInputAcceptance;
            motion.y                  = -9.8f / GameInstance.Instance.LogicFrameRate;
            return motion;
        }
    }

    Vector3 ForceMove()
    {
        Vector3 Move        = Action.UnderForceMove;
        Quaternion Rot      = Action.RootMotionRotation;
        Transform transform = gameObject.transform;
        if (transform)
        {
            float MoveInputAcceptance = GetMoveInputAcceptance();
            if (MoveInputAcceptance > 0)
            {
                Move *= MoveInputAcceptance;
            }
            transform.Rotate(Rot.eulerAngles);
            return transform.rotation * Move;
        }
        return Vector3.zero;
    }

    public override Quaternion ThisTickRotation()
    {
        return Quaternion.identity;
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
}