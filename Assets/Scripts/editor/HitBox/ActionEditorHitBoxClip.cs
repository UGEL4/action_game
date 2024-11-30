using System;
using System.Collections.Generic;
using ACTTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

[Serializable]
public class ActionEditorHitBoxClip : PlayableAsset
{
    [Serializable]
    public struct ActiveHitBoxInfo
    {
        public bool active;
        public GameObject Bone;
    }
    public List<ActiveHitBoxInfo> tempList;

    public ActionEditorHitBoxBehaviour template = new ActionEditorHitBoxBehaviour();

    public ExposedReference<GameObject> weapon;
    public ExposedReference<GameObject> characterRoot;

    public FrameIndexRange activeFrameRange;

    public Dictionary<int, List<SerializableTransformNoScale>> rayCastPointsTransformPerFrame = new();

    public void RecordPointTransform(int frameIndex, List<SerializableTransformNoScale> transforms)
    {
        if (rayCastPointsTransformPerFrame.TryGetValue(frameIndex, out List<SerializableTransformNoScale> list))
        {
            if (list.Count != transforms.Count)
            {
                List<SerializableTransformNoScale> newList = new(transforms.Count);
                for (int i = 0; i < transforms.Count; i++)
                {
                    newList.Add(transforms[i]);
                }
                list = newList;
            }
            else
            {
                for (int i = 0; i < transforms.Count; i++)
                {
                    list[i] = transforms[i];
                }
            }
        }
        else
        {
            List<SerializableTransformNoScale> newList = new(transforms.Count);
            for (int i = 0; i < transforms.Count; i++)
            {
                newList.Add( transforms[i]);
            }
            rayCastPointsTransformPerFrame.Add(frameIndex, newList);
        }
    }

    public override Playable CreatePlayable(PlayableGraph graph, GameObject owner)
    {
        var playable               = ScriptPlayable<ActionEditorHitBoxBehaviour>.Create(graph, template);
        var behaviour              = playable.GetBehaviour();
        var weaponObj              = weapon.Resolve(graph.GetResolver());
        behaviour.weapon           = weaponObj;
        var characterRootObj       = characterRoot.Resolve(graph.GetResolver());
        behaviour.characterRoot    = characterRootObj;
        behaviour.activeFrameRange = activeFrameRange;
        behaviour.clipAsset        = this;
        behaviour.director         = owner.GetComponent<PlayableDirector>();
        behaviour.OnPlayableCreateOverride();
        return playable;
    }
}

[CustomPropertyDrawer(typeof(ActionEditorHitBoxClip.ActiveHitBoxInfo))]
public class ActiveHitBoxInfoDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        EditorGUILayout.LabelField(label, EditorStyles.boldLabel);
        EditorGUILayout.BeginVertical();
        EditorGUILayout.PropertyField(property.FindPropertyRelative("active"), new GUIContent("是否激活"));
        EditorGUILayout.PropertyField(property.FindPropertyRelative("Bone"), new GUIContent("碰撞盒骨骼"));
        EditorGUILayout.EndVertical();
    }
}

[AttributeUsage(AttributeTargets.Field, AllowMultiple = false, Inherited = true)]
public class SelectActiveHidBoxPropertyAttribute : PropertyAttribute
{
    public string CustomInfo { get; set; }
    public SelectActiveHidBoxPropertyAttribute(string customInfo)
    {
        CustomInfo = customInfo;
    }
}

[CustomPropertyDrawer(typeof(SelectActiveHidBoxPropertyAttribute))]
public class SelectActiveHidBoxPropertyDrawer : PropertyDrawer
{
    public override void OnGUI(Rect position, SerializedProperty property, GUIContent label)
    {
        // 获取自定义属性的实例
        SelectActiveHidBoxPropertyAttribute customAttribute = (SelectActiveHidBoxPropertyAttribute)attribute;

        // 显示自定义信息
        //EditorGUILayout.Toggle(customAttribute.Active);
        //EditorGUILayout.ObjectField(customAttribute.go, typeof(GameObject), true);
        EditorGUI.LabelField(new Rect(position.x, position.y, position.width, EditorGUIUtility.singleLineHeight), customAttribute.CustomInfo);

        // 显示属性字段
        EditorGUI.PropertyField(new Rect(position.x, position.y + EditorGUIUtility.singleLineHeight, position.width, EditorGUIUtility.singleLineHeight), property);
    }

    public override float GetPropertyHeight(SerializedProperty property, GUIContent label)
    {
        // 自定义属性需要额外的空间来显示信息
        return EditorGUIUtility.singleLineHeight * 2;
    }
}
