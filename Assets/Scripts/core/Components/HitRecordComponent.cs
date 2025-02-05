using System.Collections.Generic;

public class HitRecordComponent : ComponentBase
{
    public List<HitRecord> HitRecordList {get; private set;}

    public HitRecordComponent(CharacterObj owner, int priority = 0) : base(owner, priority)
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

    public override void UpdateLogic(int frameIndex)
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

    public override void EndPlay()
    {
        Clear();
        base.EndPlay();
    }
}
