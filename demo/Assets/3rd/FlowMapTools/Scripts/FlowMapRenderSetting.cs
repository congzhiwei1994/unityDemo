using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace czw.FlowMapTool
{
    public class FlowMapRenderSetting
    {
        private static GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat;
        private static ClearFlag clearFlag = ClearFlag.Color;
        private static Color clearColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private const string FLOWMAP_NAME = "_flowmapRT";
        private static RenderTextureDescriptor descriptor;

        public static RenderTextureDescriptor GetRenderTextureDescriptor(int width, int height)
        {
            descriptor = new RenderTextureDescriptor(width, height, format, 0);
            descriptor.sRGB = false;
            descriptor.enableRandomWrite = false;
            descriptor.dimension = TextureDimension.Tex2D;
            descriptor.useMipMap = false;
            descriptor.autoGenerateMips = false;
            descriptor.shadowSamplingMode = ShadowSamplingMode.None;
            descriptor.mipCount = 0;
            descriptor.msaaSamples = 1;
            descriptor.vrUsage = VRTextureUsage.None;
            return descriptor;
        }

        public static RenderTexture InitRenderTexture(int width, int height)
        {
            GetRenderTextureDescriptor(width, height);
            var rt = RenderTexture.GetTemporary(descriptor);
            rt.name = FLOWMAP_NAME;

            if (!IsRTCreat(rt))
                rt.Create();
            CleatRenderTexture(rt);
            return rt;
        }

        private static void CleatRenderTexture(RenderTexture renderTexture)
        {
            var activeRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear((clearFlag & ClearFlag.Depth) != 0, (clearFlag & ClearFlag.Color) != 0, clearColor);
            RenderTexture.active = activeRT;
        }

        private static bool IsRTCreat(RenderTexture rt)
        {
            return rt.IsCreated();
        }

        public static void DrawFlowmapPass(int pass, Material material, RenderTexture source)
        {
            var tempRT = RenderTexture.GetTemporary(descriptor);
            tempRT.name = "_TempRT";
            var activeRT = RenderTexture.active;

            Graphics.Blit(source, tempRT, material, pass);
            Graphics.Blit(tempRT, source);

            RenderTexture.active = activeRT;
            tempRT.Release();
        }
    }
}