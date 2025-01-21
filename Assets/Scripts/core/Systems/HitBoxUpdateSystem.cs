using System;
using System.Collections.Generic;
using ACTTools;
using Log;
using Unity.Mathematics;
using UnityEngine;

public class HitBoxUpdateSystem
{
    private List<CharacterObj> mPlayers;
    private List<CharacterObj> mAllEnemies;

    private Dictionary<CharacterObj, List<HitBoxDataPoolSystem.HitBoxData>> mCharacterCurrentActionHitBoxData = new();

    private Dictionary<CharacterObj, AttackBoxTurnOnInfo[]> mCharacterCurrentAttackBoxTurnOnInfo;

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

    public void AddPlayer(CharacterObj player)
    {
        if (!mPlayers.Contains(player))
        {
            mPlayers.Add(player);
        }
    }

    public void AddEnemy(CharacterObj enemy)
    {
        if (!mAllEnemies.Contains(enemy))
        {
            mAllEnemies.Add(enemy);
        }
    }

    public void UpdateAttackBoxTurnOnInfo(CharacterObj target, AttackBoxTurnOnInfo[] attackPhaseList)
    {
        if (!mCharacterCurrentAttackBoxTurnOnInfo.ContainsKey(target))
        {
            mCharacterCurrentAttackBoxTurnOnInfo.Add(target, attackPhaseList);
        }
        else
        {
            mCharacterCurrentAttackBoxTurnOnInfo[target] = attackPhaseList;
        }
    }

    public void OnChangeAction(CharacterObj ch, string actionName)
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
            var actionController    = player.Action;
            var playerCurrentAction = actionController.CurAction;

            // player的攻击框
            uint playerFrameIndex = actionController.CurrentFrameIndex;
            int playerAttackPhase = GetAttackPhase(playerCurrentAction, (int)playerFrameIndex);

