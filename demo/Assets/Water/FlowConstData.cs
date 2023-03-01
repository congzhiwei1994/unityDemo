using UnityEngine;

namespace Water
{
    public class FlowConstData
    {
        public static readonly int FlowMapSize = Shader.PropertyToID("_KW_FlowMapSize");
        public static readonly int FlowMapOffset = Shader.PropertyToID("_KW_FlowMapOffset");

        public static readonly string FLOW_EDITOR_PATH = "czw/Tools/FlowMap/FlowMapEditor";
        public static readonly int FLOW_MAP_ID = Shader.PropertyToID("_KW_FlowMapTex");

        public static readonly string FOLDER_NAME = "WaterTexture/";
        public static readonly string FLOW_MAP_NAME = "FlowMap";
        
        

    }
}