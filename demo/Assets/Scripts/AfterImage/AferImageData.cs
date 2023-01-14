using UnityEngine;

namespace Render.AfterImagPool
{
    public class AferImageData : MonoBehaviour
    {
        /// <summary>
        /// Shader 名字
        /// </summary>
        public const string SHADER_NAME = "czw/AfterImageEffect";

        /// <summary>
        /// 虚影存在时间
        /// </summary>
        public const float EXIST_TIME = 1;

        /// <summary>
        /// 生成虚影的间隔时间
        /// </summary>
        public const float SPAWN_INTERVAL_TIME = 0.2f;
        
        /// <summary>
        /// shader颜色属性名字
        /// </summary>
        public const string SHADER_COLOR_NAME = "_BaseColor";
    }
}