using UnityEditor;
using UnityEngine;

public class CunstmEmptyGameObject
{
    [MenuItem("GameObject/Create Cunstm Empty GameObject", false, 0)]
    public static void CreateGameObject()
    {
        GameObject go = new GameObject("CunstmEmptyGameObject");

        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj != null)
        {
            go.transform.parent        = selectedObj.transform;
            go.transform.localPosition = Vector3.zero;
        }
        Selection.activeGameObject = go;

        go.AddComponent<CustomBounds>();
    }
}

public class HitBoxEditorGameObject
{
    [MenuItem("GameObject/可编辑的攻击框", false, 0)]
    public static void Create()
    {
        GameObject go = new GameObject("HitBox");
        GameObject selectedObj = Selection.activeGameObject;
        if (selectedObj != null)
        {
            go.transform.parent        = selectedObj.transform;
            go.transform.localPosition = Vector3.zero;
            go.transform.localScale    = Vector3.one;
            go.transform.localRotation = Quaternion.identity;
        }
        Selection.activeGameObject = go;
        go.AddComponent<WorldTransformInfo>();
        go.AddComponent<BoxColliderGizmo>();
    }
}
