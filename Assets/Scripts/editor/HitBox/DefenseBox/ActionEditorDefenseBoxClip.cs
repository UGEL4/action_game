
using ACTTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

public class ActionEditorDefenseBoxClip : PlayableAsset
{
    public ActionEditorDefenseBoxBehaviour template = new ActionEditorDefenseBoxBehaviour();

    [Header("在这个帧区间有效")]
    public FrameIndexRange ActivtFrame;

    public ExposedReference<GameObject> RefBox;

    private GameObject Box;

    public BoxColliderData mBoxData;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditorDefenseBoxBehaviour>.Create(graph, template);
        Box          = RefBox.Resolve(graph.GetResolver());
        return playable;
    }

    public BoxColliderData GetBoxData()
    {
        return mBoxData;
    }

    public void Save()
    {
        if (Box != null)
        {
            if (mBoxData == null)
            {
                mBoxData = new BoxColliderData();
            }
            mBoxData.position = Box.transform.localPosition;
            mBoxData.rotation = Box.transform.localRotation.eulerAngles;
            CastBox b = Box.GetComponent<CastBox>();
            if (b)
            {
                mBoxData.center = b.center;
                mBoxData.size   = b.size;
            }
        }
    }
}

[CustomEditor(typeof(ActionEditorDefenseBoxClip))]
public class ActionEditorDefenseBoxClipInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ActionEditorDefenseBoxClip obj = target as ActionEditorDefenseBoxClip;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            obj.Save();
        }
        EditorGUILayout.EndHorizontal();
    }
}
