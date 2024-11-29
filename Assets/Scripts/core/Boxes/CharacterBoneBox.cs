using System;
using ACTTools;

namespace ACTAction
{
    [Serializable]
    public struct CharacterBoneBox
    {
        public string BoxName;
        public BoxColliderData[] FrameData;
    }
}