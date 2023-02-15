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
        private Transform waterTran;

        public FlowMapSizeEnum texSize = FlowMapSizeEnum._2048;
        public int areaSize = 200;
        public Vector3 areaPos = new Vector3(0, 0, 0);
        public float flowSpeed = 1;
        public float brushStrength = 1;


        private Material waterMaterial;
        public static readonly int FlowMapSize = Shader.PropertyToID("_KW_FlowMapSize");
        public static readonly int FlowMapOffset = Shader.PropertyToID("_KW_FlowMapOffset");
        private FlowMapData flowMapData;

        private void OnEnable()
        {
            waterTran = transform;
#if UNITY_EDITOR
            Init();

            RenderPipelineManager.beginCameraRendering += OnBeginCameraRenderingManager;
            RenderPipelineManager.endCameraRendering += OnEndCameraRenderingManager;
#endif
        }

        private void Init()
        {
            waterMaterial = waterTran.gameObject.GetComponent<Renderer>().sharedMaterial;

            flowMapData = waterTran.gameObject.GetComponent<FlowMapData>();
            if (flowMapData == null)
            {
                flowMapData = waterTran.gameObject.AddComponent<FlowMapData>();
            }
        }

        private void BeforeCameraRendering(Camera cam)
        {
            waterMaterial.SetFloat(FlowMapSize, areaSize);
            waterMaterial.SetVector(FlowMapOffset, areaPos);
            waterTran.gameObject.GetComponent<Renderer>().sharedMaterial = waterMaterial;
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