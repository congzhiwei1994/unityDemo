using czw.RenderFeature;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace CZW.PostProcessing
{
    public class ScanRenderPass : ScriptableRenderPass
    {
        private ScanFeature.Settings settings;
        private RenderTargetHandle rth1;

        private RenderTargetIdentifier source;

        public void Setup(RenderTargetIdentifier source, ScriptableRenderer renderer)
        {
            this.source = source;
        }

        public ScanRenderPass(ScanFeature.Settings settings)
        {
            this.settings = settings;
            rth1.Init("ScanTexture");
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cameraData = renderingData.cameraData;
            // 
            if (cameraData.cameraType == CameraType.SceneView || settings.material == null)
            {
                return;
            }

            var CaulateWorldPosMatrixByRay = RenderFeatureUtils.CaulateWorldPosMatrixByRay((cameraData.camera));

            var cmd = CommandBufferPool.Get("Post-Processing");
            RenderTextureDescriptor opaqueDesc = cameraData.cameraTargetDescriptor;

            cmd.GetTemporaryRT(rth1.id, opaqueDesc);
            cmd.Blit(source, rth1.Identifier());

            cmd.SetGlobalTexture("_PostProcessingScanTexture", rth1.Identifier());
            settings.material.SetMatrix("_FrustumCornersRay", CaulateWorldPosMatrixByRay);
            settings.material.SetColor("_LineColor", settings._LineColor);
            settings.material.SetFloat("_FallOff", settings._FallOff);
            // settings.material.SetFloat("_ScanDistance", settings._ScanDistance);

            settings.material.SetColor("_ScanColor", settings._ScanColor);
            settings.material.SetColor("_DotColor", settings._DotColor);

            cmd.SetRenderTarget(source);
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, settings.material, 0, 0);
            cmd.SetViewProjectionMatrices(renderingData.cameraData.GetViewMatrix(),
                renderingData.cameraData.GetProjectionMatrix());

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            if (rth1 != null)
            {
                cmd.ReleaseTemporaryRT(rth1.id);
            }
        }
    }
}