using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace czw.FlowMapTool
{
    public class FlowMapRender : MonoBehaviour
    {
        private static GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat;
        private static ClearFlag clearFlag = ClearFlag.Color;
        private static Color clearColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private const string FLOWMAP_NAME = "_flowmapRT";

        public static RenderTexture GetRenderTexture1(int width, int height)
        {
            var descriptor = new RenderTextureDescriptor(width, height, format, 0);

            descriptor.sRGB = false;
            descriptor.useMipMap = false;
            descriptor.autoGenerateMips = false;

            var rt = RenderTexture.GetTemporary(descriptor);
            rt.name = FLOWMAP_NAME;
            if (!rt.IsCreated())
                rt.Create();

            return rt;
        }


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

            if (!rt.IsCreated())
                rt.Create();

            var activeRT = RenderTexture.active;
            RenderTexture.active = rt;
            GL.Clear((clearFlag & ClearFlag.Depth) != 0, (clearFlag & ClearFlag.Color) != 0, clearColor);
            RenderTexture.active = activeRT;

            return rt;
        }
    }
}