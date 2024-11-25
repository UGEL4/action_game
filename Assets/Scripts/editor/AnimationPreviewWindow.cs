using UnityEngine;
using UnityEditor;
using System.Threading;
using System.Collections.Generic;
using Unity.Mathematics;
using Action;
using System.Text;
using System.IO;
using System;

public class AnimationPreviewWindow : EditorWindow
{
    private AnimationClip animationClip; // 要预览的动画剪辑
    private GameObject previewObject; // 预览对象
    private Animator animator; // Animator 组件

    private AnimatorOverrideController aoc;

    private bool isPlaying = false; // 动画播放状态
    private float playbackTime = 0f; // 播放时间
    private float sliderValue = 0f;

    public GameObject sampleObj;

    [MenuItem("Window/Animation Preview")]
    public static void ShowWindow()
    {
        GetWindow<AnimationPreviewWindow>("Animation Preview");
    }

    private void OnGUI()
    {
        GUILayout.Label("Animation Preview", EditorStyles.boldLabel);

        // 选择动画剪辑
        animationClip = (AnimationClip)EditorGUILayout.ObjectField("Animation Clip", animationClip, typeof(AnimationClip), false);
        
        // 选择预览对象
        previewObject = (GameObject)EditorGUILayout.ObjectField("Preview Object", previewObject, typeof(GameObject), true);

        if (previewObject != null)
        {
            // 确保预览对象有 Animator 组件
            if (animator == null)
            {
                animator = previewObject.GetComponent<Animator>();
            }

            // 确保动画状态名称正确
            if (animationClip != null)
            {
                // 播放按钮
                if (GUILayout.Button(isPlaying ? "Stop Animation" : "Play Animation"))
                {
                    isPlaying = !isPlaying;
                    if (isPlaying)
                    {
                        playbackTime = (float)EditorApplication.timeSinceStartup;
                        Debug.Log("Playing animation: " + animationClip.name);
                        //animator.Play(animationClip.name, 0, 0f); // 替换为您的动画状态名称
                        animationClip.SampleAnimation(previewObject, 1);

                        var a = animator.GetCurrentAnimatorClipInfo(0);

                        var binds = AnimationUtility.GetCurveBindings(a[0].clip);
                        var curves = AnimationUtility.GetEditorCurve(a[0].clip, binds[0]);
                        var keyFrames = curves.keys;
                        int b = 0;
                    }
                }
            }
            else
            {
                GUILayout.Label("Please assign an animation clip.");
            }
            sliderValue = GUILayout.HorizontalSlider(sliderValue, 0, animationClip.length);
            if (animationClip != null && !isPlaying)
            {
                //AnimationCurve
                animationClip.SampleAnimation(previewObject, sliderValue);
            }

            //sampleObj = (GameObject)EditorGUILayout.ObjectField("GameObject", sampleObj, typeof(GameObject), true);
            if (GUILayout.Button("Export BoxCollider"))
            {
                // Transform bone = previewObject.transform.Find("Bip001/Bip001 Pelvis").transform;
                // var pos = previewObject.transform.Find("Bip001/Bip001 Pelvis").transform.position;
                // animationClip.SampleAnimation(previewObject, 1);
                // pos = previewObject.transform.Find("Bip001/Bip001 Pelvis").transform.position;
                // Vector3 position = previewObject.transform.InverseTransformPoint(pos);
                // Quaternion rotation = Quaternion.Inverse(previewObject.transform.rotation) * bone.rotation;
                // if (sampleObj != null)
                // {
                //     sampleObj.transform.SetLocalPositionAndRotation(position, rotation);
                // }
                ExportBox();
            }
        }
        else
        {
            GUILayout.Label("Please assign a preview object.");
        }
    }

    private void OnDisable()
    {
        EditorApplication.update -= Update;
        // 清理资源
        if (previewObject != null && animator != null)
        {
            //DestroyImmediate(animator);
            animator = null;
            aoc = null;
        }
    }

    private void OnEnable()
    {
        isPlaying = false;
        playbackTime = (float)EditorApplication.timeSinceStartup;
        // 在编辑模式下更新
        EditorApplication.update += Update;
    }

