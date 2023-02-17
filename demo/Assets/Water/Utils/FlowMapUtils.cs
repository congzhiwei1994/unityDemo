using System.IO;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace Water
{
    public class FlowMapUtils
    {
        public static FlowMapMono GetFlowMapMono(GameObject gameObject)
        {
            var mono = gameObject.GetComponent<FlowMapMono>();
            if (mono == null)
            {
                mono = gameObject.AddComponent<FlowMapMono>();
            }

            return mono;
        }

        public static void SetSceneView()
        {
            var lastScene = SceneView.lastActiveSceneView;
            if (lastScene != null)
            {
                lastScene.sceneViewState.alwaysRefresh = true;
                lastScene.sceneViewState.showSkybox = true;
                lastScene.sceneViewState.showImageEffects = true;
            }
        }

        /// <summary>
        /// Draw FlowMap Area in worldPosition 
        /// </summary>
        public static int SetDrawFlowMapArea(FlowMapData flowData)
        {
            var boundsSize = flowData.bounds.size;
            float maxValue = 0;
            if (boundsSize.x > boundsSize.z)
            {
                maxValue = boundsSize.x;
            }
            else
            {
                maxValue = boundsSize.z;
            }

            return (int)maxValue;
        }

        public static Vector3 GetRayCastPos(float height, Event e)
        {
            var mousePos = e.mousePosition;
            var plane = new Plane(Vector3.down, height);
            var ray = HandleUtility.GUIPointToWorldRay(mousePos);
            if (plane.Raycast(ray, out var distanceToPlane))
            {
                return ray.GetPoint(distanceToPlane);
            }

            return Vector3.positiveInfinity;
        }

        public static void CreatBrushHandle(Event e, int controlId, Vector3 rayCastPos, float brushRadius)
        {
            if (e.control)
                Handles.color = new Color(1, 0, 0);
            else
                Handles.color = new Color(0, 0.8f, 1);

            Handles.CircleHandleCap(controlId, (Vector3)rayCastPos, Quaternion.LookRotation(Vector3.up),
                brushRadius, EventType.Repaint);

            if (e.control)
                Handles.color = new Color(1, 0, 0, 0.2f);
            else
                Handles.color = new Color(0, 0.8f, 1, 0.25f);

            Handles.DrawSolidDisc((Vector3)rayCastPos, Vector3.up, brushRadius);
        }

        public static void CreatWaterHandle(FlowMapData flowData, FlowMapMono flowMapMono)
        {
            var boundsSize = flowData.bounds.size;
            float maxValue = 0;
            if (boundsSize.x > boundsSize.z)
            {
                maxValue = boundsSize.x;
            }
            else
            {
                maxValue = boundsSize.z;
            }

            var flowMapAreaScale = new Vector3(maxValue, 0.5f, maxValue);
            Handles.matrix = Matrix4x4.TRS(flowMapMono.transform.position, Quaternion.identity, flowMapAreaScale);

            Handles.color = new Color(0, 0, 1, 0.2f);
            Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
            Handles.color = new Color(0, 0, 1, 0.9f);
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }


        public static void Save(FlowMapData flowData, FlowMapMono mono, string path, int texSize)
        {
            flowData.brushStrength = mono.brushStrength;
            flowData.texSize = (int)mono.texSize;
            flowData.areaPos = mono.areaPos;

            SaveTexture(flowData, path, texSize);
        }

        private static void SaveTexture(FlowMapData flowData, string path, int texSize)
        {
            Texture2D texture2D = new Texture2D(flowData.texSize, flowData.texSize);

            var tex2D = TextureToTexture2D(flowData.flowTex);
            for (int x = 0; x < flowData.texSize; x++)
            {
                for (int y = 0; y < flowData.texSize; y++)
                {
                    texture2D.SetPixel(x, y, tex2D.GetPixel(x, y));
                }
            }

            texture2D.Apply();

            byte[] dataBytes = texture2D.EncodeToTGA();
            string savePath = path + FlowConstData.FLOW_MAP_NAME + "_" + texSize + ".tga";
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);
            fileStream.Write(dataBytes, 0, dataBytes.Length);
            fileStream.Close();
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        private static Texture2D TextureToTexture2D(Texture texture)
        {
            Texture2D texture2D = new Texture2D(texture.width, texture.height, TextureFormat.RGBA32, false);
            RenderTexture currentRT = RenderTexture.active;
            RenderTexture renderTexture = RenderTexture.GetTemporary(texture.width, texture.height, 32);
            Graphics.Blit(texture, renderTexture);

            RenderTexture.active = renderTexture;
            texture2D.ReadPixels(new Rect(0, 0, renderTexture.width, renderTexture.height), 0, 0);
            texture2D.Apply();

            RenderTexture.active = currentRT;
            RenderTexture.ReleaseTemporary(renderTexture);

            return texture2D;
        }

        public static Material GetFlowMapEditorMaterial()
        {
            return CoreUtils.CreateEngineMaterial(FlowConstData.FLOW_EDITOR_PATH);
        }

        public static string GetCurrentSceneFolder(string scenePath)
        {
            var folder = Path.GetDirectoryName(scenePath);
            folder = folder.Replace('\\', '/');
            folder = folder + "/" + FlowConstData.FOLDER_NAME;
            if (!Directory.Exists(folder))
                Directory.CreateDirectory(folder);

            return folder;
        }

        public static FlowMapData GetFlowMapData(FlowMapMono mono)
        {
            var flowData = mono.gameObject.GetComponent<FlowMapData>();
            if (flowData == null)
            {
                flowData = mono.gameObject.AddComponent<FlowMapData>();
            }

            return flowData;
        }
        
        public static FlowMapData GetFlowMapData(GameObject gameObject)
        {
            var flowData =gameObject.GetComponent<FlowMapData>();
            if (flowData == null)
            {
                flowData = gameObject.AddComponent<FlowMapData>();
            }

            return flowData;
        }
    }
}