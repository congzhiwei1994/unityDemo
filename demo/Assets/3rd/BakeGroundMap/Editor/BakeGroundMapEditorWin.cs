using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEditor.SceneManagement;
using System.IO;

namespace Tools.BakeGroundMap
{
    public class BakeGroundMapEditorWin : EditorWindow
    {
        private GameObject groundGO;
        private bool isLayerFold;
        private LayerMask layerMaskId;
        private BakeGroundMapMono mono;
        private BakeGroundMapUtilities utilities;
        private List<GameObject> grounds;
        private bool showGrounsGUI = false;
        private TexSize _texSize = TexSize._2048;

        private BakeGroundMapAsset _asset;
        private static string currentScene;

        private SerializedObject so;
        private SerializedProperty groundsProp;

        [MenuItem("TA/烘焙地面纹理 &E")]
        public static void OpenWindow()
        {
            var window = (BakeGroundMapEditorWin)GetWindow<BakeGroundMapEditorWin>();
            window.titleContent = new GUIContent("烘焙地面纹理");
            window.Show();
            currentScene = EditorSceneManager.GetActiveScene().path;
        }

        private void OnEnable()
        {
            utilities = new BakeGroundMapUtilities();
            Init();
        }

        private void Init()
        {
            _asset = utilities.GetAsset(EditorSceneManager.GetActiveScene());
            so = new SerializedObject(_asset);
            FindProp();
        }

        private void FindProp()
        {
            groundsProp = so.FindProperty("terrainObjects");
        }


        private void OnDestroy()
        {
        }

        private void OnGUI()
        {
            so.Update();
            if (currentScene != EditorSceneManager.GetActiveScene().path)
            {
                currentScene = EditorSceneManager.GetActiveScene().path;
                Init();
            }

            EditorGUILayout.LabelField("添加地面父物体: ");
            var groundGoOld = (GameObject)EditorGUILayout.ObjectField(groundGO, typeof(GameObject), true);
            if (groundGoOld != groundGO)
            {
                groundGO = groundGoOld;
                grounds = utilities.GetGrounds(groundGO);
                if (grounds == null || grounds.Count == 0)
                    return;
            }

            EditorGUILayout.PropertyField(groundsProp);


            EditorGUILayout.BeginHorizontal();
            {
                GUILayout.FlexibleSpace();
                if (GUILayout.Button("添加", GUILayout.Width(50)))
                {
                    if (groundGO == null)
                        return;
                    mono = utilities.GetMono(groundGO);
                    _asset.SetGrounds(grounds);
                }

                if (GUILayout.Button("Clear", GUILayout.Width(50)))
                {
                    _asset.terrainObjects.Clear();
                }
            }
            EditorGUILayout.EndHorizontal();


            if (GUILayout.Button("计算包围盒"))
            {
                mono.bounds = utilities.GetTerrainBounds(grounds);
                mono.uv = utilities.BoundsToUV(mono.bounds);
                SceneView.RepaintAll();
            }


            isLayerFold = EditorGUILayout.Foldout(isLayerFold, "Enable LayerMask");
            _asset.useLayers = isLayerFold;
            if (isLayerFold)
            {
                var idOld = EditorGUILayout.LayerField("LayerMask", layerMaskId);
                if (idOld != layerMaskId)
                {
                    layerMaskId = idOld;
                    _asset.SetLayerMask(layerMaskId);
                }
            }

            var texSizeOld = (TexSize)EditorGUILayout.EnumPopup("Teture Size", _texSize);
            if (texSizeOld != _texSize)
            {
                _texSize = texSizeOld;
                _asset.resolution = (int)_texSize;
            }

            if (GUILayout.Button("Render"))
            {
                if (mono == null)
                    return;
                utilities.Bake(mono, _asset, EditorSceneManager.GetActiveScene());
            }
        }
    }
}