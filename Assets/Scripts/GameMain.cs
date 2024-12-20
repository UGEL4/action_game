using System.Collections.Generic;
using Log;
using UnityEngine;

public class GameMain : MonoBehaviour
{
    private ulong mLogicFrameIndex;
    public List<GameObject> players = new();
    public List<GameObject> enemies = new();
    void Start()
    {
        mLogicFrameIndex = 0;
        GameInstance.Instance.Init();
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
        // }
    }

    void FixedUpdate()
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
                if (player.CanAttackTargetNow(enemy, out AttackInfo playerAttackPhase, out BeHitBoxTurnOnInfo enemyDefensePhase))
                {
                    HitRecord record = player.GetHitRecord(enemy, playerAttackPhase.AttackPhase);
                    if (record == null || (record.CooldownFrame <= 0 && record.CanHitCount > 0))
                    {
                        DoAttack(player, enemy, playerAttackPhase, "player");
                    }
                }

                if (enemy.CanAttackTargetNow(player, out AttackInfo enemyAttackPhase, out BeHitBoxTurnOnInfo playerDefensePhas))
                {
                    HitRecord record = enemy.GetHitRecord(player, enemyAttackPhase.AttackPhase);
                    if (record == null || (record.CooldownFrame <= 0 && record.CanHitCount > 0))
                    {
                        DoAttack(enemy, player,enemyAttackPhase, "enemy");
                    }
                }
            }
        } 
    }

    void DoAttack(Character attacker, Character target,AttackInfo attackPhase, string debug)
    {
        SimpleLog.Info("DoAttack: ", debug);
        //CancelTag开启
        // foreach (string cTag in attackPhase.tempBeCancelledTagTurnOn)
        // {
        //     player.GetActionController().AddTempBeCanceledTag(cTag);
        // }
        attacker.AddHitRecord(target, attackPhase.AttackPhase);
    }
}