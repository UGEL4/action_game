using System;
using UnityEngine;

namespace ACTTools.RootMotionData
{
    /*[Serializable]
    public struct Keyframe
    {
        public float Time;

        public float Value;

        public float InTangent;

        public float OutTangent;

        public int TangentMode;

        public int WeightedMode;

        public float InWeight;

        public float OutWeight;
    }*/

    [Serializable]
    public struct RootMotionData
    {
        public float[] X;
        public float[] Y;
        public float[] Z;
        public float[] RX;
        public float[] RY;
        public float[] RZ;

        public static bool HasRootMotion(RootMotionData data)
        {
            return (data.X != null && data.X.Length > 0) || (data.Y != null && data.Y.Length > 0) || (data.Z != null && data.Z.Length > 0) || (data.RX != null && data.RX.Length > 0) || (data.RY != null && data.RY.Length > 0) || (data.RZ != null && data.RZ.Length > 0);
        }
    }
}
