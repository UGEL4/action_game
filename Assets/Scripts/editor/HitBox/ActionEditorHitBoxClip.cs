using System;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;

//每个clip就是一个防御阶段(BeHitBoxTurnOnInfo)
public class ActionEditorHitBoxClip : PlayableAsset
{
    [Header("骨骼根节点")]
    public ExposedReference<GameObject> RootBone;
    private GameObject mRootBoneObj;

    [Serializable]
    public struct ActiveHitBoxInfo
    {
        public bool active;
        public GameObject Bone;
    }

    [Header("碰撞盒信息")]
    public BeHitBoxTurnOnInfo turnOnInfo;
    [Header("激活的碰撞盒列表")]
    public List<ActiveHitBoxInfo> boxList = new();

    // [Serializable]
    // public struct PhaseActiveHitBoxInfo
    // {
    //     [Tooltip("碰撞盒信息")]
    //     public BeHitBoxTurnOnInfo turnOnInfo;

    //     [Tooltip("激活的碰撞盒列表")]
    //     public List<ActiveHitBoxInfo> boxList;
    // }

    // [Tooltip("防御阶段列表")]
    // public List<PhaseActiveHitBoxInfo> phaseBoxList = new();

    private List<GameObject> mCacheBoneList = new List<GameObject>();

    public ActionEditorHitBoxBehaviour template = new ActionEditorHitBoxBehaviour();

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable = ScriptPlayable<ActionEditorHitBoxBehaviour>.Create(graph, template);
        GameObject rootBoneObj = RootBone.Resolve(graph.GetResolver());
        if (rootBoneObj != null && mRootBoneObj != rootBoneObj)
        {
            mRootBoneObj = rootBoneObj;
            boxList.Clear();
            TraverseChildren(rootBoneObj.transform, boxList);
        }
        return playable;
    }

    void TraverseChildren(Transform parent, List<ActiveHitBoxInfo> outList)
    {
        // 遍历当前 GameObject 的所有子节点
        foreach (Transform child in parent)
        {
            if (child.GetComponent<BoxCollider>() != null)
            {
                ActiveHitBoxInfo info = new ActiveHitBoxInfo()
                {
                    active = true,
                    Bone   = child.gameObject
                };
                outList.Add(info);
            }
            // 递归调用，遍历子节点的子节点
            TraverseChildren(child, outList);
        }
    }
}

[CustomPropertyDrawer(typeof(ActionEditorHitBoxClip.ActiveHitBoxInfo))]
public class ActiveHitBoxInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        var depth = property.depth;
        EditorGUI.BeginProperty(position, label, property);
        //EditorGUI.indentLevel += depth + 1;
        EditorGUI.PropertyField(position, property, new GUIContent(property.FindPropertyRelative("Bone").objectReferenceValue.name));
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;

            EditorGUILayout.BeginVertical();
            // 绘制字段
            // 获取子属性
            SerializedProperty activeProperty = property.FindPropertyRelative("active");
            SerializedProperty boneProperty   = property.FindPropertyRelative("Bone");
            EditorGUILayout.PropertyField(activeProperty, new GUIContent("是否激活"));
            EditorGUILayout.PropertyField(boneProperty, new GUIContent("碰撞盒骨骼"));
            EditorGUILayout.EndVertical();

            EditorGUI.indentLevel--;
        }
        //EditorGUI.indentLevel -= depth + 1;
        EditorGUI.EndProperty();
    }
}

public class ActionEditorHitBoxClipCustomDrawProperty : PropertyAttribute
{

}

[CustomPropertyDrawer(typeof(ActionEditorHitBoxClipCustomDrawProperty))]
public class ActionEditorHitBoxClipCustomDrawPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUI.PropertyField (position, property, label);
        if (property.isExpanded)
        {
            EditorGUI.indentLevel++;
            EditorGUILayout.BeginVertical();
            // 绘制字段
            SerializedProperty data = property.FindPropertyRelative("FrameIndexRange");
            EditorGUILayout.PropertyField(data, new GUIContent("FrameIndexRange"), true);
            EditorGUILayout.EndVertical();
            EditorGUI.indentLevel--;
        }
    }
}
