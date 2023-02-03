using UnityEngine;
using UnityEngine.Rendering;

namespace czw.RenderFeature
{
    public class RenderFeatureUtils
    {
        
        /// <summary>
        /// 获取材质
        /// </summary>
        public static Material GetMaterial(Material materal, Shader shader, string shaderPath)
        {
            if (materal == null)
            {
                if (shader == null)
                {
                    shader = Shader.Find(shaderPath);
                }

                materal = CoreUtils.CreateEngineMaterial(shader);
            }

            return materal;
        }
    }
}