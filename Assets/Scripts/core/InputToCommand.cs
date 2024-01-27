using System;
using System.Collections.Generic;
using UnityEngine;
public class InputToCommand : MonoBehaviour
{
    private List<KeyRecord> mInputList = new List<KeyRecord>();
    private List<KeyRecord> mNewInputList = new List<KeyRecord>();
    private double mCurrTimeStamp = 0.0;
    /// <summary>
    /// 保持一个按键的存在时间最多这么多秒，太早的就释放掉了
    /// </summary>
    private const float RecordKeepTime = 1.2f;

    [SerializeField] InputReader Input;
    public Character Owner { set; private get; }

    void Start()
    {
        if (Input != null)
        {
            Input.Move += OnAxisInput;
        }
    }

    void Update()
    {
        float dt = Time.deltaTime;
        int index = 0;
        /* while (index < mInputList.Count)
        {
            if (mCurrTimeStamp - mInputList[index].timeStamp > RecordKeepTime)
            {
                mInputList.RemoveAt(index);
            }
            else
            {
                index++;
            }
        } */

        //mNewInputList.Clear();

        mCurrTimeStamp += dt;
    }

    void OnAxisInput(Vector2 value)
    {
        float deadArea = 0.2f;
        bool xHasInput = Mathf.Abs(value.x) >= deadArea;
        bool yHasInput = Mathf.Abs(value.y) >= deadArea;
        if (xHasInput || yHasInput)
        {
            AddDirInput(value);
        }
    }

    void AddInput(KeyMap key, Vector2 dir)
    {
        KeyRecord keyRecord = new KeyRecord(key, mCurrTimeStamp, dir);
        mInputList.Add(keyRecord);
    }

    void AddDirInput(Vector2 value)
    {
        Vector3 ownerForward = Owner.GetForward();
        Vector3 ownerRight   = Owner.GetRight();
        Vector3 cameraForward = Owner.GetCameraForward();
        Debug.Log("朝向");
        Debug.Log(ownerForward);
        Debug.Log(ownerRight);
        Debug.Log(cameraForward);
        Vector3 tmpDir       = new Vector3(value.x, 0.0f, value.y);
        float dotF           = Vector3.Dot(tmpDir, ownerForward);
        float dotR           = Vector3.Dot(tmpDir, ownerRight);
        Debug.Log("输入");
        Debug.Log(value);
        Debug.Log(dotR);
        Debug.Log(dotR);
        if (dotF > 0.0f)
        {
            AddInput(KeyMap.Forward, value);
        }
        else
        {
            AddInput(KeyMap.Back, value);
        }
        if (dotR > 0.0f)
        {
            AddInput(KeyMap.Right, value);
        }
        else
        {
            AddInput(KeyMap.Left, value);
        }
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
    Right
}

public struct KeyRecord
{
    public double timeStamp;
    public KeyMap key;
    public Vector2 dir;
    public KeyRecord(KeyMap key, double timeStamp, Vector2 dir)
    {
        this.key       = key;
        this.timeStamp = timeStamp;
        this.dir       = dir;
    }
}