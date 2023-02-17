using System;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;
using czw.FlowMapTool;

namespace Water.Editor
{
    public class FlowMapEditorWin : EditorWindow
    {
        #region 字段

        private static string currentScene;
        private FlowMapSizeEnum texSize = FlowMapSizeEnum._1024;
        private GameObject waterGameObject;
        private FlowMapMono mono;
        private FlowMapData flowData;
        private float brushStrength;
        private float flowSpeed;

        #endregion


        [MenuItem("TA/FlowMapTool &Q")]
        public static void Open()
        {
            var window = (FlowMapEditorWin)GetWindow<FlowMapEditorWin>();
            window.titleContent = new GUIContent("FlowMapTool");
            window.Show();
            currentScene = EditorSceneManager.GetActiveScene().path;
        }

        private void OnEnable()
        {
            SceneView.duringSceneGui += OnSceneGUIEvent;
        }

        private void Reset()
        {
            texSize = (FlowMapSizeEnum)flowData.texSize;
            flowSpeed = flowData.flowSpeed;
            brushStrength = flowData.brushStrength;
        }

        private void OnGUI()
        {
            var watertOld =
                (GameObject)EditorGUILayout.ObjectField("Water", waterGameObject, (typeof(GameObject)), true);
            if (watertOld != waterGameObject)
            {
                waterGameObject = watertOld;
                mono = FlowMapUtils.GetFlowMapMono(waterGameObject);
                flowData = FlowMapUtils.GetFlowMapData(waterGameObject);

                Reset();
            }

            var texSizeOld = (FlowMapSizeEnum)FlowMapViewUtils.EnumPopupGUI("Flow Map Size", texSize);
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
                    // LoadFlowData();
                }

                if (GUILayout.Button("Save"))
                {
                    // if (flowData.flowTex == null)
                    // {
                    //     return;
                    // }
                    //
                    // FlowMapUtils.Save(flowData, mono, flowMapPath, (int)mono.texSize);
                }

                if (GUILayout.Button("Clear"))
                {
                    // flowData.flowTex = null;
                    // LoadFlowData();
                }
            }
        }


        private void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUIEvent;

            if (mono != null && waterGameObject != null)
            {
                GameObject.DestroyImmediate(mono);
            }
        }


        private void OnSceneGUIEvent(SceneView sceneView)
        {
        }
    }
}