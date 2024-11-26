using UnityEngine;

[RequireComponent(typeof(BoxCollider))]
public class BoxColliderGizmo : MonoBehaviour
{
    private void OnDrawGizmos()
    {
        // 获取 BoxCollider 组件
        BoxCollider boxCollider = GetComponent<BoxCollider>();

        // 如果 BoxCollider 存在
        if (boxCollider != null)
        {
            // 获取 BoxCollider 的中心和大小
            Vector3 center = transform.TransformPoint(boxCollider.center);
            Vector3 size = boxCollider.size;

            // 计算边框的八个顶点
            Vector3[] corners = new Vector3[8];
            corners[0] = center + transform.TransformDirection(new Vector3(-size.x, -size.y, -size.z)) * 0.5f;
            corners[1] = center + transform.TransformDirection(new Vector3(size.x, -size.y, -size.z)) * 0.5f;
            corners[2] = center + transform.TransformDirection(new Vector3(size.x, -size.y, size.z)) * 0.5f;
            corners[3] = center + transform.TransformDirection(new Vector3(-size.x, -size.y, size.z)) * 0.5f;
            corners[4] = center + transform.TransformDirection(new Vector3(-size.x, size.y, -size.z)) * 0.5f;
            corners[5] = center + transform.TransformDirection(new Vector3(size.x, size.y, -size.z)) * 0.5f;
            corners[6] = center + transform.TransformDirection(new Vector3(size.x, size.y, size.z)) * 0.5f;
            corners[7] = center + transform.TransformDirection(new Vector3(-size.x, size.y, size.z)) * 0.5f;

            // 绘制边框
            Gizmos.color = Color.green; // 设置边框颜色
            Gizmos.DrawLine(corners[0], corners[1]);
            Gizmos.DrawLine(corners[1], corners[2]);
            Gizmos.DrawLine(corners[2], corners[3]);
            Gizmos.DrawLine(corners[3], corners[0]);
            Gizmos.DrawLine(corners[4], corners[5]);
            Gizmos.DrawLine(corners[5], corners[6]);
            Gizmos.DrawLine(corners[6], corners[7]);
            Gizmos.DrawLine(corners[7], corners[4]);
            Gizmos.DrawLine(corners[0], corners[4]);
            Gizmos.DrawLine(corners[1], corners[5]);
            Gizmos.DrawLine(corners[2], corners[6]);
            Gizmos.DrawLine(corners[3], corners[7]);
        }
    }
}
