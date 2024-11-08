using System;
using System.Collections.Generic;
using System.Text;
using Log;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using Action;
using System.IO;
using Unity.VisualScripting;

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

    public void Save(bool is_game_res)
    {
        if (is_game_res)
        {
            var boxList = mBoxManager.boxObjects;
            for (int i = 0; i < boxList.Count; ++i)
            {
                var go = boxList[i];
                Matrix4x4 localToWorld = go.transform.localToWorldMatrix;
                if (go.transform.parent != null && go.transform.parent != mTargetGameObject)
                {
                    var partentGo = go.transform.parent.gameObject;
                    var parentTransform = go.transform;
                    while (true)
                    {
                        //localToWorld = parent.localToWorldMatrix * localToWorld;
                    }
                }
                
                if (go.transform.parent != null)
                {
                    //var parent = go.transform.parent;
                    //while (parent.parent != null)
                    {

                    }
                }
                var parent = go.transform.parent;
                while (parent != null)
                {
                    localToWorld = parent.localToWorldMatrix * localToWorld;
                    parent       = parent.parent;
                }
            }
            return;
        }
        StringBuilder json = new StringBuilder("{\"data\":[");
        int count = 0;
        foreach (var kv in mKeyFrames)
        {
            var key   = kv.Key;
            var value = kv.Value;
            //json.Append(JsonUtility.ToJson(key));
            json.Append(JsonUtility.ToJson(value));
            if (count < mKeyFrames.Count)
            {
                json.Append(",");
            }
            count++;
        }
        // for (int i = 0; i < Actions.Count; i++)
        // {
        //     json.Append(JsonUtility.ToJson(Actions[i]));
        //     if (i != Actions.Count - 1) json.Append(",");
        // }
        json.Append("]}");
        if (!Directory.Exists(Application.dataPath + "/Resources/GameData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/GameData");
        }
        string path = Application.dataPath + "/Resources/GameData/test_box.json";
        File.WriteAllText(path, json.ToString());
    }

    public void Load()
    {
        if (mBoxManager == null) return;
        TextAsset ta = Resources.Load<TextAsset>("GameData/test_box");
        if (ta)
        {
            KeyFrameDataContainer temp = JsonUtility.FromJson<KeyFrameDataContainer>(ta.text);
            foreach (var v in temp.data)
            {
                mKeyFrames.Add(v.frame, v);
            }
        }
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

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("save"))
        {
            obj.Save(false);
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("load"))
        {
            obj.Load();
        }
        EditorGUILayout.EndHorizontal();

        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("save_game"))
        {
            obj.Save(true);
        }
        EditorGUILayout.EndHorizontal();
    }
}

