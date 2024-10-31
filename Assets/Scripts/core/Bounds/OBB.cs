using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class OBB
{
    public Vector3 Center { get; set; }
    public Vector3 Size;
    public Quaternion Rotation { get; set; }

    public OBB(Vector3 center, Vector3 size, Quaternion rotation)
    {
        Center   = center;
        Size     = size;
        Rotation = rotation;
    }

    public bool Contains(Vector3 point)
    {
        Vector3 localPoint = Quaternion.Inverse(Rotation) * (point - Center);
        return Mathf.Abs(localPoint.x) <= Size.x / 2 &&
               Mathf.Abs(localPoint.y) <= Size.y / 2 &&
               Mathf.Abs(localPoint.z) <= Size.z / 2;
    }

    public bool Intersects(OBB other)
    {
        // 使用 Separating Axis Theorem (SAT) 检测相交
        // 这里可以实现更复杂的相交检测逻辑
        return false; // 这里需要实现相交检测逻辑
    }
}
