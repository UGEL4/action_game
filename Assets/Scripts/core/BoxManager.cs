using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class BoxManager : MonoBehaviour
{
    [SerializeField]
    List<CustomBounds> boundsList;

    void OnDrawGizmos()
    {
        Transform t = transform;
        Gizmos.color = Color.red;
        foreach (var bounds in boundsList)
            Gizmos.DrawWireCube(bounds.bounds.center, bounds.bounds.size);
    }
}
