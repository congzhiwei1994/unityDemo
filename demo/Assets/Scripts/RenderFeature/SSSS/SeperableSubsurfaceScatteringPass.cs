using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace czw.SSSS
{
    public class SeperableSubsurfaceScatteringPass : ScriptableRenderPass
    {
        private SeperableSubsurfaceScatteringFeature.Setting _setting;
        private static List<Vector4> KernelArray = new List<Vector4>();
        private Material material;
        private RenderTargetHandle Handle_SkinDiffuseRT;
        private RenderTargetHandle Handle_BlurRT;
        private RenderTargetHandle Handle_SkinDepthRT;

        ShaderTagId shaderTag;

        public SeperableSubsurfaceScatteringPass(SeperableSubsurfaceScatteringFeature.Setting _setting)
        {
            this._setting = _setting;
            material = _setting.material;

            Init();
        }

        private void Init()
        {
            Handle_SkinDiffuseRT.Init("Handle_SkinDiffuseRT");
            Handle_BlurRT.Init("Handle_BlurRT");
            Handle_SkinDepthRT.Init("Handle_SkinDepthRT");
            shaderTag = new ShaderTagId(_setting.shaderTagID);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            if (material == null)
                return;

            var sssFactor = new Vector3(_setting.SSS_Color.r, _setting.SSS_Color.g, _setting.SSS_Color.b);
            var sssFallOff = new Vector3(_setting.SSSFall0ff_Color.r, _setting.SSSFall0ff_Color.g,
                _setting.SSSFall0ff_Color.b);

            Vector3 SSSC = Vector3.Normalize(sssFactor);
            Vector3 SSSFC = Vector3.Normalize(sssFallOff);

            SSSSUtils.CalculateKernel(KernelArray, 32, SSSC, SSSFC);

            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.enableRandomWrite = true;

            cmd.GetTemporaryRT(Handle_SkinDiffuseRT.id, desc.width, desc.height, 0);
            cmd.GetTemporaryRT(Handle_BlurRT.id, desc.width, desc.height, 0);
            cmd.GetTemporaryRT(Handle_SkinDepthRT.id, desc.width, desc.height, 24, FilterMode.Point,
                RenderTextureFormat.Depth);

            var screenFactor = new Vector4(desc.width, desc.height, 1f / desc.width, 1f / desc.height);
            material.SetVector(SSSSData.ID_ScreenSize, screenFactor);
            material.SetVectorArray(SSSSData.ID_Kernel, KernelArray);
            material.SetFloat(SSSSData.ID_SSSScaler, _setting.SubsurfaceScaler);
            material.SetFloat(SSSSData.ID_ScreenSize, _setting.SubsurfaceScaler);
            material.SetFloat(SSSSData.ID_FOV, renderingData.cameraData.camera.fieldOfView);
            material.SetFloat(SSSSData.ID_MaxDistance, _setting.MaxDistance);

            //将这个RT设置为Render Target
            ConfigureTarget(Handle_BlurRT.id, Handle_SkinDepthRT.id);

            //将RT清空为黑
            ConfigureClear(ClearFlag.All, Color.black);

            cmd.SetGlobalTexture(SSSSData.ID_SSSBlurRT, Handle_BlurRT.id);
            cmd.SetGlobalTexture(SSSSData.ID_SkinDepthRT, Handle_SkinDepthRT.id);
        }  

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("SSSS CMD");

            var drawingSettings = CreateDrawingSettings(shaderTag, ref renderingData, SortingCriteria.CommonOpaque);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            cmd.Blit(Handle_BlurRT.id, Handle_SkinDiffuseRT.id, material, 0);
            cmd.Blit(Handle_SkinDiffuseRT.id, Handle_BlurRT.id, material, 1);
            
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(Handle_SkinDiffuseRT.id);
            cmd.ReleaseTemporaryRT(Handle_BlurRT.id);
            cmd.ReleaseTemporaryRT(Handle_SkinDepthRT.id);
        }

        private void Debug(Material material)
        {
            if (_setting.DisableSkin)
                material.EnableKeyword("ENABLE_SKIN_SSSS_DEBUG_ON");
            else
                material.DisableKeyword("ENABLE_SKIN_SSSS_DEBUG_ON");
        }
    }
}