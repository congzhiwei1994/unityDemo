using System.IO;
using czw.FlowMapTool;
using UnityEditor;
using UnityEngine;
using UnityEditor.SceneManagement;

namespace Water.Editor
{
    [System.Serializable]
    [CustomEditor(typeof(FlowMapMono))]
    public class FlowMapEditor : UnityEditor.Editor
    {
        private FlowMapMono mono;
        private FlowmapSetting flowmapSetting;
        private bool _isActive;
        private bool isDraw = false;
        private FlowMapData flowData;
        private Vector3 areaPos = Vector3.zero;
        private int areaSize = 500;
        private FlowMapSizeEnum texSize = FlowMapSizeEnum._1024;
        private float flowSpeed = 0.5f;
        private float brushStrength = 1;
        private FlowMapRenderSetting renderSetting;
        private bool isInit = false;
        private bool isGetPath = false;
        private string flowMapPath;

        void OnEnable()
        {
            mono = (FlowMapMono)target;
            flowData = FlowMapUtils.GetFlowMapData(mono);
            renderSetting = new FlowMapRenderSetting(mono, flowData);
            flowmapSetting = new FlowmapSetting(flowData, mono, renderSetting);
            SceneView.duringSceneGui += OnSceneGUIEvent;
        }


        public override void OnInspectorGUI()
        {
            if (mono.enabled && mono.gameObject.activeSelf)
            {
                _isActive = true;
                GUI.enabled = true;
            }
            else
            {
                _isActive = false;
                GUI.enabled = false;
            }

            DrawGUI();
        }

        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUIEvent;
        }


        void DrawGUI()
        {
            FlowMapUtils.SetSceneView();

            isDraw = GUILayout.Toggle(isDraw, "FlowMap Painter", "Button");
            if (!isDraw)
            {
                isGetPath = false;
                return;
            }

            if (!isGetPath)
            {
                flowMapPath = FlowMapUtils.GetCurrentSceneFolder(EditorSceneManager.GetActiveScene().path);
                isGetPath = true;
            }


            mono.areaPos = mono.transform.position;
            mono.areaSize = FlowMapUtils.SetDrawFlowMapArea(flowData);

            var texSizeOld = (FlowMapSizeEnum)FlowMapViewUtils.EnumPopupGUI("Flow Map resolution", texSize);
            if (texSize != texSizeOld)
            {
                texSize = texSizeOld;
                mono.texSize = texSize;
            }

            var flowSpeedOld = FlowMapViewUtils.SliderGUI("Flow Speed", flowSpeed, 0, 1);
            if (flowSpeed != flowSpeedOld)
            {
                flowSpeed = flowSpeedOld;
                mono.flowSpeed = flowSpeed;
            }

            var brushStrengthOld = FlowMapViewUtils.SliderGUI("Brush Strength", brushStrength, 0, 1);
            if (brushStrength != brushStrengthOld)
            {
                brushStrength = brushStrengthOld;
                mono.brushStrength = brushStrength;
            }

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Reset"))
                {
                    LoadFlowData();
                }

                if (GUILayout.Button("Save"))
                {
                    if (flowData.flowTex == null)
                    {
                        return;
                    }

                    FlowMapUtils.Save(flowData, mono, flowMapPath, (int)mono.texSize);
                }

                if (GUILayout.Button("Clear"))
                {
                    flowData.flowTex = null;
                    LoadFlowData();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        private void OnSceneGUIEvent(SceneView sceneView)
        {
            if (!isDraw || !_isActive)
                return;

            flowmapSetting.DrawFlowMapEditor(mono, this);
        }

        private void LoadFlowData()
        {
            texSize = (FlowMapSizeEnum)flowData.texSize;
            flowSpeed = flowData.flowSpeed;
            brushStrength = flowData.brushStrength;
        }
    }
}