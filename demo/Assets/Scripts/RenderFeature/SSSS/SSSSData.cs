using UnityEngine;

namespace czw.SSSS
{
    public class SSSSData
    {
        public static int ID_Kernel = Shader.PropertyToID("_Kernel");
        public static int ID_SSSScaler = Shader.PropertyToID("_SSSScale");
        public static int ID_ScreenSize = Shader.PropertyToID("_ScreenSize");
        public static int ID_FOV = Shader.PropertyToID("_FOV");
        public static int ID_MaxDistance = Shader.PropertyToID("_MaxDistance");
        public static int ID_SSSBlurRT = Shader.PropertyToID("_SSSBlurRT");
        public static int ID_SkinDepthRT = Shader.PropertyToID("_SkinDepthRT");
        
        public static int ID_CameraDepthTexture = Shader.PropertyToID("_CameraDepthTexture");
    }
}