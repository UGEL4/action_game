using System.Collections;
using System.Collections.Generic;
using Action;

public class CharacterAction
{
    // 动作名
    public string mActionName;
    // 动画名
    public string mAnimation;
    //基础优先级
    public int mPriority;
    //下一个自然动作
    public string mAutoNextActionName;
    public bool mAutoTerminate;
    //public List<ActionFrame> mActionFrames;
    public bool keepPlayingAnim;

    public CancelTag[] mCancelTagList;

    public BeCanceledTag[] mBeCanceledTagList;
    public TempBeCancelledTag[] mTempBeCanceledTagList;
    public ActionCommand[] mCommandList;

    public AttackInfo[] attackInfoList;
    public AttackBoxTurnOnInfo[] attackPhaseList;

    /// <summary>
    /// 这里是保持移动方向的倍率，根据游戏不同、精度不同所需要的参数不同
    /// 大多横版游戏这个倍率会有多个值
    /// 这个值的作用是当做这个动作的时候，我继续前进或者后退按照什么倍率来
    /// 在动作游戏中，我们放有些技能，按住前进会向前更多的距离，按住后则会向前较短距离甚至不会向前
    /// 靠的就是这个参数，他的倍率x角色移动速度小于动作本身位移速度，就会导致按住后动作前进距离变短
    /// 大多动作这个值应该都是0，而移动类则是1
    /// 值得注意的是，一个动作的acceptance是阶段性的，比如起跳动作起跳的蹲伏阶段是不能移动的，但是跳起来之后却可以
    /// 如果美术把起跳到最高点做在一个动作了，也问题不大，靠这个来做
    /// 如果策划填表填错了，2段重叠了，那么就取速度慢的那段
    /// </summary>
    public MoveInputAcceptance[] moveInputAcceptance;
}
