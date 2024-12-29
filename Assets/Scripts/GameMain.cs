using System.Collections.Generic;
using Log;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private ulong mLogicFrameIndex;
    public List<GameObject> players = new();
    public List<GameObject> enemies = new();
    public int DebugRunFrameRate = 60;

    private float mNextUpdateTime = 0f;
    void Start()
    {
        mNextUpdateTime = 0f;
        Application.targetFrameRate = DebugRunFrameRate;
        mLogicFrameIndex = 0;
        GameInstance.Instance.Init();
        GameInstance.Instance.FrameRate = DebugRunFrameRate;
        for (int i = 0; i < players.Count; i++)
        {
            Character ch = players[i].GetComponent<Character>();
            if (ch != null)
            {
                GameInstance.Instance.AddPlayer(ch);
                GameInstance.Instance.HitBoxUpdate.AddPlayer(ch);
                ch.Init();
                ch.LoadActions();
            }
        }
        for (int i = 0; i < enemies.Count; i++)
        {
            Character ch = enemies[i].GetComponent<Character>();
            if (ch != null)
            {
                GameInstance.Instance.AddEnemy(ch);
                GameInstance.Instance.HitBoxUpdate.AddEnemy(ch);
                ch.Init();
                ch.LoadActions();
            }
        }
    }

    void OnDestroy()
    {
        GameInstance.Instance.Destory();
    }

    void Update()
    {
        // if (player.CanAttackTargetNow(out AttackInfo attackPhase))
        // {
        //     DoAttack(attackPhase);
        // }0.033 0.0083
        if (Time.time >= mNextUpdateTime)
        {
            mNextUpdateTime = Time.time + 0.033f;
            UpdateLogic();
        }
        UpdateRender();
    }

    void UpdateRender()
    {
        List<Character> playerList = GameInstance.Instance.GetPlayerList();
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].UpdateRender();
        }

        List<Character> enemyList = GameInstance.Instance.GetEnemyList();
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].UpdateRender();
        }
    }

    void UpdateLogic()
    {
        mLogicFrameIndex++;
        GameInstance.Instance.SetLogicFrameIndex(mLogicFrameIndex);
        GameInstance.Instance.HitBoxUpdate.Update(mLogicFrameIndex);
        GameInstance.Instance.HitRecordSys.Update(mLogicFrameIndex);

        DealWithAttacks();

        List<Character> playerList = GameInstance.Instance.GetPlayerList();
        for (int i = 0; i < playerList.Count; i++)
        {
            playerList[i].UpdateLogic(mLogicFrameIndex);
        }

        List<Character> enemyList = GameInstance.Instance.GetEnemyList();
        for (int i = 0; i < enemyList.Count; i++)
        {
            enemyList[i].UpdateLogic(mLogicFrameIndex);
        }
    }

    void DealWithAttacks()
    {
        List<Character> playerList = GameInstance.Instance.GetPlayerList();
        List<Character> enemyList  = GameInstance.Instance.GetEnemyList();
        for (int i = 0; i < enemyList.Count; i++)
        {
            Character enemy = enemyList[i];
            for (int j = 0; j < playerList.Count; j++)
            {
                Character player = playerList[j];
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

    void DoAttack(Character attacker, Character target, AttackInfo attackInfo, BeHitBoxTurnOnInfo defensePhase, string debug)
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
        attacker.GetActionController().PreorderActionByActionChangeInfo(attackerChangeInfo);

        ActionChangeInfo defenderChangeInfo = 
        attackInfo.TargetActionChangeInfo.priority > defensePhase.SelfActionChangeInfo.priority 
        ? attackInfo.TargetActionChangeInfo : defensePhase.SelfActionChangeInfo;
        target.GetActionController().PreorderActionByActionChangeInfo(defenderChangeInfo);

        attacker.AddHitRecord(target, attackInfo.AttackPhase);
    }
}