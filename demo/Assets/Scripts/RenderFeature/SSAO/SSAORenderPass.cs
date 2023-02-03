using System;
using System.Collections.Generic;
using czw.SSAO.RenderFeature.SSAO;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace czw.SSAO
{
    public class SSAORenderPass : ScriptableRenderPass
    {
        private SSAORenderFeature.Setting setting = new SSAORenderFeature.Setting();
        private Material material;
        private ScriptableRenderer renderer;

        private Matrix4x4 VP = Matrix4x4.identity;

        // private DepthSource depthSource;
        public SSAORenderPass(SSAORenderFeature.Setting setting)
        {
            this.setting = setting;
        }

        public void Setup(ScriptableRenderer renderer, Material material)
        {
            this.renderer = renderer;
            this.material = material;

            switch (setting.DepthSource)
            {
                case DepthSource.Depth:
                    // ConfigureInput告诉管线, 我这个pass需要什么图, 让引擎帮忙启用对应的pass, 得到对应的图.
                    ConfigureInput(ScriptableRenderPassInput.Depth);
                    break;
                case DepthSource.DepthNormals:
                    ConfigureInput(ScriptableRenderPassInput.Normal);
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }


        public override void OnCameraSetup(CommandBuffer cmd, ref RenderingData renderingData)
        {
            RenderTextureDescriptor cameraTargetDescriptor = renderingData.cameraData.cameraTargetDescriptor;
            Vector4 ssaoParams = new Vector4(
                setting.Intensity,
                setting.Radius,
                1.0f / 1,
                setting.SampleCount
            );
            material.SetVector(SSAOData.s_SSAOParamsID, ssaoParams);

            Matrix4x4 view = renderingData.cameraData.GetViewMatrix();
            Matrix4x4 proj = renderingData.cameraData.GetProjectionMatrix();
            VP = proj * view;
            
            Matrix4x4 cview = view;
            // 最后一列为 0，0，0，1
            cview.SetColumn(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));
            
            Matrix4x4 VP_Matrix = proj * cview;
            Matrix4x4 VP_Matrix_Inv = VP_Matrix.inverse;
            
            
            Vector4 topLeftCorner = VP_Matrix_Inv.MultiplyPoint(new Vector4(-1, 1, -1, 1));
            Vector4 topRightCorner = VP_Matrix_Inv.MultiplyPoint(new Vector4(1, 1, -1, 1));
            Vector4 bottomLeftCorner = VP_Matrix_Inv.MultiplyPoint(new Vector4(-1, -1, -1, 1));
            Vector4 farCentre = VP_Matrix_Inv.MultiplyPoint(new Vector4(0, 0, 1, 1));
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
        }
    }
}