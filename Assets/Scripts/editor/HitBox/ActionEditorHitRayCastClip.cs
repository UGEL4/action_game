using System;
using System.Collections.Generic;
using ACTTools;
using Log;
using PlasticPipe.PlasticProtocol.Client.Proxies;
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
        [SerializeField]
        public ExposedReference<GameObject> data;
    }
    [Serializable]
    public class AttackRayTurnOnInfo
    {
        [Header("射线组tag")]
        public string Tag;
        [Header("射线检测发射点")]
        public List<string> PointNameList;
        [Header("射线检测有效帧")]
        public FrameIndexRange ActiveFrame;
    }

    [Header("射线组")]
    public List<AttackRayTurnOnInfo> AttackRayTurnOnInfoList;

    [Header("勾选后，会记录射线的位置")]
    public bool Record = false;

    //[HideInInspector]
    //public Dictionary<int, Dictionary<int, List<SerializableTransformNoScale>>> AllRayCastPointsTransformPerFrame = new();

    [Serializable]
    public class RayPointInfo
    {
        public string Name;
        public SerializableTransformNoScale[] transforms;
    }
    public Dictionary<int, Dictionary<string, RayPointInfo>> AllRayCastPointsTransformPerFrame = new();

    public struct RayPointInfoPreFrame
    {
        public string Name;
        public SerializableTransformNoScale transform;
    }

    public void RecordPointTransform(int group, int frameIndex, List<RayPointInfoPreFrame> perFrameInfoList)
    {
        //SimpleLog.Info("frameIndex: ", frameIndex);
        //每组射线有多个点，统一组内每个点的有效帧都一样长，也就是transforms数组长度都相等
        if (group < 0 || group >= AttackRayTurnOnInfoList.Count)
        {
            return;
        }

        var turnOnInfo = AttackRayTurnOnInfoList[group];
        int listLength = (int)(turnOnInfo.ActiveFrame.max - turnOnInfo.ActiveFrame.min) + 1;
        
        if (!AllRayCastPointsTransformPerFrame.TryGetValue(group, out Dictionary<string, RayPointInfo> rayPointInfoPair))
        {
            AllRayCastPointsTransformPerFrame.Add(group, new Dictionary<string, RayPointInfo>());
        }
        rayPointInfoPair = AllRayCastPointsTransformPerFrame[group];
        for (int i = 0; i < perFrameInfoList.Count; i++)
        {
            string name = perFrameInfoList[i].Name;
            if (!rayPointInfoPair.TryGetValue(name, out RayPointInfo dummy))
            {
                rayPointInfoPair.Add(name, new RayPointInfo());
            }
            RayPointInfo info = rayPointInfoPair[name];
            info.Name         = name;
            if (info.transforms == null)
            {
                info.transforms = new SerializableTransformNoScale[listLength];
                info.transforms[frameIndex] = perFrameInfoList[i].transform;
            }
            else
            {
                if (info.transforms.Length != listLength)
                {
                    SerializableTransformNoScale[] newList = new SerializableTransformNoScale[listLength];
                    for (int j = 0; j < info.transforms.Length && j < listLength; j++)
                    {
                        newList[j] = info.transforms[j];
                    }
                    info.transforms = newList;
                }
                info.transforms[frameIndex] = perFrameInfoList[i].transform;
            }
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable                      = ScriptPlayable<ActionEditorHitRayCastBehaviour>.Create(graph, template);
        var behaviour                     = playable.GetBehaviour();
        var weaponObj                     = weapon.Resolve(graph.GetResolver());
        var characterRootObj              = characterRoot.Resolve(graph.GetResolver());
        behaviour.characterRoot           = characterRootObj;
        behaviour.clipAsset               = this;
        behaviour.weapon                  = weaponObj;
        behaviour.director                = owner.GetComponent<PlayableDirector>();
        behaviour.AttackRayTurnOnInfoList = AttackRayTurnOnInfoList;
        behaviour.Record                  = Record;
        behaviour.OnPlayableCreateOverride();
        return playable;
    }
}
