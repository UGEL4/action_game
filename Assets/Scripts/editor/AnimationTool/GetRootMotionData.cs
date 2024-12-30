using System.IO;
using System.Text;
using ACTTools.RootMotionData;
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
    void OnGUI()
    {
        Animation = EditorGUILayout.ObjectField("AnimationClip", Animation, typeof(AnimationClip), false) as AnimationClip;
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

        RootMotionData rootMotionData = new();
        var curveBindings             = AnimationUtility.GetCurveBindings(Animation);
        foreach (var curveBinding in curveBindings)
        {
            if (curveBinding.path == "root")
            {
                Debug.Log(curveBinding.path + ", " + curveBinding.propertyName);

                if (curveBinding.propertyName.Contains("LocalPosition.x"))
                {
                    var curve        = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                    rootMotionData.X = new Keyframe[curve.length];
                    for (int i = 0; i < curve.length; i++)
                    {
                        rootMotionData.X[i] = curve.keys[i];
                    }
                }
                else if (curveBinding.propertyName.Contains("LocalPosition.y"))
                {
                    var curve        = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                    rootMotionData.Y = new Keyframe[curve.length];
                    for (int i = 0; i < curve.length; i++)
                    {
                        rootMotionData.Y[i] = curve.keys[i];
                    }
                }
                else if (curveBinding.propertyName.Contains("LocalPosition.z"))
                {
                    var curve        = AnimationUtility.GetEditorCurve(Animation, curveBinding);
                    rootMotionData.Z = new Keyframe[curve.length];
                    for (int i = 0; i < curve.length; i++)
                    {
                        rootMotionData.Z[i] = curve.keys[i];
                    }
                }
            }
        }

        StringBuilder json = new StringBuilder("{\n");
        //layout: {X:[[Time, Value, InTangent, OutTangent, TangentMode, WeightedMode, InWeight, OutWeight]], Y:[...], Z:[...]}
        string format = "[{0},{1},{2},{3},{4},{5},{6},{7}]";
        json.Append("\"X\":[");
        for (int i = 0; i < rootMotionData.X.Length; i++)
        {
            json.Append(string.Format(format, rootMotionData.X[i].time, rootMotionData.X[i].value, 
                                      rootMotionData.X[i].inTangent, rootMotionData.X[i].outTangent, (int)rootMotionData.X[i].tangentMode, 
                                      (int)rootMotionData.X[i].weightedMode, rootMotionData.X[i].inWeight, rootMotionData.X[i].outWeight));
            if (i < rootMotionData.X.Length - 1)
            {
                json.Append(',');
            }
        }
        json.Append("],\n");
        json.Append("\"Y\":[");
        for (int i = 0; i < rootMotionData.Y.Length; i++)
        {
            json.Append(string.Format(format, rootMotionData.Y[i].time, rootMotionData.Y[i].value, 
                                      rootMotionData.Y[i].inTangent, rootMotionData.Y[i].outTangent, (int)rootMotionData.Y[i].tangentMode, 
                                      (int)rootMotionData.Y[i].weightedMode, rootMotionData.Y[i].inWeight, rootMotionData.Y[i].outWeight));
            if (i < rootMotionData.Y.Length - 1)
            {
                json.Append(',');
            }
        }
        json.Append("],\n");
        json.Append("\"Z\":[");
        for (int i = 0; i < rootMotionData.Z.Length; i++)
        {
            json.Append(string.Format(format, rootMotionData.Z[i].time, rootMotionData.Z[i].value, 
                                      rootMotionData.Z[i].inTangent, rootMotionData.Z[i].outTangent, (int)rootMotionData.Z[i].tangentMode, 
                                      (int)rootMotionData.Z[i].weightedMode, rootMotionData.Z[i].inWeight, rootMotionData.Z[i].outWeight));
            if (i < rootMotionData.Z.Length - 1)
            {
                json.Append(',');
            }
        }
        json.Append("]\n}");
        File.WriteAllText(path, json.ToString());
    }
}
