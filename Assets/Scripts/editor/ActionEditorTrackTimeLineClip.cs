using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[System.Serializable]
public class ActionEditorTrackTimeLineClip : PlayableAsset
{
    public ActionEditorPlayableBehaviour template = new ActionEditorPlayableBehaviour();
    //public ExposedReference<GameObject> gameObjectReference;

    public AnimationClip animationClip;

    public AnimationPlayableAsset animationPlayableAsset;
    public AnimationTrack animationTrack;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditorPlayableBehaviour>.Create(graph, template);
        // 设置clip相关的参数
        ActionEditorPlayableBehaviour clone = playable.GetBehaviour();
        clone.animationClip                 = animationClip;
        return playable;
    }
}
