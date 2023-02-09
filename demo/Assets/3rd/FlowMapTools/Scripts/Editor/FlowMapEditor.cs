using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEditor;

namespace czw.FlowMapTool
{
    [System.Serializable]
    [CustomEditor(typeof(FlowMapMono))]
    public class FlowMapEditor : Editor
    {
        private FlowMapMono mono;
        private Event _event;
        private FlowMapSetting setting;
        private bool isDrawFlowMap = false;

        private FlowMapResolution _resolution = FlowMapResolution._1024;
        private Vector3 areaPos = Vector3.zero;
        private int areaSize = 50;
        private float flowSpeed = 0.5f;
        private float brushStrength = 0.3f;
        public Texture flowMapTex;
        private FlowData data;

        private bool isDataInit;
        private Material waterMat;

        private void OnEnable()
        {
            mono = (FlowMapMono)target;
            isDataInit = false;
            Init();
        }

        private void Init()
        {
            GetWaterMaterial();
            mono.GetMateral(waterMat);
            setting = new FlowMapSetting(mono);

            InitFlowData();
            SceneView.duringSceneGui += SceneViewEvent;
        }

        private void InitFlowData()
        {
            if (data == null)
            {
                var getData = mono.gameObject.GetComponent<FlowData>();
                if (getData == null)
                {
                    getData = mono.gameObject.AddComponent<FlowData>();
                }

                data = getData;
            }

            _resolution = data.resolution;
            areaPos = data.areaPos;
            areaSize = data.areaSize;
            flowSpeed = data.flowSpeed;
            brushStrength = data.brushStrength;
            flowMapTex = data.flowMapTex;

            isDataInit = true;
        }

        private void GetWaterMaterial()
        {
            if (waterMat == null)
            {
                waterMat = mono.gameObject.GetComponent<Renderer>().sharedMaterial;
            }
        }

        public override void OnInspectorGUI()
        {
            if (!mono.enabled && !mono.gameObject.activeSelf)
            {
                return;
            }

            setting.SetSceneViewState();
            OnDrawGUI();
        }

        private void OnDrawGUI()
        {
            isDrawFlowMap = GUILayout.Toggle(isDrawFlowMap, "Flowmap Painter", "Button");
            if (!isDrawFlowMap)
            {
                isDataInit = false;
                return;
            }

            /*          else
                      {
                          SceneView.lastActiveSceneView.LookAt(flowMap.areaPos);
                      }
          */

            if (!isDataInit)
            {
                Init();
            }

            EditorGUILayout.HelpBox(EditorDescription.FlowingEditorUsage, MessageType.Info);

            EditorGUI.BeginChangeCheck();

            var posOld = FlowMapViewUtils.Vector3GUI("FlowMap Area Position", areaPos);
            if (posOld != areaPos)
            {
                areaPos = posOld;
                mono.areaPos = areaPos;
            }

            var sizeOld = FlowMapViewUtils.IntSliderGUI("Flow Area Size", areaSize, 10, 2000);
            if (sizeOld != areaSize)
            {
                areaSize = sizeOld;
                mono.areaSize = areaSize;
            }

            var flowSpeedOld = FlowMapViewUtils.SliderGUI("Flow Speed", flowSpeed, 0, 1);
            if (flowSpeedOld != flowSpeed)
            {
                flowSpeed = flowSpeedOld;
                mono.flowSpeed = flowSpeed;
            }

            if (EditorGUI.EndChangeCheck())
            {
                mono.areaSize = areaSize;
            }

            var brushStrengthOld = FlowMapViewUtils.SliderGUI("Brush Strength", brushStrength, 0, 1);
            if (brushStrength != brushStrengthOld)
            {
                brushStrength = brushStrengthOld;
                mono.brushStrength = brushStrength;
            }

            var _resolutionOld = (FlowMapResolution)FlowMapViewUtils.EnumPopupGUI("Flow Map resolution", _resolution);
            if (_resolutionOld != _resolution)
            {
                _resolution = _resolutionOld;
                mono.resolution = _resolution;
            }


            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Load Last Save"))
                {
                    /*        if (EditorUtility.DisplayDialog("提示：", "是否加载上一次保存的纹理?", "Yes", "No"))
                            {
                            }
        */
                    Init();
                }

                if (GUILayout.Button("Save"))
                {
                    mono.Save(data);
                }

                if (GUILayout.Button("Clear"))
                {
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

            setting.Setup(Event.current, mono, this, controlId);
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= SceneViewEvent;
        }
    }
}