    private void Update()
    {
        if (isPlaying && animator != null && animationClip != null)
        {
            // playbackTime += Time.deltaTime;
            // if (playbackTime > animationClip.length)
            // {
            //     playbackTime = 0f;
            // }
            // Debug.Log("playbackTime: " + (playbackTime / animationClip.length));
            // animator.Play(animationClip.name, 0, playbackTime / animationClip.length);

            // 更新 Animator 状态
            //float next_time = (float)EditorApplication.timeSinceStartup - playbackTime;
            //playbackTime = (float)EditorApplication.timeSinceStartup;
            //animator.Update(next_time);
        }
    }

    [Serializable]
    struct BoxColliderData
    {
        public Vector3 position;
        public Vector3 center;
        public Vector3 size;
        public Quaternion rotation;
    };

    [Serializable]
    struct BoxColliderDataSerializeStruct
    {
        public Dictionary<string, List<BoxColliderData>> allBoxColliderDataMap;
    };
    
    private void ExportBox()
    {
        float frameRate = 1 / 30.0f;
        float totalTime = animationClip.length;
        int totalFrame = (int)math.floor(totalTime / frameRate);
        float sampleTime = 0.0f;
        Transform rootBone = previewObject.transform.Find("Bip001");
        Transform[] allChildBone = rootBone.GetComponentsInChildren<Transform>(true);
        List<Transform> boxTransforms = new List<Transform>();
        for (int i = 0; i < allChildBone.Length; ++i)
        {
            if (allChildBone[i].GetComponent<BoxCollider>() != null)
            {
                boxTransforms.Add(allChildBone[i]);
            }
        }
        Dictionary<string, List<BoxColliderData>> allBoxColliderDataMap = new();
        while (true)
        {
            animationClip.SampleAnimation(previewObject, sampleTime);
            for (int i = 0; i < boxTransforms.Count; ++i)
            {
                if (!allBoxColliderDataMap.ContainsKey(boxTransforms[i].name))
                {
                    List<BoxColliderData> boxColliderDatas = new List<BoxColliderData>(totalFrame);
                    allBoxColliderDataMap.Add(boxTransforms[i].name, boxColliderDatas);
                }
                BoxColliderData boxColliderData = new BoxColliderData();
                boxColliderData.position        = previewObject.transform.InverseTransformPoint(boxTransforms[i].position);
                boxColliderData.rotation        = Quaternion.Inverse(previewObject.transform.rotation) * boxTransforms[i].rotation;
                BoxCollider collider            = boxTransforms[i].GetComponent<BoxCollider>();
                boxColliderData.center          = collider.center;
                boxColliderData.size            = collider.size;
                allBoxColliderDataMap[boxTransforms[i].name].Add(boxColliderData);
            }

            sampleTime = sampleTime + frameRate;
            if (sampleTime >= totalTime)
            {
                break;

            }
        }

        StringBuilder json = new StringBuilder("{\"data\":[");
        int count          = 0;
        foreach (var kv in allBoxColliderDataMap)
        {
            var key   = kv.Key;
            var value = kv.Value;
            json.Append("{\"name\":\"" + key + "\",");
            json.Append("\"frameData\":[");
            for (int i = 0; i < value.Count; ++i)
            {
                json.Append(JsonUtility.ToJson(value[i]));
                if (i < value.Count - 1)
                {
                    json.Append(",");
                }
            }
            json.Append("]");
            if (count < allBoxColliderDataMap.Count - 1)
            {
                json.Append("},");
            }
            else
            {
                json.Append("}");
            }
            count++;
        }
        json.Append("]}");
        if (!Directory.Exists(Application.dataPath + "/Resources/GameData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/GameData");
        }
        string path = Application.dataPath + "/Resources/GameData/test_box_colliders.json";
        File.WriteAllText(path, json.ToString());
    }
}

[Serializable]
class ListWrapper<T>
{
    public List<T> items; // 包含 List 的字段
}

public class JsonHelper<T>
{
    [System.Serializable]
    private class Wrapper
    {
        public T[] Items;
    }

    public static string ToJson(Dictionary<string, T> dictionary)
    {
        Wrapper wrapper = new Wrapper();
        wrapper.Items = new T[dictionary.Count];
        dictionary.Values.CopyTo(wrapper.Items, 0);
        return JsonUtility.ToJson(wrapper);
    }
}