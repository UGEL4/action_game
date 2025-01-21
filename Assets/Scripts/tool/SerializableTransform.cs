using System;
using UnityEngine;

namespace ACTTools
{
    [Serializable]
    public struct SerializableTransformNoScale
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }

    [Serializable]
    public struct PositionRotationData
    {
        public Vector3 Position;
        public Vector3 Rotation;
    }

    [Serializable]
    public class BoxColliderData
    {
        public Vector3 position;
        public Vector3 rotation;
        public Vector3 center;
        public Vector3 size;
    }

    [Serializable]
    public class ACTTransform
    {
        public Vector3 Position;
        public Vector3 Scale;
        public Quaternion Rotation;
    }
}
