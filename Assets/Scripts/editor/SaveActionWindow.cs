using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
            if (track.GetType() == typeof(ActionEditorHitBoxTrack))
            {
                GetHitBoxTurnOnInfo(track, out characterAction.defensePhases);
            }
        }

        characterAction.attackPhaseList = allTurnOnInfo;
        StringBuilder json              = new StringBuilder("{\"data\":");
        json.Append(JsonUtility.ToJson(characterAction, true));
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
        var clips     = track.GetClips();
        allTurnOnInfo = new AttackBoxTurnOnInfo[clips.Count()];
        int c         = 0;
        foreach (var clip in clips)
        {
            ActionEditorHitRayCastClip clipAsset = clip.asset as ActionEditorHitRayCastClip;

            var map                  = clipAsset.rayCastPointsTransformPerFrame;
            AttackBoxTurnOnInfo info = clipAsset.attackBoxTurnOnInfo;
            // int rayCount             = map[(int)info.FrameIndexRange.min].Count;
            // info.RayPointDataList    = new AttackRayPointData[rayCount];
            // info.AttackPhase         = c;
            // int frameCount           = map.Count;
            // for (int i = 0; i < rayCount; ++i)
            // {
            //     AttackRayPointData pointData = new AttackRayPointData();
            //     pointData.RayPointTransforms = new PositionRotationData[frameCount];
            //     int start                    = (int)info.FrameIndexRange.min;
            //     for (int j = 0; j < frameCount; ++j)
            //     {
            //         var trans = map[j + start];
            //         var data  = new PositionRotationData() {
            //             Position = trans[i].Position,
            //             Rotation = trans[i].Rotation
            //         };
            //         pointData.RayPointTransforms[j] = data;
            //     }
            //     info.RayPointDataList[i] = pointData;
            // }
            allTurnOnInfo[c++] = info;
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

    void GetHitBoxTurnOnInfo(TrackAsset track, out BeHitBoxTurnOnInfo[] hitBoxTurnOnInfo)
    {
        var clips        = track.GetClips();
        hitBoxTurnOnInfo = new BeHitBoxTurnOnInfo[clips.Count()];
        int i            = 0;
        foreach (var clip in clips)
        {
            ActionEditorHitBoxClip clipAsset = clip.asset as ActionEditorHitBoxClip;
            BeHitBoxTurnOnInfo info          = clipAsset.turnOnInfo;
            List<string> tempList            = new();
            foreach (var boxInfo in clipAsset.boxList)
            {
                if (boxInfo.active && boxInfo.Bone != null)
                {
                    tempList.Add(boxInfo.Bone.name);
                }
            }
            info.Tags = new string[tempList.Count];
            tempList.CopyTo(info.Tags);

            hitBoxTurnOnInfo[i++] = info;
        }
    }
}
