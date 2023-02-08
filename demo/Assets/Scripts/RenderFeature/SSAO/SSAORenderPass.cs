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
    public enum ShaderPasses
    {
        AO = 0,
        BlurHorizontal = 1,
        BlurVertical = 2,
        BlurFinal = 3,
        AfterOpaque = 4
    }

    public class SSAORenderPass : ScriptableRenderPass
    {
        private SSAORenderFeature.Setting setting = new SSAORenderFeature.Setting();
        private Material material;
        private ScriptableRenderer renderer;
        private Matrix4x4 m_CameraViewProjections = Matrix4x4.identity;

        private Vector4 m_CameraTopLeftCorner = Vector4.one;
        private Vector4 m_CameraXExtent = Vector4.one;
        private Vector4 m_CameraYExtent = Vector4.one;
        private Vector4 m_CameraZExtent = Vector4.one;

        private bool m_SupportsR8RenderTextureFormat = SystemInfo.SupportsRenderTextureFormat(RenderTextureFormat.R8);
        private RenderTextureDescriptor m_AOPassDescriptor;
        private RenderTextureDescriptor m_BlurPassesDescriptor;
        private RenderTextureDescriptor m_FinalDescriptor;

        private ProfilingSampler m_ProfilingSampler = new ProfilingSampler("SSAO");
        private ShaderPasses shaderPassIndex;

        private RenderTargetIdentifier m_SSAOTexture1Target =
            new RenderTargetIdentifier(SSAOData.s_SSAOTexture1ID, 0, CubemapFace.Unknown, -1);

        private RenderTargetIdentifier m_SSAOTexture2Target =
            new RenderTargetIdentifier(SSAOData.s_SSAOTexture2ID, 0, CubemapFace.Unknown, -1);

        private RenderTargetIdentifier m_SSAOTexture3Target =
            new RenderTargetIdentifier(SSAOData.s_SSAOTexture3ID, 0, CubemapFace.Unknown, -1);

        public SSAORenderPass(SSAORenderFeature.Setting setting)
        {
            this.setting = setting;
        }

        public void Setup(ScriptableRenderer renderer, Material material)
        {
            this.renderer = renderer;
            this.material = material;

            // 判断渲染时机
            // Rendering after PrePasses is usually correct except when depth priming is in play:
            // then we rely on a depth resolve taking place after the PrePasses in order to have it ready for SSAO.
            // Hence we set the event to RenderPassEvent.AfterRenderingPrePasses + 1 at the earliest.
            renderPassEvent = (setting.passEvent == RenderPassEvent.AfterRenderingOpaques)
                ? RenderPassEvent.AfterRenderingOpaques
                : RenderPassEvent.AfterRenderingPrePasses + 1;

            // 判断使用什么类型的RT
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
            Vector4 ssaoParams = new Vector4(
                setting.Intensity,
                setting.Radius,
                1.0f / 1,
                setting.SampleCount
            );
            material.SetVector(SSAOData.s_SSAOParamsID, ssaoParams);

            // 获取VP矩阵
            Matrix4x4 view = renderingData.cameraData.GetViewMatrix();
            Matrix4x4 proj = renderingData.cameraData.GetProjectionMatrix();
            m_CameraViewProjections = proj * view;

            //创建新的V矩阵，并且最后一列设置为 0，0，0，1
            Matrix4x4 cview = view;
            cview.SetColumn(3, new Vector4(0.0f, 0.0f, 0.0f, 1.0f));

            // 转换到世界空间
            Matrix4x4 VP_Matrix = proj * cview;
            Matrix4x4 VP_Matrix_Inv = VP_Matrix.inverse;

            //把proj空间下的最远的点(最远左上点, 最远右上点, 最远右下点, 最远中心点), 转换到world空间坐标.
            Vector4 topLeftCorner = VP_Matrix_Inv.MultiplyPoint(new Vector4(-1, 1, -1, 1));
            Vector4 topRightCorner = VP_Matrix_Inv.MultiplyPoint(new Vector4(1, 1, -1, 1));
            Vector4 bottomLeftCorner = VP_Matrix_Inv.MultiplyPoint(new Vector4(-1, -1, -1, 1));
            Vector4 farCentre = VP_Matrix_Inv.MultiplyPoint(new Vector4(0, 0, 1, 1));

            m_CameraTopLeftCorner = topLeftCorner;
            m_CameraXExtent = topRightCorner - topLeftCorner;
            m_CameraYExtent = bottomLeftCorner - topLeftCorner;
            m_CameraZExtent = farCentre;

            // 把参数设置进材质
            SetMaterials(renderingData.cameraData.camera.nearClipPlane);
            GetDescriptor(renderingData.cameraData.cameraTargetDescriptor);

            // 为了效果好, 这里RT的用FilterMode用Bilinear.
            cmd.GetTemporaryRT(SSAOData.s_SSAOTexture1ID, m_AOPassDescriptor, FilterMode.Bilinear);
            cmd.GetTemporaryRT(SSAOData.s_SSAOTexture2ID, m_BlurPassesDescriptor, FilterMode.Bilinear);
            cmd.GetTemporaryRT(SSAOData.s_SSAOTexture3ID, m_BlurPassesDescriptor, FilterMode.Bilinear);
            cmd.GetTemporaryRT(SSAOData.s_SSAOTextureFinalID, m_FinalDescriptor, FilterMode.Bilinear);

            // 画布的输入，如果是AfterOpaque, 就画在Color RT上类似于贴上去, 否则就创建一个RT, 物体着色的时候进行采样, 让其变暗.
            // Configure targets and clear color
            ConfigureTarget(setting.passEvent == RenderPassEvent.AfterRenderingOpaques
                ? renderer.cameraColorTarget
                : SSAOData.s_SSAOTexture2ID);
            ConfigureClear(ClearFlag.None, Color.white);
        }

        private void SetMaterials(float nearClipPlane)
        {
            // 把参数设置进材质
            material.SetVector(SSAOData.s_ProjectionParams2ID,
                new Vector4(1.0f / nearClipPlane, 0.0f, 0.0f, 0.0f));
            material.SetMatrix(SSAOData.s_CameraViewProjectionsID, m_CameraViewProjections);
            material.SetVector(SSAOData.s_CameraViewTopLeftCornerID, m_CameraTopLeftCorner);
            material.SetVector(SSAOData.s_CameraViewXExtentID, m_CameraXExtent);
            material.SetVector(SSAOData.s_CameraViewYExtentID, m_CameraYExtent);
            material.SetVector(SSAOData.s_CameraViewZExtentID, m_CameraZExtent);
        }

        private void GetDescriptor(RenderTextureDescriptor cameraTargetDescriptor)
        {
            // Set up the descriptors
            RenderTextureDescriptor descriptor = cameraTargetDescriptor;
            descriptor.msaaSamples = 1;
            descriptor.depthBufferBits = 0;

            // SSAO Descriptor
            m_AOPassDescriptor = descriptor;
            m_AOPassDescriptor.width /= 1;
            m_AOPassDescriptor.height /= 1;
            m_AOPassDescriptor.colorFormat = RenderTextureFormat.ARGB32;

            // BlurPasses Descriptor 
            m_BlurPassesDescriptor = descriptor;
            m_BlurPassesDescriptor.colorFormat = RenderTextureFormat.ARGB32;

            m_FinalDescriptor = descriptor;
            // AO的结果图是一个0~1的黑白图, 所以单通道的R8就可以了. 不过可能存在一些设备不支持, 就用ARGB32.
            m_FinalDescriptor.colorFormat =
                m_SupportsR8RenderTextureFormat ? RenderTextureFormat.R8 : RenderTextureFormat.ARGB32;
        }


        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            if (material == null)
            {
                Debug.LogErrorFormat(
                    "{0}.Execute(): Missing material. ScreenSpaceAmbientOcclusion pass will not execute. Check for missing reference in the renderer resources.",
                    GetType().Name);
                return;
            }

            CommandBuffer cmd = CommandBufferPool.Get("SSAO");
            using (new ProfilingScope(cmd, m_ProfilingSampler))
            {
                // 设置画布大小
                SetSourceSize(cmd, m_AOPassDescriptor);

                Vector4 scaleBiasRt = new Vector4(-1, 1.0f, -1.0f, 1.0f);
                cmd.SetGlobalVector(Shader.PropertyToID("_ScaleBiasRt"), scaleBiasRt);

                // Execute the SSAO
                Render(cmd, m_SSAOTexture1Target, ShaderPasses.AO);
            }

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        public override void OnCameraCleanup(CommandBuffer cmd)
        {
            cmd.ReleaseTemporaryRT(SSAOData.s_SSAOTexture1ID);
            cmd.ReleaseTemporaryRT(SSAOData.s_SSAOTexture2ID);
            cmd.ReleaseTemporaryRT(SSAOData.s_SSAOTexture3ID);
            cmd.ReleaseTemporaryRT(SSAOData.s_SSAOTextureFinalID);
        }

        // 设置RT, 全屏绘制某个pass. 因为是全部覆盖的后处理绘制,
        // 所以不关心(DontCare)输入的颜色 和 depth, 只需要储存(Store)输出的颜色就好了.
        private void Render(CommandBuffer cmd, RenderTargetIdentifier target, ShaderPasses pass)
        {
            cmd.SetRenderTarget(
                target,
                RenderBufferLoadAction.DontCare,
                RenderBufferStoreAction.Store,
                target,
                RenderBufferLoadAction.DontCare,
                RenderBufferStoreAction.DontCare
            );
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, material, 0, (int)pass);
        }

        /// <summary>
        /// 传递画布尺寸(考虑动态画布缩放)到Shader中.
        /// </summary>
        private void SetSourceSize(CommandBuffer cmd, RenderTextureDescriptor desc)
        {
            float width = desc.width;
            float height = desc.height;
            if (desc.useDynamicScale)
            {
                width *= ScalableBufferManager.widthScaleFactor;
                height *= ScalableBufferManager.heightScaleFactor;
            }

            cmd.SetGlobalVector(SSAOData._SourceSize, new Vector4(width, height, 1.0f / width, 1.0f / height));
        }
    }
}