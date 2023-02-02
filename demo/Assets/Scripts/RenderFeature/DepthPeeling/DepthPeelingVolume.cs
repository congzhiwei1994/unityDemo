using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace czw.DepthPeeling
{
    [Serializable]
    public class DepthPeelingValue : IEquatable<DepthPeelingValue>
    {
        [Range(1, 10)] public int passCount;
   
        public bool Equals(DepthPeelingValue other)
        {
            return this.passCount == other.passCount;
        }
    }

    [Serializable]
    public class DepthPeelingParams : VolumeParameter<DepthPeelingValue>
    {
        public DepthPeelingParams()
        {
            value = new DepthPeelingValue()
            {
                passCount = 1,
            };
        }

        public DepthPeelingParams(VolumeParameter value, bool overideState = false) : base(default, false)
        {
        }
    }

    [VolumeComponentMenu("Czw/Charater/DepthPeeling Hair")]
    public class DepthPeelingVolume : VolumeComponent, IPostProcessComponent
    {
        public DepthPeelingParams Params = new DepthPeelingParams();

        public bool IsActive()
        {
            return true;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}