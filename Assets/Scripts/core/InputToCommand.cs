using System;
using System.Collections.Generic;
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
            if (mCurrTimeStamp - mInputList[index].timeStamp > RecordKeepTime)
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
    }

    public void AddInput(KeyMap key)
    {
        KeyRecord keyRecord = new KeyRecord(key, mCurrTimeStamp);
        mInputList.Add(keyRecord);
    }

    public bool ActionOccur(ActionCommand actionCmd)
    {
        double lastStamp = mCurrTimeStamp - Math.Max(actionCmd.validInSecond, Time.deltaTime);
        for (int i = 0; i < actionCmd.keySequences.Length; i++)
        {
            bool found = false;
            for (int j = 0; j < mInputList.Count; j++)
            {
                if (mInputList[j].timeStamp >= lastStamp && mInputList[j].key == actionCmd.keySequences[i])
                {
                    found = true;
                    lastStamp = mInputList[j].timeStamp;
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
}

public struct KeyRecord
{
    public double timeStamp;
    public KeyMap key;
    public KeyRecord(KeyMap key, double timeStamp)
    {
        this.key       = key;
        this.timeStamp = timeStamp;
    }
}