using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ACTAction;
using ACTTools;
using Unity.Mathematics;
using UnityEditor;
using UnityEngine;

public class ExportBoxColliderAnimationWindow : EditorWindow
{
    [MenuItem("Window/导出骨骼碰撞盒动画数据")]
    public static void ShowWindow()
    {
        GetWindow<ExportBoxColliderAnimationWindow>("导出骨骼碰撞盒动画数据");
    }

    private string FileName;
    private AnimationClip Animation;
    private GameObject PreviewObj;
    private GameObject RootBone;
    private int FrameCount;
    void OnGUI()
    {
        FileName   = EditorGUILayout.TextField("文件名", FileName);
        FrameCount = EditorGUILayout.IntField("导出帧数", FrameCount);
        Animation  = (AnimationClip)EditorGUILayout.ObjectField("动画", Animation, typeof(AnimationClip), false);
        PreviewObj = (GameObject)EditorGUILayout.ObjectField("预览对象", PreviewObj, typeof(GameObject), true);
        RootBone   = (GameObject)EditorGUILayout.ObjectField("根骨骼", RootBone, typeof(GameObject), true);
        if (PreviewObj != null && RootBone != null)
        {
            if (Animation != null)
            {
                if (GUILayout.Button("导出"))
                {
                    if (FileName == null || FileName == "" || FileName.Trim() == "")
                    {
                        GUILayout.Label("输入文件名");
                    }
                    else
                    {
                        Export();
                    }
                }
            }
        }
    }

    void Export()
    {
        float frameRate = 1.0f / FrameCount;
        float totalTime = Animation.length;
        int totalFrame = (int)math.floor(totalTime / frameRate);
        float sampleTime = 0.0f;
        //Transform rootBone = FindChild(PreviewObj.transform, "Bip001");
        Transform rootBone            = RootBone.transform;
        Transform[] allChildBone      = rootBone.GetComponentsInChildren<Transform>(true);
        List<Transform> boxTransforms = new List<Transform>();
        for (int i = 0; i < allChildBone.Length; ++i)
        {
            if (allChildBone[i].GetComponent<BoxCollider>() != null)
            {
                boxTransforms.Add(allChildBone[i]);
            }
        }
        //Dictionary<string, List<BoxColliderData>> allBoxColliderDataMap = new();
        Dictionary<string, BoxColliderData[]> allBoxColliderDataMap = new();
        int frame = 0;
        while (true)
        {
            sampleTime = frame * frameRate;
            Animation.SampleAnimation(PreviewObj, sampleTime);
            for (int i = 0; i < boxTransforms.Count; ++i)
            {
                string name = boxTransforms[i].name;
                if (!allBoxColliderDataMap.ContainsKey(name))
                {
                    BoxColliderData[] FrameData = new BoxColliderData[totalFrame];
                    allBoxColliderDataMap.Add(name, FrameData);
                }
                Vector3 position     = PreviewObj.transform.InverseTransformPoint(boxTransforms[i].position);
                Vector3 rotation     = (Quaternion.Inverse(PreviewObj.transform.rotation) * boxTransforms[i].rotation).eulerAngles;
                BoxCollider collider = boxTransforms[i].GetComponent<BoxCollider>();
                Vector3 center       = collider.center;
                Vector3 size         = collider.size;

                allBoxColliderDataMap[name][frame].position = position;
                allBoxColliderDataMap[name][frame].rotation = rotation;
                allBoxColliderDataMap[name][frame].center   = center;
                allBoxColliderDataMap[name][frame].size     = size;
            }

            frame++;
            //sampleTime += frameRate;
            if (frame >= totalFrame)
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
            CharacterBoneBox data = new CharacterBoneBox()
            {
                BoxName = key,
                FrameData = value
            };
            // json.Append("{\"name\":\"" + key + "\",");
            // json.Append("\"frameData\":");
            json.Append(JsonUtility.ToJson(data));
            if (count < allBoxColliderDataMap.Count - 1)
            {
                json.Append(",");
            }
            // else
            // {
            //     json.Append("}");
            // }
            count++;
        }
        json.Append("]}");
        if (!Directory.Exists(Application.dataPath + "/Resources/GameData/ActionBoneColliderAnimation"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/GameData/ActionBoneColliderAnimation");
        }
        string path = Application.dataPath + "/Resources/GameData/ActionBoneColliderAnimation/" + FileName + ".json";
        File.WriteAllText(path, json.ToString());
    }

    Transform FindChild(Transform root, string name)
    {
        if (root.gameObject.name == name) return root;

        Transform child = root.Find(name);
        if (child != null) return child;

        foreach (Transform c in root.transform)
        {
            child = FindChild(c, name);
            if (child != null) return child;
        }

        return child;
    }
}
