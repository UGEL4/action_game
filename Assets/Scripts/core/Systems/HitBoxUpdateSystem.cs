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
                if (frame <= playerCurrentAction.attackPhaseList[j].FrameIndexRange.min || frame > playerCurrentAction.attackPhaseList[j].FrameIndexRange.max)
                {
                    continue;
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
                    }
                }
            }
        }
    }

    void Hit(AttackBoxTurnOnInfo attackBoxInfo, Character attacker, BoxColliderData defenseBoxInfo, Character target, int frame)
    {
        for (int i = 0; i < attackBoxInfo.RayPointDataList.Length; ++i)
        {
            var point = attackBoxInfo.RayPointDataList[i];
            int lastFrame = frame - 1;
            if (frame < point.RayPointTransforms.Length && lastFrame >= 0 && lastFrame < point.RayPointTransforms.Length)
            {
                var pointTrans = point.RayPointTransforms[frame];
                var oldPointTrans = point.RayPointTransforms[lastFrame];
                var worldPos = attacker.transform.localToWorldMatrix * new Vector4(pointTrans.Position.x, pointTrans.Position.y, pointTrans.Position.z, 1);
                var worldPosOld = attacker.transform.localToWorldMatrix * new Vector4(oldPointTrans.Position.x, oldPointTrans.Position.y, oldPointTrans.Position.z, 1);

                Ray ray = new Ray(worldPos, worldPosOld - worldPos);
                //Matrix4x4 localMatrix = Matrix4x4.TRS(defenseBoxInfo.position, Quaternion.Euler(defenseBoxInfo.rotation), new Vector3(1, 1, 1));
                GetWorldBoxColliderData(defenseBoxInfo, target.transform, out Vector3[] boxWorldPos);
            }
        }
    }

    void GetWorldBoxColliderData(BoxColliderData localData, Transform rootTransform, 
        out Vector3[] worldPoint)
    {
        // // 将本地旋转转换为四元数
        // Quaternion localQuaternion = Quaternion.Euler(localData.rotation);
        
        // // 计算世界位置
        // Vector3 localCenter = localData.position + localData.center; // 本地中心位置
        // worldPosition = parentTransform.TransformPoint(localCenter);
        
        // // 计算世界旋转
        // worldRotation = parentTransform.rotation * localQuaternion;
        
        // // 计算世界大小（如果需要考虑缩放的话）
        // worldSize = Vector3.Scale(localData.size, parentTransform.lossyScale);

        var boxMatrix        = Matrix4x4.TRS(localData.position, Quaternion.Euler(localData.rotation), new Vector3(1, 1, 1));
        var rootLocalToWorld = rootTransform.localToWorldMatrix;
        var localToWorld     = rootLocalToWorld * boxMatrix;
        worldPoint           = new Vector3[8];
        //下面4个点
        worldPoint[0] = localToWorld * (localData.center + new Vector3(-localData.size.x, -localData.size.y, -localData.size.z) * 0.5f);
        worldPoint[1] = localToWorld * (localData.center + new Vector3(localData.size.x, -localData.size.y, -localData.size.z) * 0.5f);
        worldPoint[2] = localToWorld * (localData.center + new Vector3(localData.size.x, -localData.size.y, localData.size.z) * 0.5f);
        worldPoint[3] = localToWorld * (localData.center + new Vector3(-localData.size.x, -localData.size.y, localData.size.z) * 0.5f);
        //上面4个点
        worldPoint[4] = localToWorld * (localData.center + new Vector3(-localData.size.x, localData.size.y, -localData.size.z) * 0.5f);
        worldPoint[5] = localToWorld * (localData.center + new Vector3(localData.size.x, localData.size.y, -localData.size.z) * 0.5f);
        worldPoint[6] = localToWorld * (localData.center + new Vector3(localData.size.x, localData.size.y, localData.size.z) * 0.5f);
        worldPoint[7] = localToWorld * (localData.center + new Vector3(-localData.size.x, localData.size.y, localData.size.z) * 0.5f);
    }

    void GetWorldBoxColliderData(BoxColliderData localData, Transform rootTransform, out BoxColliderData worldData)
    {
        worldData = new BoxColliderData();
        var boxMatrix        = Matrix4x4.TRS(localData.position, Quaternion.Euler(localData.rotation), new Vector3(1, 1, 1));
        var rootLocalToWorld = rootTransform.localToWorldMatrix;
        var localToWorld     = rootLocalToWorld * boxMatrix;
        worldData.center = localToWorld * localData.center;//world center
    }
}
