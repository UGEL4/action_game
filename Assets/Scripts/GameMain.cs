using System.Collections.Generic;
using Log;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private ulong mLogicFrameIndex;
    public List<CharacterObj> players = new();
    public List<CharacterObj> enemies = new();
    public int DebugRunFrameRate = 60;

    private float mNextUpdateTime = 0f;

    private long mRenderFrameIndex = 0;

    private float mFrameInterval = 0f;
    private float mFrameTimer = 0f;

    void Start()
    {
        mNextUpdateTime = 0f;
        mFrameTimer     = 0f;
        Application.targetFrameRate = DebugRunFrameRate;
        mLogicFrameIndex = 0;
        mRenderFrameIndex = 0;
        GameInstance.Instance.Init();
        GameInstance.Instance.FrameRate = DebugRunFrameRate;

        mFrameInterval =  1f / GameInstance.Instance.LogicFrameRate;

        //todo
        CharacterObj player = new MainPlayerCharacterObj();
        players.Add(player);

        for (int i = 0; i < players.Count; i++)
        {
            CharacterObj ch = players[i];
            if (ch != null)
            {
                GameInstance.Instance.AddPlayer(ch);
                GameInstance.Instance.HitBoxUpdate.AddPlayer(ch);
                ch.Init();
                ch.LoadActions();
                ch.BeginPlay();
            }
        }

        CharacterObj enemy = new TestPlayerCharacterObj();
        enemies.Add(enemy);
        for (int i = 0; i < enemies.Count; i++)
        {
            CharacterObj ch = enemies[i];
            if (ch != null)
            {
                GameInstance.Instance.AddEnemy(ch);
                GameInstance.Instance.HitBoxUpdate.AddEnemy(ch);
                ch.Init();
                ch.LoadActions();
                ch.BeginPlay();
            }
        }
    }

    void OnDestroy()
    {
        GameInstance.Instance.Destory();
        players.Clear();
        enemies.Clear();
    }

    void Update()
    {
        mFrameTimer += Time.deltaTime;
        if (mFrameTimer >= mFrameInterval)
        {
            mFrameTimer -= mFrameInterval;
            UpdateLogic();
            //SimpleLog.Warn("Update Logic: ", t, Time.time, mNextUpdateTime, mLogicFrameIndex, mRenderFrameIndex);
        }
        UpdateRender();
        //SimpleLog.Warn("Update Render: ", mFrameTimer, Time.time, mLogicFrameIndex, mRenderFrameIndex);
    }

    void UpdateRender()
    {
        mRenderFrameIndex++;
        List<CharacterObj> playerList = GameInstance.Instance.GetPlayerList();
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].UpdateRender(Time.deltaTime);
        }

        List<CharacterObj> enemyList = GameInstance.Instance.GetEnemyList();
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].UpdateRender(Time.deltaTime);
        }
    }

    void UpdateLogic()
    {
        mLogicFrameIndex++;
        GameInstance.Instance.SetLogicFrameIndex(mLogicFrameIndex);
        GameInstance.Instance.HitBoxUpdate.Update(mLogicFrameIndex);
        GameInstance.Instance.HitRecordSys.Update(mLogicFrameIndex);

        DealWithAttacks();

        List<CharacterObj> playerList = GameInstance.Instance.GetPlayerList();
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].UpdateLogic((int)mLogicFrameIndex);
        }

        List<CharacterObj> enemyList = GameInstance.Instance.GetEnemyList();
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].UpdateLogic((int)mLogicFrameIndex);
        }
    }

    void DealWithAttacks()
    {
        List<CharacterObj> playerList = GameInstance.Instance.GetPlayerList();
        List<CharacterObj> enemyList  = GameInstance.Instance.GetEnemyList();
        for (int i = 0; i < enemyList.Count; i++)
        {
            CharacterObj enemy = enemyList[i];
            for (int j = 0; j < playerList.Count; j++)
            {
                CharacterObj player = playerList[j];
                if (player.CanAttackTargetNow(enemy, out AttackInfo playerAttackInfo, out BeHitBoxTurnOnInfo enemyDefensePhase))
                {
                    HitRecord record = player.GetHitRecord(enemy, playerAttackInfo.AttackPhase);
                    if (record == null || (record.CooldownFrame <= 0 && record.CanHitCount > 0))
                    {
                        DoAttack(player, enemy, playerAttackInfo, enemyDefensePhase, "player");
                    }
                }

                if (enemy.CanAttackTargetNow(player, out AttackInfo enemyAttackInfo, out BeHitBoxTurnOnInfo playerDefensePhas))
                {
                    HitRecord record = enemy.GetHitRecord(player, enemyAttackInfo.AttackPhase);
                    if (record == null || (record.CooldownFrame <= 0 && record.CanHitCount > 0))
                    {
                        DoAttack(enemy, player,enemyAttackInfo, playerDefensePhas, "enemy");
                    }
                }
            }
        } 
    }

    void DoAttack(CharacterObj attacker, CharacterObj target, AttackInfo attackInfo, BeHitBoxTurnOnInfo defensePhase, string debug)
    {
        SimpleLog.Info("DoAttack: ", debug);
        //CancelTag开启
        // foreach (string cTag in attackPhase.tempBeCancelledTagTurnOn)
        // {
        //     player.GetActionController().AddTempBeCanceledTag(cTag);
        // }
        ActionChangeInfo attackerChangeInfo = 
        attackInfo.SelfActionChangeInfo.priority >= defensePhase.TargetActionChangeInfo.priority 
        ? attackInfo.SelfActionChangeInfo : defensePhase.TargetActionChangeInfo;
        attacker.Action.PreorderActionByActionChangeInfo(attackerChangeInfo);

        ActionChangeInfo defenderChangeInfo = 
        attackInfo.TargetActionChangeInfo.priority > defensePhase.SelfActionChangeInfo.priority 
        ? attackInfo.TargetActionChangeInfo : defensePhase.SelfActionChangeInfo;
        target.Action.PreorderActionByActionChangeInfo(defenderChangeInfo);

        target.SetForceMove(new MoveInfo {
            InFrame      = attackInfo.PushPower.InFrame,
            moveDistance = attackInfo.PushPower.moveDistance,
            tweenMethod  = attackInfo.PushPower.tweenMethod
        });

        attacker.AddHitRecord(target, attackInfo.AttackPhase);
    }
}