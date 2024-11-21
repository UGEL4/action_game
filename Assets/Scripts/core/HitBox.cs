using System;
using System.Collections.Generic;
using UnityEngine;

namespace Action
{
    public class HitBox
    {
    }

    public class RectBox
    {
        Bounds bounds;
    }

    [Serializable]
    public struct BoxData
    {
        // transform
        public Vector3 position;
        public Quaternion rotation;
        public Vector3 scale;
        public Vector3 worldPosition;
        public Quaternion worldRotation;
        public Vector3 localScale;
        public OBB bounds;
    }

    [Serializable]
    public struct KeyFrameData
    {
        public int frame;
        public List<BoxData> boxDataList;
    }

    [Serializable]
    public struct KeyFrameDataContainer
    {
        public List<KeyFrameData> data;
    }
}