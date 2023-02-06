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

        private void InitFlowMapRT(int size, int areaSize)
        {
            
        }
        
        public void DrawOnFlowMap(Vector3 pos, Vector3 dir, float radius, float strength, bool eraseMode = false)
        {
            InitializeFlowMapEditorResources();
            // SetParams2Mat(pos, dir, radius, strength, eraseMode);
        }
        
        void InitializeFlowMapEditorResources()
        {
           
        }
        
        // public void SetParams2Mat(Vector3 brushPosition, Vector3 brushMoveDirection, float circleRadius,
        //     float brushStrength, bool eraseMode = false)
        // {
        //     var brushSize = _currentFlowmapData.AreaSize / circleRadius;
        //     var uv = new Vector2(brushPosition.x / _currentFlowmapData.AreaSize + 0.5f,
        //         brushPosition.z / _currentFlowmapData.AreaSize + 0.5f);
        //     if (brushMoveDirection.magnitude < 0.001f) brushMoveDirection = Vector3.zero;
        //
        //     FlowMaterial.SetVector("_MousePos", uv);
        //     FlowMaterial.SetVector("_Direction", new Vector2(brushMoveDirection.x, brushMoveDirection.z));
        //     FlowMaterial.SetFloat("_Size", brushSize * 0.75f);
        //     FlowMaterial.SetFloat("_BrushStrength", brushStrength / (circleRadius * 3));
        //     FlowMaterial.SetFloat("isErase", eraseMode ? 1 : 0);
        //
        //     DrawFlowmapPass(0);
        // }
    }
}