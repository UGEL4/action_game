using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttachToObject : MonoBehaviour
{
    public GameObject objectA;   // 需要挂载的对象
    public GameObject objectB;   // 挂载到的目标对象
    public string childName;      // 对象 A 中要作为挂点的子节点名称

    void Start()
    {
        // 找到要作为挂点的子节点
        Transform childTransform = objectA.transform.Find(childName);
        if (childTransform != null)
        {
            Vector3 offset = objectA.transform.InverseTransformPoint(childTransform.position);
            // 将对象 A 的父级设置为对象 B
            objectA.transform.SetParent(objectB.transform);

            Vector3 scale = objectA.transform.localScale;
            objectA.transform.localPosition = new Vector3(-offset.x * scale.x, -offset.y * scale.y, -offset.z * scale.z);

            //or
            // 将对象 A 的父级设置为对象 B
            // objectA.transform.SetParent(objectB.transform);
            // 设置对象 A 的位置为对象 B 的位置
            // objectA.transform.position = objectB.transform.position;
            
            // 直接设置 objectA 的局部位置，相对子节点位置
            // Vector3 scaleA = objectA.transform.localScale;
            // Vector3 localPos = new Vector3(
            //     -childTransform.localPosition.x * scaleA.x,
            //     -childTransform.localPosition.y * scaleA.y,
            //     -childTransform.localPosition.z * scaleA.z
            // );
            // objectA.transform.localPosition = localPos;
        }
        else
        {
            Debug.LogError("未找到子节点: " + childName);
        }
    }
}
