using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace czw.FlowMapTool
{
    public class FlowMapDrawSetting
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

        private FlowMapRenderSetting _renderSetting;
        private RenderTexture renderTexture;

        public FlowMapDrawSetting(FlowMapMono mono)
        {
            this.mono = mono;
            GetFlowMapRenderSetting();
        }


        public void Setup(Event e, Editor editor, int controlId, Material material)
        {
            this.e = e;

            this.editor = editor;
            this.controlId = controlId;
            this.waterMat = material;

            DrawSetting();
        }

        // 绘制设置
        private void DrawSetting()
        {
            if (Application.isPlaying)
                return;

            pos = FlowMapUtils.GetRaycastPos(mono.transform.position.y, e);
            offset = new Vector3(-mono.areaPos.x, 0, -mono.areaPos.z) + (Vector3)pos;

            if (float.IsInfinity(pos.x))
                return;

            radius = FlowMapUtils.SetBrush(e, mono.areaSize, radius);
            FlowMapUtils.CreatBrushHandle(controlId, pos, radius, e);
            //  FlowMapEditorUtils.CreatCubeHandle(flowMap.areaSize, flowMap.areaPos);

            GetMouseEvent();
        }

        private void GetMouseEvent()
        {
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


        private void DrawFlowMap()
        {
            var descriptor = _renderSetting.GetRenderTextureDescriptor((int)mono.resolution, (int)mono.resolution);
            renderTexture = _renderSetting.GetRenderTexture(descriptor);
            _renderSetting.ClearRenderTexture(renderTexture);


            GetFlowMapMat();
            SetWaterMaterial(renderTexture);
            SetFlowMapEditorMaterial();
            DrawFlowmapPass(0, flowMat, renderTexture, descriptor);

            Release();
        }


        private void GetFlowMapMat()
        {
            if (flowMat == null)
            {
                flowMat = CoreUtils.CreateEngineMaterial("czw/Tools/FlowMap/FlowMapEditor");
            }
        }


        private void DrawFlowmapPass(int pass, Material material, RenderTexture source,
            RenderTextureDescriptor descriptor)
        {
            var tempRT = RenderTexture.GetTemporary(descriptor);

            // tempRT.graphicsFormat = format;
            tempRT.stencilFormat = GraphicsFormat.None;

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
            waterMat.SetTexture(FlowMapNameData.FLOWMAP_NAME, texture);
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

        private void GetFlowMapRenderSetting()
        {
            if (_renderSetting == null)
            {
                _renderSetting = mono.gameObject.GetComponent<FlowMapRenderSetting>();
                if (_renderSetting == null)
                {
                    _renderSetting = mono.gameObject.AddComponent<FlowMapRenderSetting>();
                }
            }
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