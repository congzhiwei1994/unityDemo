using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Test.RenderFeature
{
    public class TestRenderPass1 : ScriptableRenderPass
    {
        private TestRenderFeature.Setting _setting;
        private RenderTargetIdentifier source;
        private RenderTargetIdentifier dest;
        private int tempRT_ID;

        public TestRenderPass1(TestRenderFeature.Setting _setting)
        {
            this._setting = _setting;

            tempRT_ID = Shader.PropertyToID("_TempRT");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            source = renderingData.cameraData.renderer.cameraColorTarget;
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            cmd.GetTemporaryRT(tempRT_ID, descriptor);
            dest = new RenderTargetIdentifier(tempRT_ID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (_setting.material == null)
                return;

            var cmd = CommandBufferPool.Get("Test cmd1");
            Blit(cmd, source, dest);
            cmd.SetGlobalTexture(_setting.globalName_ShaderTex, dest);
            _setting.material.SetColor(_setting.materialName_mainColor, _setting.color);

            cmd.SetRenderTarget(source);

            // ����VP����
            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity);
            // ����ȫ����Mesh
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, _setting.material, 0, 0);
            // DrawMesh֮��Ҫ���û�֮ǰ��VP���󣬷�����滭������ľ����Ǵ����
            cmd.SetViewProjectionMatrices(renderingData.cameraData.GetViewMatrix(),
                renderingData.cameraData.GetProjectionMatrix());

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempRT_ID);
        }
    }
}