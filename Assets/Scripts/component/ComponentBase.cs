using System;

[Serializable]
public class ComponentBase
{
    protected CharacterObj mOwner;

    public CharacterObj Owner => mOwner;

    public ComponentBase(CharacterObj owner)
    {
        mOwner = owner;
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