            //检测player的攻击对每个enemy产生的碰撞, 同时也检测每个enemy的攻击对player产生的碰撞
            for (int e = 0; e < mAllEnemies.Count; ++e)
            {
                //player的攻击对enemy产生的碰撞
                var enemy                 = mAllEnemies[e];
                var enemyActionController = enemy.Action;
                var enemyCurrentAction    = enemyActionController.CurAction;
                uint enemyFrameIndex      = enemyActionController.CurrentFrameIndex;

                //DrawWireCube(mCharacterCurrentActionHitBoxData[enemy], (int)enemyFrameIndex, enemy.transform);

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
        if (action.attackPhaseList == null)
        {
            return attackPhase;
        }
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
        if (action.defensePhases == null)
        {
            return defensePhases;
        }
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

    void CheckHit(CharacterObj attacker, AttackBoxTurnOnInfo attackBoxInfo, int attackerFrameIndex, CharacterObj target, BeHitBoxTurnOnInfo defenseBoxInfo, int targetFrameIndex)
    {
        if (!mCharacterCurrentActionHitBoxData.ContainsKey(target))
        {
            return;
        }
        var attackActionController = attacker.Action;
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

    private void DrawWireCube(List<HitBoxDataPoolSystem.HitBoxData> frameDataList, int frameIndex, Transform rootTransform)
    {
        for (int n = 0; n < frameDataList.Count; n++)
        {
            if (frameIndex >= frameDataList[n].AllFrameData.Count) break;
            BoxColliderData defenseBoxInfo = frameDataList[n].AllFrameData[frameIndex];
            Vector3 wCenter                = rootTransform.TransformPoint(defenseBoxInfo.center);
            Vector3 wP                     = rootTransform.TransformPoint(defenseBoxInfo.position);
            // 计算大小的一半
            Vector3 halfSize = defenseBoxInfo.size * 0.5f;
            // Vector3 p = target.transform.TransformPoint(defenseBoxInfo.position);
            Quaternion r = Quaternion.Euler(defenseBoxInfo.rotation);

            //Debug.DrawRay(wP, wP + new Vector3(1, 1, 1), Color.blue, 1.0f);
            // Vector3 ro = r.eulerAngles;
            // Vector3 pos = defenseBoxInfo.position;
            // var boxMatrix    = Matrix4x4.TRS(pos, Quaternion.Euler(defenseBoxInfo.rotation), new Vector3(1, 1, 1));
            // var worldToLocal = (target.transform.localToWorldMatrix * boxMatrix).inverse;
            //  计算角点
            // Vector3 center = frameDataList[n].AllFrameData[index].position + frameDataList[n].AllFrameData[index].center;
            Vector3[] corners = new Vector3[8];
            corners[0]        = wCenter + r * new Vector3(halfSize.x, halfSize.y, halfSize.z);
            corners[1]        = wCenter + r * new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            corners[2]        = wCenter + r * new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            corners[3]        = wCenter + r * new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            corners[4]        = wCenter + r * new Vector3(-halfSize.x, halfSize.y, halfSize.z);
            corners[5]        = wCenter + r * new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            corners[6]        = wCenter + r * new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            corners[7]        = wCenter + r * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            // 绘制边界线
            Debug.DrawLine(corners[0], corners[1], Color.red, 0.1f);
            Debug.DrawLine(corners[0], corners[2], Color.red, 0.1f);
            Debug.DrawLine(corners[0], corners[4], Color.red, 0.1f);
            Debug.DrawLine(corners[1], corners[3], Color.red, 0.1f);
            Debug.DrawLine(corners[1], corners[5], Color.red, 0.1f);
            Debug.DrawLine(corners[2], corners[3], Color.red, 0.1f);
            Debug.DrawLine(corners[2], corners[6], Color.red, 0.1f);
            Debug.DrawLine(corners[3], corners[7], Color.red, 0.1f);
            Debug.DrawLine(corners[4], corners[5], Color.red, 0.1f);
            Debug.DrawLine(corners[4], corners[6], Color.red, 0.1f);
            Debug.DrawLine(corners[5], corners[7], Color.red, 0.1f);
            Debug.DrawLine(corners[6], corners[7], Color.red, 0.1f);
        }
    }

    bool IsHit(AttackBoxTurnOnInfo attackBoxInfo, CharacterObj attacker, BoxColliderData defenseBoxInfo, CharacterObj target, int frame, out string gropuTag)
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
                    Debug.DrawLine(worldPos, worldPosOld, Color.red, 1.0f);
                    SimpleLog.Info("frame: ", frame, lastFrame, oldPointTrans.Position);
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
        // 创建碰撞盒的局部旋转
        //Quaternion colliderRotation = Quaternion.Euler(boxInfo.rotation);
        // 将射线起点和终点转换到父物体的局部坐标系
        //Vector3 worldToParentStart = rootTransform.InverseTransformPoint(startPos);
        //Vector3 worldToParentEnd = rootTransform.InverseTransformPoint(endPos);
        // 将父物体的坐标转换到碰撞盒的局部坐标系
        //startPos = Quaternion.Inverse(colliderRotation) * (worldToParentStart - boxInfo.position);
        //endPos = Quaternion.Inverse(colliderRotation) * (worldToParentEnd - boxInfo.position);
        //var boxMatrix    = Matrix4x4.TRS(boxInfo.position, Quaternion.Euler(boxInfo.rotation), new Vector3(1, 1, 1));
        //var worldToLocal = (rootTransform.localToWorldMatrix * boxMatrix).inverse;
        // 创建平移矩阵（父节点）
        //Matrix4x4 parentTranslationMatrix = Matrix4x4.TRS(rootTransform.position, rootTransform.rotation, Vector3.one);
        //var worldToLocal = boxMatrix * parentTranslationMatrix.inverse;
        //startPos         = worldToLocal.MultiplyPoint(startPos);
        //endPos           = worldToLocal.MultiplyPoint(endPos);
        Ray ray          = new Ray(startPos, endPos - startPos);
        bool hit         = RayIntersectsAABB(rootTransform,ray, boxInfo.position, boxInfo.rotation, boxInfo.center, boxInfo.size, out float tNear, out float tFar);

        // Vector3 wCenter = rootTransform.TransformPoint(boxInfo.center);
        // // 计算AABB的最小点和最大点
        // Quaternion r = Quaternion.Euler(boxInfo.rotation);
        // Vector3 aabbMin = wCenter + r * (boxInfo.size * -0.5f); // AABB的最小点
        // Vector3 aabbMax = wCenter + r * (boxInfo.size * 0.5f); // AABB的最大点
        // Debug.DrawLine(aabbMin, aabbMax, Color.green, 1.0f);
        return hit;
    }

    bool RayIntersectsAABB(Transform rootTransform, Ray ray, Vector3 boxPos, Vector3 boxRot, Vector3 center, Vector3 size, out float tNear, out float tFar)
    {
        Vector3 worldCenter = rootTransform.TransformPoint(center);
        // 计算盒子的最小和最大边界
        Vector3 halfSize = size * 0.5f;
        //Vector3 boxMin = center - halfSize;
        //Vector3 boxMax = center + halfSize;
        Quaternion rot = Quaternion.Euler(boxRot);
        // 创建一个旋转矩阵
        //Matrix4x4 rotationMatrix = Matrix4x4.Rotate(quaternion.Euler(boxRot));
        // 计算父节点的变换
        //Matrix4x4 parentMatrix = Matrix4x4.TRS(rootTransform.position, rootTransform.rotation, rootTransform.localScale);
        // 将盒子的角点转换到世界坐标
        Vector3[] corners = new Vector3[8];
        corners[0]        = worldCenter + rot * new Vector3(halfSize.x, halfSize.y, halfSize.z);
        corners[1]        = worldCenter + rot * new Vector3(halfSize.x, halfSize.y, -halfSize.z);
        corners[2]        = worldCenter + rot * new Vector3(halfSize.x, -halfSize.y, halfSize.z);
        corners[3]        = worldCenter + rot * new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
        corners[4]        = worldCenter + rot * new Vector3(-halfSize.x, halfSize.y, halfSize.z);
        corners[5]        = worldCenter + rot * new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
        corners[6]        = worldCenter + rot * new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
        corners[7]        = worldCenter + rot * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
        // 应用旋转
        // for (int i = 0; i < corners.Length; i++)
        // {
        //     corners[i] = rotationMatrix.MultiplyPoint3x4(corners[i]);
        // }
        // 应用父节点的变换
        // for (int i = 0; i < corners.Length; i++)
        // {
        //     corners[i] = parentMatrix.MultiplyPoint3x4(corners[i]);
        // }
        Debug.DrawLine(corners[0], corners[1], Color.red, 0.1f);
        Debug.DrawLine(corners[0], corners[2], Color.red, 0.1f);
        Debug.DrawLine(corners[0], corners[4], Color.red, 0.1f);
        Debug.DrawLine(corners[1], corners[3], Color.red, 0.1f);
        Debug.DrawLine(corners[1], corners[5], Color.red, 0.1f);
        Debug.DrawLine(corners[2], corners[3], Color.red, 0.1f);
        Debug.DrawLine(corners[2], corners[6], Color.red, 0.1f);
        Debug.DrawLine(corners[3], corners[7], Color.red, 0.1f);
        Debug.DrawLine(corners[4], corners[5], Color.red, 0.1f);
        Debug.DrawLine(corners[4], corners[6], Color.red, 0.1f);
        Debug.DrawLine(corners[5], corners[7], Color.red, 0.1f);
        Debug.DrawLine(corners[6], corners[7], Color.red, 0.1f);

        // 计算包围盒
        Bounds boxBounds = new Bounds(corners[0], Vector3.zero);
        foreach (var corner in corners)
        {
            boxBounds.Encapsulate(corner);
        }
        // 检测光线与盒子的相交
        tNear = 0;
        tFar  = 0;
        return boxBounds.IntersectRay(ray);

        // 计算AABB的最小点和最大点
        /*Vector3 aabbMin = center - size * 0.5f; // AABB的最小点
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

        return tNear <= tFar;*/
    }

    bool AABBIntersectsAABB(Transform rootTransformA1, BoxColliderData A1, Transform rootTransformA2, BoxColliderData A2)
    {
        Vector3 worldCenterA1 = rootTransformA1.TransformPoint(A1.center);
        // 计算盒子的最小和最大边界
        Vector3 halfSizeA1 = A1.size * 0.5f;
        //Vector3 boxMin = center - halfSize;
        //Vector3 boxMax = center + halfSize;
        Quaternion rotA1 = Quaternion.Euler(A1.rotation);
        // 创建一个旋转矩阵
        //Matrix4x4 rotationMatrix = Matrix4x4.Rotate(quaternion.Euler(boxRot));
        // 计算父节点的变换
        //Matrix4x4 parentMatrix = Matrix4x4.TRS(rootTransform.position, rootTransform.rotation, rootTransform.localScale);
        // 将盒子的角点转换到世界坐标
        Vector3[] cornersA1 = new Vector3[8];
        cornersA1[0]        = worldCenterA1 + rotA1 * new Vector3(halfSizeA1.x, halfSizeA1.y, halfSizeA1.z);
        cornersA1[1]        = worldCenterA1 + rotA1 * new Vector3(halfSizeA1.x, halfSizeA1.y, -halfSizeA1.z);
        cornersA1[2]        = worldCenterA1 + rotA1 * new Vector3(halfSizeA1.x, -halfSizeA1.y, halfSizeA1.z);
        cornersA1[3]        = worldCenterA1 + rotA1 * new Vector3(halfSizeA1.x, -halfSizeA1.y, -halfSizeA1.z);
        cornersA1[4]        = worldCenterA1 + rotA1 * new Vector3(-halfSizeA1.x, halfSizeA1.y, halfSizeA1.z);
        cornersA1[5]        = worldCenterA1 + rotA1 * new Vector3(-halfSizeA1.x, halfSizeA1.y, -halfSizeA1.z);
        cornersA1[6]        = worldCenterA1 + rotA1 * new Vector3(-halfSizeA1.x, -halfSizeA1.y, halfSizeA1.z);
        cornersA1[7]        = worldCenterA1 + rotA1 * new Vector3(-halfSizeA1.x, -halfSizeA1.y, -halfSizeA1.z);
        // 计算包围盒
        Bounds boxBoundsA1 = new Bounds(cornersA1[0], Vector3.zero);
        foreach (var corner in cornersA1)
        {
            boxBoundsA1.Encapsulate(corner);
        }

        Vector3 worldCenterA2 = rootTransformA2.TransformPoint(A2.center);
        // 计算盒子的最小和最大边界
        Vector3 halfSizeA2 = A2.size * 0.5f;
        Quaternion rotA2 = Quaternion.Euler(A2.rotation);
        // 将盒子的角点转换到世界坐标
        Vector3[] cornersA2 = new Vector3[8];
        cornersA2[0]        = worldCenterA2 + rotA2 * new Vector3(halfSizeA2.x, halfSizeA2.y, halfSizeA2.z);
        cornersA2[1]        = worldCenterA2 + rotA2 * new Vector3(halfSizeA2.x, halfSizeA2.y, -halfSizeA2.z);
        cornersA2[2]        = worldCenterA2 + rotA2 * new Vector3(halfSizeA2.x, -halfSizeA2.y, halfSizeA2.z);
        cornersA2[3]        = worldCenterA2 + rotA2 * new Vector3(halfSizeA2.x, -halfSizeA2.y, -halfSizeA2.z);
        cornersA2[4]        = worldCenterA2 + rotA2 * new Vector3(-halfSizeA2.x, halfSizeA2.y, halfSizeA2.z);
        cornersA2[5]        = worldCenterA2 + rotA2 * new Vector3(-halfSizeA2.x, halfSizeA2.y, -halfSizeA2.z);
        cornersA2[6]        = worldCenterA2 + rotA2 * new Vector3(-halfSizeA2.x, -halfSizeA2.y, halfSizeA2.z);
        cornersA2[7]        = worldCenterA2 + rotA2 * new Vector3(-halfSizeA2.x, -halfSizeA2.y, -halfSizeA2.z);
        // 计算包围盒
        Bounds boxBoundsA2 = new Bounds(cornersA2[0], Vector3.zero);
        foreach (var corner in cornersA2)
        {
            boxBoundsA2.Encapsulate(corner);
        }

        return boxBoundsA1.Intersects(boxBoundsA2);
    }
}
