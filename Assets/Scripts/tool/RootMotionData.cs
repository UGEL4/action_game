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
        public Keyframe[] X;
        public Keyframe[] Y;
        public Keyframe[] Z;
    }
}
