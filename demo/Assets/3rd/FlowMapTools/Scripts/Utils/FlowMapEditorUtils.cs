using System.Drawing;
using UnityEditor;
using UnityEngine;
using Color = UnityEngine.Color;

namespace czw.FlowMapTool
{
    public class FlowMapEditorUtils
    {
        const int MaxHeight = 1080;

        public static Vector2Int GetScreenSizeLimited()
        {
            return new Vector2Int(1, 1);
        }

        /// <summary>
        ///  获取射线相交点
        /// </summary>
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



        public static void CreatBrushHandle(int controlId, Vector3 pos, float radius, Event e)
        {
            if (e.control)
                Handles.color = new Color(1, 1, 0);
            else
                Handles.color = new Color(0, 0.8f, 1);
            
            // 绘制外边框
            Handles.CircleHandleCap(controlId, (Vector3)pos, Quaternion.LookRotation(Vector3.up),
                radius, EventType.Repaint);

            if (e.control)
                Handles.color = new Color(1, 0, 0, 0.2f);
            else
                Handles.color = new Color(0, 0.8f, 1, 0.25f);
            // 绘制内部
            Handles.DrawSolidDisc((Vector3)pos, Vector3.up, radius);
        }

        public static void CreatCubeHandle(int areaSize, Vector3 areaPos)
        {
            var flowMapAreaScale = new Vector3(areaSize, 0.5f, areaSize);
            //转到世界空间
            Handles.matrix = Matrix4x4.TRS(areaPos, Quaternion.identity, flowMapAreaScale);
            Handles.color = new Color(0, 0.75f, 1, 0.2f);
            Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
            Handles.color = new Color(0, 0.75f, 1, 0.9f);
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }


        public static void Draw(Vector3 pos, Vector3 dir, float radius, float strength, Material material,
            bool eraseMode = false)
        {
            FlowMapRenderSetting.InitRenderTexture(1024, 1024);
        }
    }
}