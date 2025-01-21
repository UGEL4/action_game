#if UNITY_EDITOR
using System.Collections;
using UnityEditor;
using UnityEditor.EditorTools;
using UnityEditor.IMGUI.Controls;
using UnityEngine;

[EditorTool("Edit Cast Shape", typeof(CastBox))]
public class CastBoxTool : CastShapeTool<CastBox>
{

    private readonly BoxBoundsHandle m_boundsHandle = new BoxBoundsHandle();

    protected override PrimitiveBoundsHandle boundsHandle
    {
        get {
            return m_boundsHandle;
        }
    }

    protected override void CopyColliderPropertiesToHandle(CastBox castShape)
    {
        m_boundsHandle.center = TransformColliderCenterToHandleSpace(castShape.transform, castShape.center);
        m_boundsHandle.size   = Vector3.Scale(castShape.size, castShape.transform.lossyScale);
    }

    protected override void CopyHandlePropertiesToCollider(CastBox castShape)
    {
        castShape.center = TransformHandleCenterToColliderSpace(castShape.transform, m_boundsHandle.center);
        Vector3 size     = Vector3.Scale(m_boundsHandle.size, InvertScaleVector(castShape.transform.lossyScale));
        size             = new Vector3(Mathf.Abs(size.x), Mathf.Abs(size.y), Mathf.Abs(size.z));
        castShape.size   = size;
    }
}

[CustomEditor(typeof(CastBox))]
[CanEditMultipleObjects]
public class CastBoxEditor : Editor
{

    private SerializedProperty m_script;
    private SerializedProperty m_center;
    private SerializedProperty m_size;
    private SerializedProperty m_always_draw;

    private void OnEnable()
    {
        m_script      = serializedObject.FindProperty("m_Script");
        m_center      = serializedObject.FindProperty("center");
        m_size        = serializedObject.FindProperty("size");
        m_always_draw = serializedObject.FindProperty("AlwaysDraw");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();

        GUI.enabled = false;
        EditorGUILayout.PropertyField(m_script);
        GUI.enabled = true;
        EditorGUILayout.EditorToolbarForTarget(EditorGUIUtility.TrTempContent("Edit Shape"), target);
        EditorGUILayout.PropertyField(m_center);
        EditorGUILayout.PropertyField(m_size);
        EditorGUILayout.PropertyField(m_always_draw);

        serializedObject.ApplyModifiedProperties();
    }
}
#endif