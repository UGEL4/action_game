using UnityEngine;

public abstract class CastShapeBase : MonoBehaviour
{
#if UNITY_EDITOR
    protected readonly Color m_gizomsColor = Color.cyan;

    protected virtual void Reset()
    {
    }

    protected virtual void OnDrawGizmosSelected()
    {
    }

    protected virtual void OnDrawGizmos()
    {
        
    }
#endif
}
