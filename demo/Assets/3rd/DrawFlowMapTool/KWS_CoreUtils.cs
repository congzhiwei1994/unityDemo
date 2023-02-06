using UnityEngine;

namespace _3rd.DrawFlowMapTool
{
    public static partial class KWS_CoreUtils
    {
        const int MaxHeight = 1080;

        public static Vector2Int GetScreenSizeLimited()
        {
            var width = Screen.width;
            var height = Screen.height;

            if (height > MaxHeight)
            {
                width = (int)(MaxHeight * width / (float)height);
                height = MaxHeight;
            }

            return new Vector2Int(width, height);
        }
    }
}