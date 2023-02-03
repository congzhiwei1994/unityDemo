using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace czw.SSSS
{
    public class SeperableSubsurfaceScatteringPass : ScriptableRenderPass
    {
        private Shader shader;
        private string shaderPath = "czw/RenderFeature/Skin/SSSS_Blur";
        private Material material;

        private SeperableSubsurfaceScatteringFeature.Setting _setting;
        private static List<Vector4> KernelArray = new List<Vector4>();
        private RenderTargetHandle handle_DiffuseTemp;
        private RenderTargetHandle handle_Diffuse;
        private RenderTargetHandle handle_Depth;
        ShaderTagId shaderTag;

        // private Color SSS_ColorOld;
        // private Color SSSFall0ff_ColorOld;

        public SeperableSubsurfaceScatteringPass(SeperableSubsurfaceScatteringFeature.Setting _setting)
        {
            this._setting = _setting;
            Init();
        }

        private void Init()
        {
            handle_DiffuseTemp.Init("Handle_DiffuseRT_Temp");
            handle_Diffuse.Init("Handle_DiffuseRT");
            handle_Depth.Init("Handle_SkinDepthRT");
            shaderTag = new ShaderTagId(_setting.shaderTagID);
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            material = czw.RenderFeature.RenderFeatureUtils.GetMaterial(material, shader, shaderPath);
            
            var desc = renderingData.cameraData.cameraTargetDescriptor;
            desc.enableRandomWrite = true;
            cmd.GetTemporaryRT(handle_DiffuseTemp.id, desc.width, desc.height, 0);
            cmd.GetTemporaryRT(handle_Diffuse.id, desc.width, desc.height, 0);
            cmd.GetTemporaryRT(handle_Depth.id, desc.width, desc.height, 24, FilterMode.Point,
                RenderTextureFormat.Depth);


            // 不需要每帧都计算
            // if (SSS_ColorOld != _setting.SSS_Color || SSSFall0ff_ColorOld != _setting.SSSFall0ff_Color)
            // {
            // SSS_ColorOld = _setting.SSS_Color;
            // SSSFall0ff_ColorOld = _setting.SSSFall0ff_Color;

            var sssFactor = new Vector3(_setting.SSS_Color.r, _setting.SSS_Color.g, _setting.SSS_Color.b);
            var sssFallOff = new Vector3(_setting.SSSFall0ff_Color.r, _setting.SSSFall0ff_Color.g,
                _setting.SSSFall0ff_Color.b);

            Vector3 SSSC = Vector3.Normalize(sssFactor);
            Vector3 SSSFC = Vector3.Normalize(sssFallOff);

            SSSSKernel.CalculateKernel(KernelArray, 32, SSSC, SSSFC);

            var screenFactor = new Vector4(desc.width, desc.height, 1f / desc.width, 1f / desc.height);
            material.SetVector(SSSSData.ID_ScreenSize, screenFactor);
            material.SetVectorArray(SSSSData.ID_Kernel, KernelArray);
            material.SetFloat(SSSSData.ID_SSSScaler, _setting.SubsurfaceScaler);
            material.SetFloat(SSSSData.ID_ScreenSize, _setting.SubsurfaceScaler);
            material.SetFloat(SSSSData.ID_FOV, renderingData.cameraData.camera.fieldOfView);
            material.SetFloat(SSSSData.ID_MaxDistance, _setting.MaxDistance);
            // }

            ConfigureClear(ClearFlag.All, Color.black);
            //将这个RT设置为Render Target
            ConfigureTarget(handle_Diffuse.id, handle_Depth.id);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            CommandBuffer cmd = CommandBufferPool.Get("SSSS CMD");

            var drawingSettings = CreateDrawingSettings(shaderTag, ref renderingData, SortingCriteria.CommonOpaque);
            var filteringSettings = new FilteringSettings(RenderQueueRange.opaque);
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref filteringSettings);

            cmd.SetGlobalTexture(SSSSData.ID_DepthRT, handle_Depth.id);
            // 进行卷积操作
            cmd.Blit(handle_Diffuse.id, handle_DiffuseTemp.id, material, 0);
            cmd.Blit(handle_DiffuseTemp.id, handle_Diffuse.id, material, 1);

            cmd.SetGlobalTexture(SSSSData.ID_DiffuseRT, handle_Diffuse.id);

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(handle_DiffuseTemp.id);
            cmd.ReleaseTemporaryRT(handle_Diffuse.id);
            cmd.ReleaseTemporaryRT(handle_Depth.id);
        }
    }
}