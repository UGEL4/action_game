using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CastBox : CastShapeBase
{
    public Vector3 center;
    public Vector3 size = Vector3.one;

#if UNITY_EDITOR

    public bool AlwaysDraw = false;
    protected override void Reset()
    {
        center = Vector3.zero;
        size   = Vector3.one;
    }

    protected override void OnDrawGizmosSelected()
    {
        if (AlwaysDraw)
        {
            return;
        }
        Draw();
    }
    
    protected override void OnDrawGizmos()
    {
        if (AlwaysDraw)
        {
            Draw();
        }
    }

    void Draw()
    {
        Matrix4x4 gizmosMatrixRecord = Gizmos.matrix;
        Color gizmosColorRecord      = Gizmos.color;
        Gizmos.color                 = m_gizomsColor;
        Gizmos.matrix                = transform.localToWorldMatrix;
        Gizmos.DrawWireCube(center, size);
        Gizmos.color  = gizmosColorRecord;
        Gizmos.matrix = gizmosMatrixRecord;
    }
#endif
}
