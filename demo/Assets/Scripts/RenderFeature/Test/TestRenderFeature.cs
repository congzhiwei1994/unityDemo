using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

namespace Test.RenderFeature
{
    public class TestRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Setting
        {
            public Color color;
            public Material material;
            public RenderPassEvent _event = RenderPassEvent.AfterRenderingOpaques;
            public string globalName_ShaderTex = "_TestRenderFeatureTex";
            public string materialName_mainColor = "_MinColor";
        }

        public Setting _setting = new Setting();
        private TestRenderPass0 _pass0;
        private TestRenderPass1 _pass1;
        private TestRenderPass2 _pass2;

        public override void Create()
        {
            // _pass0 = new TestRenderPass0(_setting);
            _pass1 = new TestRenderPass1(_setting);
            _pass2 = new TestRenderPass2(_setting);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_setting.material == null)
                return;

            // _pass0.renderPassEvent = _setting._event;
            // renderer.EnqueuePass(_pass0);

            // _pass1.renderPassEvent = _setting._event;
            // renderer.EnqueuePass(_pass1);
            
            _pass2.Setup(renderer.cameraColorTarget, renderer);
            _pass2.renderPassEvent = _setting._event;
            renderer.EnqueuePass(_pass2);
        }
    }
}