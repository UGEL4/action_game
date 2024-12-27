using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.SocialPlatforms;

public class MovementComponent : MonoBehaviour
{
    [SerializeField] PlayerController playerController;
    [SerializeField] CharacterController controller;
    [SerializeField] Character owner;
    public GameObject RenderObj;
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

    float GetPositionLerpT()
    {
        int renderFrameRate = GameInstance.Instance.FrameRate;
        int logicFrameRate  = GameInstance.Instance.LogicFrameRate;
        float t             = (float)logicFrameRate / renderFrameRate;
        //float t = 1f / renderFrameRate / (1f / logicFrameRate);
        t = Mathf.Clamp01(t);
        return t;
    }

    void HandleMovement()
    {
        if (!RenderObj) return;
        RenderObj.transform.position = Vector3.Lerp(RenderObj.transform.position, transform.position, GetPositionLerpT());
        //controller.Move(motion);
        float MoveInputAcceptance = owner.GetMoveInputAcceptance();
        if (MoveInputAcceptance <= 0.0) return;
        //var moveDir = new Vector3(input.Direction.x, 0.0f, input.Direction.y).normalized;
        var adjDir  = Quaternion.AngleAxis(mCameraTransform.eulerAngles.y, Vector3.up) * playerController.CurrMoveDir;
        if (adjDir.magnitude > 0.0f)
        {
            var targetRot = Quaternion.LookRotation(adjDir);
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, 2.0f * Time.deltaTime);
            transform.LookAt(transform.position + adjDir);
            //HandleRotation();

            Vector3 moveDir = adjDir * speed * Time.deltaTime * MoveInputAcceptance;
            controller.Move(moveDir);
        }
    }

    /////////////////////////////logic
    private int mSpeed = 1; //每帧位移0.2
    private float mRenderUnit = 0.2f;

    private Vector3 mPosition = Vector3.zero;

    //逻辑单位到渲染单位的转换
    public float LogicUnitToRenderUnit(int unit)
    {
        return unit * mRenderUnit;
    }

    public Vector3 NatureMove()
    {
        var adjDir  = Quaternion.AngleAxis(mCameraTransform.eulerAngles.y, Vector3.up) * playerController.CurrMoveDir;
        if (adjDir.magnitude > 0.0f)
        {
            adjDir.Normalize();
            float MoveInputAcceptance = owner.GetMoveInputAcceptance();
            Vector3 motion = adjDir * LogicUnitToRenderUnit(mSpeed) /** MoveInputAcceptance*/;
            //Vector3 motion = adjDir * 6 * Time.fixedDeltaTime /** MoveInputAcceptance*/;
            //mPosition = mPosition + motion;
            controller.Move(motion);
            // if (RenderObj)
            //     RenderObj.transform.position = Vector3.Lerp(RenderObj.transform.position, transform.position, GetPositionLerpT());
        }
        return new Vector3(1, 1, 1);
    }
    /////////////////////////////logic
}
