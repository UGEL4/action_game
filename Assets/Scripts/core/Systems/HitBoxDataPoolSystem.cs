using System;
using System.Collections;
using System.Collections.Generic;
using ACTTools;
using UnityEngine;
public class HitBoxDataPoolSystem
{
    //<action name, <bone name, frame data>>
    private Dictionary<string, Dictionary<string, List<BoxColliderData>>> mActionHitBoxDataMap;

    public Dictionary<string, List<BoxColliderData>> GetActionHitBoxData(string actionName)
    {
        if (mActionHitBoxDataMap.TryGetValue(actionName, out Dictionary<string, List<BoxColliderData>> data))
        {
            return data;
        }
        return new Dictionary<string, List<BoxColliderData>>();
    }
}
