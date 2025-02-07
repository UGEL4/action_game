using System;

[Serializable]
public class ComponentBase
{
    protected CharacterObj mOwner;

    public CharacterObj Owner => mOwner;

    public int mPriority = 0;

    protected System.Type mType = typeof(ComponentBase);

    public ComponentBase(CharacterObj owner, int priority = 0, System.Type type = null)
    {
        mOwner    = owner;
        mPriority = priority;
        mType     = type;
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

    public System.Type GetComponentType()
    {
        return mType;
    }
}