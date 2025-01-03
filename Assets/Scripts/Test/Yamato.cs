using System.Collections;
using System.Collections.Generic;
using Log;
using Unity.Mathematics;
using UnityEngine;

public class Yamato : MonoBehaviour
{
    // Start is called before the first frame update
    public GameObject RightHand;
    public GameObject LeftHand;
    private bool mNeedAttack = false;
    public int StartFrame = 0;
    public int EndFrame = 0;

    public GameObject Root;
    public GameObject Part00;

    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateLogic(int frame)
    {
        if (mNeedAttack)
        {
            if (frame == StartFrame)
            {
                Part00.transform.SetParent(RightHand.transform);
                Part00.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), quaternion.identity);
                Part00.transform.localScale = Vector3.one;
            }
            else if (frame >= EndFrame)
            {
                OnAttackEnd();
            }
        }
    }

    public void OnAttack(int startFrame, int endFrame)
    {
        StartFrame = startFrame;
        EndFrame   = endFrame;
        mNeedAttack = true;
    }

    public void OnAttackEnd()
    {
        mNeedAttack = false;
        Part00.transform.SetParent(Root.transform);
        Part00.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), quaternion.identity);
        Part00.transform.localScale = Vector3.one;
    }
}
