using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace czw.FlowMapTool
{
    public class FlowMapRenderSetting : MonoBehaviour
    {
        private GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat;
        private ClearFlag clearFlag = ClearFlag.Color;
        private Color clearColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private string FLOWMAP_NAME = "_flowmapRT";
        private RenderTextureDescriptor descriptor;

        public RenderTextureDescriptor GetRenderTextureDescriptor(int width, int height)
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
            descriptor.stencilFormat = GraphicsFormat.None;
            return descriptor;
        }

        public RenderTexture GetRenderTexture(RenderTextureDescriptor descriptor)
        {
            var renderTexture = RenderTexture.GetTemporary(descriptor);
            renderTexture.graphicsFormat = format;
            renderTexture.stencilFormat = GraphicsFormat.None;

            renderTexture.name = FLOWMAP_NAME;
            if (!renderTexture.IsCreated())
            {
                renderTexture.Create();
            }

            return renderTexture;
        }

        public void ClearRenderTexture(RenderTexture renderTexture)
        {
            var activeRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear((clearFlag & ClearFlag.Depth) != 0, (clearFlag & ClearFlag.Color) != 0, clearColor);
            RenderTexture.active = activeRT;
        }
        
    }
}