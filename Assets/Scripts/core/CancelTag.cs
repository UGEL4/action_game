using System;

[Serializable]
public struct CancelTag
{
    public string tag;
    public int priority;

    /// <summary>
    /// 这个动作会从normalized多少的地方开始播放
    /// </summary>
    public float startFromPercentage;
    
    /// <summary>
    /// 动画融合进来的百分比时间长度
    /// </summary>
    public float fadeInPercentage;
}

[Serializable]
public struct BeCanceledTag
{
    public string[] cancelTag;

    /// <summary>
    /// 动画融合出去的时间
    /// Unity推荐用normalized作为一个标尺，因为用second对于做动画本身有点要求
    /// 当然也可能是我对CrossFadeInFixedTime理解有误
    /// </summary>
    public float fadeOutPercentage;

    public int priority;

    public PercentageRange range;

    public static BeCanceledTag FromTemp(TempBeCancelledTag tag, float fromPercentage) => new BeCanceledTag
    {
        cancelTag = tag.cancelTag,
        priority  = tag.increasePriority,
        range     = new PercentageRange(fromPercentage, fromPercentage + tag.percentage)
    };
}

[Serializable]
public struct TempBeCancelledTag
{
    /// <summary>
    /// 因为需要被索引，所以需要一个id
    /// </summary>
    public string id;
    
    /// <summary>
    /// 在当前动作中，有百分之多少的时间是开启的
    /// 从开启的时间往后算
    /// </summary>
    public float percentage;
    
    /// <summary>
    /// 可以Cancel的CancelTag
    /// </summary>
    public string[] cancelTag;

    /// <summary>
    /// 动画融合出去的时间
    /// Unity推荐用normalized作为一个标尺，因为用second对于做动画本身有点要求
    /// 当然也可能是我对CrossFadeInFixedTime理解有误
    /// </summary>
    public float fadeOutPercentage;
    
    /// <summary>
    /// 当从这里被Cancel，动作会增加多少优先级
    /// </summary>
    public int increasePriority;
}