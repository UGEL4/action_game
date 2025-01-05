using Log;
using UnityEngine;

public class YamatoObj
{
    public GameObject WeaponObj;
    public Yamato MonoScript;

    public CharacterObj Owner;

    public YamatoObj(CharacterObj owner)
    {
        Owner = owner;
        //加载perfab
        var prefab = Resources.Load<GameObject>("Prefabs/Character/Weapon/wp03_000");
        WeaponObj  = GameObject.Instantiate(prefab);
        if (!WeaponObj)
        {
            SimpleLog.Error("无法实例化", prefab.name);
            return;
        }

        Transform LeftHand  = Owner.transform.Find("Model/body/root/Hip/Waist/Stomach/Chest/L_Shoulder/L_UpperArm/L_Forearm/L_Hand/L_WeaponHand");
        Transform RightHand = Owner.transform.Find("Model/body/root/Hip/Waist/Stomach/Chest/R_Shoulder/R_UpperArm/R_Forearm/R_Hand/R_WeaponHand");
        MonoScript = WeaponObj.GetComponent<Yamato>();
        if (MonoScript)
        {
            MonoScript.LeftHand  = LeftHand.gameObject;
            MonoScript.RightHand = RightHand.gameObject;
            MonoScript.transform.SetParent(LeftHand);
            MonoScript.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
            MonoScript.transform.localScale = Vector3.one;
        }
    }

    public void BeginPlay()
    {

    }

    public void UpdateLogic(int frame)
    {
        if (MonoScript)
        {
            MonoScript.UpdateLogic(frame);
        }
    }

    public void EndPlay()
    {
        WeaponObj = null;
        MonoScript = null;
        Owner = null;
    }

    public void OnActionNotify(string param)
    {
        string[] _params = param.Split(",");
        if (_params != null)
        {
            for (int i = 0; i < _params.Length; i++)
            {
                if (_params[i] == "ToRightHand")
                {
                    MonoScript.OnAttack();
                }
                else if (_params[i] == "ToLeftHand")
                {
                    MonoScript.OnAttackEnd();
                }
                else if (_params[i] == "FixedPart01")
                {
                    MonoScript.FixedPart01();
                }
                else if (_params[i] == "ResetPart01")
                {
                    MonoScript.ResetPart01();
                }
                else if (_params[i] == "RoataBlade")
                {
                    MonoScript.RoataBlade();
                }
            }
        }
    }
}