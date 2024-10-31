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
