using UnityEditor;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Water.Editor
{
    public class FlowSetting
    {
        private float brushRadius = 2f;
        private bool leftKeyPressed;
        private bool isFlowMapChanged;
        private Vector3 lastPos = Vector3.positiveInfinity;
        public FlowRenderTexture _flowmapRT = new FlowRenderTexture();
        public Texture2D _flowMapTex2D;
        private FlowMapData flowData;
        private FlowMapMono mono;
        private Material flowEditorMaterial;

        private GraphicsFormat graphicsFormat = GraphicsFormat.R16G16B16A16_SFloat;

        public FlowSetting()
        {
            if (flowEditorMaterial == null)
                flowEditorMaterial = FlowMapUtils.GetFlowMapEditorMaterial();
        }

        public void Init(FlowMapMono mono, FlowMapData flowData)
        {
            this.mono = mono;
            this.flowData = flowData;
        }

        private void GetBrushRadius(Event e)
        {
            brushRadius -= (e.delta.y * brushRadius) / 40f;
            brushRadius =
                Mathf.Clamp(brushRadius, 0.1f, mono.areaSize);
        }

        public void DrawFlowMapEditor(EditorWindow editor, Event e)
        {
            var waterPos = mono.transform.position;

            if (e.type == EventType.ScrollWheel)
            {
                GetBrushRadius(e);
            }

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);
            if (e.type == EventType.ScrollWheel)
                e.Use();

            var rayCastPos = FlowMapUtils.GetRayCastPos(mono.transform.position.y, e);
            if (float.IsInfinity(rayCastPos.x))
                return;


            FlowMapUtils.CreatBrushHandle(e, controlId, rayCastPos, brushRadius);
            FlowMapUtils.CreatWaterHandle(flowData, mono);
            if (Event.current.button == 0)
            {
                if (e.type == EventType.MouseDown)
                    leftKeyPressed = true;

                if (e.type == EventType.MouseUp)
                {
                    leftKeyPressed = false;
                    isFlowMapChanged = true;
                    lastPos = Vector3.positiveInfinity;

                    editor.Repaint();
                }
            }

            var flowPos = new Vector3(-waterPos.x, 0, -waterPos.z) + (Vector3)rayCastPos;
            if (leftKeyPressed)
            {
                if (float.IsPositiveInfinity(lastPos.x))
                {
                    lastPos = flowPos;
                }
                else
                {
                    var brushDir = (flowPos - lastPos);
                    lastPos = flowPos;

                    GetFlowMapRT();
                    DrawOnFlowMap(flowPos, brushDir, brushRadius, 1, flowData,
                        false);
                }
            }
        }


        private void GetFlowMapRT()
        {
            int texSize = (int)mono.texSize;

            if (_flowmapRT.isInitialized && _flowmapRT.rt.width != texSize)
            {
                var tempRT = new FlowRenderTexture();
                tempRT.Alloc("_flowmapRT", texSize, texSize, 0);
                Graphics.Blit(_flowmapRT.rt, tempRT.rt);
                _flowmapRT.Release(true);
                _flowmapRT = tempRT;
            }
            else
                _flowmapRT.Alloc("_flowmapRT", texSize, texSize, 0, ClearFlag.Color);


            if (_flowMapTex2D != null)
            {
                var activeRT = RenderTexture.active;
                Graphics.Blit(_flowMapTex2D, _flowmapRT.rt);

                RenderTexture.active = activeRT;

                // KW_Extensions.SafeDestroy(_flowMapTex2D);
            }

            Shader.SetGlobalTexture(FlowConstData.FLOW_MAP_ID, _flowmapRT.rt);
            flowData.flowTex = _flowmapRT.rt;
        }

        public void DrawOnFlowMap(Vector3 brushPosition, Vector3 brushMoveDirection, float circleRadius,
            float brushStrength, FlowMapData flowData, bool eraseMode = false)
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

            var areaSize = maxValue;
            var brushSize = areaSize / circleRadius;
            var uv = new Vector2(brushPosition.x / areaSize + 0.5f, brushPosition.z / areaSize + 0.5f);
            if (brushMoveDirection.magnitude < 0.001f)
                brushMoveDirection = Vector3.zero;

            flowEditorMaterial.SetVector("_MousePos", uv);
            flowEditorMaterial.SetVector("_Direction", new Vector2(brushMoveDirection.x, brushMoveDirection.z));
            flowEditorMaterial.SetFloat("_Size", brushSize * 0.75f);
            flowEditorMaterial.SetFloat("_BrushStrength", brushStrength / (circleRadius * 3));
            flowEditorMaterial.SetFloat("isErase", eraseMode ? 1 : 0);

            var tempRT = new FlowRenderTexture("_TempRT", _flowmapRT);
            var activeRT = RenderTexture.active;

            Graphics.Blit(_flowmapRT.rt, tempRT.rt, flowEditorMaterial, 0);
            Graphics.Blit(tempRT.rt, _flowmapRT.rt);
            RenderTexture.active = activeRT;
            tempRT.Release();
        }
    }
}