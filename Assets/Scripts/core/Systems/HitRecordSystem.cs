using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class HitRecordSystem
{
    public List<HitRecordComponent> HitRecordList { get; private set; } = new();

    public void Init()
    {
        HitRecordList.Clear();
    }

    public void Destory()
    {
        HitRecordList.Clear();
        HitRecordList = null;
    }

    public void Register(HitRecordComponent record)
    {
        if (!HitRecordList.Contains(record))
        {
            HitRecordList.Add(record);
        }
    }

    public void Update(ulong frameIndex)
    {
        var components = HitRecordList;
        for (int i = 0; i < components.Count; i++)
        {
            components[i].Update(frameIndex);
        }
    }
}
