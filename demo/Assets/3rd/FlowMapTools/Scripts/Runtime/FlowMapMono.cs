using System;
using UnityEngine;
using UnityEngine.Profiling;
using System.Threading.Tasks;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace czw.FlowMapTool
{
    public enum FlowMapResolution
    {
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
    }

    [DisallowMultipleComponent]
    [ExecuteAlways]
    [Serializable]
    public class FlowMapMono : MonoBehaviour
    {
        public FlowMapResolution resolution = FlowMapResolution._1024;
        public int areaSize = 200;
        public Vector3 areaPos = Vector3.zero;
        public float flowSpeed = 0.5f;
        public float brushStrength = 0.5f;
        public Texture flowMapTex;
        private FlowData data;
        private bool isWaterResInit;
        private Material material;

        public static readonly int flowMapSize_Id = Shader.PropertyToID("_FlowMapSize");
        public static readonly int flowMapOffset_Id = Shader.PropertyToID("_FlowMapOffset");
        public static readonly int flowMapSpeed_Id = Shader.PropertyToID("_FlowMapSpeed");

        /// <summary>
        /// 把数据存储到 FlowData
        /// </summary>
        public void Save(FlowData flowData)
        {
            flowData.resolution = resolution;
            flowData.areaPos = areaPos;
            flowData.areaSize = areaSize;
            flowData.flowSpeed = flowSpeed;
            flowData.brushStrength = brushStrength;
            flowData.flowMapTex = flowMapTex;
        }

        public void SetWaterMateral(Material material)
        {
            this.material = material;
        }

        private void OnEnable()
        {
            SubscribeBeforeCameraRendering();
            SubscribeAfterCameraRendering();
        }

        private void OnDisable()
        {
            UnsubscribeBeforeCameraRendering();
            UnsubscribeAfterCameraRendering();
        }

        private void OnBeforeCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            SetWaterParams();
        }

        private void OnAfterCameraRendering(ScriptableRenderContext context, Camera camera)
        {
        }


        private void SetWaterParams()
        {
            material.SetFloat(flowMapSize_Id, areaSize);
            material.SetVector(flowMapOffset_Id, areaPos);
            material.SetFloat(flowMapSpeed_Id, flowSpeed);
        }

        #region 事件

        void SubscribeBeforeCameraRendering()
        {
            Debug.LogError("SubscribeBeforeCameraRendering");
            RenderPipelineManager.beginCameraRendering += OnBeforeCameraRendering;
        }

        void UnsubscribeBeforeCameraRendering()
        {
            RenderPipelineManager.beginCameraRendering -= OnBeforeCameraRendering;
        }

        void SubscribeAfterCameraRendering()
        {
            RenderPipelineManager.endCameraRendering += OnAfterCameraRendering;
        }

        void UnsubscribeAfterCameraRendering()
        {
            RenderPipelineManager.endCameraRendering -= OnAfterCameraRendering;
        }

        #endregion
    }
}