using System.Collections;
using System.Collections.Generic;
using System.Linq;
using ACTTools;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class HitBoxUpdateSystem
{
    private List<Character> mPlayers = new();
    private List<Character> mAllEnemies = new();
    public void Update(int frame)
    {
        var pool = GameInstance.Instance.HitBoxDataPool;
        //获取当前帧的数据
        for (int i = 0; i < mPlayers.Count; ++i)
        {
            var actionController = mPlayers[i].GetActionController();
            var playerCurrentAction = actionController.CurAction;

            int attackPhase = -1;
            for (int j = 0; j < playerCurrentAction.attackPhaseList.Length; ++j)
            {
                for (int k = 0; k < playerCurrentAction.attackPhaseList[j].FrameIndexRange.Length; k++)
                {
                    if (frame <=playerCurrentAction.attackPhaseList[j].FrameIndexRange[k].min
                    || frame > playerCurrentAction.attackPhaseList[j].FrameIndexRange[k].max)
                    {
                        continue;
                    }
                }
                attackPhase = j;
                break;
            }
            if (attackPhase == -1)
            {
                //当前帧，没有攻击框开启
                continue;
            }
            AttackBoxTurnOnInfo attackBoxInfo = playerCurrentAction.attackPhaseList[attackPhase];

            for (int e = 0; e < mAllEnemies.Count; ++e)
            {
                var enemy = mAllEnemies[e];
                var enemyActionController = enemy.GetActionController();
                var enemyplayerCurrentAction = enemyActionController.CurAction;
                // 找到当前动作对应的碰撞盒数据
                int defensePhases = -1;
                var data          = pool.GetActionHitBoxData(enemyplayerCurrentAction.mActionName);
                if (data.Count > 0)
                {
                    for (int j = 0; j < enemyplayerCurrentAction.defensePhases.Length; ++j)
                    {
                        if (frame < enemyplayerCurrentAction.defensePhases[j].FrameIndexRange.min || frame >= enemyplayerCurrentAction.defensePhases[j].FrameIndexRange.max)
                        {
                            continue;
                        }
                        defensePhases = j;
                        break;
                    }
                }
                if (defensePhases == -1)
                {
                    //当前帧，没有防御框开启
                    continue;
                }

                for (int k = 0; k < enemyplayerCurrentAction.defensePhases[defensePhases].Tags.Length; ++k)
                {
                    string tag = playerCurrentAction.defensePhases[defensePhases].Tags[k];
                    if (data.TryGetValue(tag, out var boxList))
                    {
                        //射线检测
                        for (int n = 0; n < boxList.Count; ++n)
                        {
                            if (IsHit(attackBoxInfo, mPlayers[i], boxList[n], enemy, frame))
                            {

                            }
                        }
                    }
                }
            }
        }
    }

    bool IsHit(AttackBoxTurnOnInfo attackBoxInfo, Character attacker, BoxColliderData defenseBoxInfo, Character target, int frame)
    {
        for (int i = 0; i < attackBoxInfo.FrameIndexRange.Length; ++i)
        {
            if (frame <= attackBoxInfo.FrameIndexRange[i].min || frame > attackBoxInfo.FrameIndexRange[i].max)
            {
                continue;
            }

            frame        = frame - (int)attackBoxInfo.FrameIndexRange[i].min; //frame不是从0开始，需要得到数组下标
            var rayGroup = attackBoxInfo.RayPointGroupList[i];
            for (int j = 0; j < rayGroup.Points.Length; j++)
            {
                var point = rayGroup.Points[j];
                int lastFrame = frame -1;
                if (frame < point.RayPointTransforms.Length && lastFrame >= 0 && lastFrame < point.RayPointTransforms.Length)
                {
                    var pointTrans    = point.RayPointTransforms[frame];
                    var oldPointTrans = point.RayPointTransforms[lastFrame];
                    var worldPos      = attacker.transform.localToWorldMatrix * new Vector4(pointTrans.Position.x, pointTrans.Position.y, pointTrans.Position.z, 1);
                    var worldPosOld   = attacker.transform.localToWorldMatrix * new Vector4(oldPointTrans.Position.x, oldPointTrans.Position.y, oldPointTrans.Position.z, 1);
                    if (IsHitBox(worldPos, worldPosOld, defenseBoxInfo, target.transform))
                    {
                        return true;
                    }
                }
            }
        }
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
