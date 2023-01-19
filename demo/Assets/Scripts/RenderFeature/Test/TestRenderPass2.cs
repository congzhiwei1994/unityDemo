using UnityEngine;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Test.RenderFeature
{
    public class TestRenderPass2 : ScriptableRenderPass
    {
        private RenderTargetHandle _handle;
        private ScriptableRenderer _renderer;
        private RenderTargetIdentifier _source;
        private TestRenderFeature.Setting _setting;

        public void Setup(RenderTargetIdentifier descriptor, ScriptableRenderer renderer)
        {
            _source = descriptor;
            _renderer = renderer;
        }

        public TestRenderPass2(TestRenderFeature.Setting _setting)
        {
            this._setting = _setting;
            _handle.Init("_TempRT");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            
            var cameraData = renderingData.cameraData;
            var cameraTargetDes = cameraData.cameraTargetDescriptor;

            var cmd = CommandBufferPool.Get("Test cmd");
            cmd.GetTemporaryRT(_handle.id, cameraTargetDes);

            Blit(cmd, _source, _handle.Identifier());

            cmd.SetGlobalTexture(_setting.globalName_ShaderTex, _handle.Identifier());
            _setting.material.SetColor(_setting.materialName_mainColor, _setting.color);
            
            cmd.SetRenderTarget(_source, _renderer.cameraDepthTarget);
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _setting.material, 0, 0);
            cmd.SetViewProjectionMatrices(renderingData.cameraData.GetViewMatrix(),
                renderingData.cameraData.GetProjectionMatrix());

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(_handle.id);
        }
    }
}