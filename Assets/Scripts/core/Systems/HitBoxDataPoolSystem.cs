using System;
using System.Collections;
using System.Collections.Generic;
using ACTAction;
using ACTTools;
using UnityEngine;
public class HitBoxDataPoolSystem
{
    public struct HitBoxData
    {
        public string BoxName;
        public List<BoxColliderData> AllFrameData;
    }
    //<action name, <bone name, frame data>>
    private Dictionary<string, Dictionary<string, List<BoxColliderData>>> mActionHitBoxDataMap;

    public void Init()
    {
        mActionHitBoxDataMap = new ();
    }

    public void Destory()
    {
        mActionHitBoxDataMap.Clear();
        mActionHitBoxDataMap = null;
    }

    public Dictionary<string, List<BoxColliderData>> GetActionHitBoxData(string actionName)
    {
        if (mActionHitBoxDataMap.TryGetValue(actionName, out Dictionary<string, List<BoxColliderData>> data))
        {
            return data;
        }
        return new Dictionary<string, List<BoxColliderData>>();
    }

    private struct HitBoxContiner
    {
        public CharacterBoneBox[] data;
    }

    public void LoadHitBoxData(string actionName)
    {
        if (!mActionHitBoxDataMap.ContainsKey(actionName))
        {
            mActionHitBoxDataMap.Add(actionName, new Dictionary<string, List<BoxColliderData>>());
        }
        var hitBoxMap = mActionHitBoxDataMap[actionName];
        TextAsset ta  = Resources.Load<TextAsset>("GameData/ActionBoneColliderAnimation/" + actionName);
        if (ta)
        {
            HitBoxContiner aic = JsonUtility.FromJson<HitBoxContiner>(ta.text);
            foreach (CharacterBoneBox info in aic.data)
            {
                if (!hitBoxMap.ContainsKey(info.BoxName))
                {
                    hitBoxMap.Add(info.BoxName, new List<BoxColliderData>());
                }
                List<BoxColliderData> dataList = hitBoxMap[info.BoxName];
                for (int i = 0; i < info.FrameData.Length; i++)
                {
                    dataList.Add(info.FrameData[i]);
                }
            }
        }
    }
}
