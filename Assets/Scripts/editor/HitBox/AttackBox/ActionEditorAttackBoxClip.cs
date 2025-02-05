
using System;
using System.Collections.Generic;
using ACTTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public class ActionEditorAttackBoxClip : PlayableAsset
{
    public ActionEditorAttackBoxBehaviour template = new ActionEditorAttackBoxBehaviour();

    [Header("在这个帧区间有效")]
    public List<FrameIndexRange> ActivtFrame = new();

    public List<GameObject> BoxList;

    public List<BoxColliderData> BoxDataList = new();

    public List<ExposedReferenceGameObject> RefBoxList;

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditorAttackBoxBehaviour>.Create(graph, template);
        for (int i = 0; i < RefBoxList.Count; i++)
        {
            var box = RefBoxList[i].Reference.Resolve(graph.GetResolver());
            if (box)
            {
                if (i >= BoxList.Count)
                {
                    BoxList.Insert(i, box);
                }
                else
                {
                    BoxList[i] = box;
                }
            }
        }
        return playable;
    }

    public void Save()
    {
        for (int i = 0; i < BoxList.Count; i++)
        {
            if (BoxList[i])
            {
                if (i >= BoxDataList.Count)
                {
                    BoxColliderData data = new();
                    data.position = BoxList[i].transform.localPosition;
                    data.rotation = BoxList[i].transform.localRotation.eulerAngles;
                    CastBox collider = BoxList[i].GetComponent<CastBox>();
                    if (collider)
                    {
                        data.center = collider.center;
                        data.size   = collider.size;
                    }
                    BoxDataList.Insert(i, data);
                }
                else
                {
                    BoxDataList[i].position = BoxList[i].transform.localPosition;
                    BoxDataList[i].rotation = BoxList[i].transform.localRotation.eulerAngles;
                    CastBox collider = BoxList[i].GetComponent<CastBox>();
                    if (collider)
                    {
                        BoxDataList[i].center = collider.center;
                        BoxDataList[i].size   = collider.size;
                    }
                }
            }
        }
    }
}

[CustomEditor(typeof(ActionEditorAttackBoxClip))]
public class ActionEditorAttackBoxClipInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ActionEditorAttackBoxClip obj = target as ActionEditorAttackBoxClip;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("Save"))
        {
            obj.Save();
        }
        EditorGUILayout.EndHorizontal();
    }
}

