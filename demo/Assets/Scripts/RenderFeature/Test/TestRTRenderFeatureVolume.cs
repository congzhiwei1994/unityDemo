using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;


namespace Jefford
{
    [Serializable]
    public class TestRTRenderFeatureValue : IEquatable<TestRTRenderFeatureValue>
    {
        public bool m_isEnable;
        public Material m_material;
        [Range(1,6)]
        public int m_downSample;

        public bool Equals(TestRTRenderFeatureValue other)
        {
            return this.m_material == other.m_material && this.m_isEnable == other.m_isEnable &&
                   this.m_downSample == other.m_downSample;
        }
    }

    [Serializable]
    public class TestRTParams : VolumeParameter<TestRTRenderFeatureValue>
    {
        public TestRTParams()
        {
            value = new TestRTRenderFeatureValue()
            {
                m_material = null,
                m_downSample = 1,
                m_isEnable = false
            };
        }

        public TestRTParams(VolumeParameter value, bool overideState = false) : base(default, false)
        {
        }
    }

    // 菜单
    [VolumeComponentMenu("Jefford/TestRTRenderFeature")]
    public class TestRTRenderFeatureVolume : VolumeComponent, IPostProcessComponent
    {
        public TestRTParams _testRTParams = new TestRTParams();
        public bool IsActive()
        {
            return false;
        }

        public bool IsTileCompatible()
        {
            return false;
        }
    }
}