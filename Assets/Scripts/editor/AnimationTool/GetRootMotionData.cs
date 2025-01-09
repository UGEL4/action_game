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
    private bool SkipFirstFrame;
    void OnGUI()
    {
        Animation         = EditorGUILayout.ObjectField("AnimationClip", Animation, typeof(AnimationClip), false) as AnimationClip;
        EvaluateFrameRate = EditorGUILayout.IntField("采样帧率", EvaluateFrameRate);
        SkipFirstFrame    = EditorGUILayout.Toggle("是否跳过第0帧", SkipFirstFrame);
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
        AnimationCurve RX = null;
        AnimationCurve RY = null;
        AnimationCurve RZ = null;
        AnimationCurve RW = null;
        foreach (var curveBinding in curveBindings)
        {
            if (curveBinding.path == "root")
            {
                Debug.Log(curveBinding.path + ", " + curveBinding.propertyName);

                if (curveBinding.propertyName.Contains("LocalPosition.x"))
                {
                    X = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                }
                else if (curveBinding.propertyName.Contains("LocalPosition.y"))
                {
                    Y = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                }
                else if (curveBinding.propertyName.Contains("LocalPosition.z"))
                {
                    Z = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                }
                if (curveBinding.propertyName.Contains("LocalRotation.x"))
                {
                    RX = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                }
                else if (curveBinding.propertyName.Contains("LocalRotation.y"))
                {
                    RY = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                }
                else if (curveBinding.propertyName.Contains("LocalRotation.z"))
                {
                    RZ = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                }
                else if (curveBinding.propertyName.Contains("LocalRotation.w"))
                {
                    RW = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                }
            }
            if (X != null && Y != null && Z != null && RX != null && RY != null && RZ != null && RW != null)
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
        if (RX.length > 0)
        {
            float time = RX[RX.length - 1].time;
            if (time > maxTime)
            {
                maxTime = time;
            }
        }
        if (RY.length > 0)
        {
            float time = RY[RY.length - 1].time;
            if (time > maxTime)
            {
                maxTime = time;
            }
        }
        if (RZ.length > 0)
        {
            float time = RZ[RZ.length - 1].time;
            if (time > maxTime)
            {
                maxTime = time;
            }
        }
        if (RW.length > 0)
        {
            float time = RZ[RZ.length - 1].time;
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
            rootMotionData.X  = new float[totalFrame];
            rootMotionData.Y  = new float[totalFrame];
            rootMotionData.Z  = new float[totalFrame];
            rootMotionData.RX = new float[totalFrame];
            rootMotionData.RY = new float[totalFrame];
            rootMotionData.RZ = new float[totalFrame];
            while (true)
            {
                if (frame >= totalFrame)
                {
                    break;
                }
                float time              = (SkipFirstFrame ? (frame + 1) : frame) * step;
                float x                 = X.Evaluate(time);
                float y                 = Y.Evaluate(time);
                float z                 = Z.Evaluate(time);
                rootMotionData.X[frame] = x;
                rootMotionData.Y[frame] = y;
                rootMotionData.Z[frame] = z;
                float rx                = RX.Evaluate(time);
                float ry                = RY.Evaluate(time);
                float rz                = RZ.Evaluate(time);
                float rw                = RW.Evaluate(time);
                var rot = new Quaternion(rx, ry, rz, rw);
                rootMotionData.X[frame] = x;
                rootMotionData.Y[frame] = y;
                rootMotionData.Z[frame] = z;
                rootMotionData.RX[frame] = rot.eulerAngles.x;
                rootMotionData.RY[frame] = rot.eulerAngles.y;
                rootMotionData.RZ[frame] = rot.eulerAngles.z;
                frame++;
            }
        }
        StringBuilder json = new StringBuilder("{\n");
        //StringBuilder json = new StringBuilder();
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
        json.Append("],\n");
        json.Append("\"RX\":[");
        for (int i = 0; i < rootMotionData.RX.Length; i++)
        {
            json.Append(rootMotionData.RX[i]);
            if (i < rootMotionData.RX.Length - 1)
            {
                json.Append(',');
            }
        }
        json.Append("],\n");
        json.Append("\"RY\":[");
        for (int i = 0; i < rootMotionData.RY.Length; i++)
        {
            json.Append(rootMotionData.RY[i]);
            if (i < rootMotionData.RY.Length - 1)
            {
                json.Append(',');
            }
        }
        json.Append("],\n");
        json.Append("\"RZ\":[");
        for (int i = 0; i < rootMotionData.RZ.Length; i++)
        {
            json.Append(rootMotionData.RZ[i]);
            if (i < rootMotionData.RZ.Length - 1)
            {
                json.Append(',');
            }
        }
        json.Append("]\n}");
        
        //json.Append(JsonUtility.ToJson(rootMotionData));
        File.WriteAllText(path, json.ToString());
    }
}
