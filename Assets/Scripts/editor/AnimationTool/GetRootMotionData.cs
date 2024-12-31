using System.IO;
using System.Text;
using ACTTools.RootMotionData;
using Log;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;

public class GetRootMotionData : EditorWindow
{
    [MenuItem("Tools/Animation/Get Root Motion Data")]
    public static void ShowWindow()
    {
        GetWindow<GetRootMotionData>("Get Root Motion Data");
    }

    private AnimationClip Animation;
    private int EvaluateFrameRate;
    void OnGUI()
    {
        Animation         = EditorGUILayout.ObjectField("AnimationClip", Animation, typeof(AnimationClip), false) as AnimationClip;
        EvaluateFrameRate = EditorGUILayout.IntField("采样帧率", EvaluateFrameRate);
        if (Animation)
        {
            if (GUILayout.Button("Save"))
            {
                Save();
            }
        }
    }

    void Save()
    {
        string path = EditorUtility.SaveFilePanel("Save File", Application.dataPath + "/Resources/GameData/RootMotion", "NewFile.json", "json");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        if (EvaluateFrameRate <= 0)
        {
            SimpleLog.Error("没有设置采样帧率");
            return;
        }

        RootMotionData rootMotionData = new();
        var curveBindings             = AnimationUtility.GetCurveBindings(Animation);
        AnimationCurve X = null;
        AnimationCurve Y = null;
        AnimationCurve Z = null;
        foreach (var curveBinding in curveBindings)
        {
            if (curveBinding.path == "root")
            {
                Debug.Log(curveBinding.path + ", " + curveBinding.propertyName);

                if (curveBinding.propertyName.Contains("LocalPosition.x"))
                {
                    X = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                    // rootMotionData.X = new Keyframe[curve.length];
                    // for (int i = 0; i < curve.length; i++)
                    // {
                    //     rootMotionData.X[i] = curve.keys[i];
                    // }
                }
                else if (curveBinding.propertyName.Contains("LocalPosition.y"))
                {
                    Y = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                    // rootMotionData.Y = new Keyframe[curve.length];
                    // for (int i = 0; i < curve.length; i++)
                    // {
                    //     rootMotionData.Y[i] = curve.keys[i];
                    // }
                }
                else if (curveBinding.propertyName.Contains("LocalPosition.z"))
                {
                    Z = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                    // rootMotionData.Z = new Keyframe[curve.length];
                    // for (int i = 0; i < curve.length; i++)
                    // {
                    //     rootMotionData.Z[i] = curve.keys[i];
                    // }
                }
            }
            if (X != null && Y != null && Z != null)
            {
                break;
            }
        }
        float maxTime = 0f;
        if (X.length > 0)
        {
            maxTime = X[X.length - 1].time;
        }
        if (Y.length > 0)
        {
            float time = Y[Y.length - 1].time;
            if (time > maxTime)
            {
                maxTime = time;
            }
        }
        if (Z.length > 0)
        {
            float time = Z[Z.length - 1].time;
            if (time > maxTime)
            {
                maxTime = time;
            }
        }
        int totalFrame = (int)(maxTime * EvaluateFrameRate);
        int frame      = 0;
        float step     = 1f / EvaluateFrameRate;
        if (totalFrame > 0)
        {
            rootMotionData.X = new float[totalFrame];
            rootMotionData.Y = new float[totalFrame];
            rootMotionData.Z = new float[totalFrame];
            while (true)
            {
                if (frame >= totalFrame)
                {
                    break;
                }
                float time              = frame * step;
                float x                 = X.Evaluate(time);
                float y                 = Y.Evaluate(time);
                float z                 = Z.Evaluate(time);
                rootMotionData.X[frame] = x;
                rootMotionData.Y[frame] = y;
                rootMotionData.Z[frame] = z;
                frame++;
            }
        }
        StringBuilder json = new StringBuilder("{\n");
        //layout: {X:[frame0, frame1, ...], Y:[...], Z:[...]}
        //string format = "{0},{1},{2},{3},{4},{5},{6},{7}]";
        json.Append("\"X\":[");
        for (int i = 0; i < rootMotionData.X.Length; i++)
        {
            json.Append(rootMotionData.X[i]);
            if (i < rootMotionData.X.Length - 1)
            {
                json.Append(',');
            }
        }
        json.Append("],\n");
        json.Append("\"Y\":[");
        for (int i = 0; i < rootMotionData.Y.Length; i++)
        {
            json.Append(rootMotionData.Y[i]);
            if (i < rootMotionData.Y.Length - 1)
            {
                json.Append(',');
            }
        }
        json.Append("],\n");
        json.Append("\"Z\":[");
        for (int i = 0; i < rootMotionData.Z.Length; i++)
        {
            json.Append(rootMotionData.Z[i]);
            if (i < rootMotionData.Z.Length - 1)
            {
                json.Append(',');
            }
        }
        json.Append("]\n}");
        File.WriteAllText(path, json.ToString());
    }
}
