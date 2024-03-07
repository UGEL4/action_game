using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ActionED : MonoBehaviour
{
    [SerializeField]
    CharacterAction action;
}

public class ActionEditor : EditorWindow
{
    ActionED action;
    [MenuItem("Window/Action Editor")]
    public static void ShowWindow()
    {
        //EditorWindow.GetWindowWithRect(typeof(ActionEditor), new Rect(0, 0, 100, 100), true, "Action Editor Title");
        EditorWindow.GetWindow(typeof(ActionEditor));
    }
    void OnGUI()
    {
        action = new ActionED();
        Type type = action.GetType();
        System.Reflection.PropertyInfo[] propertyInfos = type.GetProperties();
        foreach (System.Reflection.PropertyInfo p in propertyInfos)
        {
            EditorGUILayout.BeginVertical();
            EditorGUILayout.LabelField(p.Name);
            //EditorGUILayout.LabelField(p.GetValue(action).ToString());
            EditorGUILayout.EndVertical();
        }
    }
}


[CustomEditor(typeof(ActionEdit))]
public class ActionEditInspector : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        ActionEdit obj = target as ActionEdit;
        EditorGUILayout.BeginHorizontal();
        if (GUILayout.Button("save"))
        {
            obj.Save();
        }
        EditorGUILayout.EndHorizontal();
    }
}