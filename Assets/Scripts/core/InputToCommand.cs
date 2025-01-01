using System;
using System.Collections.Generic;
using Log;
using UnityEngine;
public class InputToCommand
{
    private List<KeyRecord> mInputList = new List<KeyRecord>();
    private List<KeyRecord> mNewInputList = new List<KeyRecord>();
    private double mCurrTimeStamp = 0.0;
    /// <summary>
    /// 保持一个按键的存在时间最多这么多秒，太早的就释放掉了
    /// </summary>
    private const float RecordKeepTime = 1.2f;

    private ulong mCurrFrame = 0;
    private const byte RecordKeepFrameCount = 30;

    private Character mOwner;

    public InputToCommand(Character owner)
    {
        mOwner = owner;
    }

    public void Tick()
    {
        float dt = Time.deltaTime;
        int index = 0;
        while (index < mInputList.Count)
        {
            if (mCurrFrame - mInputList[index].frame > RecordKeepFrameCount)
            //if (mCurrTimeStamp - mInputList[index].timeStamp > RecordKeepTime)
            {
                mInputList.RemoveAt(index);
            }
            else
            {
                index++;
            }
        }

        //mNewInputList.Clear();

        mCurrTimeStamp += dt;
        mCurrFrame += 1;
    }

    public void AddInput(KeyMap key)
    {
        SimpleLog.Warn("AddInput", key, mCurrFrame);
        KeyRecord keyRecord = new KeyRecord(key, mCurrTimeStamp, mCurrFrame);
        mInputList.Add(keyRecord);
    }

    public bool ActionOccur(ActionCommand actionCmd)
    {
        ulong lastFrame = mCurrFrame - Math.Min(mCurrFrame, actionCmd.validInFrameCount);
        //double lastStamp = mCurrTimeStamp - Math.Max(actionCmd.validInSecond, Time.deltaTime);
        for (int i = 0; i < actionCmd.keySequences.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < mInputList.Count; j++)
            {
                if (mInputList[j].frame >= lastFrame && mInputList[j].key == actionCmd.keySequences[i])
                //if (mInputList[j].timeStamp >= lastStamp && mInputList[j].key == actionCmd.keySequences[i])
                {
                    found = true;
                    //lastStamp = mInputList[j].timeStamp;
                    lastFrame = mInputList[j].frame;
                    break;
                }
            }
            if (found) continue;
            return false;
        }
        return true;
    }
}

[Serializable]
public enum KeyMap
{
    NoDir = 0,
    NoInput = 1,
    Back,
    Forward,
    Left,
    Right,
    DirInput,
    ButtonX,
    ButtonY,
    ButtonA,
    ButtonB,
}

public struct KeyRecord
{
    public double timeStamp;
    public ulong frame;
    public KeyMap key;
    public KeyRecord(KeyMap key, double timeStamp, ulong frame)
    {
        this.key       = key;
        this.timeStamp = timeStamp;
        this.frame     = frame;
    }
}