using System;

[Serializable]
public struct CancelTag
{
    public string tag;
    public int priority;
}

[Serializable]
public struct BeCanceledTag
{
    public string[] cancelTag;
    public int priority;
}
