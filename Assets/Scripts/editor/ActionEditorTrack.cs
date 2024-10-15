using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(GameObject))]
[TrackClipType(typeof(ActionEditorTrackTimeLineClip))]
public class ActionEditorTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject owner, int inputCount)
    {
        return ScriptPlayable<ActionEditorMixer>.Create(graph, inputCount);
    }
}

[System.Serializable]
public class ActionEditorTrackTimeLineClip : PlayableAsset, ITimelineClipAsset
{
    private ActionEditorPlayableBehaviour template = new ActionEditorPlayableBehaviour();
    //public ExposedReference<GameObject> gameObjectReference;

    public AnimationClip animationClip;
    public ClipCaps clipCaps
    {
        get { return ClipCaps.None; }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditorPlayableBehaviour>.Create(graph, template);
        // 设置clip相关的参数
        ActionEditorPlayableBehaviour clone = playable.GetBehaviour();
        clone.animationClip = animationClip;
        return playable;
    }
}

// Clip的行为类
public class ActionEditorPlayableBehaviour : PlayableBehaviour
{
    // 在这里添加你的clip逻辑
    public AnimationClip animationClip;
}

public class ActionEditorMixer : PlayableBehaviour
{

}
