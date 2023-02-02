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

        private static int ID_DepthPeelingPassCount;

        private string shaderName = "czw/Character/Hair/DepthPeelingBlend";

        private string blendDepthTexName = "_DepthTex";
        private string maxDepthTexName = "_MaxDepthTex";
        private string passTag = "DepthPeelingPass";
        private string passCountName = "_DepthPeelingPassCount";
        private Material blendMat;
        private ScriptableRenderer renderer;
        private RenderTargetIdentifier source;

        public DepthPeelingRenderPass(DepthPeelingRenderFeature.Setting setting)
        {
            this.setting = setting;
            Init();
        }

        public void Setup(RenderTargetIdentifier source, ScriptableRenderer renderer)
        {
            this.source = source;
            this.renderer = renderer;
        }

        private void Init()
        {
            RenderQueueRange renderQueueRange = (setting.renderQueueType == RenderQueueType.Opaque)
                ? RenderQueueRange.opaque
                : RenderQueueRange.transparent;

            shaderTag = new ShaderTagId(passTag);

            filteringSettings = new FilteringSettings(renderQueueRange);
            profilingSampler = new ProfilingSampler(passTag + setting.renderQueueType);

            ID_DepthPeelingPassCount = Shader.PropertyToID(passCountName);
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var volumeStake = VolumeManager.instance.stack;
            var volume = volumeStake.GetComponent<DepthPeelingVolume>();

            if (volume == null)
            {
                return;
            }

            var passCount = volume.Params.value.passCount;
            colorRTs = new List<int>(passCount);
            depthRTs = new List<int>(passCount);

            var drawingSettings = CreateDrawingSettings(shaderTag, ref renderingData, setting.sortingCriteria);

            var width = renderingData.cameraData.camera.pixelWidth;
            var height = renderingData.cameraData.camera.pixelHeight;
            var cmd = CommandBufferPool.Get("Depth Peeling");

            using (new ProfilingScope(cmd, profilingSampler))
            {
                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();


                for (int i = 0; i < passCount; i++)
                {
                    colorRTs.Add(Shader.PropertyToID($"_DepthPeelingColor{i}"));
                    depthRTs.Add(Shader.PropertyToID($"_DepthPeelingDepth{i}"));

                    // 获取RT,其中depthRT的FilterMode 必须为 point
                    cmd.GetTemporaryRT(colorRTs[i], width, height, 0);
                    cmd.GetTemporaryRT(depthRTs[i], width, height, 32, FilterMode.Point, RenderTextureFormat.RFloat);
                    // 将pass数传给shader
                    cmd.SetGlobalInt(ID_DepthPeelingPassCount, i);

                    // 第一次循环，不需要把深度传进shader，第二次循环就将上一次的深度传进shader，来进行深度比较
                    if (i > 0)
                        cmd.SetGlobalTexture(maxDepthTexName, depthRTs[i - 1]);

                    // 设置MRT，将深度渲染在不同的RT上
                    cmd.SetRenderTarget(new RenderTargetIdentifier[] { colorRTs[i], depthRTs[i] }, depthRTs[i]);
                    cmd.ClearRenderTarget(true, true, Color.black);

                    context.ExecuteCommandBuffer(cmd);
                    cmd.Clear();

                    // 绘制半透明
                    context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);
                }

                cmd.SetRenderTarget(source, renderer.cameraDepthTarget);
                // cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget);

                // -------------------------------------------------------
                // 从后往前进行半透明颜色混合
                for (int i = passCount - 1; i >= 0; i--)
                {
                    var dest = renderingData.cameraData.renderer.cameraColorTarget;

                    cmd.SetGlobalTexture(blendDepthTexName, depthRTs[i]);
                    int pass = 0;
                    // 第一次混合时，与黑色混合
                    if (i == passCount - 1)
                    {
                        pass = 1;
                    }

                    cmd.Blit(colorRTs[i], dest, GetMatetial(), pass);
                    cmd.ReleaseTemporaryRT(colorRTs[i]);
                    cmd.ReleaseTemporaryRT(depthRTs[i]);
                }

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }

            // cmd.SetRenderTarget(BuiltinRenderTextureType.CameraTarget, BuiltinRenderTextureType.CameraTarget);
            cmd.SetRenderTarget(source, renderer.cameraDepthTarget);
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
            cmd.Clear();
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }

        private Material GetMatetial()
        {
            if (blendMat == null)
            {
                blendMat = new Material(Shader.Find(shaderName));
            }

            return blendMat;
        }
    }
}