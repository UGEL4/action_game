using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class ActionEdit : MonoBehaviour
{
    [SerializeField] string FileName = "Default";

    [SerializeField] List<CharacterAction> Actions = new List<CharacterAction>();

    //[Button]
    public void Save()
    {
        if (Actions.Count == 0) return;
        StringBuilder json = new StringBuilder("{\"data\":[");
        for (int i = 0; i < Actions.Count; i++)
        {
            json.Append(JsonUtility.ToJson(Actions[i]));
            if (i != Actions.Count - 1) json.Append(",");
        }
        json.Append("]}");
        if (!Directory.Exists(Application.dataPath + "/Resources/GameData"))
        {
            Directory.CreateDirectory(Application.dataPath + "/Resources/GameData");
        }
        string path = Application.dataPath + "/Resources/GameData/" + FileName + ".json";
        File.WriteAllText(path, json.ToString());
    }
}
