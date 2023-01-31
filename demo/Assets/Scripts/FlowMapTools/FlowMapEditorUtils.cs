using UnityEditor;
using UnityEngine;

namespace czw.FlowMapTool
{
    public class FlowMapEditorUtils
    {
        const int MaxHeight = 1080;

        public static Vector2Int GetScreenSizeLimited()
        {
            return new Vector2Int(1, 1);
        }

        // 获取射线相交点
        public static Vector3 GetRaycastPos(float height, Event e)
        {
            var mousePos = e.mousePosition;
            var plane = new Plane(Vector3.down, height);
            // 创建射线
            var ray = HandleUtility.GUIPointToWorldRay(mousePos);
            // 射线与plane相交
            if (plane.Raycast(ray, out var distance))
            {
                // 获得相交点
                return ray.GetPoint(distance);
            }

            return Vector3.positiveInfinity;
        }
    }
}