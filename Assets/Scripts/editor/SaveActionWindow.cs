using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using ACTTools;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class SaveActionWindow : EditorWindow
{
    private TimelineAsset timelineAsset;

    [MenuItem("Window/SaveAction")]
    public static void ShowWindow()
    {
        GetWindow<SaveActionWindow>("SaveAction");
    }

    void OnGUI()
    {
        GUILayout.Label("timelineAsset", EditorStyles.boldLabel);
        timelineAsset = (TimelineAsset)EditorGUILayout.ObjectField("timelineAsset", timelineAsset, typeof(TimelineAsset), false);
        if (timelineAsset != null)
        {
            if (GUILayout.Button("Save"))
            {
                Save();
            }
        }
    }

    void Save()
    {
        AttackBoxTurnOnInfo[] allTurnOnInfo = new AttackBoxTurnOnInfo[0];
        CharacterAction characterAction     = new CharacterAction();
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            if (track.GetType() == typeof(ActionEditorHitRayCastTrack))
            {
                GetRayCastPointTurnOnInfo(track, out allTurnOnInfo);
            }
            if (track.GetType() == typeof(ActionEditorActionDataTrack))
            {
                GetActionData(track, out characterAction);
            }
        }

        characterAction.attackPhaseList = allTurnOnInfo;
        StringBuilder json              = new StringBuilder("{\"data\":");
        json.Append(JsonUtility.ToJson(characterAction));
        if (!Directory.Exists(Application.dataPath + "/Resources/GameData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/GameData");
        }
        json.Append("}");
        string path = Application.dataPath + "/Resources/GameData/test_action.json";
        File.WriteAllText(path, json.ToString());
    }

    void GetRayCastPointTurnOnInfo(TrackAsset track, out AttackBoxTurnOnInfo[] allTurnOnInfo)
    {
        string trackName = track.name;
        ActionEditorHitRayCastClip clipAsset = null;
        foreach (var clip in track.GetClips())
        {
            clipAsset = clip.asset as ActionEditorHitRayCastClip;
            break;
        }
        if (clipAsset == null)
        {
            allTurnOnInfo = new AttackBoxTurnOnInfo[0];
            return;
        }
        {
            var map        = clipAsset.rayCastPointsTransformPerFrame;
            int rayCount   = map[(int)clipAsset.activeFrameRange.min].Count;
            int frameCount = map.Count;
            allTurnOnInfo  = new AttackBoxTurnOnInfo[rayCount];
            for (int i = 0; i < rayCount; ++i)
            {
                AttackBoxTurnOnInfo info = new AttackBoxTurnOnInfo();
                info.FrameIndexRange     = clipAsset.activeFrameRange;
                info.RayPointTransforms  = new PositionRotationData[frameCount];
                int start = (int)clipAsset.activeFrameRange.min;
                for (int j = 0; j < frameCount; ++j)
                {
                    var trans = map[j + start];
                    var data  = new PositionRotationData() {
                        Position = trans[i].Position,
                        Rotation = trans[i].Rotation
                    };
                    info.RayPointTransforms[j] = data;
                }
                allTurnOnInfo[i] = info;
            }
        }
    }

    void GetActionData(TrackAsset track, out CharacterAction action)
    {
        action = new CharacterAction();
        ActionEditorActionDataClip clipAsset = null;
        foreach (var clip in track.GetClips())
        {
            clipAsset = clip.asset as ActionEditorActionDataClip;
            break;
        }
        if (clipAsset != null)
        {
            action = clipAsset.action;
        }
    }
}
