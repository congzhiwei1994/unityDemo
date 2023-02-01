using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace czw.GostShadow
{
    public class GostShadowConstData
    {
        public const string SHADER_NAME = "czw/unlit/Transparent";

        /// <summary>
        /// 虚影存在时间
        /// </summary>
        public const float EXIST_TIME = 1;

        /// <summary>
        /// 生成虚影的间隔时间
        /// </summary>
        public const float SPAWN_INTERVAL_TIME = 0.2f;
    }
}