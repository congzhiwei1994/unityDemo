using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

namespace czw.FlowMapTool
{
    [System.Serializable]
    [CustomEditor(typeof(FlowMap))]
    public class FlowMapEditor : Editor
    {
        private FlowMap flowMapMono;
        private Event _event;
        private FlowMapEditorSetting editorSetting;
        private bool isDrawFlowMap = false;

        private FlowMapResolution _resolution = FlowMapResolution._1024;
        private Vector3 areaPos = Vector3.zero;
        private int areaSize = 50;
        private float flowSpeed = 0.5f;
        private float brushStrength = 0.3f;
        public Texture flowMapTex;
        private WaterSetting waterSetting;

        private bool isInit;

        private void OnEnable()
        {
            isInit = false;

            flowMapMono = (FlowMap)target;
            editorSetting = new FlowMapEditorSetting();
            Init();

            SceneView.duringSceneGui += SceneViewEvent;
        }

        private void Init()
        {
            Debug.LogError("FlowMapEditor------Init()");

            if (waterSetting == null)
            {
                var getWaterSetting = flowMapMono.gameObject.GetComponent<WaterSetting>();
                if (getWaterSetting == null)
                {
                    getWaterSetting = flowMapMono.gameObject.AddComponent<WaterSetting>();
                }

                waterSetting = getWaterSetting;
            }

            _resolution = waterSetting.resolution;
            areaPos = waterSetting.areaPos;
            areaSize = waterSetting.areaSize;
            flowSpeed = waterSetting.flowSpeed;
            brushStrength = waterSetting.brushStrength;
            flowMapTex = waterSetting.flowMapTex;

            isInit = true;
        }


        public override void OnInspectorGUI()
        {
            if (!flowMapMono.enabled && !flowMapMono.gameObject.activeSelf)
            {
                return;
            }

            editorSetting.SetSceneViewState();
            OnDrawGUI();
        }

        private void OnDrawGUI()
        {
            isDrawFlowMap = GUILayout.Toggle(isDrawFlowMap, "Flowmap Painter", "Button");
            if (!isDrawFlowMap)
            {
                isInit = false;
                return;
            }
               
            /*          else
                      {
                          SceneView.lastActiveSceneView.LookAt(flowMap.areaPos);
                      }
          */

            if (!isInit)
            {
                Init();
            }

            EditorGUILayout.HelpBox(EditorDescription.FlowingEditorUsage, MessageType.Info);

            EditorGUI.BeginChangeCheck();

            var posOld = FlowMapViewUtils.Vector3GUI("FlowMap Area Position", areaPos);
            if (posOld != areaPos)
            {
                areaPos = posOld;
                flowMapMono.areaPos = areaPos;
            }

            var sizeOld = FlowMapViewUtils.IntSliderGUI("Flow Area Size", areaSize, 10, 2000);
            if (sizeOld != areaSize)
            {
                areaSize = sizeOld;
                flowMapMono.areaSize = areaSize;
            }

            var flowSpeedOld = FlowMapViewUtils.SliderGUI("Flow Speed", flowSpeed, 0, 1);
            if (flowSpeedOld != flowSpeed)
            {
                flowSpeed = flowSpeedOld;
                flowMapMono.flowSpeed = flowSpeed;
            }

            if (EditorGUI.EndChangeCheck())
            {
                flowMapMono.areaSize = areaSize;
            }

            var brushStrengthOld = FlowMapViewUtils.SliderGUI("Brush Strength", brushStrength, 0, 1);
            if (brushStrength != brushStrengthOld)
            {
                brushStrength = brushStrengthOld;
                flowMapMono.brushStrength = brushStrength;
            }

            var _resolutionOld = (FlowMapResolution)FlowMapViewUtils.EnumPopupGUI("Flow Map resolution", _resolution);
            if (_resolutionOld != _resolution)
            {
                _resolution = _resolutionOld;
                flowMapMono.resolution = _resolution;
            }


            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Load Last Save"))
                {
                    // if (EditorUtility.DisplayDialog("提示：", "是否加载上一次保存的纹理?", "Yes", "No"))
                    // {
                    //   
                    // }
                    //
                    Init();
                }

                if (GUILayout.Button("Save"))
                {
                    flowMapMono.Save(waterSetting);
                }

                if (GUILayout.Button("Clear"))
                {
                    editorSetting.Clear();
                }
            }
            EditorGUILayout.EndHorizontal();
        }

        
        private void SceneViewEvent(SceneView sceneView)
        {
            if (!isDrawFlowMap)
                return;

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);

            editorSetting.Setup(Event.current, flowMapMono, this, controlId);
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= SceneViewEvent;
        }
    }
}