using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CZW.PostProcessing
{
    public class ScanFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Settings
        {
            public RenderPassEvent _event = RenderPassEvent.AfterRenderingTransparents;
            public Material material;
            // public Vector3 _ScanCenter;
            [Range(0, 10)] public float _FallOff;
            [Range(0, 10)] public float _ScanDistance;
            // public Texture _NoiseTexture;
            [ColorUsage(false, true)] public Color _ScanColor;
            [ColorUsage(false, true)] public Color _LineColor;

            [ColorUsage(false, true)] public Color _DotColor = Color.red;
            // public float _NoiseTextureTlling = 5;
        }

        public Settings _settings = new Settings();
        ScanRenderPass _pass;

        public override void Create()
        {
            _pass = new ScanRenderPass(_settings);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            _pass.renderPassEvent = _settings._event;
            _pass.Setup(renderer.cameraColorTarget, renderer);
            renderer.EnqueuePass(_pass);
        }
    }
}