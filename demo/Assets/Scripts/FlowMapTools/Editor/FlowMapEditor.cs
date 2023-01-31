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
        private FlowMap _flowMap;
        private Event _event;

        private bool isDrawFlowMap = false;
        private bool isSave = false;
        private bool isClear = false;
        private Vector3 position = Vector3.zero;
        private float flowSpeed = 0.5f;

        private void OnEnable()
        {
            isDrawFlowMap = false;
        }

        public override void OnInspectorGUI()
        {
            _flowMap = (FlowMap)target;
            if (!_flowMap.enabled && !_flowMap.gameObject.activeSelf)
                return;

            SetSceneViewState();


            DrwaGUI();
        }

        private void DrwaGUI()
        {
            isDrawFlowMap = GUILayout.Toggle(isDrawFlowMap, "Flowmap Painter", "Button");
            if (!isDrawFlowMap)
                return;

            EditorGUILayout.HelpBox(EditorDescription.FlowingEditorUsage, MessageType.Info);

            // 位置
            position = FlowMapViewUtils.Vector3GUI("FlowMap Area Position", position);


            EditorGUILayout.BeginHorizontal();
            {
                if (GUILayout.Toggle(isSave, "Save", "Button"))
                {
                }

                if (GUILayout.Toggle(isClear, "Clear", "Button"))
                {
                }
            }
            EditorGUILayout.EndHorizontal();
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
        }
    }
}