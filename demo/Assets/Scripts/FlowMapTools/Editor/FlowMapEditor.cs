using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace czw.FlowMapTool
{
    [System.Serializable]
    [CustomEditor(typeof(FlowMap))]
    public class FlowMapEditor : Editor
    {
        private FlowMap flowMap;
        private Event _event;
        private FlowMapEditorSetting editorSetting;
        private bool isDrawFlowMap = false;
        private FlowMapResolution _resolution = FlowMapResolution._1024;
        private Vector3 areaPos = Vector3.zero;
        private int areaSize = 50;
        private float flowSpeed = 0.5f;
        private float brushStrength = 0.3f;

        private void OnEnable()
        {
            editorSetting = new FlowMapEditorSetting();
            SceneView.duringSceneGui += SceneViewEvent;
        }


        public override void OnInspectorGUI()
        {
            flowMap = (FlowMap)target;
            if (!flowMap.enabled && !flowMap.gameObject.activeSelf)
            {
                return;
            }

            SetSceneViewState();
            DrawGUI();
        }

        private void DrawGUI()
        {
            isDrawFlowMap = GUILayout.Toggle(isDrawFlowMap, "Flowmap Painter", "Button");
            if (!isDrawFlowMap)
                return;

            EditorGUILayout.HelpBox(EditorDescription.FlowingEditorUsage, MessageType.Info);
            
            var posOld = FlowMapViewUtils.Vector3GUI("FlowMap Area Position", areaPos);
            if (posOld != areaPos)
            {
                areaPos = posOld;
                flowMap.areaPos = areaPos;
            }

            var sizeOld = FlowMapViewUtils.IntSliderGUI("Flow Area Size", areaSize, 10, 2000);
            if (sizeOld != areaSize)
            {
                areaSize = sizeOld;
                flowMap.areaSize = areaSize;
            }

            flowSpeed = FlowMapViewUtils.SliderGUI("Flow Speed", flowSpeed, 0, 1);

            brushStrength = FlowMapViewUtils.SliderGUI("Brush Strength", brushStrength, 0, 1);

            _resolution = (FlowMapResolution)FlowMapViewUtils.EnumPopupGUI("Flow Map resolution", _resolution);

            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Button("Load Last Save"))
                {
                    if (EditorUtility.DisplayDialog("提示：", "是否加载上一次保存的纹理?", "Yes", "No"))
                    {
                    }
                }

                if (GUILayout.Button("Save"))
                {
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

            editorSetting.Init(Event.current, flowMap, this);
            editorSetting.DrawSetting();
        }

        private void SetSceneViewState()
        {
            var lastScene = SceneView.lastActiveSceneView;
            if (lastScene != null)
            {
                lastScene.sceneViewState.alwaysRefresh = true;
                lastScene.sceneViewState.showSkybox = true;
                lastScene.sceneViewState.showImageEffects = true;
            }
        }

        private void OnDisable()
        {
            SceneView.duringSceneGui -= SceneViewEvent;
        }
    }
}