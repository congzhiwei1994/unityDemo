using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Water
{
    public class TemporaryRenderTexture
    {
        public RenderTextureDescriptor descriptor;
        public RenderTexture rt;
        string _name;
        public bool isInitialized;

        private TextureDimension dimension = TextureDimension.Tex2D;
        private GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat;
        private VRTextureUsage vrUsage = VRTextureUsage.None;
        private FilterMode filterMode = FilterMode.Bilinear;
        private ShadowSamplingMode shadowSamplingMode = ShadowSamplingMode.None;
        private int mipMapCount = 0;
        private int msaaSamples = 1;
        private bool autoGenerateMips = false;
        private bool useMipMap = false;
        private bool useRandomWrite = false;
        private Color clearColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private ClearFlag clearFlag = ClearFlag.Color;

        public TemporaryRenderTexture()
        {
        }

        public TemporaryRenderTexture(string name, TemporaryRenderTexture source)
        {
            descriptor = source.descriptor;
            rt = RenderTexture.GetTemporary(descriptor);
            rt.name = name;
            _name = name;
            if (!rt.IsCreated()) rt.Create();
            isInitialized = true;
        }

        public void Alloc(string name, int width, int height, int depth)
        {
            if (rt == null)
            {
                descriptor = new RenderTextureDescriptor(width, height, format, depth);
                descriptor.sRGB = false;
                descriptor.useMipMap = false;
                descriptor.autoGenerateMips = false;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated()) rt.Create();
                isInitialized = true;
            }
            else if (rt.width != width || rt.height != height || !isInitialized || _name != name)
            {
                if (isInitialized)
                    Release();

                descriptor.width = width;
                descriptor.height = height;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated()) rt.Create();
                isInitialized = true;
            }
        }


        public void Alloc(string name, int width, int height, int depth, ClearFlag clearFlag)
        {
            if (rt == null)
            {
                descriptor = new RenderTextureDescriptor(width, height, format, depth);
                descriptor.sRGB = false;
                descriptor.enableRandomWrite = useRandomWrite;
                descriptor.dimension = dimension;
                descriptor.useMipMap = useMipMap;
                descriptor.autoGenerateMips = autoGenerateMips;
                descriptor.shadowSamplingMode = shadowSamplingMode;
                descriptor.mipCount = mipMapCount;
                descriptor.msaaSamples = msaaSamples;
                descriptor.vrUsage = vrUsage;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated())
                    rt.Create();

                ClearRenderTexture(rt, clearFlag);
                isInitialized = true;
            }
            else if (rt.width != width || rt.height != height || rt.dimension != dimension ||
                     rt.useMipMap != useMipMap || !isInitialized || _name != name)
            {
                if (isInitialized)
                    Release();

                descriptor.width = width;
                descriptor.height = height;
                descriptor.dimension = dimension;
                descriptor.useMipMap = useMipMap;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated())
                    rt.Create();
                ClearRenderTexture(rt, clearFlag);
                isInitialized = true;
            }
        }

        public void ClearRenderTexture(RenderTexture rt, ClearFlag clearFlag)
        {
            var activeRT = RenderTexture.active;
            RenderTexture.active = rt;
            GL.Clear((clearFlag & ClearFlag.Depth) != 0, (clearFlag & ClearFlag.Color) != 0, clearColor);
            RenderTexture.active = activeRT;
        }

        public void Release(bool unlink = false)
        {
            if (rt != null)
            {
                RenderTexture.ReleaseTemporary(rt);
                isInitialized = false;
                if (unlink)
                    rt = null;
            }
        }
    }
}