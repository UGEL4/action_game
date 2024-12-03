using System;
using System.Collections.Generic;
using ACTTools;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

[Serializable]
public class ActionEditorHitRayCastClip : PlayableAsset
{
    public ActionEditorHitRayCastBehaviour template = new ActionEditorHitRayCastBehaviour();

    public ExposedReference<GameObject> weapon;
    public ExposedReference<GameObject> characterRoot;
    public AttackBoxTurnOnInfo attackBoxTurnOnInfo;

    public Dictionary<int, List<SerializableTransformNoScale>> rayCastPointsTransformPerFrame = new();

    public void RecordPointTransform(int frameIndex, List<SerializableTransformNoScale> transforms)
    {
        if (rayCastPointsTransformPerFrame.TryGetValue(frameIndex, out List<SerializableTransformNoScale> list))
        {
            if (list.Count != transforms.Count)
            {
                List<SerializableTransformNoScale> newList = new(transforms.Count);
                for (int i = 0; i < transforms.Count; i++)
                {
                    newList.Add(transforms[i]);
                }
                list = newList;
            }
            else
            {
                for (int i = 0; i < transforms.Count; i++)
                {
                    list[i] = transforms[i];
                }
            }
        }
        else
        {
            List<SerializableTransformNoScale> newList = new(transforms.Count);
            for (int i = 0; i < transforms.Count; i++)
            {
                newList.Add( transforms[i]);
            }
            rayCastPointsTransformPerFrame.Add(frameIndex, newList);
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable               = ScriptPlayable<ActionEditorHitRayCastBehaviour>.Create(graph, template);
        var behaviour              = playable.GetBehaviour();
        var weaponObj              = weapon.Resolve(graph.GetResolver());
        behaviour.weapon           = weaponObj;
        var characterRootObj       = characterRoot.Resolve(graph.GetResolver());
        behaviour.characterRoot    = characterRootObj;
        behaviour.activeFrameRange = attackBoxTurnOnInfo.FrameIndexRange;
        behaviour.clipAsset        = this;
        behaviour.director         = owner.GetComponent<PlayableDirector>();
        behaviour.OnPlayableCreateOverride();
        return playable;
    }
}
