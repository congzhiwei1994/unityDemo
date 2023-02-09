using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace czw.FlowMapTool
{
    public class FlowMapSetting
    {
        private Event e;
        private float radius = 2.0f;
        private Vector3 pos;
        private Vector3 offset;
        private FlowMapMono mono;
        private Vector3 dir;
        private bool eraseMode = false;
        private Editor editor;

        private bool leftKeyPressed;
        private bool isFlowMapChanged;
        private Vector3 flowMapLastPos;
        private Material flowMat;
        private Material waterMat;
        private int controlId;

        private GraphicsFormat format = GraphicsFormat.R16G16B16A16_SFloat;
        private ClearFlag clearFlag = ClearFlag.Color;
        private Color clearColor = new Color(0.5f, 0.5f, 0.5f, 0.5f);
        private string FLOWMAP_NAME = "_flowmapRT";
        private RenderTextureDescriptor descriptor;
        private RenderTexture renderTexture;


        public FlowMapSetting(FlowMapMono mono)
        {
            this.mono = mono;
        }

        public void Setup(Event e, FlowMapMono flowMapMono, Editor editor, int controlId)
        {
            this.e = e;
            this.mono = flowMapMono;
            this.editor = editor;
            this.controlId = controlId;

            DrawSetting();
        }

        public void DrawSetting()
        {
            if (Application.isPlaying)
                return;

            // scene 窗口滚轮滑动
            if (e.type == EventType.ScrollWheel)
            {
                SetBrushRadius(mono.areaSize);
            }

            // 滚轮移动
            if (e.type == EventType.ScrollWheel)
                e.Use();

            var waterHeight = mono.transform.position.y;
            // 世界空间射线相交的位置
            pos = FlowMapEditorUtils.GetRaycastPos(waterHeight, e);
            // 计算偏移
            offset = new Vector3(-mono.areaPos.x, 0, -mono.areaPos.z) + (Vector3)pos;

            if (float.IsInfinity(pos.x))
                return;

            FlowMapEditorUtils.CreatBrushHandle(controlId, pos, radius, e);
            //  FlowMapEditorUtils.CreatCubeHandle(flowMap.areaSize, flowMap.areaPos);

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
            if (renderTexture == null)
            {
                var descriptor = GetRenderTextureDescriptor((int)mono.resolution, (int)mono.resolution);
                GetRenderTexture(descriptor);
            }

            if (mono.flowMapTex != null)
            {
            }

            Release();
            // SetWaterMaterial(flowRT);
            // SetFlowMapEditorMaterial();
            // FlowMapRenderSetting.DrawFlowmapPass(0, flowMat, flowRT);
        }

        public void SaveTexture()
        {
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


        private RenderTextureDescriptor GetRenderTextureDescriptor(int width, int height)
        {
            var descriptor = new RenderTextureDescriptor(width, height, format, 0);
            descriptor.sRGB = false;
            descriptor.enableRandomWrite = false;
            descriptor.dimension = TextureDimension.Tex2D;
            descriptor.useMipMap = false;
            descriptor.autoGenerateMips = false;
            descriptor.shadowSamplingMode = ShadowSamplingMode.None;
            descriptor.mipCount = 0;
            descriptor.msaaSamples = 1;
            descriptor.vrUsage = VRTextureUsage.None;
            return descriptor;
        }

        private void GetRenderTexture(RenderTextureDescriptor descriptor)
        {
            renderTexture = RenderTexture.GetTemporary(descriptor);
            renderTexture.name = FLOWMAP_NAME;
            if (!renderTexture.IsCreated())
            {
                renderTexture.Create();
            }

            CleatRenderTexture(renderTexture);
        }

        private void CleatRenderTexture(RenderTexture renderTexture)
        {
            var activeRT = RenderTexture.active;
            RenderTexture.active = renderTexture;
            GL.Clear((clearFlag & ClearFlag.Depth) != 0, (clearFlag & ClearFlag.Color) != 0, clearColor);
            RenderTexture.active = activeRT;
        }


        private void DrawFlowmapPass(int pass, Material material, RenderTexture source)
        {
            var tempRT = RenderTexture.GetTemporary(descriptor);
            tempRT.name = "_TempRT";
            var activeRT = RenderTexture.active;

            Graphics.Blit(source, tempRT, material, pass);
            Graphics.Blit(tempRT, source);

            RenderTexture.active = activeRT;
            tempRT.Release();
        }
        
        private void Release()
        {
            if (renderTexture != null)
            {
                RenderTexture.ReleaseTemporary(renderTexture);
            }
        }

        /// <summary>
        ///  获取水材质
        /// </summary>
        private void SetWaterMaterial(Texture texture)
        {
            waterMat.SetTexture(FlowMapEditorData.FLOWMAP_NAME, texture);
            mono.gameObject.GetComponent<Renderer>().sharedMaterial = waterMat;
        }

        private void SetFlowMapEditorMaterial(bool eraseMode = false)
        {
            var brushSize = mono.areaSize / radius;
            var uv = new Vector2(pos.x / mono.areaSize + 0.5f, pos.z / mono.areaSize + 0.5f);

            if (dir.magnitude < 0.001f)
                dir = Vector3.zero;

            flowMat.SetVector("_MousePos", uv);
            flowMat.SetVector("_Direction", new Vector2(dir.x, dir.z));
            flowMat.SetFloat("_Size", brushSize * 0.75f);
            flowMat.SetFloat("_BrushStrength", mono.brushStrength / (radius * 3));
            flowMat.SetFloat("isErase", eraseMode ? 1 : 0);
        }


        /// <summary>
        /// 鼠标左键是否按下
        /// </summary>
        private bool IsMouseLeftPressed()
        {
            return Event.current.button == 0;
        }
    }
}