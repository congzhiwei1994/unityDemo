using UnityEngine;
using UnityEngine.Experimental.Rendering;
using UnityEngine.Rendering;
using Water;

namespace KWS
{
    public class KW_FlowMap : MonoBehaviour
    {
        public TemporaryRenderTexture _flowmapRT = new TemporaryRenderTexture();
        public Texture2D _flowMapTex2D;
        private KW_FlowMapData flowData;
        public static readonly int KW_FlowMapTex = Shader.PropertyToID("_KW_FlowMapTex");
        private Material FlowMaterial;

        public void Init(KW_FlowMapData flowData)
        {
            this.flowData = flowData;
            if (FlowMaterial == null)
            {
                FlowMaterial = CoreUtils.CreateEngineMaterial("czw/Tools/FlowMap/FlowMapEditor");
            }
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

            flowData.AreaSize = areaSize;
            flowData.TextureSize = size;
            flowData.FlowTexture = _flowmapRT.rt;
            Shader.SetGlobalTexture(KW_FlowMapTex, _flowmapRT.rt);
             // WaterInstance.SetTextures((KWS_ShaderConstants.FlowmapID.KW_FlowMapTex, _flowmapRT.rt));
        }

        public void DrawOnFlowMap(Vector3 brushPosition, Vector3 brushMoveDirection, float circleRadius,
            float brushStrength, bool eraseMode = false)
        {
            var brushSize = flowData.AreaSize / circleRadius;
            var uv = new Vector2(brushPosition.x / flowData.AreaSize + 0.5f,
                brushPosition.z / flowData.AreaSize + 0.5f);
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
            
            // if (_flowmapRT.rt != null)
            // {
            //     RenderTexture.ReleaseTemporary(_flowmapRT.rt);
            // }
        }
    }
}