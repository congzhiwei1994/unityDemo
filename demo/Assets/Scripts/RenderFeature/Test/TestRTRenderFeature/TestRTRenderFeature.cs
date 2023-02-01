using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using UnityEngine.Serialization;

namespace Jefford
{
    public class TestRTRenderFeature : ScriptableRendererFeature
    {
        /// <summary>
        /// Setting 设置
        /// </summary>
        [System.Serializable]
        public class TestRTSettings
        {
            public string passTags = "TestRTRenderFeature";
            public RenderPassEvent m_event = RenderPassEvent.AfterRenderingOpaques;
            public MyCustormSettings myCustormSettings = new MyCustormSettings();
            public StencilStateData stencilStateData = new StencilStateData();
            public MyDepthSettings myDepthSettings = new MyDepthSettings();
            public MyCameraSettings myCameraSettings = new MyCameraSettings();
            public MyFilterSettings myFilterSettings = new MyFilterSettings();

            public Material overrideMaterial = null;
            public int overrideMaterialPassIndex = 0;
        }

        [System.Serializable]
        public class MyCustormSettings
        {
            public bool isEnableVolumeSet = false;
            public Material meshMaterial = null;
            public int downSample = 1;
        }

        /// <summary>
        /// 过滤设置
        /// </summary>
        [System.Serializable]
        public class MyFilterSettings
        {
            public RenderQueueType RenderQueueType = RenderQueueType.Opaque;
            public LayerMask LayerMask = 1;
            public string[] passNames;
        }

        /// <summary>
        /// 深度设置
        /// </summary>
        [System.Serializable]
        public class MyDepthSettings
        {
            public bool overrideDepthState = false;
            public bool enableZWrite = true;
            public CompareFunction depthCompareFunction = CompareFunction.LessEqual;
        }

        /// <summary>
        ///  相机设置
        /// </summary>
        [System.Serializable]
        public class MyCameraSettings
        {
            public bool overrideCamera = false;
            public bool restoreCamera = true;
            public Vector4 offset;
            public float cameraFieldOfView = 60.0f;
        }

        // -----------------------------------------------------------------------

        TestRTRenderPass renderPass;
        public TestRTSettings testRTSettings = new TestRTSettings();

        public override void Create()
        {
            if (testRTSettings.m_event < RenderPassEvent.BeforeRenderingPrePasses)
            {
                testRTSettings.m_event = RenderPassEvent.BeforeRenderingPrePasses;
            }

            renderPass = new TestRTRenderPass(testRTSettings);
            renderPass.renderPassEvent = testRTSettings.m_event;

            // 设置模板
            if (testRTSettings.stencilStateData.overrideStencilState)
            {
                renderPass.SetStencilState(testRTSettings);
            }

            // 设置深度
            if (testRTSettings.myDepthSettings.overrideDepthState)
            {
                renderPass.SetDetphState(testRTSettings);
            }
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
        {
            renderPass.Setup(renderer.cameraColorTarget, renderer);
            renderer.EnqueuePass(renderPass);
        }
    }
}