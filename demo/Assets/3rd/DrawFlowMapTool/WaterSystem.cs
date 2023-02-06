using UnityEditor;
using UnityEngine;

namespace _3rd.DrawFlowMapTool
{
    public class WaterSystem : MonoBehaviour
    {
        private Transform _waterTransform;

        internal static bool IsRTHandleInitialized;
        // -------------------------------------------------------

        // -------------------------------------------------------


#if UNITY_EDITOR
        [MenuItem("GameObject/Effects/Water System")]
        static void CreateWaterSystemEditor(MenuCommand menuCommand)
        {
            var go = new GameObject("Water System");
            go.transform.position = SceneView.lastActiveSceneView.camera.transform.TransformPoint(Vector3.forward * 3f);
            go.AddComponent<WaterSystem>();
            GameObjectUtility.SetParentAndAlign(go, menuCommand.context as GameObject);
            Undo.RegisterCreatedObjectUndo(go, "Create " + go.name);
            Selection.activeObject = go;
        }
#endif

        private void OnEnable()
        {
            _waterTransform = transform;

            if (!WaterSystem.IsRTHandleInitialized)
            {
                // 获得屏幕的缩放比例
                var screenSize = KWS_CoreUtils.GetScreenSizeLimited();
                WaterSystem.IsRTHandleInitialized = true;
                // KWS_RTHandles.Initialize(screenSize.x, screenSize.y, false, KWS.MSAASamples.None);
            }
            //
            // SubscribeBeforeCameraRendering();
            // SubscribeAfterCameraRendering();
        }
    }
}