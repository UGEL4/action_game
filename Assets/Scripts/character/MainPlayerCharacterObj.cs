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
}