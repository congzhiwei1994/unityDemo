using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;

namespace Water
{
    [DisallowMultipleComponent]
    public class KW_FlowMap : MonoBehaviour
    {
        public static readonly int KW_FlowMapTex = Shader.PropertyToID("_KW_FlowMapTex");

        public TemporaryRenderTexture _flowmapRT = new TemporaryRenderTexture();
        public Texture2D _flowMapTex2D;

        private FlowMapData data;

        private FlowMapData _currentFlowmapMapData
        {
            get
            {
                if (data == null)
                {
                    data = GetFlowData();
                }

                return data;
            }
        }

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

        private FlowMapData GetFlowData()
        {
            var newData = this.gameObject.GetComponent<FlowMapData>();
            if (newData == null)
            {
                newData = this.gameObject.AddComponent<FlowMapData>();
            }

            return newData;
        }

        public void InitializeFlowMapEditorResources(int size, int areaSize)
        {
            if (_flowmapRT.isInitialized && _flowmapRT.rt.width != size)
            {
                var tempRT = new TemporaryRenderTexture();
                tempRT.Alloc("_flowmapRT", size, size, 0, GraphicsFormat.R16G16B16A16_SFloat);
                Graphics.Blit(_flowmapRT.rt, tempRT.rt);
                _flowmapRT.Release(true);
                _flowmapRT = tempRT;
            }
            else
                _flowmapRT.Alloc("_flowmapRT", size, size, 0, GraphicsFormat.R16G16B16A16_SFloat, ClearFlag.Color,
                    new Color(0.5f, 0.5f, 0.5f, 0.5f));


            if (_flowMapTex2D != null)
            {
                var activeRT = RenderTexture.active;
                Graphics.Blit(_flowMapTex2D, _flowmapRT.rt);
                RenderTexture.active = activeRT;
                // KW_Extensions.SafeDestroy(_flowMapTex2D);
            }

            // if (_currentFlowmapMapData == null)
            //     _currentFlowmapMapData = new FlowMapData();

            // _currentFlowmapMapData.areaSize = areaSize;
            // _currentFlowmapMapData.texSize = size;

            SetTextures();
        }

        public void DrawOnFlowMap(Vector3 brushPosition, Vector3 brushMoveDirection, float circleRadius,
            float brushStrength, bool eraseMode = false)
        {
            // var brushSize = _currentFlowmapData.areaSize / circleRadius;
            // var uv        = new Vector2(brushPosition.x / _currentFlowmapData.areaSize + 0.5f, brushPosition.z / _currentFlowmapData.areaSize + 0.5f);

            var brushSize = 100 / circleRadius;
            var uv = new Vector2(brushPosition.x / 100 + 0.5f, brushPosition.z / 100 + 0.5f);
            if (brushMoveDirection.magnitude < 0.001f) brushMoveDirection = Vector3.zero;

            FlowMaterial.SetVector("_MousePos", uv);
            FlowMaterial.SetVector("_Direction", new Vector2(brushMoveDirection.x, brushMoveDirection.z));
            FlowMaterial.SetFloat("_Size", brushSize * 0.75f);
            FlowMaterial.SetFloat("_BrushStrength", brushStrength / (circleRadius * 3));
            FlowMaterial.SetFloat("isErase", eraseMode ? 1 : 0);

            DrawFlowmapPass(0);
        }

        private void DrawFlowmapPass(int pass)
        {
            var tempRT = new TemporaryRenderTexture("_TempRT", _flowmapRT);
            var activeRT = RenderTexture.active;

            Graphics.Blit(_flowmapRT.rt, tempRT.rt, FlowMaterial, pass);
            Graphics.Blit(tempRT.rt, _flowmapRT.rt);

            RenderTexture.active = activeRT;
            
            
            tempRT.Release();
        }

        public void SetTextures()
        {
            Shader.SetGlobalTexture(KW_FlowMapTex, _flowmapRT.rt);
            _currentFlowmapMapData.flowTex = _flowmapRT.rt;
        }
     
    }
}