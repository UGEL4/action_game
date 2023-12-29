using Cinemachine;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    [SerializeField] CinemachineFreeLook freeLookCam;
    [SerializeField] InputReader input;

    void Start()
    {
        input.Look += Turn;
    }

    void OnDestroy()
    {
        input.Look -= Turn;
    }

    void Update()
    {

    }

    void Turn(Vector2 xy)
    {
        float x = xy.x * Time.deltaTime * 200.0f;
        float y = xy.y * Time.deltaTime * 200.0f;
        freeLookCam.m_XAxis.m_InputAxisValue =  xy.x;
        freeLookCam.m_YAxis.m_InputAxisValue =  xy.y;
    }
}