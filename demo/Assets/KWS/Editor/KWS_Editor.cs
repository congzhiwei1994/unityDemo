using UnityEditor;
using UnityEngine;

namespace KWS
{
    [System.Serializable]
    [CustomEditor(typeof(WaterSystem))]
    public class KWS_Editor : Editor
    {
        private KWS_EditorFlowmap flowmapEditor = new KWS_EditorFlowmap();
        private WaterSystem _waterSystem;
        private bool isFlowMapPainter = false;

        void OnEnable()
        {
            _waterSystem = (WaterSystem)target;

            SceneView.duringSceneGui += OnSceneGUICustom;
        }


        void OnDisable()
        {
            SceneView.duringSceneGui -= OnSceneGUICustom;
        }

        void OnSceneGUICustom(SceneView sceneView)
        {
            DrawWaterEditor();
        }

        void DrawWaterEditor()
        {
            flowmapEditor.DrawFlowMapEditor(_waterSystem, this);
        }

        public override void OnInspectorGUI()
        {
            _waterSystem = (WaterSystem)target;

            UpdateWaterGUI();
        }

        void UpdateWaterGUI()
        {
            var lastScene = SceneView.lastActiveSceneView;
            if (lastScene != null)
            {
                lastScene.sceneViewState.alwaysRefresh = true;
                lastScene.sceneViewState.showSkybox = true;
                lastScene.sceneViewState.showImageEffects = true;
                lastScene.sceneLighting = true;
            }

            isFlowMapPainter = GUILayout.Toggle(isFlowMapPainter, "Flowmap Painter", "Button");
            if (!isFlowMapPainter)
            {
                return;
            }
            

            _waterSystem.FlowMapAreaPosition =
                EditorGUILayout.Vector3Field("FlowMap Area Position", _waterSystem.FlowMapAreaPosition);

            var newAreaSize = EditorGUILayout.IntSlider(new GUIContent("Flowmap Area Size"),
                _waterSystem.FlowMapAreaSize, 10, 2000);

            _waterSystem.FlowMapTextureResolution =
                (FlowmapTextureResolutionEnum)EditorGUILayout.EnumPopup("Flowmap resolution",
                    _waterSystem.FlowMapTextureResolution);

            _waterSystem.FlowMapBrushStrength =
                EditorGUILayout.Slider("Brush Strength", _waterSystem.FlowMapBrushStrength, 0.01f, 1);



            if (GUILayout.Button("Load Latest Saved"))
            {
            }

            if (GUILayout.Button("Delete All"))
            {
            }

            if (GUILayout.Button("Save All"))
            {
            }
        }
    }
}