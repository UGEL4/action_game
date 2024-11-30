using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.AttributeUsage(System.AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class MyCustomPropertyAttribute : PropertyAttribute
{
    //public bool Active;
    //public GameObject go;

    public string CustomInfo { get; set; }
    public MyCustomPropertyAttribute(string customInfo)
    {
        CustomInfo = customInfo;
    }
}

[CustomPropertyDrawer(typeof(MyCustomPropertyAttribute))]
public class MyCustomPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 获取自定义属性的实例
        MyCustomPropertyAttribute customAttribute = (MyCustomPropertyAttribute)attribute;

        // 显示自定义信息
        //EditorGUILayout.Toggle(customAttribute.Active);
        //EditorGUILayout.ObjectField(customAttribute.go, typeof(GameObject), true);
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), "aaa");

        // 显示属性字段
        EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 自定义属性需要额外的空间来显示信息
        return EditorGUIUtility.singleLineHeight * 2;
    }
}
