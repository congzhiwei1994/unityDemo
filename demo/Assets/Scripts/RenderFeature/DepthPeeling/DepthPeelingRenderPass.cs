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

        List<int> colorRTs;
        List<int> depthRTs;

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


            colorRTs = new List<int>(setting.passNumber);
            depthRTs = new List<int>(setting.passNumber);
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
            }
        }

        /// <summary>
        /// 从前往后画 半透明颜色RT 
        /// </summary>
        private void DrawFrontToBack(CommandBuffer cmd, int width, int height)
        {
            for (int i = 0; i < setting.passNumber; i++)
            {
                colorRTs.Add(Shader.PropertyToID($"_DepthPeelingColor{i}"));
                depthRTs.Add(Shader.PropertyToID($"_DepthPeelingDepth{i}"));

                cmd.GetTemporaryRT(colorRTs[i], width, height, 0);
                cmd.GetTemporaryRT(depthRTs[i], width, height, 32, FilterMode.Point, RenderTextureFormat.RFloat);

                // 只传大于第一层的深度
                if (i > 0)
                    cmd.SetGlobalTexture(setting.shaderDepthTexName, depthRTs[i - 1]);

                cmd.SetRenderTarget(new RenderTargetIdentifier[] { colorRTs[i], depthRTs[i] }, depthRTs[i]);
            }
        }
    }
}