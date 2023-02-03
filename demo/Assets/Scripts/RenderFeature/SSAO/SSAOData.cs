using UnityEngine;

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
    }
}