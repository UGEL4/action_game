using UnityEngine;
using UnityEngine.Playables;

public class ActionEditActionClip : PlayableAsset
{
    [SerializeField]
    private ActionEditActionBehaviour template = new ActionEditActionBehaviour();
    
    public CharacterAction action;
    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditActionBehaviour>.Create(graph, template);
        return playable;
    }
}
