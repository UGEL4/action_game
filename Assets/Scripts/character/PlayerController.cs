using Cinemachine;
using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] CinemachineFreeLook freeLookCam;
    [SerializeField] InputReader input;
    // Start is called before the first frame update

    private Vector3 mCurMoveDir = new Vector3();

    Transform mainCamera;
    void Awake()
    {
        controller  = GetComponent<CharacterController>();
        mainCamera  = Camera.main.transform;
        //freeLookCam.Follow = transform;
        //freeLookCam.LookAt = transform;
        //freeLookCam.OnTargetObjectWarped(transform, transform.position - freeLookCam.transform.position - Vector3.forward);

        //input
        InitializeInput();
    }

    void InitializeInput()
    {
        input.Move += OnMove;
    }

    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        //var moveDir = new Vector3(input.Direction.x, 0.0f, input.Direction.y).normalized;
        var adjDir  = Quaternion.AngleAxis(mainCamera.eulerAngles.y, Vector3.up) * mCurMoveDir;
        if (adjDir.magnitude > 0.0f)
        {
            var targetRot = Quaternion.LookRotation(adjDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 2.0f * Time.deltaTime);
            transform.LookAt(transform.position + adjDir);
            HandleRotation();

            controller.Move(adjDir * 10.0f * Time.deltaTime);
        }
    }

    void HandleRotation()
    {
        
    }

    void OnMove(Vector2 xy)
    {
        mCurMoveDir.x = xy.x;
        mCurMoveDir.z = xy.y;
        mCurMoveDir.y = 0.0f;
        mCurMoveDir.Normalize();
    }

    public bool IsMoving()
    {
        return mCurMoveDir.x != 0.0f || mCurMoveDir.z != 0.0f;
    }
}
