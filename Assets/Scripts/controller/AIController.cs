public class AIController : IController
{
    private CharacterObj mOwner;

    public void SetOwner(CharacterObj owner)
    {
        mOwner = owner;
    }
}