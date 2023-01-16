using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace czw.SSSS
{
    public class SeperableSubsurfaceScatteringFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Setting
        {
            public RenderPassEvent _event = RenderPassEvent.BeforeRenderingOpaques;
            public bool DebugSkin = false;
            public bool DisableSH = false;
            [Range(0, 5)] public float SubsurfaceScaler = 0.25f;
            public Color SSS_Color = new Color(0.48f, 0.41f, 0.28f, 1f);
            public Color SSSFall0ff_Color = new Color(1.0f, 0.37f, 0.3f, 1f);
            [Range(0, 100)] public float MaxDistance;
            public string shaderTagID = "SkinSSSS";
        }

        private SeperableSubsurfaceScatteringPass _pass;
        public Setting _setting = new Setting();

        public override void Create()
        {
            _pass = new SeperableSubsurfaceScatteringPass(_setting);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _pass.renderPassEvent = _setting._event;
            renderer.EnqueuePass(_pass);
        }
    }
}