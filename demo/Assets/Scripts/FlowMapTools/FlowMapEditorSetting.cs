using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;

namespace czw.FlowMapTool
{
    public class FlowMapEditorSetting
    {
        private Event e;
        private float brushRadius = 2.0f;
        private FlowMap flowMap;
        private Editor editor;

        private bool leftKeyPressed;
        private bool isFlowMapChanged;
        private Vector3 flowMapLastPos;

        public void Init(Event e, FlowMap flowMap, Editor editor)
        {
            this.e = e;
            this.flowMap = flowMap;
            this.editor = editor;
        }

        public void DrawSetting()
        {
            if (Application.isPlaying)
                return;

            SetBrushRadius();
            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);

            if (e.type == EventType.ScrollWheel)
                e.Use();

            var waterHeight = flowMap.transform.position.y;
            var pos = FlowMapEditorUtils.GetRaycastPos(waterHeight, e);
            var offset = new Vector3(-flowMap.areaPos.x, 0, -flowMap.areaPos.z) + (Vector3)pos;

            if (float.IsInfinity(pos.x))
                return;

            CreattBrushHandle(controlId, pos);
            CreatCubeHandle();

            if (Event.current.button == 0)
            {
                if (e.type == EventType.MouseDown)
                    leftKeyPressed = true;

                if (e.type == EventType.MouseUp)
                {
                    leftKeyPressed = false;
                    isFlowMapChanged = true;
                    flowMapLastPos = Vector3.positiveInfinity;

                    editor.Repaint();
                }
            }

            if (leftKeyPressed)
            {
                if (float.IsPositiveInfinity(flowMapLastPos.x))
                {
                    flowMapLastPos = offset;
                }
                else
                {
                    var brushDir = (offset - flowMapLastPos);
                    flowMapLastPos = offset;
                    // waterSystem.DrawOnFlowMap(flowPosWithOffset, brushDir, floatMapCircleRadiusDefault, waterSystem.FlowMapBrushStrength, e.control);
                }
            }
        }

        
        /// <summary>
        /// 设置笔刷半径
        /// </summary>
        private void SetBrushRadius()
        {
            // scene 窗口滚轮滑动
            if (e.type == EventType.ScrollWheel)
            {
                brushRadius -= (e.delta.y * brushRadius) / 40;
                brushRadius = Mathf.Clamp(brushRadius, 0.1f, flowMap.areaSize);
            }
        }

        private void CreattBrushHandle(int controlId, Vector3 pos)
        {
            if (e.control)
                Handles.color = new Color(1, 0, 0);
            else
                Handles.color = new Color(0, 0.8f, 1);
            // 绘制外边框
            Handles.CircleHandleCap(controlId, (Vector3)pos, Quaternion.LookRotation(Vector3.up),
                brushRadius, EventType.Repaint);

            if (e.control)
                Handles.color = new Color(1, 0, 0, 0.2f);
            else
                Handles.color = new Color(0, 0.8f, 1, 0.25f);
            // 绘制内部
            Handles.DrawSolidDisc((Vector3)pos, Vector3.up, brushRadius);
        }

        private void CreatCubeHandle()
        {
            var flowMapAreaScale = new Vector3(flowMap.areaSize, 0.5f, flowMap.areaSize);
            //转到世界空间
            Handles.matrix = Matrix4x4.TRS(flowMap.areaPos, Quaternion.identity, flowMapAreaScale);
            Handles.color = new Color(0, 0.75f, 1, 0.2f);
            Handles.CubeHandleCap(0, Vector3.zero, Quaternion.identity, 1, EventType.Repaint);
            Handles.color = new Color(0, 0.75f, 1, 0.9f);
            Handles.DrawWireCube(Vector3.zero, Vector3.one);
        }

        public static void Save()
        {
        }

        public static void Clear()
        {
        }
    }
}