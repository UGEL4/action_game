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

    [Serializable]
    public class Warp
    {
        public ExposedReference<GameObject> data;
        public int a;
    }
    [Serializable]
    public class AttackRayTurnOnInfo
    {
        [Header("射线检测发射点")]
        public List<Warp> Points;
        [Header("射线检测有效帧")]
        public FrameIndexRange ActiveFrame;
    }

    [Header("射线组")]
    public List<AttackRayTurnOnInfo> AttackRayTurnOnInfoList;

    public Dictionary<int, List<SerializableTransformNoScale>> rayCastPointsTransformPerFrame = new();

    [HideInInspector]
    public Dictionary<int, Dictionary<int, List<SerializableTransformNoScale>>> AllRayCastPointsTransformPerFrame = new();

    public void RecordPointTransform(int group, int frameIndex, List<SerializableTransformNoScale> transforms)
    {
        if (group < 0 || group >= AttackRayTurnOnInfoList.Count)
        {
            return;
        }
        Dictionary<int, List<SerializableTransformNoScale>> perFrameInfo;
        if (!AllRayCastPointsTransformPerFrame.TryGetValue(group, out perFrameInfo))
        {
            AllRayCastPointsTransformPerFrame.Add(group, new Dictionary<int, List<SerializableTransformNoScale>>());
        }
        perFrameInfo = AllRayCastPointsTransformPerFrame[group];
        if (perFrameInfo.TryGetValue(frameIndex, out List<SerializableTransformNoScale> list))
        {
            if (list.Count != transforms.Count)
            {
                List<SerializableTransformNoScale> newList = new(transforms.Count);
                for (int i = 0; i < transforms.Count; i++)
                {
                    newList.Add(transforms[i]);
                }
                perFrameInfo[frameIndex] = newList;
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
            perFrameInfo.Add(frameIndex, newList);
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable            = ScriptPlayable<ActionEditorHitRayCastBehaviour>.Create(graph, template);
        // var behaviour           = playable.GetBehaviour();
        // var weaponObj           = weapon.Resolve(graph.GetResolver());
        // var characterRootObj    = characterRoot.Resolve(graph.GetResolver());
        // behaviour.characterRoot = characterRootObj;
        // behaviour.clipAsset     = this;
        // behaviour.weapon        = weaponObj;
        // behaviour.director      = owner.GetComponent<PlayableDirector>();
        // behaviour.AttackRayTurnOnInfoList = AttackRayTurnOnInfoList;
        // behaviour.OnPlayableCreateOverride();
        return playable;
    }
}
