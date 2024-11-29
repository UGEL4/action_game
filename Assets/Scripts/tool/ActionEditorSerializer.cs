using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using System.Collections.Generic;
//using Newtonsoft.Json; // 使用Json.NET进行JSON序列化

public class ActionEditorSerializer : MonoBehaviour
{
    public TimelineAsset timelineAsset;

    public void Save()
    {
        if (timelineAsset != null)
        {
            List<TrackData> trackDataList = new List<TrackData>();

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                TrackData trackData = new TrackData
                {
                    trackName = track.name,
                    clips = new List<ClipData>()
                };

                foreach (var clip in track.GetClips())
                {
                    ClipData clipData = new ClipData
                    {
                        clipName = clip.displayName,
                        startTime = clip.start,
                        duration = clip.duration
                    };
                    trackData.clips.Add(clipData);
                }

                trackDataList.Add(trackData);
            }

            //string json = JsonConvert.SerializeObject(trackDataList, Formatting.Indented);
            //Debug.Log(json);
        }
    }
}

[System.Serializable]
public class TrackData
{
    public string trackName;
    public List<ClipData> clips;
}

[System.Serializable]
public class ClipData
{
    public string clipName;
    public double startTime;
    public double duration;
}
