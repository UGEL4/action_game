using System;
using System.Collections;
using System.Collections.Generic;
using ACTTools;
using Unity.Mathematics;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Playables;

[Serializable]
public struct TestBoxFrameDataContainer
{
    public Wrapper[] data;
    [Serializable]
    public struct Wrapper
    {
        public string name;
        public BoxColliderData[] frameData;
    }
}
public class TestBoxFrameData : MonoBehaviour
{
    private int mframeCount = 0;
    private TestBoxFrameDataContainer mData;
    private List<GameObject> mGameObjects = new();

    public bool animated = false;
    
    // Start is called before the first frame update
    void Start()
    {
        mframeCount = 0;
        ReadJsonData();
    }

    void Update()
    {
        
    }

    void FixedUpdate()
    {
        if (animated) SampleFrameData();
    }

    void SampleFrameData()
    {
        for (int i = 0; i < mData.data.Length; i++)
        {
            BoxColliderData[] list = mData.data[i].frameData;
            //Matrix4x4 rt = Matrix4x4.TRS(list[mframeCount].position, list[mframeCount].rotation, new Vector3(1, 1, 1));
            Vector3 pos    = list[mframeCount].position;
            Quaternion rot = Quaternion.Euler(list[mframeCount].rotation);
            if (mframeCount > 0)
            {
                Vector3 lastPos    = list[mframeCount - 1].position;
                Quaternion lastRot = Quaternion.Euler(list[mframeCount - 1].rotation);

                pos = (pos + lastPos) * 0.5f;
                rot = math.slerp(rot, lastRot, 0.5f);

                //Matrix4x4 lastRT  = Matrix4x4.TRS(list[mframeCount - 1].position, list[mframeCount - 1].rotation, new Vector3(1, 1, 1));
            }
            //Matrix4x4 m = Matrix4x4.TRS(pos, rot, new Vector3(1, 1, 1));
            mGameObjects[i].transform.SetLocalPositionAndRotation(pos, rot);
            //mGameObjects[i].transform.SetLocalPositionAndRotation(list[mframeCount].position, list[mframeCount].rotation);
        }
        mframeCount++;
        if (mframeCount >= mData.data[0].frameData.Length)
        {
            mframeCount = 0;
        }
    }

    void ReadJsonData()
    {
        TextAsset ta = Resources.Load<TextAsset>("GameData/test_box_colliders");
        if (ta)
        {
            mData = JsonUtility.FromJson<TestBoxFrameDataContainer>(ta.text);
            foreach (var v in mData.data)
            {
                GameObject go = new GameObject(v.name);
                go.transform.SetParent(transform);
                go.transform.SetLocalPositionAndRotation(v.frameData[0].position, Quaternion.Euler(v.frameData[0].rotation));
                BoxCollider box = go.AddComponent<BoxCollider>();
                box.center      = v.frameData[0].center;
                box.size        = v.frameData[0].size;
                mGameObjects.Add(go);
            }
        }
    }
}
