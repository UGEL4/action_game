using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovementComponent : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] CharacterController controller;
    public float speed = 10;

    // Update is called once per frame
    private Transform mCameraTransform;
    void Start()
    {
        mCameraTransform = Camera.main.transform;
    }
    void Update()
    {
        HandleMovement();
    }

    void HandleMovement()
    {
        //var moveDir = new Vector3(input.Direction.x, 0.0f, input.Direction.y).normalized;
        var adjDir  = Quaternion.AngleAxis(mCameraTransform.eulerAngles.y, Vector3.up) * playerController.CurrMoveDir;
        if (adjDir.magnitude > 0.0f)
        {
            var targetRot = Quaternion.LookRotation(adjDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 2.0f * Time.deltaTime);
            transform.LookAt(transform.position + adjDir);
            //HandleRotation();

            Vector3 moveDir = adjDir * speed * Time.deltaTime;
            controller.Move(moveDir);
        }
    }
}
