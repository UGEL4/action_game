using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms;
using UnityEngine.Timeline;

[Serializable]
public struct BoxData
{
    public Transform transform;
    public OBB bounds;
    public string name;
}

[Serializable]
public struct KeyFrameData
{
    public int frame;
    public List<BoxData> boxDataList;
}

public class ActionEditActionBehaviour : PlayableBehaviour
{
    public bool record = false;
    public GameObject target;
    public BoxManager boxManager;
    List<KeyFrameData> keyFrameDatas = new();
    Dictionary<int, KeyFrameData> keyFrames = new();

    private bool isFirstFrame = true;
    private int curFrame = 0;

    public PlayableDirector director;

    private FrameData lastFrameData;

    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        //记录上一帧的数据
        if (record)
        {
            if (curFrame > 0)
            {
                KeyFrameData frameData;
                if (keyFrames.TryGetValue(curFrame - 1, out frameData))
                {
                    var boxList = boxManager.boxObjects;
                    bool isNull = false;
                    if (frameData.boxDataList == null)
                    {
                        frameData.boxDataList = new List<BoxData>(boxList.Count);
                        isNull                = true;
                    }
                    for (int i = 0; i < boxList.Count; ++i)
                    {
                        CustomBounds bounds = boxList[i].GetComponent<CustomBounds>();
                        BoxData boxData     = new BoxData();
                        boxData.transform   = boxList[i].transform;
                        boxData.bounds      = bounds.bounds;
                        boxData.name        = boxList[i].name;
                        if (isNull)
                        {
                            frameData.boxDataList.Add(boxData);
                        }
                        else
                        {
                            frameData.boxDataList[i] = boxData;
                        }
                    }
                }
                else
                {
                    KeyFrameData newFrameData = new KeyFrameData();
                    newFrameData.frame        = curFrame - 1;
                    var boxList               = boxManager.boxObjects;
                    newFrameData.boxDataList  = new List<BoxData>(boxList.Count);
                    for (int i = 0; i < boxList.Count; ++i)
                    {
                        BoxData boxData     = new BoxData();
                        boxData.transform   = boxList[i].transform;
                        CustomBounds bounds = boxList[i].GetComponent<CustomBounds>();
                        boxData.bounds      = bounds.bounds;
                        boxData.name        = boxList[i].name;
                        newFrameData.boxDataList.Add(boxData);
                    }
                    keyFrames.Add(curFrame - 1, newFrameData);
                }
            }
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
        //if (!isFirstFrame)
        {
            PlayableDirector d = director;
            int frameNumber = (int)(director.time / director.playableAsset.duration); // 计算帧号
            KeyFrameData frameData;
            if (keyFrames.TryGetValue(curFrame, out frameData))
            {
                var boxList = boxManager.boxObjects;
                for (int i = 0; i < boxList.Count; ++i)
                {
                    if (i < frameData.boxDataList.Count)
                    {
                        var boxData = frameData.boxDataList[i];
                        boxList[i].transform.position   = boxData.transform.position;
                        boxList[i].transform.rotation   = boxData.transform.rotation;
                        boxList[i].transform.localScale = boxData.transform.localScale;
                        CustomBounds bounds = boxList[i].GetComponent<CustomBounds>();
                        if (bounds != null)
                        {
                            bounds.bounds = boxData.bounds;
                        }
                    }
                }
            }
        }
        ++curFrame;
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }

    public override void OnGraphStart(Playable playable)
    {
        isFirstFrame = true;
        curFrame = 0;
    }

    public override void OnGraphStop(Playable playable)
    {
        isFirstFrame = true;
        curFrame = 0;
    }
}
