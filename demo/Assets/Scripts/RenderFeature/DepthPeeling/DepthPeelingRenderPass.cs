using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace czw.DepthPeeling
{
    public class DepthPeelingRenderPass : ScriptableRenderPass
    {
        private DepthPeelingRenderFeature.Setting setting;
        private ShaderTagId shaderTag;
        private ProfilingSampler profilingSampler;
        private FilteringSettings filteringSettings;

        public DepthPeelingRenderPass(DepthPeelingRenderFeature.Setting setting)
        {
            this.setting = setting;
            Init();
        }

        private void Init()
        {
            RenderQueueRange renderQueueRange = (setting.renderQueueType == RenderQueueType.Opaque)
                ? RenderQueueRange.opaque
                : RenderQueueRange.transparent;
            shaderTag = new ShaderTagId(setting.passTags);

            filteringSettings = new FilteringSettings(renderQueueRange);
            profilingSampler = new ProfilingSampler(setting.passTags + setting.renderQueueType);
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var drawSetting = CreateDrawingSettings(shaderTag, ref renderingData, setting.sortingCriteria);

            var cmd = CommandBufferPool.Get("Depth Peeling");
            using (new ProfilingScope(cmd, profilingSampler))
            {
                // Start profilling
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();

                List<int> colorRTs = new List<int>(setting.passNumber);
                List<int> depthRTs = new List<int>(setting.passNumber);
            }
        }
    }
}