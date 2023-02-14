using czw.FlowMapTool;
using UnityEditor;
using UnityEngine;

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


        void OnEnable()
        {
            mono = (FlowMapMono)target;
            Init();
            SceneView.duringSceneGui += OnSceneGUIEvent;
        }

        private void Init()
        {
            InitFlowData();
            flowmapSetting = new FlowmapSetting(flowData);
        }

        private void InitFlowData()
        {
            if (flowData == null)
            {
                flowData = mono.gameObject.GetComponent<FlowMapData>();
                if (flowData == null)
                {
                    flowData = mono.gameObject.AddComponent<FlowMapData>();
                }
            }

            areaPos = flowData.areaPos;
            areaSize = flowData.areaSize;
            texSize = (FlowMapSizeEnum)flowData.texSize;
            flowSpeed = flowData.flowSpeed;
            brushStrength = flowData.brushStrength;
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

        void OnSceneGUIEvent(SceneView sceneView)
        {
            DrawWaterEvent();
        }

        void DrawGUI()
        {
            var lastScene = SceneView.lastActiveSceneView;
            if (lastScene != null)
            {
                lastScene.sceneViewState.alwaysRefresh = true;
                lastScene.sceneViewState.showSkybox = true;
                lastScene.sceneViewState.showImageEffects = true;
            }

            // isDraw = EditorGUILayout.Toggle("FlowMap Painter", isDraw, "Button");
            // if (!isDraw)
            // {
            //     return;
            // }

            var areaPosOld = FlowMapViewUtils.Vector3GUI("FlowMap Area Position", areaPos);
            if (areaPosOld != areaPos)
            {
                areaPos = areaPosOld;
                mono.areaPos = areaPos;
            }

            var areaSizeOld = FlowMapViewUtils.IntSliderGUI("Flow Area Size", areaSize, 10, 2000);
            if (areaSize != areaSizeOld)
            {
                areaSize = areaSizeOld;
                mono.areaSize = areaSize;
            }

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
                    InitFlowData();
                }

                if (GUILayout.Button("Save"))
                {
                    if (flowData.flowTex == null)
                    {
                        return;
                    }

                    flowmapSetting.Save();
                }

                if (GUILayout.Button("Clear"))
                {
                    flowData.flowTex = null;
                    InitFlowData();
                }
            }
            EditorGUILayout.EndHorizontal();
        }


        void DrawWaterEvent()
        {
            flowmapSetting.DrawFlowMapEditor(mono, this);
        }
    }
}