using System;
using System.Collections.Generic;
using System.Text;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using Action;
using System.IO;

[Serializable]
public class ActionEditHitBoxClip : PlayableAsset
{
    public ExposedReference<GameObject> target;

    private GameObject mTargetGameObject;

    public SimpleColliderBox box;
    public FrameIndexRange activeFrameRange;

    [Tooltip("勾选后，播放到下一帧时，会记录上一帧的数据")]
    [SerializeField]
    public bool Record = false;
    [SerializeField]
    public ActionEditHitBoxBehaviour template = new ActionEditHitBoxBehaviour();

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

    /// <summary>
    /// 一个攻击框数据
    /// </summary>
    [Serializable]
    public class HitBoxData
    {
        public GameObject visualGameobject;
        public string tag;
        public FrameIndexRange activeFrameRange;
    }
    public List<HitBoxData> hitBoxDataList;

    public Vector3 position;
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
        // var boxList         = BoxMgr.boxObjects;
        // mDefaultBoxDataList = new List<BoxData>(boxList.Count);
        // for (int i = 0; i < boxList.Count; ++i)
        // {
        //     var boxData         = new BoxData();
        //     boxData.position    = boxList[i].transform.localPosition;
        //     boxData.rotation    = boxList[i].transform.localRotation;
        //     boxData.scale       = boxList[i].transform.localScale;
        //     CustomBounds bounds = boxList[i].GetComponent<CustomBounds>();
        //     if (bounds != null)
        //     {
        //         boxData.bounds = bounds.bounds;
        //     }
        //     DefaultBoxDataList.Add(boxData);
        // }
    }

    public void OnCreateClip(ExposedReference<GameObject> data)
    {
        target = data;
    }

    public void Save(bool is_game_res)
    {
        StringBuilder json = new StringBuilder("{\"data\":[");
        int count          = 0;
        if (is_game_res)
        {
            var boxList       = mBoxManager.boxObjects;
            var rootTransform = mTargetGameObject.transform;
            foreach (var kv in mKeyFrames)
            {
                var value          = kv.Value;
                var boxDataList    = value.boxDataList;
                var newBoxDataList = new List<BoxData>(boxDataList.Count);
                for (int i = 0; i < boxDataList.Count; ++i)
                {
                    if (i < boxList.Count)
                    {
                        var go          = boxList[i];
                        var goTransform = go.transform;
                        if (rootTransform != null)
                        {
                            // var pos        = rootTransform.InverseTransformPoint(goTransform.position);
                            // var dir        = rootTransform.InverseTransformDirection(goTransform.forward);
                            // Quaternion rot = Quaternion.LookRotation(dir, rootTransform.up);

                            BoxData newBoxData = new BoxData {
                                position = boxDataList[i].worldPosition,
                                rotation = boxDataList[i].worldRotation,
                                scale    = goTransform.localScale,
                                bounds   = boxDataList[i].bounds
                            };
                            newBoxDataList.Add(newBoxData);
                        }
                        else
                        {
                            newBoxDataList.Add(boxDataList[i]);
                        }
                    }
                }
                KeyFrameData frameData = new KeyFrameData {
                    frame       = value.frame,
                    boxDataList = newBoxDataList
                };

                json.Append(JsonUtility.ToJson(frameData));
                if (count < mKeyFrames.Count - 1)
                {
                    json.Append(",");
                }
                count++;
            }
        }
        else
        {
            foreach (var kv in mKeyFrames)
            {
                var key   = kv.Key;
                var value = kv.Value;
                json.Append(JsonUtility.ToJson(value));
                if (count < mKeyFrames.Count - 1)
                {
                    json.Append(",");
                }
                count++;
            }
        }
        json.Append("]}");
        if (!Directory.Exists(Application.dataPath + "/Resources/GameData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/GameData");
        }
        string path;
        if (is_game_res)
        {
            path = Application.dataPath + "/Resources/GameData/test_box_game.json";
        }
        else
        {
            path = Application.dataPath + "/Resources/GameData/test_box.json";
        }
        File.WriteAllText(path, json.ToString());
    }

    public void Load()
    {
        if (mBoxManager == null)
        {
            var trasnform     = mTargetGameObject.transform.Find("box_root");
            if (trasnform != null)
            {
                var childGo = trasnform.gameObject;
                mBoxManager = childGo.GetComponent<BoxManager>();
            }
            if (mBoxManager == null) return;
        }
        mKeyFrames.Clear();
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
