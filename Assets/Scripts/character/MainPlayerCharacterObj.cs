using Cinemachine;
using UnityEngine;

public class MainPlayerCharacterObj : CharacterObj
{
    public MainPlayerCharacterObj()
    : base()
    {

    }

    public override void BeginPlay()
    {
        base.BeginPlay();

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
}