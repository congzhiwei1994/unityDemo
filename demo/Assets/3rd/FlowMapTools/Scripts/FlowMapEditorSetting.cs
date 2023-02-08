using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace czw.FlowMapTool
{
    public class FlowMapEditorSetting
    {
        private Event e;
        private float radius = 2.0f;
        private Vector3 pos;
        private Vector3 offset;
        private FlowMap flowMap;
        private Vector3 dir;
        private bool eraseMode = false;
        private Editor editor;

        private bool leftKeyPressed;
        private bool isFlowMapChanged;
        private Vector3 flowMapLastPos;
        private Material flowMat;
        private Material waterMat;
        private int controlId;

        public void Setup(Event e, FlowMap flowMap, Editor editor, int controlId)
        {
            this.e = e;
            this.flowMap = flowMap;
            this.editor = editor;
            this.controlId = controlId;
            GetFlowMaterial();
            GetWaterMaterial();

            DrawSetting();
        }

        public void DrawSetting()
        {
            if (Application.isPlaying)
                return;

            // scene 窗口滚轮滑动
            if (e.type == EventType.ScrollWheel)
            {
                SetBrushRadius(flowMap.areaSize);
            }
            
            // 滚轮移动
            if (e.type == EventType.ScrollWheel)
                e.Use();

            var waterHeight = flowMap.transform.position.y;
            // 世界空间射线相交的位置
            pos = FlowMapEditorUtils.GetRaycastPos(waterHeight, e);
            // 计算偏移
            offset = new Vector3(-flowMap.areaPos.x, 0, -flowMap.areaPos.z) + (Vector3)pos;

            if (float.IsInfinity(pos.x))
                return;

            FlowMapEditorUtils.CreatBrushHandle(controlId, pos, radius, e);
             FlowMapEditorUtils.CreatCubeHandle(flowMap.areaSize, flowMap.areaPos);

            // 鼠标左键按下
            if (IsMouseLeftPressed())
            {
                // 鼠标左键按下
                if (e.type == EventType.MouseDown)
                    leftKeyPressed = true;

                // 鼠标左键抬起
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
                    flowMapLastPos = offset;
                }
                else
                {
                    dir = offset - flowMapLastPos;
                    flowMapLastPos = offset;
                    // DrawFlowMap();
                }
            }
        }


        /// <summary>
        /// 设置笔刷半径
        /// </summary>
        public void SetBrushRadius(int areaSize)
        {
            radius -= (e.delta.y * radius) / 40;
            radius = Mathf.Clamp(radius, 0.1f, areaSize);
        }

        private void DrawFlowMap()
        {
            var flowRT = FlowMapRenderSetting.GetRenderTexture((int)flowMap.resolution, (int)flowMap.resolution);
            SetWaterMaterial(flowRT);
            SetFlowMapEditorMaterial();
            FlowMapRenderSetting.DrawFlowmapPass(0, flowMat, flowRT);
        }

        public void SetSceneViewState()
        {
            var lastScene = SceneView.lastActiveSceneView;
            if (lastScene != null)
            {
                lastScene.sceneViewState.alwaysRefresh = true;
                lastScene.sceneViewState.showSkybox = true;
                lastScene.sceneViewState.showImageEffects = true;
            }
        }

        private void GetFlowMaterial()
        {
            if (flowMat == null)
            {
                var shader = Shader.Find(FlowMapData.FLOW_SHADER_PATH);
                if (shader == null)
                {
                    Debug.LogError("没有找到的Shader,路径为:" + FlowMapData.FLOW_SHADER_PATH);
                    return;
                }

                flowMat = CoreUtils.CreateEngineMaterial(shader);
            }
        }

        /// <summary>
        ///  获取水材质
        /// </summary>
        private void GetWaterMaterial()
        {
            if (waterMat == null)
            {
                waterMat = flowMap.gameObject.GetComponent<Renderer>().sharedMaterial;
            }
        }

        /// <summary>
        ///  设置水材质
        /// </summary>
        /// <param name="texture"></param>
        private void SetWaterMaterial(Texture texture)
        {
            waterMat.SetTexture(FlowMapData.FLOWMAP_NAME, texture);
        }

        private void SetFlowMapEditorMaterial(bool eraseMode = false)
        {
            var brushSize = flowMap.areaSize / radius;
            var uv = new Vector2(pos.x / flowMap.areaSize + 0.5f, pos.z / flowMap.areaSize + 0.5f);

            if (dir.magnitude < 0.001f)
                dir = Vector3.zero;

            flowMat.SetVector("_MousePos", uv);
            flowMat.SetVector("_Direction", new Vector2(dir.x, dir.z));
            flowMat.SetFloat("_Size", brushSize * 0.75f);
            flowMat.SetFloat("_BrushStrength", flowMap.brushStrength / (radius * 3));
            flowMat.SetFloat("isErase", eraseMode ? 1 : 0);
        }


        /// <summary>
        /// 鼠标左键是否按下
        /// </summary>
        private bool IsMouseLeftPressed()
        {
            return Event.current.button == 0;
        }


        public void Save()
        {
        }

        public void Clear()
        {
        }
    }
}