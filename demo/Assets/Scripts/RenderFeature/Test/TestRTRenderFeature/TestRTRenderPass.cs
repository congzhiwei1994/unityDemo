using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Experimental.Rendering.Universal;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Jefford
{
    public class TestRTRenderPass : ScriptableRenderPass
    {
        private ScriptableRenderer m_renderer;
        private Material m_DrawMeshMaterial;
        private FilteringSettings _filteringSettings;
        private List<ShaderTagId> _shaderTagIds = new List<ShaderTagId>();
        private RenderQueueType _renderQueueType;
        private TestRTRenderFeature.MyCameraSettings _myCameraSettings;
        private bool _isEnableVolumeSet;
        private Material _overrideMaterial;
        private int _overrideMaterialPassIndex;
        private TestRTRenderFeature.MyCustormSettings _myCustormSettings;

        private RenderStateBlock _renderStateBlock;
        private ProfilingSampler _profilingSampler;
        private RenderTargetHandle _renderTargetHandle;
        private RenderTargetIdentifier _sourceID;
        private int downSample = 1;

        /// <summary>
        /// 构造函数，在Creat中调用
        /// </summary>
        /// <param name="settings"></param>
        public TestRTRenderPass(TestRTRenderFeature.TestRTSettings settings)
        {
            _renderTargetHandle.Init("_TestRTRenderFeature");

            var filterData = settings.myFilterSettings;
            _renderQueueType = filterData.RenderQueueType;
            RenderQueueRange renderQueueRange = (filterData.RenderQueueType == RenderQueueType.Opaque)
                ? RenderQueueRange.opaque
                : RenderQueueRange.transparent;

            _myCameraSettings = settings.myCameraSettings;
            _overrideMaterial = settings.overrideMaterial;
            _overrideMaterialPassIndex = settings.overrideMaterialPassIndex;
            _myCustormSettings = settings.myCustormSettings;

            _renderStateBlock = new RenderStateBlock(RenderStateMask.Nothing);
            _filteringSettings = new FilteringSettings(renderQueueRange, filterData.LayerMask);
            _profilingSampler = new ProfilingSampler(settings.passTags + _renderQueueType);

            // 添加Shader LightMode Tags
            if (settings.myFilterSettings.passNames != null && settings.myFilterSettings.passNames.Length > 0)
            {
                foreach (var passName in settings.myFilterSettings.passNames)
                {
                    _shaderTagIds.Add(new ShaderTagId(passName));
                }
            }
            else
            {
                _shaderTagIds.Add(new ShaderTagId("SRPDefaultUnlit"));
                _shaderTagIds.Add(new ShaderTagId("UniversalForward"));
                _shaderTagIds.Add(new ShaderTagId("UniversalForwardOnly"));
                _shaderTagIds.Add(new ShaderTagId("LightweightForward"));
            }
        }

        public void SetStencilState(TestRTRenderFeature.TestRTSettings settings)
        {
            var data = settings.stencilStateData;
            StencilState stencilState = new StencilState();
            stencilState.enabled = true;
            stencilState.SetCompareFunction(data.stencilCompareFunction);
            stencilState.SetPassOperation(data.passOperation);
            stencilState.SetFailOperation(data.failOperation);
            stencilState.SetZFailOperation(data.zFailOperation);

            _renderStateBlock.mask |= RenderStateMask.Stencil;
            _renderStateBlock.stencilReference = data.stencilReference;
            _renderStateBlock.stencilState = stencilState;
        }

        public void SetDetphState(TestRTRenderFeature.TestRTSettings settings)
        {
            var data = settings.myDepthSettings;
            _renderStateBlock.mask |= RenderStateMask.Depth;
            _renderStateBlock.depthState = new DepthState(data.enableZWrite, data.depthCompareFunction);
        }

        public void Setup(RenderTargetIdentifier sourceID, ScriptableRenderer renderer)
        {
            this._sourceID = sourceID;
            this.m_renderer = renderer;
        }

        public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
        {
        }

        public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
        {
            var camera = renderingData.cameraData.camera; // 拿到当前的相机
            // 筛选渲染相机
            if (camera.cameraType != CameraType.Game || renderingData.cameraData.renderType != CameraRenderType.Base)
                return;


            // -------------------------------------------- 通过Volume 设置材质 --------------------------------------------
            if (_myCustormSettings.isEnableVolumeSet)
            {
                var volumeStack = VolumeManager.instance.stack; // 拿到当前场景中的所有的Volume 
                var volume = volumeStack.GetComponent<TestRTRenderFeatureVolume>();
                if (volume == null || !volume.active)
                    return;


                var value = volume._testRTParams.value;
                if (!value.m_isEnable || value.m_material == null)
                    return;
                m_DrawMeshMaterial = value.m_material;
                downSample = value.m_downSample;
            }
            else
            {
                m_DrawMeshMaterial = _myCustormSettings.meshMaterial;
                downSample = _myCustormSettings.downSample;
            }

            if (m_DrawMeshMaterial == null)
            {
                Debug.LogError("m_DrawMeshMaterial Is Null");
                return;
            }

            // -------------------------------------------- 排序设置 ----------------------------------------------
            SortingCriteria sortingCriteria = (_renderQueueType == RenderQueueType.Opaque)
                ? SortingCriteria.CommonOpaque
                : renderingData.cameraData.defaultOpaqueSortFlags;


            // -------------------------------------------- DrawingSetting设置 ----------------------------------------------
            var drawingSettings = CreateDrawingSettings(_shaderTagIds, ref renderingData, sortingCriteria);
            drawingSettings.overrideMaterial = _overrideMaterial;
            drawingSettings.overrideMaterialPassIndex = _overrideMaterialPassIndex;


            // -------------------------------------------- 获取当前的RT设置 ----------------------------------------------
            var cameraTargetDes = renderingData.cameraData.cameraTargetDescriptor; // 获取当前相机RT的描述
            // 将获取的RT进行降采样
            var downSamplecameraTargetDes = new RenderTextureDescriptor(cameraTargetDes.width / downSample,
                cameraTargetDes.height / downSample);
            downSamplecameraTargetDes.depthBufferBits = 0; // 将其深度设置为0

            // --------------------------------------------

            var cmd = CommandBufferPool.Get("TestRT"); // 在池中获取一个Command Buffer

            // -------------------------------------------- overrideCamera --------------------------------------------
            using (new ProfilingScope(cmd, _profilingSampler))
            {
                if (_myCameraSettings.overrideCamera)
                {
                    var peojectMatrix = Matrix4x4.Perspective(_myCameraSettings.cameraFieldOfView, camera.aspect,
                        camera.nearClipPlane,
                        camera.farClipPlane);
                    // 矫正矩阵：https://docs.unity3d.com/ScriptReference/GL.GetGPUProjectionMatrix.html
                    GL.GetGPUProjectionMatrix(peojectMatrix,
                        renderingData.cameraData.IsCameraProjectionMatrixFlipped());
                    var viewMatrix = renderingData.cameraData.GetViewMatrix();
                    RenderingUtils.SetViewAndProjectionMatrices(cmd, viewMatrix, peojectMatrix, true);
                }

                context.ExecuteCommandBuffer(cmd);
                cmd.Clear();
            }

            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref _filteringSettings,
                ref _renderStateBlock);

            MyCustormPostProcessing(cmd, downSamplecameraTargetDes, renderingData);

            // 设置回之前的视空间矩阵和投影矩阵
            cmd.SetViewProjectionMatrices(renderingData.cameraData.GetViewMatrix(),
                renderingData.cameraData.GetProjectionMatrix());

            context.ExecuteCommandBuffer(cmd);
            CommandBufferPool.Release(cmd);
        }

        // -------------------------------------------- 后处理 --------------------------------------------
        public void MyCustormPostProcessing(CommandBuffer cmd, RenderTextureDescriptor cameraTaget,
            RenderingData renderingData)
        {
            cmd.GetTemporaryRT(_renderTargetHandle.id, cameraTaget); // 按照获取当前相机RT的描述设置来获取的RT
            Blit(cmd, _sourceID, _renderTargetHandle.Identifier());
            // cmd.Blit(_sourceID, _renderTargetHandle.Identifier()); // 将 m_sourceID -->> Blit -->> m_renderTargetHandle
            cmd.SetGlobalTexture("_TestRTRenderFeature",
                _renderTargetHandle.Identifier()); // 将 m_renderTargetHandle设置为全局变量

            cmd.SetRenderTarget(_sourceID, m_renderer.cameraDepthTarget); // 将RT设置回原来的RenderTarget

            cmd.SetViewProjectionMatrices(Matrix4x4.identity, Matrix4x4.identity); // 设置视空间矩阵和投影矩阵
            // 绘制全屏Mesh
            cmd.DrawMesh(RenderingUtils.fullscreenMesh, Matrix4x4.identity, m_DrawMeshMaterial, 0, 0);
        }

        public override void FrameCleanup(CommandBuffer cmd)
        {
            if (m_DrawMeshMaterial == null)
            {
                return;
            }

            m_DrawMeshMaterial = null;
            // 用完释放
            cmd.ReleaseTemporaryRT(_renderTargetHandle.id);
        }
    }
}