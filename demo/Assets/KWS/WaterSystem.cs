using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;
using UnityEngine.Profiling;
using UnityEngine.Rendering;

namespace KWS
{
    public enum FlowmapTextureResolutionEnum
    {
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
    }


    [ExecuteAlways]
    [Serializable]
    public partial class WaterSystem : MonoBehaviour
    {
        public Vector3 FlowMapAreaPosition = new Vector3(0, 0, 0);
        public int FlowMapAreaSize = 200;
        public FlowmapTextureResolutionEnum FlowMapTextureResolution = FlowmapTextureResolutionEnum._2048;
        public float FlowMapBrushStrength = 0.75f;

        private KW_FlowMapData flowData;

        private Transform _waterTransform;
        private Camera _currentCamera;
        private Material waterMaterial;
        public static readonly int KW_FlowMapSize = Shader.PropertyToID("KW_FlowMapSize");
        public static readonly int KW_FlowMapOffset = Shader.PropertyToID("KW_FlowMapOffset");

        private KW_FlowMap flowMap;

// #if UNITY_EDITOR
//         [MenuItem("GameObject/Effects/Water System")]
//         static void CreateWaterSystemEditor(MenuCommand menuCommand)
//         {
//             var go = new GameObject("Water System");
//             go.transform.position = SceneView.lastActiveSceneView.camera.transform.TransformPoint(Vector3.forward * 3f);
//             go.AddComponent<WaterSystem>();
//             GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
//             Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
//             Selection.activeObject = go;
//         }
// #endif

        private void OnEnable()
        {
            waterMaterial = this.GetComponent<Renderer>().sharedMaterial;

            flowData = this.GetComponent<KW_FlowMapData>();
            if (flowData == null)
            {
                flowData = this.gameObject.AddComponent<KW_FlowMapData>();
            }

            flowMap = this.GetComponent<KW_FlowMap>();
            if (flowMap == null)
            {
                flowMap = this.AddComponent<KW_FlowMap>();
            }

            flowMap.Init(flowData);

            _waterTransform = transform;
            SubscribeBeforeCameraRendering();
            SubscribeAfterCameraRendering();
        }

        void OnDisable()
        {
            UnsubscribeBeforeCameraRendering();
            UnsubscribeAfterCameraRendering();
        }


        void OnBeforeCameraRendering(Camera cam)
        {
            _currentCamera = cam;

            Profiler.BeginSample("Water.Rendering");
            RenderWater();
            Profiler.EndSample();
        }

        void OnAfterCameraRendering(Camera cam)
        {
            if (cam.cameraType != CameraType.Game && cam.cameraType != CameraType.SceneView) return;
        }

        void SubscribeBeforeCameraRendering()
        {
            RenderPipelineManager.beginCameraRendering += RenderPipelineManagerOnbeginCameraRendering;
        }

        void UnsubscribeBeforeCameraRendering()
        {
            RenderPipelineManager.beginCameraRendering -= RenderPipelineManagerOnbeginCameraRendering;
        }

        private void RenderPipelineManagerOnbeginCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            OnBeforeCameraRendering(cam);
        }

        void SubscribeAfterCameraRendering()
        {
            RenderPipelineManager.endCameraRendering += RenderPipelineManagerOnendCameraRendering;
        }

        void UnsubscribeAfterCameraRendering()
        {
            RenderPipelineManager.endCameraRendering -= RenderPipelineManagerOnendCameraRendering;
        }

        private void RenderPipelineManagerOnendCameraRendering(ScriptableRenderContext ctx, Camera cam)
        {
            OnAfterCameraRendering(cam);
        }

        void RenderWater()
        {
            ReadFlowMap();
            UpdateShaderParameters();
        }

        void ReadFlowMap()
        {
            if (flowData == null)
            {
                Debug.LogError("缺少FlowMap Data");
            }

            FlowMapTextureResolution = (FlowmapTextureResolutionEnum)flowData.TextureSize;
            FlowMapAreaSize = flowData.AreaSize;
        }


        public void UpdateShaderParameters()
        {
            waterMaterial.SetFloat(KW_FlowMapSize, 2000);
            waterMaterial.SetVector(KW_FlowMapOffset, FlowMapAreaPosition);

            this.GetComponent<Renderer>().sharedMaterial = waterMaterial;
        }


        ////////////////////////////////////////////////////////////////////////////////////////
        public void DrawOnFlowMap(Vector3 brushPosition, Vector3 brushMoveDirection, float circleRadius,
            float brushStrength, bool eraseMode = false)
        {
            flowMap.InitializeFlowMapEditorResources(2048, 200);
            flowMap.DrawOnFlowMap(brushPosition, brushMoveDirection, circleRadius, brushStrength, eraseMode);
        }
    }
}