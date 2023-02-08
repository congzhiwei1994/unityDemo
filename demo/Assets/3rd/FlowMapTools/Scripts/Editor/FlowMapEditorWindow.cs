using System;
using Unity.VisualScripting;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace czw.FlowMapTool
{
    public class FlowMapEditorWindow : EditorWindow
    {
        private static string currentScene;
        
        [MenuItem("TA/FlowMapTool &Q")]
        public static void OpenWindow()
        {
            var window = (FlowMapEditorWindow)GetWindow<FlowMapEditorWindow>();
            window.titleContent = new GUIContent("FlowMapTool");
            window.Show();
            currentScene = EditorSceneManager.GetActiveScene().path;
        }

        private void OnEnable()
        {
            
        }

        private void Init()
        {
            
        }

        private void OnGUI()
        {
            
        }


        private void OnDisable()
        {
            
        }
    }
}