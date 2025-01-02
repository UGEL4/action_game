using System.Collections;
using System.Collections.Generic;

public sealed class GameInstance
{
    private static readonly GameInstance _instance = new GameInstance();
    public static GameInstance Instance => _instance;
    public HitBoxDataPoolSystem HitBoxDataPool {get; private set;}
    public HitBoxUpdateSystem HitBoxUpdate {get; private set;}

    public HitRecordSystem HitRecordSys {get; private set;}

    private ulong mLogicFrameIndex;

    private List<CharacterObj> mPlayerList = new();
    private List<CharacterObj> mEnemyList = new();

    public int FrameRate = 0;
    public int LogicFrameRate = 30;
    
    private GameInstance() { }

    public void Init()
    {
        mLogicFrameIndex = 0;
        HitBoxDataPool = new HitBoxDataPoolSystem();
        HitBoxUpdate   = new HitBoxUpdateSystem();
        HitRecordSys   = new HitRecordSystem();
        HitBoxDataPool.Init();
        HitBoxUpdate.Init();
        HitRecordSys.Init();
        mPlayerList.Clear();
        mEnemyList.Clear();
    }

    public void Destory()
    {
        for (int i = 0; i < mEnemyList.Count; i++)
        {
            mEnemyList[i].Destroy();
        }
        for (int i = 0; i < mPlayerList.Count; i++)
        {
            mPlayerList[i].Destroy();
        }
        mEnemyList = null;
        mPlayerList = null;
        HitRecordSys.Destory();
        HitRecordSys = null;
        HitBoxUpdate.Destory();
        HitBoxUpdate = null;
        HitBoxDataPool.Destory();
        HitBoxDataPool = null;
    }

    public ulong GetLogicFrameIndex()
    {
        return mLogicFrameIndex;
    }

    public void SetLogicFrameIndex(ulong frameIndex)
    {
        mLogicFrameIndex = frameIndex;
    }

    public void AddPlayer(CharacterObj player)
    {
        if (!mPlayerList.Contains(player))
        {
            mPlayerList.Add(player);
        }
    }

    public void AddEnemy(CharacterObj enemy)
    {
        if (!mEnemyList.Contains(enemy))
        {
            mEnemyList.Add(enemy);
        }
    }

    public List<CharacterObj> GetPlayerList()
    {
        return mPlayerList;
    }

    public List<CharacterObj> GetEnemyList()
    {
        return mEnemyList;
    }
}
