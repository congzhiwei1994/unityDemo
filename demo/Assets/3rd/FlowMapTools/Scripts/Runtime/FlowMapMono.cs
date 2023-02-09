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

        public void Save(FlowData flowData)
        {
            flowData.resolution = resolution;
            flowData.areaPos = areaPos;
            flowData.areaSize = areaSize;
            flowData.flowSpeed = flowSpeed;
            flowData.brushStrength = brushStrength;
            flowData.flowMapTex = flowMapTex;
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

        void OnBeforeCameraRendering(ScriptableRenderContext context, Camera camera)
        {
            SetParams();
        }

        void OnAfterCameraRendering(ScriptableRenderContext context, Camera camera)
        {
        }

        public void GetMateral(Material material)
        {
            this.material = material;
        }

        private void SetParams()
        {
            material.SetFloat(flowMapSize_Id, areaSize);
            material.SetVector(flowMapOffset_Id, areaPos);
            material.SetFloat(flowMapSpeed_Id, flowSpeed);

            Debug.LogError("设置：areaSize,areaPos,flowSpeed--到材质-----" + "FlowMapMono");
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