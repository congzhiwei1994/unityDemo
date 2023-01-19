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
            public bool DebugPass1 = false;
        }

        public Setting _setting = new Setting();
        private TestRenderPass0 _pass0;
        private TestRenderPass1 _pass1;

        public override void Create()
        {
            if (_setting.DebugPass1)
                _pass1 = new TestRenderPass1(_setting);
            else
                _pass0 = new TestRenderPass0(_setting);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            if (_setting.material == null)
                return;

            if (_setting.DebugPass1)
            {
                _pass1.renderPassEvent = _setting._event;
                renderer.EnqueuePass(_pass1);
            }
            else
            {
                _pass0.renderPassEvent = _setting._event;
                renderer.EnqueuePass(_pass0);
            }
        }
    }
}