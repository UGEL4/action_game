using System;
using System.Collections.Generic;
using ACTTools;
using UnityEngine;

public class HitBoxUpdateSystem
{
    private List<Character> mPlayers;
    private List<Character> mAllEnemies;

    private Dictionary<Character, List<HitBoxDataPoolSystem.HitBoxData>> mCharacterCurrentActionHitBoxData = new();

    public void Init()
    {
        mPlayers    = new();
        mAllEnemies = new();
    }

    public void Destory()
    {
        mAllEnemies.Clear();
        mAllEnemies = null;
        mPlayers.Clear();
        mPlayers = null;
    }

    public void AddPlayer(Character player)
    {
        if (!mPlayers.Contains(player))
        {
            mPlayers.Add(player);
        }
    }

    public void AddEnemy(Character enemy)
    {
        if (!mAllEnemies.Contains(enemy))
        {
            mAllEnemies.Add(enemy);
        }
    }

    public void OnChangeAction(Character ch, string actionName)
    {
        var hitBoxDataMap = GameInstance.Instance.HitBoxDataPool.GetActionHitBoxData(actionName);
        if (!mCharacterCurrentActionHitBoxData.ContainsKey(ch))
        {
            mCharacterCurrentActionHitBoxData.Add(ch, new List<HitBoxDataPoolSystem.HitBoxData>());
        }
        else
        {
            // 清理上一个动作的数据
            mCharacterCurrentActionHitBoxData[ch].Clear();
        }
        foreach (var pair in hitBoxDataMap)
        {
            HitBoxDataPoolSystem.HitBoxData data = new();
            data.BoxName                         = pair.Key;
            data.AllFrameData                    = pair.Value;
            mCharacterCurrentActionHitBoxData[ch].Add(data);
        }
    }

    public void Update(ulong frame)
    {
        // 获取当前帧的数据
        for (int i = 0; i < mPlayers.Count; ++i)
        {
            var player              = mPlayers[i];
            var actionController    = player.GetActionController();
            var playerCurrentAction = actionController.CurAction;

            // player的攻击框
            uint playerFrameIndex = actionController.CurrentFrameIndex;
            int playerAttackPhase = GetAttackPhase(playerCurrentAction, (int)playerFrameIndex);

            //检测player的攻击对每个enemy产生的碰撞, 同时也检测每个enemy的攻击对player产生的碰撞
            for (int e = 0; e < mAllEnemies.Count; ++e)
            {
                //player的攻击对enemy产生的碰撞
                var enemy                 = mAllEnemies[e];
                var enemyActionController = enemy.GetActionController();
                var enemyCurrentAction    = enemyActionController.CurAction;
                uint enemyFrameIndex      = enemyActionController.CurrentFrameIndex;
                if (playerAttackPhase != -1)
                {
                    //找到当前动作对应的碰撞盒数据
                    int enemyDefensePhases = GetDefensePhase(enemyCurrentAction, (int)enemyFrameIndex);
                    if (enemyDefensePhases != -1)
                    {
                        CheckHit(player, playerCurrentAction.attackPhaseList[playerAttackPhase], (int)playerFrameIndex,
                                 enemy, enemyCurrentAction.defensePhases[enemyDefensePhases], (int)enemyFrameIndex);
                    }
                }

                //enemy的攻击对player产生的碰撞
                int playerDefensePhase = GetDefensePhase(playerCurrentAction, (int)playerFrameIndex);
                if (playerDefensePhase == -1)
                {
                    // 当前帧，没有防御框开启
                    continue;
                }

                int enemyAttackPhase = GetAttackPhase(enemyCurrentAction, (int)enemyFrameIndex);
                if (enemyAttackPhase == -1)
                {
                    // 当前帧，没有攻击框开启
                    continue;
                }
                CheckHit(enemy, enemyCurrentAction.attackPhaseList[enemyAttackPhase], (int)enemyFrameIndex, player, playerCurrentAction.defensePhases[playerDefensePhase], (int)playerFrameIndex);
            }
        }
    }

    int GetAttackPhase(CharacterAction action, int frameIndex)
    {
        int attackPhase = -1;
        for (int j = 0; j < action.attackPhaseList.Length; j++)
        {
            for (int k = 0; k < action.attackPhaseList[j].FrameIndexRange.Length; k++)
            {
                if (frameIndex <= action.attackPhaseList[j].FrameIndexRange[k].min || frameIndex > action.attackPhaseList[j].FrameIndexRange[k].max)
                {
                    continue;
                }
                attackPhase = j;
                break;
            }
            if (attackPhase != -1)
            {
                break;
            }
        }
        return attackPhase;
    }

    int GetDefensePhase(CharacterAction action, int frameIndex)
    {
        int defensePhases = -1;
        for (int j = 0; j < action.defensePhases.Length; ++j)
        {
            if (frameIndex < action.defensePhases[j].FrameIndexRange.min || frameIndex > action.defensePhases[j].FrameIndexRange.max)
            {
                continue;
            }
            defensePhases = j;
            break;
        }
        return defensePhases;
    }

