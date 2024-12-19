using System.Collections.Generic;

public class HitRecordComponent
{
    public List<HitRecord> HitRecordList {get; private set;}

    public HitRecordComponent()
    {
        HitRecordList = new List<HitRecord>();
    }

    public void AddHitRecord(HitRecord hitRecord)
    {
        if (!HitRecordList.Contains(hitRecord))
        {
            HitRecordList.Add(hitRecord);
        }
    }

    public void Update(ulong frameIndex)
    {
        var hitRecord = HitRecordList;
        for (int i = 0; i < hitRecord.Count; i++)
        {
            hitRecord[i].Update();
        }
    }

    public void Clear()
    {
        HitRecordList.Clear();
    }
}
