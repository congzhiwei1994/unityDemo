using System;
using UnityEngine;
using UnityEngine.Rendering;

namespace Water
{
    public enum FlowMapSizeEnum
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
        public FlowMapSizeEnum texSize = FlowMapSizeEnum._2048;
        public int areaSize = 200;
        public Vector3 areaPos = new Vector3(0, 0, 0);
        public Texture flowTex;
        public float flowSpeed = 1;
        public float brushStrength = 1;

        private Material waterMaterial;

        private FlowMapData flowMapData;

        private void OnEnable()
        {
#if UNITY_EDITOR
            Init();

            RenderPipelineManager.beginCameraRendering += OnBeginCameraRenderingManager;
            RenderPipelineManager.endCameraRendering += OnEndCameraRenderingManager;
#endif
        }

        private void Init()
        {
            waterMaterial = this.gameObject.GetComponent<Renderer>().sharedMaterial;
            FlowMapData();
        }

        private void BeforeCameraRendering(Camera cam)
        {
            SetWaterParams();
        }

        private void AfterCameraRendering(Camera cam)
        {
            if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView)
                return;
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            RenderPipelineManager.beginCameraRendering -= OnBeginCameraRenderingManager;
            RenderPipelineManager.endCameraRendering -= OnEndCameraRenderingManager;
#endif
        }

        private void FlowMapData()
        {
            flowMapData = this.gameObject.GetComponent<FlowMapData>();
            if (flowMapData == null)
            {
                flowMapData = this.gameObject.AddComponent<FlowMapData>();
            }
        }

        private void SetWaterParams()
        {
            waterMaterial.SetFloat(FlowConstData.FlowMapSize, areaSize);
            waterMaterial.SetVector(FlowConstData.FlowMapOffset, areaPos);
            this.gameObject.GetComponent<Renderer>().sharedMaterial = waterMaterial;
        }

        #region 事件

        private void OnBeginCameraRenderingManager(ScriptableRenderContext ctx, Camera cam)
        {
            BeforeCameraRendering(cam);
        }

        private void OnEndCameraRenderingManager(ScriptableRenderContext ctx, Camera cam)
        {
            AfterCameraRendering(cam);
        }

        #endregion
    }
}