
using UnityEngine;
using UnityEngine.Playables;

public class ActionEditorAttackBoxClip : PlayableAsset
{
    public ActionEditorAttackBoxBehaviour template = new ActionEditorAttackBoxBehaviour();

    [Header("在这个帧区间有效")]
    public FrameIndexRange ActivtFrame;

    public ExposedReference<GameObject> RefBox;

    public GameObject Box;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditorAttackBoxBehaviour>.Create(graph, template);
        Box          = RefBox.Resolve(graph.GetResolver());
        return playable;
    }
}
