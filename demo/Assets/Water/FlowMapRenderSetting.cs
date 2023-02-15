using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Water
{
    [DisallowMultipleComponent]
    public class FlowMapRenderSetting : MonoBehaviour
    {
        public static readonly int KW_FlowMapTex = Shader.PropertyToID("_KW_FlowMapTex");

        public TemporaryRenderTexture _flowmapRT = new TemporaryRenderTexture();
        public Texture2D _flowMapTex2D;

        private FlowMapData data;

        private FlowMapData flowData;
        private Material _flowMaterial;

        private Material FlowMaterial
        {
            get
            {
                if (_flowMaterial == null)
                    _flowMaterial = GetFlowMapEditorMaterial();
                return _flowMaterial;
            }
        }

        private Material GetFlowMapEditorMaterial()
        {
            return CoreUtils.CreateEngineMaterial("czw/Tools/FlowMap/FlowMapEditor");
        }


        public void InitializeFlowMapEditorResources(int texSize, FlowMapData flowData)
        {
            if (_flowmapRT.isInitialized && _flowmapRT.rt.width != texSize)
            {
                var tempRT = new TemporaryRenderTexture();
                tempRT.Alloc("_flowmapRT", texSize, texSize, 0, GraphicsFormat.R16G16B16A16_SFloat);
                Graphics.Blit(_flowmapRT.rt, tempRT.rt);
                _flowmapRT.Release(true);
                _flowmapRT = tempRT;
            }
            else
                _flowmapRT.Alloc("_flowmapRT", texSize, texSize, 0, GraphicsFormat.R16G16B16A16_SFloat, ClearFlag.Color,
                    new Color(0.5f, 0.5f, 0.5f, 0.5f));


            if (_flowMapTex2D != null)
            {
                var activeRT = RenderTexture.active;
                Graphics.Blit(_flowMapTex2D, _flowmapRT.rt);

                RenderTexture.active = activeRT;

                // KW_Extensions.SafeDestroy(_flowMapTex2D);
            }

            Shader.SetGlobalTexture(KW_FlowMapTex, _flowmapRT.rt);
            flowData.flowTex = _flowmapRT.rt;
        }

        public void DrawOnFlowMap(Vector3 brushPosition, Vector3 brushMoveDirection, float circleRadius,
            float brushStrength, FlowMapData flowData, bool eraseMode = false, FlowMapMono mono = null )
        {
            // var brushSize = _currentFlowmapData.areaSize / circleRadius;
            // var uv        = new Vector2(brushPosition.x / _currentFlowmapData.areaSize + 0.5f, brushPosition.z / _currentFlowmapData.areaSize + 0.5f);
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
// Debug.LogError(maxValue);
            // maxValue *= 0.1f;
            // maxValue = boundsSize.z * boundsSize.x;
            
            var areaSize = maxValue;
            var brushSize = areaSize / circleRadius;
            var uv = new Vector2(brushPosition.x / areaSize + 0.5f, brushPosition.z / areaSize + 0.5f);
            if (brushMoveDirection.magnitude < 0.001f)
                brushMoveDirection = Vector3.zero;

            FlowMaterial.SetVector("_MousePos", uv);
            FlowMaterial.SetVector("_Direction", new Vector2(brushMoveDirection.x, brushMoveDirection.z));
            FlowMaterial.SetFloat("_Size", brushSize * 0.75f);
            FlowMaterial.SetFloat("_BrushStrength", brushStrength / (circleRadius * 3));
            FlowMaterial.SetFloat("isErase", eraseMode ? 1 : 0);

            var tempRT = new TemporaryRenderTexture("_TempRT", _flowmapRT);
            var activeRT = RenderTexture.active;

            Graphics.Blit(_flowmapRT.rt, tempRT.rt, FlowMaterial, 0);
            Graphics.Blit(tempRT.rt, _flowmapRT.rt);
            RenderTexture.active = activeRT;
            tempRT.Release();
        }
    }
}