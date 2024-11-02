using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;

[TrackBindingType(typeof(GameObject))]
[TrackClipType(typeof(ActionEditActionClip))]
public class ActionEditActionTrack : TrackAsset
{
    public ExposedReference<GameObject> target;
    protected override void OnCreateClip(TimelineClip clip)
    {
        ActionEditActionClip c = clip.asset as ActionEditActionClip;
        if (c != null)
        {
            c.OnCreateClip(target);
        }
    }
}

