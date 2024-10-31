using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[Serializable]
public struct BoxData
{
    Transform transform;
    OBB bounds;
}

[Serializable]
public struct KeyFrameData
{
    int frame;
    BoxData boxData;
}

public class ActionEditActionBehaviour : PlayableBehaviour
{
    public bool record = false;
    public GameObject target;
    public BoxManager boxManager;
    List<KeyFrameData> keyFrameDatas = new();

    public PlayableDirector director;

    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
        TimelineAsset t = director.playableAsset as TimelineAsset;
        int a = 0;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }

    public override void OnGraphStart(Playable playable)
    {
        base.OnGraphStart(playable);
    }

    public override void OnGraphStop(Playable playable)
    {
        base.OnGraphStop(playable);
    }
}
