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
    public int mOrder;
    //下一个自然动作
    public CharacterAction mNextAction;
    //public List<ActionFrame> mActionFrames;
}
