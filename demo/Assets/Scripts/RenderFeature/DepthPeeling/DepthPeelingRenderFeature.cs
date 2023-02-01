using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace czw.DepthPeeling
{
    public class DepthPeelingRenderFeature : ScriptableRendererFeature
    {
        [System.Serializable]
        public class Setting
        {
            public RenderPassEvent _event = RenderPassEvent.AfterRenderingOpaques;
            [Range(0, 10)] public int passNumber = 5;
            public RenderQueueType renderQueueType = RenderQueueType.Transparent;
            public SortingCriteria sortingCriteria = SortingCriteria.CommonOpaque;
        }

        public Setting setting = new Setting();
        private DepthPeelingRenderPass pass;

        public override void Create()
        {
            pass = new DepthPeelingRenderPass(setting);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            pass.renderPassEvent = setting._event;
            pass.Setup(renderer.cameraColorTarget, renderer);
            renderer.EnqueuePass(pass);
        }
    }
}