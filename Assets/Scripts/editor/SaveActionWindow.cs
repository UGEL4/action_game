using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using ACTTools;
using Unity.Burst.Intrinsics;
using UnityEditor;
using UnityEngine;
using UnityEngine.Timeline;

public class SaveActionWindow : EditorWindow
{
    private TimelineAsset timelineAsset;
    private string fileName;

    [MenuItem("Tools/Action/SaveAction")]
    public static void ShowWindow()
    {
        GetWindow<SaveActionWindow>("SaveAction");
    }

    void OnGUI()
    {
        GUILayout.Label("timelineAsset", EditorStyles.boldLabel);
        timelineAsset = (TimelineAsset)EditorGUILayout.ObjectField("timelineAsset", timelineAsset, typeof(TimelineAsset), false);
        fileName = EditorGUILayout.TextField("fileName", fileName);
        if (timelineAsset != null)
        {
            if (GUILayout.Button("Save"))
            {
                if (fileName == null || fileName == string.Empty)
                {
                    return;
                }
                Save();
            }
        }
    }

    void Save()
    {
        string path = EditorUtility.SaveFilePanel("Save File", Application.dataPath + "/Resources/GameData/ActionData", "NewFile.json", "json");
        if (string.IsNullOrEmpty(path))
        {
            return;
        }

        AttackBoxTurnOnInfo[] allTurnOnInfo = new AttackBoxTurnOnInfo[1];
        BeHitBoxTurnOnInfo[] defensePhases  = new BeHitBoxTurnOnInfo[1];
        CharacterAction characterAction     = new CharacterAction();
        foreach (var track in timelineAsset.GetOutputTracks())
        {
            // if (track.GetType() == typeof(ActionEditorHitRayCastTrack))
            // {
            //     GetRayCastPointTurnOnInfo(track, out allTurnOnInfo);
            // }
            if (track.GetType() == typeof(ActionEditorActionDataTrack))
            {
                GetActionData(track, out characterAction);
            }
            if (track.GetType() == typeof(ActionEditorHitBoxTrack))
            {
                //GetHitBoxTurnOnInfo(track, out defensePhases);
            }
            if (track.GetType() == typeof(ActionEditorAttackBoxTrack))
            {
                GetAttackBoxTurnOnInfo(track, out AttackBoxTurnOnInfo attackBox);
                allTurnOnInfo[0] = attackBox;
            }
            if (track.GetType() == typeof(ActionEditorDefenseBoxTrack))
            {
                GetHitBoxTurnOnInfo(track, out BeHitBoxTurnOnInfo hitBoxTurnOnInfo);
                defensePhases[0] = hitBoxTurnOnInfo;

            }
        }

        characterAction.attackPhaseList = allTurnOnInfo;
        characterAction.defensePhases   = defensePhases;
        StringBuilder json              = new StringBuilder("{\"data\":");
        json.Append(JsonUtility.ToJson(characterAction, true));
        if (!Directory.Exists(Application.dataPath + "/Resources/GameData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/GameData");
        }
        json.Append("}");
        //string path = Application.dataPath + "/Resources/GameData/" + fileName + ".json";
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

            AttackBoxTurnOnInfo info = new();
            info.AttackPhase         = c;

            var AllRayCastPointsTransformPerFrame = clipAsset.AllRayCastPointsTransformPerFrame;
            int groupCount                        = AllRayCastPointsTransformPerFrame.Count;
            info.FrameIndexRange                  = new FrameIndexRange[groupCount];
            for (int i = 0; i < clipAsset.AttackRayTurnOnInfoList.Count; i++)
            {
                //name
                //List<string> nameList = clipAsset.AttackRayTurnOnInfoList[i].PointNameList;
                //
                info.FrameIndexRange[i] = clipAsset.AttackRayTurnOnInfoList[i].ActiveFrame;
            }

            info.RayPointGroupList = new AttackRayPointGroup[groupCount];
            foreach (var data in AllRayCastPointsTransformPerFrame)
            {
                int group                      = data.Key;
                AttackRayPointGroup pointGroup = new();
                pointGroup.Points              = new AttackRayPointData[data.Value.Count];
                pointGroup.Tag                 = clipAsset.AttackRayTurnOnInfoList[group].Tag;
                int pointCount                 = 0;
                foreach (var pointInfoPair in data.Value)
                {
                    pointGroup.Points[pointCount].RayPointTransforms = new PositionRotationData[pointInfoPair.Value.transforms.Length];
                    for (int j = 0; j < pointInfoPair.Value.transforms.Length; j++)
                    {
                        pointGroup.Points[pointCount].RayPointTransforms[j].Position = pointInfoPair.Value.transforms[j].Position;
                        pointGroup.Points[pointCount].RayPointTransforms[j].Rotation = pointInfoPair.Value.transforms[j].Rotation;
                    }
                    pointCount++;
                }
                info.RayPointGroupList[group] = pointGroup;
            }
            allTurnOnInfo[c++] = info;
        }
    }

    void GetAttackBoxTurnOnInfo(TrackAsset track, out AttackBoxTurnOnInfo turnOnInfo)
    {
        var clips                              = track.GetClips();
        List<BoxColliderData> AttackBoxes      = new();
        List<FrameIndexRange> FrameIndexRanges = new();
        foreach (var clip in clips)
        {
            ActionEditorAttackBoxClip clipAsset = clip.asset as ActionEditorAttackBoxClip;
            AttackBoxes.AddRange(clipAsset.BoxDataList);
            FrameIndexRanges.AddRange(clipAsset.ActivtFrame);
        }
        turnOnInfo                 = new AttackBoxTurnOnInfo();
        turnOnInfo.AttackBoxes     = AttackBoxes.ToArray();
        turnOnInfo.FrameIndexRange = FrameIndexRanges.ToArray();
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
        // int i            = 0;
        // foreach (var clip in clips)
        // {
        //     ActionEditorHitBoxClip clipAsset = clip.asset as ActionEditorHitBoxClip;
        //     BeHitBoxTurnOnInfo info          = new();
        //     info.SelfActionChangeInfo        = clipAsset.SelfActionChangeInfo;
        //     info.TargetActionChangeInfo      = clipAsset.TargetActionChangeInfo;
        //     info.FrameIndexRange             = clipAsset.ActiveFrameRang;
        //     //BeHitBoxTurnOnInfo info          = clipAsset.turnOnInfo;
        //     List<string> tempList            = new();
        //     foreach (var boxInfo in clipAsset.boxList)
        //     {
        //         if (boxInfo.active && boxInfo.Bone != null)
        //         {
        //             tempList.Add(boxInfo.Bone.name);
        //         }
        //     }
        //     info.Tags = new string[tempList.Count];
        //     tempList.CopyTo(info.Tags);

        //     hitBoxTurnOnInfo[i++] = info;
        // }
    }

    void GetHitBoxTurnOnInfo(TrackAsset track, out BeHitBoxTurnOnInfo hitBoxTurnOnInfo)
    {
        var clips        = track.GetClips();
        hitBoxTurnOnInfo = new BeHitBoxTurnOnInfo();
        hitBoxTurnOnInfo.FrameIndexRange = new FrameIndexRange[clips.Count()];
        hitBoxTurnOnInfo.DefenseBoxes    = new BoxColliderData[clips.Count()];
        int i            = 0;
        
        foreach (var clip in clips)
        {
            ActionEditorDefenseBoxClip clipAsset = clip.asset as ActionEditorDefenseBoxClip;
            BoxColliderData boxData = clipAsset.mBoxData;
            hitBoxTurnOnInfo.DefenseBoxes[i] = boxData;
            hitBoxTurnOnInfo.FrameIndexRange[i] = clipAsset.ActivtFrame;
            i++;
        }
    }
}
