using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Water.Editor
{
    public class FlowmapSetting
    {
        private float floatMapCircleRadiusDefault = 2f;
        private bool leftKeyPressed;
        private bool isFlowMapChanged;
        private Vector3 flowMapLastPos = Vector3.positiveInfinity;
        private FlowMapData flowData;

        public FlowmapSetting(FlowMapData flowData)
        {
            this.flowData = flowData;
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

            var e = Event.current;
            if (e.type == EventType.ScrollWheel)
            {
                floatMapCircleRadiusDefault -= (e.delta.y * floatMapCircleRadiusDefault) / 40f;
                floatMapCircleRadiusDefault =
                    Mathf.Clamp(floatMapCircleRadiusDefault, 0.1f, flowMapMono.areaSize);
            }

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);
            if (e.type == EventType.ScrollWheel)
                e.Use();

            var waterPos = flowMapMono.transform.position;
            var waterHeight = flowMapMono.transform.position.y;

            var flowmapWorldPos = GetMouseWorldPosProjectedToWater(waterHeight, e);
            if (float.IsInfinity(flowmapWorldPos.x)) return;
            var flowPosWithOffset =
                new Vector3(-flowMapMono.areaPos.x, 0, -flowMapMono.areaPos.z) +
                (Vector3)flowmapWorldPos;
            // flowPosWithOffset = flowmapWorldPos -
            //                     new Vector3(flowMapMono.areaPos.x, 0, flowMapMono.areaPos.z);

            Handles.color = e.control ? new Color(1, 0, 0) : new Color(0, 0.8f, 1);
            Handles.CircleHandleCap(controlId, (Vector3)flowmapWorldPos, Quaternion.LookRotation(Vector3.up),
                floatMapCircleRadiusDefault, EventType.Repaint);

            Handles.color = e.control ? new Color(1, 0, 0, 0.2f) : new Color(0, 0.8f, 1, 0.25f);
            Handles.DrawSolidDisc((Vector3)flowmapWorldPos, Vector3.up, floatMapCircleRadiusDefault);


            // var flowMapAreaPos = new Vector3(waterPos.x + flowMapMono.FlowMapOffset.x, waterPos.y, waterPos.z + flowMapMono.FlowMapOffset.y);
            // var flowMapAreaScale = new Vector3(flowMapMono.areaSize, 0.5f, flowMapMono.areaSize);
            // Handles.matrix = Matrix4x4.TRS(flowMapMono.areaPos, Quaternion.identity, flowMapAreaScale);
            //
            //
            // Handles.color = new Color(0, 0.75f, 1, 0.2f);
            // Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
            // Handles.color = new Color(0, 0.75f, 1, 0.9f);
            // Handles.DrawWireCube(Vector3.zero, Vector3.one);

            if (Event.current.button == 0)
            {
                if (e.type == EventType.MouseDown)
                {
                    leftKeyPressed = true;
                    //flowMapMono.flowMap.LastDrawFlowMapPosition = flowPosWithOffset;
                }

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
                    flowMapLastPos = flowPosWithOffset;
                }
                else
                {
                    var brushDir = (flowPosWithOffset - flowMapLastPos);
                    flowMapLastPos = flowPosWithOffset;
                    Debug.LogError(flowPosWithOffset);
                    flowMapMono.DrawOnFlowMap(flowPosWithOffset, brushDir, floatMapCircleRadiusDefault,
                       1, e.control);
                }
            }
        }


        public void Save()
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