    void CheckHit(Character attacker, AttackBoxTurnOnInfo attackBoxInfo, int attackerFrameIndex, Character target, BeHitBoxTurnOnInfo defenseBoxInfo, int targetFrameIndex)
    {
        if (!mCharacterCurrentActionHitBoxData.ContainsKey(target))
        {
            return;
        }
        var attackActionController = attacker.GetActionController();
        var frameDataList          = mCharacterCurrentActionHitBoxData[target];
        for (int k = 0; k < defenseBoxInfo.Tags.Length; ++k)
        {
            string tag = defenseBoxInfo.Tags[k];
            for (int n = 0; n < frameDataList.Count; n++)
            {
                if (tag == frameDataList[n].BoxName)
                {
                    int index = (int)(targetFrameIndex - defenseBoxInfo.FrameIndexRange.min);
                    if (index < 0)
                    {
                        break;
                    }
                    if (index >= frameDataList[n].AllFrameData.Count)
                    {
                        index = frameDataList[n].AllFrameData.Count - 1;
                    }
                    if (IsHit(attackBoxInfo, attacker, frameDataList[n].AllFrameData[index], target, attackerFrameIndex, out string gropuTag))
                    {
                        attackActionController.OnAttackBoxHit(gropuTag, defenseBoxInfo);
                    }
                    break;
                }
            }
        }
    }

    bool IsHit(AttackBoxTurnOnInfo attackBoxInfo, Character attacker, BoxColliderData defenseBoxInfo, Character target, int frame, out string gropuTag)
    {
        for (int i = 0; i < attackBoxInfo.FrameIndexRange.Length; ++i)
        {
            if (frame <= attackBoxInfo.FrameIndexRange[i].min || frame > attackBoxInfo.FrameIndexRange[i].max)
            {
                continue;
            }

            frame        -= (int)attackBoxInfo.FrameIndexRange[i].min; // frame不是从0开始，需要得到数组下标
            var rayGroup = attackBoxInfo.RayPointGroupList[i];
            for (int j = 0; j < rayGroup.Points.Length; j++)
            {
                var point     = rayGroup.Points[j];
                int lastFrame = frame - 1;
                if (frame < point.RayPointTransforms.Length && lastFrame >= 0 && lastFrame < point.RayPointTransforms.Length)
                {
                    var pointTrans    = point.RayPointTransforms[frame];
                    var oldPointTrans = point.RayPointTransforms[lastFrame];
                    var worldPos      = attacker.transform.localToWorldMatrix * new Vector4(pointTrans.Position.x, pointTrans.Position.y, pointTrans.Position.z, 1);
                    var worldPosOld   = attacker.transform.localToWorldMatrix * new Vector4(oldPointTrans.Position.x, oldPointTrans.Position.y, oldPointTrans.Position.z, 1);
                    if (IsHitBox(worldPos, worldPosOld, defenseBoxInfo, target.transform))
                    {
                        gropuTag = rayGroup.Tag;
                        return true;
                    }
                }
            }
        }
        gropuTag = string.Empty;
        return false;
    }

    bool IsHitBox(Vector3 startPos, Vector3 endPos, BoxColliderData boxInfo, Transform rootTransform)
    {
        var boxMatrix    = Matrix4x4.TRS(boxInfo.position, Quaternion.Euler(boxInfo.rotation), new Vector3(1, 1, 1));
        var worldToLocal = (rootTransform.localToWorldMatrix * boxMatrix).inverse;
        startPos         = worldToLocal.MultiplyPoint3x4(startPos);
        endPos           = worldToLocal.MultiplyPoint3x4(endPos);
        Ray ray          = new Ray(startPos, endPos - startPos);
        bool hit         = RayIntersectsAABB(ray, boxInfo.center, boxInfo.size, out float tNear, out float tFar);
        return hit;
    }

    bool RayIntersectsAABB(Ray ray, Vector3 center, Vector3 size, out float tNear, out float tFar)
    {
        // 计算AABB的最小点和最大点
        Vector3 aabbMin = center - size * 0.5f; // AABB的最小点
        Vector3 aabbMax = center + size * 0.5f; // AABB的最大点

        tNear = float.NegativeInfinity;
        tFar  = float.PositiveInfinity;

        for (int i = 0; i < 3; i++)
        {
            float invD = 1.0f / (i == 0 ? ray.direction.x : (i == 1 ? ray.direction.y : ray.direction.z));
            float t0   = (i == 0 ? aabbMin.x : (i == 1 ? aabbMin.y : aabbMin.z) - (i == 0 ? ray.origin.x : (i == 1 ? ray.origin.y : ray.origin.z))) * invD;
            float t1   = (i == 0 ? aabbMax.x : (i == 1 ? aabbMax.y : aabbMax.z) - (i == 0 ? ray.origin.x : (i == 1 ? ray.origin.y : ray.origin.z))) * invD;

            if (invD < 0.0f)
            {
                // 交换 t0 和 t1
                float temp = t0;
                t0         = t1;
                t1         = temp;
            }

            tNear = Mathf.Max(tNear, t0);
            tFar  = Mathf.Min(tFar, t1);

            if (tNear > tFar)
                return false;
        }

        return tNear <= tFar;
    }
}
