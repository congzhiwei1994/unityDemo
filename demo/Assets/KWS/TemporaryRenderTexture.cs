using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace KWS
{
    public class TemporaryRenderTexture
    {
        public RenderTextureDescriptor descriptor;
        public RenderTexture rt;
        string _name;
        public bool isInitialized;

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

        public void Alloc(string name, int width, int height, int depth, GraphicsFormat format)
        {
            if (rt == null)
            {
#if UNITY_2019_2_OR_NEWER
                descriptor = new RenderTextureDescriptor(width, height, format, depth);
#else
                    descriptor =
 new RenderTextureDescriptor(width, height, GraphicsFormatUtility.GetRenderTextureFormat(format), depth);
#endif

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
                if (isInitialized) Release();

                descriptor.width = width;
                descriptor.height = height;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated()) rt.Create();
                isInitialized = true;
            }
        }

        public void Alloc(string name, int width, int height, int depth, GraphicsFormat format, bool useMipMap,
            bool useRandomWrite = false, TextureDimension dimension = TextureDimension.Tex2D,
            bool autoGenerateMips = false, int mipMapCount = 0, int msaaSamples = 1,
            VRTextureUsage vrUsage = VRTextureUsage.None,
            FilterMode filterMode = FilterMode.Bilinear,
            ShadowSamplingMode shadowSamplingMode = ShadowSamplingMode.None)
        {
            if (rt == null)
            {
#if UNITY_2019_2_OR_NEWER
                descriptor = new RenderTextureDescriptor(width, height, format, depth);
#else
                    descriptor =
 new RenderTextureDescriptor(width, height, GraphicsFormatUtility.GetRenderTextureFormat(format), depth);
#endif

                descriptor.sRGB = false;
                descriptor.enableRandomWrite = useRandomWrite;
                descriptor.dimension = dimension;
                descriptor.useMipMap = useMipMap;
                descriptor.autoGenerateMips = autoGenerateMips;
                descriptor.shadowSamplingMode = shadowSamplingMode;
#if UNITY_2019_2_OR_NEWER
                descriptor.mipCount = mipMapCount;
#endif
                descriptor.msaaSamples = msaaSamples;
                descriptor.vrUsage = vrUsage;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated()) rt.Create();
                isInitialized = true;
            }
            else if (rt.width != width || rt.height != height || rt.dimension != dimension ||
                     rt.useMipMap != useMipMap || !isInitialized || _name != name)
            {
                if (isInitialized) Release();

                descriptor.width = width;
                descriptor.height = height;
                descriptor.dimension = dimension;
                descriptor.useMipMap = useMipMap;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated()) rt.Create();
                isInitialized = true;
            }
        }

        public static void ClearRenderTexture(RenderTexture rt, ClearFlag clearFlag, Color clearColor)
        {
            var activeRT = RenderTexture.active;
            RenderTexture.active = rt;
            GL.Clear((clearFlag & ClearFlag.Depth) != 0, (clearFlag & ClearFlag.Color) != 0, clearColor);
            RenderTexture.active = activeRT;
        }

        public void Alloc(string name, int width, int height, int depth, GraphicsFormat format, ClearFlag clearFlag,
            Color clearColor,
            bool useRandomWrite = false, TextureDimension dimension = TextureDimension.Tex2D, bool useMipMap = false,
            bool autoGenerateMips = false, int mipMapCount = 0, int msaaSamples = 1,
            VRTextureUsage vrUsage = VRTextureUsage.None,
            FilterMode filterMode = FilterMode.Bilinear,
            ShadowSamplingMode shadowSamplingMode = ShadowSamplingMode.None)
        {
            if (rt == null)
            {
#if UNITY_2019_2_OR_NEWER
                descriptor = new RenderTextureDescriptor(width, height, format, depth);
#else
                    descriptor =
 new RenderTextureDescriptor(width, height, GraphicsFormatUtility.GetRenderTextureFormat(format), depth);
#endif

                descriptor.sRGB = false;
                descriptor.enableRandomWrite = useRandomWrite;
                descriptor.dimension = dimension;
                descriptor.useMipMap = useMipMap;
                descriptor.autoGenerateMips = autoGenerateMips;
                descriptor.shadowSamplingMode = shadowSamplingMode;
#if UNITY_2019_2_OR_NEWER
                descriptor.mipCount = mipMapCount;
#endif
                descriptor.msaaSamples = msaaSamples;
                descriptor.vrUsage = vrUsage;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated()) rt.Create();
                ClearRenderTexture(rt, clearFlag, clearColor);
                isInitialized = true;
            }
            else if (rt.width != width || rt.height != height || rt.dimension != dimension ||
                     rt.useMipMap != useMipMap || !isInitialized || _name != name)
            {
                if (isInitialized) Release();

                descriptor.width = width;
                descriptor.height = height;
                descriptor.dimension = dimension;
                descriptor.useMipMap = useMipMap;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated()) rt.Create();
                ClearRenderTexture(rt, clearFlag, clearColor);
                isInitialized = true;
            }
        }

        public void AllocDepth(string name, int width, int height, TextureDimension dimension = TextureDimension.Tex2D,
            ShadowSamplingMode shadowSamplingMode = ShadowSamplingMode.None)
        {
            if (rt == null)
            {
#if UNITY_2021_2_OR_NEWER
                // descriptor = new RenderTextureDescriptor(width, height, SystemInfo.GetGraphicsFormat(DefaultFormat.LDR), SystemInfo.GetGraphicsFormat(DefaultFormat.DepthStencil));
                descriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.Depth, 32);
#else
                    descriptor = new RenderTextureDescriptor(width, height, RenderTextureFormat.Depth, 32);
#endif
                descriptor.dimension = dimension;
                descriptor.shadowSamplingMode = shadowSamplingMode;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                if (!rt.IsCreated()) rt.Create();
                isInitialized = true;
            }
            else if (rt.width != width || rt.height != height || rt.dimension != dimension || !isInitialized ||
                     _name != name)
            {
                if (isInitialized) Release();

                descriptor.width = width;
                descriptor.height = height;
                descriptor.dimension = dimension;

                rt = RenderTexture.GetTemporary(descriptor);
                rt.name = name;
                _name = name;
                isInitialized = true;
                if (!rt.IsCreated()) rt.Create();
            }
        }

        public void Release(bool unlink = false)
        {
            if (rt != null)
            {
                RenderTexture.ReleaseTemporary(rt);
                isInitialized = false;
                if (unlink) rt = null;
            }
        }
    }
}