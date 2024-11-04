using System;
using System.Collections.Generic;
using Log;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ActionEditActionClip : PlayableAsset
{
    [SerializeField]
    public CharacterAction action;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditActionBehaviour>.Create(graph);
        return playable;
    }
}
