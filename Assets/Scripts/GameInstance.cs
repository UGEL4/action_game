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

    private List<Character> mPlayerList = new();
    private List<Character> mEnemyList = new();
    
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
        mEnemyList.Clear();
        mEnemyList = null;
        mPlayerList.Clear();
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

    public void AddPlayer(Character player)
    {
        if (!mPlayerList.Contains(player))
        {
            mPlayerList.Add(player);
        }
    }

    public void AddEnemy(Character enemy)
    {
        if (!mEnemyList.Contains(enemy))
        {
            mEnemyList.Add(enemy);
        }
    }

    public List<Character> GetPlayerList()
    {
        return mPlayerList;
    }

    public List<Character> GetEnemyList()
    {
        return mEnemyList;
    }
}
