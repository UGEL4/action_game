using UnityEngine;
using UnityEngine.Timeline;

//[TrackBindingType(typeof(GameObject))]
[TrackClipType(typeof(ActionEditActionClip))]
public class ActionEditActionTrack : TrackAsset
{
    public ExposedReference<GameObject> RootBone;
}

