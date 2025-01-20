using System.Collections.Generic;
using Cinemachine;
using Log;
using UnityEngine;

public class MainPlayerCharacterObj : CharacterObj
{
    public YamatoObj Y;

    public MainPlayerCharacterObj()
    : base()
    {
        Y = new YamatoObj(this);
    }

    public override void Init()
    {
        base.Init();

        ModelRoot = mGameObject.transform.Find("Model").gameObject;

        ModelComp.SetModelRoot(ModelRoot);
        List<string> prefabs = new List<string>()
        {
            "Prefabs/Character/Player/Vergil/Body/body",
            "Prefabs/Character/Player/Vergil/Hair/hair",
        };
        ModelComp.LoadModel(prefabs);

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
        base.Destroy();
    }

    public override void HandleInAirAction()
    {
        if (!mMovementComponent.IsGrounded && mMovementComponent.GetCharacterController().isGrounded)
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
        else if (!mMovementComponent.IsGrounded && !mMovementComponent.GetCharacterController().isGrounded)
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
}