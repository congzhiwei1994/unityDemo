using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;


namespace czw.SSAO
{
    namespace RenderFeature.SSAO
    {
        public enum DepthSource
        {
            Depth = 0,
            DepthNormals = 1
        }

        [DisallowMultipleRendererFeature]
        public class SSAORenderFeature : ScriptableRendererFeature
        {
            [System.Serializable]
            public class Setting
            {
                public RenderPassEvent passEvent = RenderPassEvent.AfterRenderingTransparents;
                [Range(0, 5)] public float Intensity = 1;
                [Range(0, 2)] public float Radius = 0.2f;
                [Range(1, 10)] public int SampleCount = 4;
                public DepthSource DepthSource = DepthSource.Depth;
            }

            public Setting setting = new Setting();
            private SSAORenderPass pass;
            private Material materal;
            private Shader shader;


            public override void Create()
            {
                materal = GetMaterial();
                pass = new SSAORenderPass(setting);
            }

            public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
            {
                // pass.renderPassEvent = setting.passEvent;
                pass.Setup(renderer, materal);
                renderer.EnqueuePass(pass);
            }

            private Material GetMaterial()
            {
                if (materal == null)
                {
                    materal = czw.RenderFeature.RenderFeatureUtils.GetMaterial(materal, shader, SSAOData.shaderPath);
                }

                return materal;
            }

            // Dispose在切换RendererData的时候会触发, 销毁创建的材质.
            protected override void Dispose(bool disposing)
            {
                if (materal != null)
                {
                    CoreUtils.Destroy(materal);
                }
            }
        }
    }
}