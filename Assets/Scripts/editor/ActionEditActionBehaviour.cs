using System;
using System.Collections.Generic;
using Log;
using UnityEditor;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.SocialPlatforms;
using UnityEngine.Timeline;



public class ActionEditActionBehaviour : PlayableBehaviour
{
    public ActionEditActionClip asset;
    private readonly double frameTime = 0.03333333;
    public bool Record { get{ return asset.record; } }
    public GameObject target;
    public BoxManager boxManager;
    List<KeyFrameData> keyFrameDatas        = new();
    Dictionary<int, KeyFrameData> keyFrames = new();

    List<BoxData> defaultBoxDataList;

    private bool isFirstFrame = true;
    private int curFrame      = 0;
    private int lastFrame     = -1;

    public PlayableDirector director;

    private FrameData lastFrameData;

    public void InitDefaultData()
    {
        var boxList        = boxManager.boxObjects;
        defaultBoxDataList = new List<BoxData>(boxList.Count);
        for (int i = 0; i < boxList.Count; ++i)
        {
            var boxData         = new BoxData();
            boxData.position = boxList[i].transform.localPosition;
            boxData.rotation = boxList[i].transform.localRotation;
            boxData.scale    = boxList[i].transform.localScale;
            CustomBounds bounds = boxList[i].GetComponent<CustomBounds>();
            if (bounds != null)
            {
                boxData.bounds = bounds.bounds;
            }
            defaultBoxDataList.Add(boxData);
        }
    }

    public void ClearFrameDataList()
    {
        keyFrames.Clear();
        // SimpleLog.Info("ClearFrameDataList");
    }

    public override void OnPlayableCreate(Playable playable)
    {
        base.OnPlayableCreate(playable);
        SimpleLog.Info("OnPlayableCreate");
    }

    public override void PrepareFrame(Playable playable, FrameData info)
    {
        // 记录上一帧的数据
        if (Record)
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
                        boxData.position    = boxList[i].transform.position;
                        boxData.rotation    = boxList[i].transform.rotation;
                        boxData.scale       = boxList[i].transform.localScale;
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
                        boxData.position    = boxList[i].transform.position;
                        boxData.rotation    = boxList[i].transform.rotation;
                        boxData.scale       = boxList[i].transform.localScale;
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
        // if (!isFirstFrame)
        {
            // int frameNumber = (int)(director.time / director.playableAsset.duration); // 计算帧号
            var maxFrameNum = (int)(director.playableAsset.duration / frameTime);
            bool firstFrame = lastFrame == -1;
            curFrame        = (int)(director.time / frameTime); // 计算帧号
            if (lastFrame == curFrame && !firstFrame)
            {
                return;
            }
            lastFrame = curFrame; // 计算帧号
            if (curFrame == maxFrameNum || (!firstFrame && curFrame == 0))
            {
                curFrame     = 0;
                lastFrame    = -1;
                isFirstFrame = true;
                SimpleLog.Info("FirstFrame:", curFrame, lastFrame);
            }
            // SimpleLog.Info("frameNumer:", curFrame, lastFrame);
            KeyFrameData frameData;
            if (keyFrames.TryGetValue(curFrame, out frameData))
            {
                var boxList = boxManager.boxObjects;
                for (int i = 0; i < boxList.Count; ++i)
                {
                    if (i < frameData.boxDataList.Count)
                    {
                        var boxData                     = frameData.boxDataList[i];
                        boxList[i].transform.position   = boxData.position;
                        boxList[i].transform.rotation   = boxData.rotation;
                        boxList[i].transform.localScale = boxData.scale;
                        CustomBounds bounds             = boxList[i].GetComponent<CustomBounds>();
                        if (bounds != null)
                        {
                            bounds.bounds = boxData.bounds;
                        }
                    }
                }
            }
            else
            {
                var boxList = boxManager.boxObjects;
                for (int i = 0; i < boxList.Count; ++i)
                {
                    if (i < defaultBoxDataList.Count)
                    {
                        var boxData                        = defaultBoxDataList[i];
                        boxList[i].transform.localPosition = boxData.position;
                        boxList[i].transform.localRotation = boxData.rotation;
                        boxList[i].transform.localScale    = boxData.scale;
                        CustomBounds bounds                = boxList[i].GetComponent<CustomBounds>();
                        if (bounds != null)
                        {
                            bounds.bounds = boxData.bounds;
                        }
                    }
                }
            }
        }
    }

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }

    public override void OnGraphStart(Playable playable)
    {
        isFirstFrame = true;
        curFrame     = 0;
    }

    public override void OnGraphStop(Playable playable)
    {
        isFirstFrame = true;
        curFrame     = 0;
    }
}
