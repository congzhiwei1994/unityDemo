using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace czw.FlowMapTool
{
    public class FlowMapRTSetting
    {
        private static GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat;
        private static ClearFlag clearFlag = ClearFlag.Color;
        private static Color clearColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private const string FLOWMAP_NAME = "_flowmapRT";


        // 创建并且设置RT
        public static RenderTexture GetRenderTexture(int width, int height)

        {
            var descriptor = new RenderTextureDescriptor(width, height, format, 0);

            descriptor.sRGB = false;
            descriptor.enableRandomWrite = false;
            descriptor.dimension = TextureDimension.Tex2D;
            descriptor.useMipMap = false;
            descriptor.autoGenerateMips = false;
            descriptor.shadowSamplingMode = ShadowSamplingMode.None;
            descriptor.mipCount = 0;

            descriptor.msaaSamples = 1;
            descriptor.vrUsage = VRTextureUsage.None;

            var rt = RenderTexture.GetTemporary(descriptor);
            rt.name = FLOWMAP_NAME;

            if (!IsRTCreat(rt))
                rt.Create();

            var activeRT = RenderTexture.active;
            RenderTexture.active = rt;

            GL.Clear((clearFlag & ClearFlag.Depth) != 0, (clearFlag & ClearFlag.Color) != 0, clearColor);
            RenderTexture.active = activeRT;
            return rt;
        }

        private static bool IsRTCreat(RenderTexture rt)
        {
            return rt.IsCreated();
        }

        // private void DrawFlowmapPass(int pass, RenderTextureDescriptor renderTexture, Material material)
        // {
        //     var tempRT = RenderTexture.GetTemporary(renderTexture);
        //     tempRT.name = "_TempRT";
        //     var activeRT = RenderTexture.active;
        //
        //     Graphics.Blit(renderTexture, tempRT, material, pass);
        //     Graphics.Blit(tempRT,renderTexture);
        //
        //     RenderTexture.active = activeRT;
        //     tempRT.Release();
        // }
    }
}