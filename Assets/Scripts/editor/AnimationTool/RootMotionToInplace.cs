using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RootMotionToInplace : EditorWindow
{
    private AnimationClip _animationClip;

    [MenuItem("Tools/Animation/Root Motion To Inplace Window")]
    public static void ShowWindow()
    {
        GetWindow<RootMotionToInplace>("Root Motion To Inplace");
    }

    private void OnGUI()
    {
        //GUILayout.Label("Remove Root Node Position from Animation Clip", EditorStyles.boldLabel);
        _animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", _animationClip, typeof(AnimationClip), false);

        if (GUILayout.Button("Exchange"))
        {
            if (_animationClip != null)
            {
                RemovePositionFromRootNode();
            }
            else
            {
                Debug.LogWarning("Please select a valid AnimationClip.");
            }
        }
    }

    private void RemovePositionFromRootNode()
    {
        // 创建一个新的动画片段
        AnimationClip newAnimationClip = new AnimationClip();
        
        // 复制动画剪辑的设置
        AnimationUtility.SetAnimationClipSettings(newAnimationClip, AnimationUtility.GetAnimationClipSettings(_animationClip));

        // 获取动画曲线
        EditorCurveBinding[] bindings = AnimationUtility.GetCurveBindings(_animationClip);

        foreach (var binding in bindings)
        {
            // 只处理根节点（假设根节点为 Transform.root）
            if (binding.path == "root")
            {
                // 过滤掉位移信息
                if (binding.propertyName != "m_LocalPosition.x" &&
                    //binding.propertyName != "m_LocalPosition.y" &&
                    binding.propertyName != "m_LocalPosition.z")
                {
                    // 获取该曲线
                    AnimationCurve curve = AnimationUtility.GetEditorCurve(_animationClip, binding);
                    // 将曲线添加到新的动画片段中
                    AnimationUtility.SetEditorCurve(newAnimationClip, binding, curve);
                }
            }
            else
            {
                // 处理非根节点
                AnimationCurve curve = AnimationUtility.GetEditorCurve(_animationClip, binding);
                AnimationUtility.SetEditorCurve(newAnimationClip, binding, curve);
            }
        }

        // 保存新的动画剪辑
        string path = EditorUtility.SaveFilePanel("Save New Animation Clip", "Assets", _animationClip.name + "_Modified.anim", "anim");
        if (!string.IsNullOrEmpty(path))
        {
            path = FileUtil.GetProjectRelativePath(path);
            AssetDatabase.CreateAsset(newAnimationClip, path);
            AssetDatabase.SaveAssets();
            AssetDatabase.Refresh();
            Debug.Log("New animation clip created at: " + path);
        }
    }
}
