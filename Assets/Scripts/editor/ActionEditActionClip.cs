using System;
using System.Collections.Generic;
using Log;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ActionEditActionClip : PlayableAsset
{
    public ExposedReference<GameObject> target;

    private GameObject mTargetGameObject;
    
    [SerializeField]
    public bool record = false;
    [SerializeField]
    private ActionEditActionBehaviour template = new ActionEditActionBehaviour();

    private ActionEditActionBehaviour m_Playable;

#region KeyFrameDatas
    private readonly double frameTime = 0.03333333;
    private BoxManager mBoxManager;
    private List<KeyFrameData> mKeyFrameDatas        = new();
    private Dictionary<int, KeyFrameData> keyFrames = new();

    private List<BoxData> defaultBoxDataList;

    private bool isFirstFrame = true;
    private int curFrame      = 0;
    private int lastFrame     = -1;
#endregion

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable    = ScriptPlayable<ActionEditActionBehaviour>.Create(graph, template);
        if (m_Playable == null)
        {
            var behaviour   = playable.GetBehaviour();
            behaviour.asset = this;
            GameObject go   = target.Resolve(graph.GetResolver());
            if (go != null)
            {
                if (mTargetGameObject != go)
                {
                    mTargetGameObject = go;
                }
                var trasnform = go.transform.Find("box_root");
                if (trasnform != null)
                {
                    var childGo          = trasnform.gameObject;
                    BoxManager mgr       = childGo.GetComponent<BoxManager>();
                    behaviour.boxManager = mgr;
                }
            }
            behaviour.target   = go;
            behaviour.director = owner.GetComponent<PlayableDirector>();
            behaviour.InitDefaultData();
            m_Playable = behaviour;
        }
        return playable;
    }

    public void ClearFrameDataList()
    {
        if (m_Playable != null) m_Playable.ClearFrameDataList();
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

    public void OnCreateClip(ExposedReference<GameObject> data)
    
    {
        target = data;
    }
}

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

[CustomEditor(typeof(ActionEditActionClip))]
public class ActionEditActionBehaviourInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ActionEditActionClip obj = target as ActionEditActionClip;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("clear record frame data list"))
        {
            obj.ClearFrameDataList();
        }
        EditorGUILayout.EndHorizontal();
    }
}
