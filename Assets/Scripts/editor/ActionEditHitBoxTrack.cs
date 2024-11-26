using UnityEngine;
using UnityEngine.Timeline;

//[TrackBindingType(typeof(GameObject))]
[TrackClipType(typeof(ActionEditHitBoxClip))]
public class ActionEditHitBoxTrack : TrackAsset
{
    // public ExposedReference<GameObject> target;
    // protected override void OnCreateClip(TimelineClip clip)
    // {
    //     ActionEditHitBoxClip c = clip.asset as ActionEditHitBoxClip;
    //     if (c != null)
    //     {
    //         c.OnCreateClip(target);
    //     }
    // }
}


