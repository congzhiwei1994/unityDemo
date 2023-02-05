using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace czw.SSAO
{
    public class SSAOData
    {
        /// <summary>
        /// SSAO shader 路径
        /// </summary>
        public const string shaderPath = "czw/RenderFeature/SSAO";

        /// <summary>
        /// 材质参数
        /// </summary>
        public static readonly int s_SSAOParamsID = Shader.PropertyToID("_SSAOParams");

        public static readonly int s_CameraViewXExtentID = Shader.PropertyToID("_CameraViewXExtent");
        public static readonly int s_CameraViewYExtentID = Shader.PropertyToID("_CameraViewYExtent");
        public static readonly int s_CameraViewZExtentID = Shader.PropertyToID("_CameraViewZExtent");
        public static readonly int s_ProjectionParams2ID = Shader.PropertyToID("_ProjectionParams2");
        public static readonly int s_CameraViewProjectionsID = Shader.PropertyToID("_CameraViewProjections");
        public static readonly int s_CameraViewTopLeftCornerID = Shader.PropertyToID("_CameraViewTopLeftCorner");

        public static readonly int _SourceSize = Shader.PropertyToID("_SourceSize");

        // RenderTargetIdentifier , 是和ID进行绑定的. 在后面渲染的时候用到, 作用是避免每次传递的时候都进行创建, 所以在这里先初始化.
        public static readonly int s_SSAOTexture1ID = Shader.PropertyToID("_SSAO_OcclusionTexture1");
        public static readonly int s_SSAOTexture2ID = Shader.PropertyToID("_SSAO_OcclusionTexture2");
        public static readonly int s_SSAOTexture3ID = Shader.PropertyToID("_SSAO_OcclusionTexture3");
        public static readonly int s_SSAOTextureFinalID = Shader.PropertyToID("_SSAO_OcclusionTexture");


    }
}