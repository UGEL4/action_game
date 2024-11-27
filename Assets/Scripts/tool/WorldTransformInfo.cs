using UnityEngine;

[ExecuteInEditMode]
public class WorldTransformInfo : MonoBehaviour
{
    public Vector3 position;
    public Vector3 rotation;
    public Vector3 scale;
    // Start is called before the first frame update
    void Start()
    {
        position = transform.position;
        rotation = transform.rotation.eulerAngles;
        scale    = transform.localScale;
    }

    void Update()
    {
        position = transform.position;
        rotation = transform.rotation.eulerAngles;
        scale    = transform.localScale;
    }
}
