using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Test.RenderFeature
{
    public class TestRenderPass0 : ScriptableRenderPass
    {
        private TestRenderFeature.Setting _setting;
        private RenderTargetIdentifier source;
        private RenderTargetIdentifier dest;
        private int tempRT_ID;

        public TestRenderPass0(TestRenderFeature.Setting _setting)
        {
            this._setting = _setting;
            tempRT_ID = Shader.PropertyToID("_TempRT");
        }

        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            // 源纹理
            source = renderingData.cameraData.renderer.cameraColorTarget;
            // 当前相机的描述
            var descriptor = renderingData.cameraData.cameraTargetDescriptor;
            // 根据当前相机的描述，在cmd中获取临时RT的ID
            cmd.GetTemporaryRT(tempRT_ID, descriptor);
            // 通过 tempRT_ID 来实例一个 RenderTargetIdentifier(RT标识符)
            dest = new RenderTargetIdentifier(tempRT_ID);
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var cmd = CommandBufferPool.Get("Test cmd");

            _setting.material.SetColor(_setting.materialName_mainColor, _setting.color);

            // 将 source 中的信息 copy 到 dest 中。
            cmd.Blit(source, dest, _setting.material, 0);
            // 再将 dest copy 到 source
            cmd.Blit(dest, source);
            // 执行 cmd
            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(tempRT_ID);
        }
    }
}