using System.Collections.Generic;
using Log;
using UnityEngine;
using UnityEngine.Playables;
using Action;
public class ActionEditHitBoxBehaviour : PlayableBehaviour
{
    public ActionEditHitBoxClip Asset;
    private readonly double frameTime = 0.03333333;
    public bool Record
    {
        get {
            return Asset.Record;
        }
    }
    public GameObject target;
    private bool isFirstFrame = true;
    private int curFrame      = 0;
    private int lastFrame     = -1;

    public PlayableDirector director;

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
            var boxManager = Asset.BoxMgr;
            var keyFrames  = Asset.KeyFrames;
            if (curFrame > 0)
            {
                KeyFrameData frameData;
                if (keyFrames.TryGetValue(curFrame, out frameData))
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
                        boxData.position    = boxList[i].transform.localPosition;
                        boxData.rotation    = boxList[i].transform.localRotation;
                        boxData.scale       = boxList[i].transform.localScale;
                        boxData.bounds      = bounds.bounds;
                        //boxData.name        = boxList[i].name;
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
                    newFrameData.frame        = curFrame;
                    var boxList               = boxManager.boxObjects;
                    newFrameData.boxDataList  = new List<BoxData>(boxList.Count);
                    for (int i = 0; i < boxList.Count; ++i)
                    {
                        BoxData boxData     = new BoxData();
                        boxData.position    = boxList[i].transform.localPosition;
                        boxData.rotation    = boxList[i].transform.localRotation;
                        boxData.scale       = boxList[i].transform.localScale;
                        CustomBounds bounds = boxList[i].GetComponent<CustomBounds>();
                        boxData.bounds      = bounds.bounds;
                        //boxData.name        = boxList[i].name;
                        newFrameData.boxDataList.Add(boxData);
                    }
                    keyFrames.Add(curFrame, newFrameData);
                }
            }
        }
    }

    public override void ProcessFrame(Playable playable, FrameData info, object playerData)
    {
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
            return;
        }
        //SimpleLog.Info("CurrentFrame:", curFrame, lastFrame);
        // SimpleLog.Info("frameNumer:", curFrame, lastFrame);
        var boxManager = Asset.BoxMgr;
        var keyFrames  = Asset.KeyFrames;
        KeyFrameData frameData;
        if (keyFrames.TryGetValue(curFrame, out frameData))
        {
            var boxList = boxManager.boxObjects;
            for (int i = 0; i < boxList.Count; ++i)
            {
                if (i < frameData.boxDataList.Count)
                {
                    var boxData                     = frameData.boxDataList[i];
                    boxList[i].transform.localPosition   = boxData.position;
                    boxList[i].transform.localRotation   = boxData.rotation;
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
            var defaultBoxDataList = Asset.DefaultBoxDataList;
            var boxList            = boxManager.boxObjects;
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

    public override void OnBehaviourPause(Playable playable, FrameData info)
    {
        base.OnBehaviourPause(playable, info);
    }

    public override void OnGraphStart(Playable playable)
    {
        isFirstFrame = true;
        curFrame     = 0;
        lastFrame    = -1;
        isFirstFrame = true;
        SimpleLog.Info("OnGraphStart:", curFrame, lastFrame);
    }

    public override void OnGraphStop(Playable playable)
    {
        isFirstFrame = true;
        curFrame     = 0;
        lastFrame    = -1;
        isFirstFrame = true;
        SimpleLog.Info("OnGraphStop:", curFrame, lastFrame);
    }
}

