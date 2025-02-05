using System;

[Serializable]
public class ComponentBase
{
    protected CharacterObj mOwner;

    public CharacterObj Owner => mOwner;

    public int mPriority = 0;

    public ComponentBase(CharacterObj owner,int priority = 0)
    {
        mOwner = owner;
        mPriority = priority;
    }

    public virtual void UpdateLogic(int frameIndex)
    {

    }

    public virtual void UpdateRender(float deltaTime)
    {

    }

    public virtual void BeginPlay()
    {

    }

    public virtual void EndPlay()
    {
        mOwner = null;
    }
}