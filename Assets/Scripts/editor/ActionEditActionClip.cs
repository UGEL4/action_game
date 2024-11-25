using System;
using System.Collections.Generic;
using Action;
using Log;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ActionEditActionClip : PlayableAsset
{
    [SerializeField]
    public CharacterAction action;

    [Serializable]
    public struct HitBoxStruct
    {
        public string name;
        public GameObject obj;
        public FrameIndexRange activeFrameRange;
    }

    public ExposedReference<GameObject> RootBoneRef;
    private GameObject RootBone;

    [SerializeField]
    public List<HitBoxStruct> hitboxes = new List<HitBoxStruct>();

    public void InitHidBox()
    {
        if (RootBone != null)
        {
            Transform rootBone       = RootBone.transform;
            Transform[] allChildBone = rootBone.GetComponentsInChildren<Transform>(true);
            for (int i = 0; i < allChildBone.Length; ++i)
            {
                if (allChildBone[i].GetComponent<BoxCollider>() != null)
                {
                    HitBoxStruct hitBox = new HitBoxStruct() {
                        name = allChildBone[i].name,
                        obj  = allChildBone[i].gameObject
                    };
                    hitboxes.Add(hitBox);
                }
            }
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable  = ScriptPlayable<ActionEditActionBehaviour>.Create(graph);
        GameObject go = RootBoneRef.Resolve(graph.GetResolver());
        if (go != null && (RootBone == null || RootBone != go))
        {
            RootBone = go;
            InitHidBox();
        }
        GameObject targetGo = target.Resolve(graph.GetResolver());
        if (targetGo != null)
        {
            if (mTargetGameObject != targetGo)
            {
                InitTargetGameObject(targetGo);
                InitDefaultData();
            }
        }
        var behaviour   = playable.GetBehaviour();
        behaviour.Asset = this;
        behaviour.target   = targetGo;
        behaviour.director = owner.GetComponent<PlayableDirector>();
        return playable;
    }

    public ExposedReference<GameObject> target;
    private GameObject mTargetGameObject;
    [Tooltip("勾选后，播放到下一帧时，会记录上一帧的数据")]
    [SerializeField]
    public bool Record = false;
    #region KeyFrameDatas
    private BoxManager mBoxManager;
    public BoxManager BoxMgr
    {
        get {
            return mBoxManager;
        }
    }
    private Dictionary<int, KeyFrameData> mKeyFrames = new();
    public Dictionary<int, KeyFrameData> KeyFrames
    {
        get {
            return mKeyFrames;
        }
    }

    private List<BoxData> mDefaultBoxDataList;
    public List<BoxData> DefaultBoxDataList
    {
        get {
            return mDefaultBoxDataList;
        }
    }
#endregion

    private void InitTargetGameObject(GameObject go)
    {
        mTargetGameObject = go;
        var trasnform     = go.transform.Find("box_root");
        if (trasnform != null)
        {
            var childGo = trasnform.gameObject;
            mBoxManager = childGo.GetComponent<BoxManager>();
        }
    }
    private void InitDefaultData()
    {
        var boxList         = BoxMgr.boxObjects;
        mDefaultBoxDataList = new List<BoxData>(boxList.Count);
        for (int i = 0; i < boxList.Count; ++i)
        {
            var boxData         = new BoxData();
            boxData.position    = boxList[i].transform.localPosition;
            boxData.rotation    = boxList[i].transform.localRotation;
            boxData.scale       = boxList[i].transform.localScale;
            CustomBounds bounds = boxList[i].GetComponent<CustomBounds>();
            if (bounds != null)
            {
                boxData.bounds = bounds.bounds;
            }
            DefaultBoxDataList.Add(boxData);
        }
    }
}
