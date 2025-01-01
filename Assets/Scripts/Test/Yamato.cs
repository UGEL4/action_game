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
    public GameObject Suit;
    private bool mNeedAttack = false;
    public int EndFrame = 0;

    void Start()
    {
        
    }

    // Update is called once per frame
    public void UpdateLogic(int frame)
    {
        if (mNeedAttack)
        {
            if (frame == 10)
            {
                transform.SetParent(RightHand.transform);
                transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), quaternion.identity);
                Suit.SetActive(false);
            }
            else if (frame >= EndFrame)
            {
                OnAttackEnd();
            }
        }
    }

    public void OnAttack()
    {
        mNeedAttack = true;
    }

    public void OnAttackEnd()
    {
        mNeedAttack = false;
        transform.SetParent(LeftHand.transform);
        transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), quaternion.identity);
        Suit.SetActive(true);
    }
}
