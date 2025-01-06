using Unity.Mathematics;
using UnityEngine;

public class Yamato : MonoBehaviour
{
    public GameObject RightHand;
    public GameObject LeftHand;
    public GameObject Root;
    public GameObject Part00;
    public GameObject Part01;

    private Vector3 mOriginalPos;
    private Quaternion mOriginalRot;

    void Start()
    {
        mOriginalPos = Part01.transform.localPosition;
        mOriginalRot = Part01.transform.localRotation;
        //Part00       = transform.Find("root/_00").gameObject;
        //Part01       = transform.Find("root/_01").gameObject;
    }

    public void UpdateLogic(int frame)
    {

    }

    public void OnAttack()
    {
        Part00.transform.SetParent(RightHand.transform);
        Part00.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), quaternion.identity);
        Part00.transform.localScale = Vector3.one;
    }

    public void OnAttackEnd()
    {
        Part00.transform.SetParent(Root.transform);
        Part00.transform.SetLocalPositionAndRotation(new Vector3(0, 0, 0), quaternion.identity);
        Part00.transform.localScale = Vector3.one;
    }

    public void FixedPart01(Vector3 position, Vector3 rotation)
    {
        Part01.transform.localPosition = position;
        Part01.transform.localRotation = Quaternion.Euler(rotation);
    }

    public void FixedPart00(Vector3 position, Vector3 rotation)
    {
        Part00.transform.localPosition = position;
        Part00.transform.localRotation = Quaternion.Euler(rotation);
    }

    public void ResetPart01()
    {
        Part01.transform.localPosition = mOriginalPos;
        Part01.transform.localRotation = mOriginalRot;
    }

    public void RoataBlade()
    {
        Part00.transform.localRotation = Quaternion.Euler(0, 70, 180);;
    }
}
