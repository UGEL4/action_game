using System.Collections;
using System.Collections.Generic;

public sealed class GameInstance
{
    private static readonly GameInstance _instance = new GameInstance();
    public static GameInstance Instance => _instance;
    public HitBoxDataPoolSystem HitBoxDataPool {get; private set;}
    public HitBoxUpdateSystem HitBoxUpdate {get; private set;}

    private GameInstance() { }

    public void Init()
    {
        HitBoxDataPool = new HitBoxDataPoolSystem();
        HitBoxUpdate   = new HitBoxUpdateSystem();
    }
}
