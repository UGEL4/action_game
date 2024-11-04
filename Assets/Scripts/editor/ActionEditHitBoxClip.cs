using System;
using System.Collections.Generic;
using Log;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;


[Serializable]
public struct BoxData
{
    //transform
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
    public OBB bounds;
    public string name;
}

[Serializable]
public struct KeyFrameData
{
    public int frame;
    public List<BoxData> boxDataList;
}

[Serializable]
public class ActionEditHitBoxClip : PlayableAsset
{
    public ExposedReference<GameObject> target;

    private GameObject mTargetGameObject;
    
    [SerializeField]
    public bool Record = false;
    [SerializeField]
    public ActionEditHitBoxBehaviour template = new ActionEditHitBoxBehaviour();

#region KeyFrameDatas
    private BoxManager mBoxManager;
    public BoxManager BoxMgr { get { return mBoxManager; } }
    
    private Dictionary<int, KeyFrameData> mKeyFrames = new();
    public Dictionary<int, KeyFrameData> KeyFrames { get { return mKeyFrames;}}

    private List<BoxData> mDefaultBoxDataList;
    public List<BoxData> DefaultBoxDataList { get { return mDefaultBoxDataList;}}
#endregion

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditHitBoxBehaviour>.Create(graph, template);
        {
            var behaviour   = playable.GetBehaviour();
            behaviour.Asset = this;
            GameObject go   = target.Resolve(graph.GetResolver());
            if (go != null)
            {
                if (mTargetGameObject != go)
                {
                    InitTargetGameObject(go);
                    InitDefaultData();
                }
            }
            behaviour.target   = go;
            behaviour.director = owner.GetComponent<PlayableDirector>();
        }
        return playable;
    }

    public void ClearFrameDataList()
    {
        KeyFrames.Clear();
    }

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
            var boxData      = new BoxData();
            boxData.position = boxList[i].transform.localPosition;
            boxData.rotation = boxList[i].transform.localRotation;
            boxData.scale    = boxList[i].transform.localScale;
            CustomBounds bounds = boxList[i].GetComponent<CustomBounds>();
            if (bounds != null)
            {
                boxData.bounds = bounds.bounds;
            }
            DefaultBoxDataList.Add(boxData);
        }
    }

    public void OnCreateClip(ExposedReference<GameObject> data)
    {
        target = data;
    }
}

[CustomEditor(typeof(ActionEditHitBoxClip))]
public class ActionEditHitBoxBehaviourInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ActionEditHitBoxClip obj = target as ActionEditHitBoxClip;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("clear record frame data list"))
        {
            obj.ClearFrameDataList();
        }
        EditorGUILayout.EndHorizontal();
    }
}

