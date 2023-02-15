using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Water.Editor
{
    public class FlowmapSetting
    {
        private float brushRadius = 2f;
        private bool leftKeyPressed;
        private bool isFlowMapChanged;
        private Vector3 flowMapLastPos = Vector3.positiveInfinity;
        private FlowMapData flowData;
        private FlowMapMono mono;
        private FlowMapRenderSetting renderSetting;

        public FlowmapSetting(FlowMapData flowData, FlowMapMono mono, FlowMapRenderSetting renderSetting)
        {
            this.flowData = flowData;
            this.mono = mono;
            this.renderSetting = renderSetting;
        }

        public Vector3 GetMouseWorldPosProjectedToWater(float height, Event e)
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


        public void DrawFlowMapEditor(FlowMapMono flowMapMono, UnityEditor.Editor editor)
        {
            if (Application.isPlaying)
                return;
            var waterPos = flowMapMono.transform.position;

            var e = Event.current;
            if (e.type == EventType.ScrollWheel)
            {
                brushRadius -= (e.delta.y * brushRadius) / 40f;
                brushRadius =
                    Mathf.Clamp(brushRadius, 0.1f, flowMapMono.areaSize);
            }

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);
            if (e.type == EventType.ScrollWheel)
                e.Use();
            // 射线相交
            var rayCastPos = GetMouseWorldPosProjectedToWater(flowMapMono.transform.position.y, e);
            if (float.IsInfinity(rayCastPos.x))
                return;

            var flowPos = new Vector3(-waterPos.x, 0, -waterPos.z) + (Vector3)rayCastPos;

            CreatBrushHandle(e, controlId, rayCastPos);
            CreatWaterHandle(flowMapMono);
            CreatWaterHandle1( flowMapMono);
            if (Event.current.button == 0)
            {
                if (e.type == EventType.MouseDown)
                    leftKeyPressed = true;

                if (e.type == EventType.MouseUp)
                {
                    leftKeyPressed = false;
                    isFlowMapChanged = true;
                    flowMapLastPos = Vector3.positiveInfinity;

                    editor.Repaint();
                }
            }

            if (leftKeyPressed)
            {
                if (float.IsPositiveInfinity(flowMapLastPos.x))
                {
                    flowMapLastPos = flowPos;
                }
                else
                {
                    var brushDir = (flowPos - flowMapLastPos);
                    flowMapLastPos = flowPos;
                    DrawOnFlowMap(flowPos, brushDir, brushRadius,
                        1, e.control);
                }
            }
        }

        private void CreatBrushHandle(Event e, int controlId, Vector3 rayCastPos)
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

        private void CreatWaterHandle(FlowMapMono flowMapMono)
        {
            
            //   var flowMapAreaPos = new Vector3(waterPos.x + flowMapMono.FlowMapOffset.x, waterPos.y, waterPos.z + flowMapMono.FlowMapOffset.y);
            var flowMapAreaScale = new Vector3(flowMapMono.areaSize, 0.5f, flowMapMono.areaSize);
            Handles.matrix = Matrix4x4.TRS(flowMapMono.transform.position, Quaternion.identity, flowMapAreaScale);

            Handles.color = new Color(0, 0.75f, 1, 0.2f);
            Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
            Handles.color = new Color(0, 0.75f, 1, 0.9f);
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }
        
        private void CreatWaterHandle1(FlowMapMono flowMapMono)
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
            

            //   var flowMapAreaPos = new Vector3(waterPos.x + flowMapMono.FlowMapOffset.x, waterPos.y, waterPos.z + flowMapMono.FlowMapOffset.y);
            var flowMapAreaScale = new Vector3(maxValue, 0.5f, maxValue);
            Handles.matrix = Matrix4x4.TRS(flowMapMono.transform.position, Quaternion.identity, flowMapAreaScale);

            Handles.color = new Color(0, 0, 1, 0.2f);
            Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
            Handles.color = new Color(0, 0, 1, 0.9f);
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }

        public void DrawOnFlowMap(Vector3 brushPos, Vector3 brushDir, float brushRadius,
            float brushStrength, bool eraseMode = false)
        {
            renderSetting.InitializeFlowMapEditorResources((int)mono.texSize, flowData);
            renderSetting.DrawOnFlowMap(brushPos, brushDir, brushRadius, brushStrength, flowData,
                eraseMode, mono);
        }


        public void Save()
        {
            SaveTexture();
            SaveFlowData();
        }

        private void SaveFlowData()
        {
            flowData.brushStrength = mono.brushStrength;
            flowData.texSize = (int)mono.texSize;
            flowData.areaPos = mono.areaPos;
        }

        private void SaveTexture()
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
            //保存图片
            byte[] dataBytes = texture2D.EncodeToTGA();
            string savePath = Application.dataPath + "/SampleCircle.tga";
            FileStream fileStream = File.Open(savePath, FileMode.OpenOrCreate);
            fileStream.Write(dataBytes, 0, dataBytes.Length);
            fileStream.Close();
            UnityEditor.AssetDatabase.SaveAssets();
            UnityEditor.AssetDatabase.Refresh();
        }

        private Texture2D TextureToTexture2D(Texture texture)
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
    }
}