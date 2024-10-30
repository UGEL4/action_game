using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(GameObject))]
[TrackClipType(typeof(ActionEditActionClip))]
public class ActionEditActionTrack : TrackAsset
{
    public override Playable CreateTrackMixer(PlayableGraph graph, GameObject owner, int inputCount)
    {
        return ScriptPlayable<ActionEditorMixer>.Create(graph, inputCount);
    }
}
