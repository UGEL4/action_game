using System;
using UnityEngine;

[Serializable]
[ExecuteInEditMode]
public class CustomBounds : MonoBehaviour
{
    [SerializeField]
    public OBB bounds = new OBB();

    #if UNITY_EDITOR
    public Vector3 position;
    #endif

    void Awake()
    {
        // Vector3 center = transform.position;
        // Vector3 size = new Vector3(1, 1, 1);
        // Quaternion rotation = transform.rotation;
        // bounds = new OBB(center, size, rotation);
    }

    void Update()
    {
        Vector3 center      = transform.position;
        Quaternion rotation = transform.rotation;
        bounds.Center       = center;
        bounds.Rotation     = rotation;

        #if UNITY_EDITOR
        position = transform.position;
        #endif
    }

#if UNITY_EDITOR
    void OnDrawGizmos()
    {
        //if (bounds != null)
        {
            Gizmos.color = Color.red;

            // 计算 OBB 的八个角点
            Vector3[] corners = new Vector3[8];
            Vector3 halfSize  = bounds.Size / 2;

            corners[0] = bounds.Center + bounds.Rotation * new Vector3(-halfSize.x, -halfSize.y, -halfSize.z);
            corners[1] = bounds.Center + bounds.Rotation * new Vector3(halfSize.x, -halfSize.y, -halfSize.z);
            corners[2] = bounds.Center + bounds.Rotation * new Vector3(halfSize.x, halfSize.y, -halfSize.z);
            corners[3] = bounds.Center + bounds.Rotation * new Vector3(-halfSize.x, halfSize.y, -halfSize.z);
            corners[4] = bounds.Center + bounds.Rotation * new Vector3(-halfSize.x, -halfSize.y, halfSize.z);
            corners[5] = bounds.Center + bounds.Rotation * new Vector3(halfSize.x, -halfSize.y, halfSize.z);
            corners[6] = bounds.Center + bounds.Rotation * new Vector3(halfSize.x, halfSize.y, halfSize.z);
            corners[7] = bounds.Center + bounds.Rotation * new Vector3(-halfSize.x, halfSize.y, halfSize.z);

            // 绘制 OBB 的边界
            for (int i = 0; i < 4; i++)
            {
                Gizmos.DrawLine(corners[i], corners[(i + 1) % 4]);         // 底面
                Gizmos.DrawLine(corners[i + 4], corners[(i + 1) % 4 + 4]); // 顶面
                Gizmos.DrawLine(corners[i], corners[i + 4]);               // 连接上下
            }
        }
    }
#endif
}
