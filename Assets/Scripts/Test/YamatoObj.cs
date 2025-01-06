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
        }
    }

    public void BeginPlay()
    {

        var model = Owner.transform.Find("Model");
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
        // string[] _params = param.Split(",");
        // if (_params != null)
        // {
        //     for (int i = 0; i < _params.Length; i++)
        //     {
        //         if (_params[i] == "ToRightHand")
        //         {
        //             MonoScript.OnAttack();
        //         }
        //         else if (_params[i] == "ToLeftHand")
        //         {
        //             MonoScript.OnAttackEnd();
        //         }
        //         else if (_params[i] == "FixedPart01")
        //         {
        //             MonoScript.FixedPart01();
        //         }
        //         else if (_params[i] == "ResetPart01")
        //         {
        //             MonoScript.ResetPart01();
        //         }
        //         else if (_params[i] == "RoataBlade")
        //         {
        //             MonoScript.RoataBlade();
        //         }
        //     }
        // }
    }

    public void ActionNotify(string functionNamem, string[] _params)
    {
        if (functionNamem == "ToRightHand")
        {
            MonoScript.OnAttack();
        }
        else if (functionNamem == "ToLeftHand")
        {
            MonoScript.OnAttackEnd();
        }
        else if (functionNamem == "FixedPart01")
        {
            Vector3 position = Vector3.zero;
            Vector3 rotation = Vector3.zero;
            if (_params.Length > 0)
            {
                string[] args = _params[0].Split(',');
                if (args.Length < 3)
                {
                    SimpleLog.Error("YamatoObj.ActionNotify error => FixedPart01(Vector3 position, Vector3 rotation): position args.Length < 3");
                }
                else
                {
                    position = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
                }
            }
            if (_params.Length > 1)
            {
                string[] args = _params[1].Split(',');
                if (args.Length < 3)
                {
                    SimpleLog.Error("YamatoObj.ActionNotify error => FixedPart01(Vector3 position, Vector3 rotation): rotation args.Length < 3");
                }
                else
                {
                    rotation = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
                }
            }
            MonoScript.FixedPart01(position, rotation);
        }
        else if (functionNamem == "FixedPart00")
        {
            Vector3 position = Vector3.zero;
            Vector3 rotation = Vector3.zero;
            if (_params.Length > 0)
            {
                string[] args = _params[0].Split(',');
                if (args.Length < 3)
                {
                    SimpleLog.Error("YamatoObj.ActionNotify error => FixedPart00(Vector3 position, Vector3 rotation): position args.Length < 3");
                }
                else
                {
                    position = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
                }
            }
            if (_params.Length > 1)
            {
                string[] args = _params[1].Split(',');
                if (args.Length < 3)
                {
                    SimpleLog.Error("YamatoObj.ActionNotify error => FixedPart00(Vector3 position, Vector3 rotation): rotation args.Length < 3");
                }
                else
                {
                    rotation = new Vector3(float.Parse(args[0]), float.Parse(args[1]), float.Parse(args[2]));
                }
            }
            MonoScript.FixedPart00(position, rotation);
        }
        else if (functionNamem == "ResetPart01")
        {
            MonoScript.ResetPart01();
        }
    }
}