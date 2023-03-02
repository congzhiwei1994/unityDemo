using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Water.Editor
{
    public class FlowRenderSetting
    {
        public TemporaryRenderTexture _flowmapRT = new TemporaryRenderTexture();
        public Texture2D _flowMapTex2D;
        private FlowMapData flowData;
        private FlowMapMono mono;
        private Material flowEditorMaterial;
        
        public FlowRenderSetting()
        {
            if (flowEditorMaterial == null)
                flowEditorMaterial = FlowMapUtils.GetFlowMapEditorMaterial();
        }

        public void Draw(Vector3 brushPos, Vector3 brushDir, float brushRadius,
            float brushStrength, bool eraseMode = false)
        {
            GetFlowMapRT((int)mono.texSize);
            DrawOnFlowMap(brushPos, brushDir, brushRadius, brushStrength, flowData,
                eraseMode, mono);
        }


        private void GetFlowMapRT(int texSize)
        {
            if (_flowmapRT.isInitialized && _flowmapRT.rt.width != texSize)
            {
                var tempRT = new TemporaryRenderTexture();
                tempRT.Alloc("_flowmapRT", texSize, texSize, 0);
                Graphics.Blit(_flowmapRT.rt, tempRT.rt);
                _flowmapRT.Release(true);
                _flowmapRT = tempRT;
            }
            else
                _flowmapRT.Alloc("_flowmapRT", texSize, texSize, 0,  ClearFlag.Color);


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
            float brushStrength, FlowMapData flowData, bool eraseMode = false, FlowMapMono mono = null)
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

            var tempRT = new TemporaryRenderTexture("_TempRT", _flowmapRT);
            var activeRT = RenderTexture.active;

            Graphics.Blit(_flowmapRT.rt, tempRT.rt, flowEditorMaterial, 0);
            Graphics.Blit(tempRT.rt, _flowmapRT.rt);
            RenderTexture.active = activeRT;
            tempRT.Release();
        }
    }
}