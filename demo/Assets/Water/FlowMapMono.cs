using System;
using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;
using UnityEngine.Serialization;


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
    public partial class FlowMapMono : MonoBehaviour
    {
        private Transform waterTran;
        private KW_FlowMap _flowMapRenderSetting;

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
            _flowMapRenderSetting = waterTran.gameObject.GetComponent<KW_FlowMap>();
            if (_flowMapRenderSetting == null)
            {
                _flowMapRenderSetting = waterTran.gameObject.AddComponent<KW_FlowMap>();
            }
        
            flowMapData = waterTran.gameObject.GetComponent<FlowMapData>();
            if (flowMapData == null)
            {
                flowMapData = waterTran.gameObject.AddComponent<FlowMapData>();
            }
        }

        private void BeforeCameraRendering(Camera cam)
        {
            Profiler.BeginSample("Water.Rendering");
            RenderWater();
            Profiler.EndSample();
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


        private void RenderWater()
        {
            // ReadFlowMap();
            UpdateShaderParameters();
        }


        private void UpdateShaderParameters()
        {
            waterMaterial.SetFloat(FlowMapSize, areaSize);
            waterMaterial.SetVector(FlowMapOffset, areaPos);
            SetWaterMat();
        }

        private void SetWaterMat()
        {
            waterTran.gameObject.GetComponent<Renderer>().sharedMaterial = waterMaterial;
        }


        public void DrawOnFlowMap(Vector3 brushPosition, Vector3 brushMoveDirection, float circleRadius,
            float brushStrength, bool eraseMode = false)
        {
            InitializeFlowMapEditorResources();
            _flowMapRenderSetting.DrawOnFlowMap(brushPosition, brushMoveDirection, circleRadius, brushStrength, eraseMode);
        }

        void InitializeFlowMapEditorResources()
        {
            _flowMapRenderSetting.InitializeFlowMapEditorResources((int)texSize, 500);
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