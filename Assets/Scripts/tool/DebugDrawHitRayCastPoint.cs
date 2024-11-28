using UnityEngine;

[ExecuteInEditMode]
public class DebugDrawHitRayCastPoint : MonoBehaviour
{
    private Vector3 lastPoint;
    public Transform endPoint;

    void Start()
    {
        lastPoint = transform.position;
    }

    void Update()
    {
        if (endPoint != null)
        {
            Debug.DrawLine(transform.position, endPoint.position, Color.red, 1.0f);
        }
        Debug.DrawLine(transform.position, lastPoint, Color.blue, 1.0f);
        lastPoint = transform.position;
    }
}
