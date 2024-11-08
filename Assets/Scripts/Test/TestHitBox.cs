using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestHitBox : MonoBehaviour
{
    // Start is called before the first frame update
    public BoxManager boxManager;
    void Start()
    {
        Load();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void Load()
    {
        TextAsset ta = Resources.Load<TextAsset>("GameData/test_box");
        if (ta)
        {
            Action.KeyFrameDataContainer temp = JsonUtility.FromJson<Action.KeyFrameDataContainer>(ta.text);
            foreach (var v in temp.data)
            {
                //mKeyFrames.Add(v.frame, v);
                if (v.frame == 8)
                {
                    
                    GameObject go = new GameObject("box");
                    go.transform.SetParent(transform);
                    CustomBounds b = go.AddComponent<CustomBounds>();
                    go.transform.localPosition = v.boxDataList[0].position;
                    go.transform.localRotation = v.boxDataList[0].rotation;
                    go.transform.localScale = v.boxDataList[0].scale;
                    b.bounds = v.boxDataList[0].bounds;
                    boxManager.boxObjects.Add(go);
                }
            }
        }
    }
}
