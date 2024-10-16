using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(GameObject))]
[TrackClipType(typeof(ActionEditorTrackTimeLineClip))]
public class ActionEditorTrack : TrackAsset
{
    // public override Playable CreateTrackMixer(PlayableGraph graph, GameObject owner, int inputCount)
    // {
    //     return ScriptPlayable<ActionEditorMixer>.Create(graph, inputCount);
    // }
}
// Clip的行为类
public class ActionEditorPlayableBehaviour : PlayableBehaviour
{
    // 在这里添加你的clip逻辑
    public AnimationClip animationClip;
}

[System.Serializable]
public class ActionEditorMixer : PlayableBehaviour
{
    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        base.ProcessFrame(playable, info, playerData);
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }
}
