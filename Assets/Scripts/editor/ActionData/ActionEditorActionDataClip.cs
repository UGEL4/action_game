using System;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ActionEditorActionDataClip : PlayableAsset
{
    public CharacterAction action;
    public ActionEditorActionDataBehaviour template = new ActionEditorActionDataBehaviour();
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditorActionDataBehaviour>.Create(graph, template);
        return playable;
    }
}
