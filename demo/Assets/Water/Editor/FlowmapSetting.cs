using System.IO;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

namespace Water.Editor
{
    public class FlowmapSetting
    {
        private float brushRadius = 2f;
        private bool leftKeyPressed;
        private bool isFlowMapChanged;
        private Vector3 flowMapLastPos = Vector3.positiveInfinity;
        private FlowMapData flowData;
        private FlowMapMono mono;
        private FlowMapRenderSetting renderSetting;

        
        public FlowmapSetting()
        {

        }
        
        public FlowmapSetting(FlowMapData flowData, FlowMapMono mono, FlowMapRenderSetting renderSetting)
        {
            this.flowData = flowData;
            this.mono = mono;
            this.renderSetting = renderSetting;
        }

        public void DrawFlowMapEditor(FlowMapMono flowMapMono, UnityEditor.Editor editor)
        {
            if (Application.isPlaying)
                return;
            var waterPos = flowMapMono.transform.position;

            var e = Event.current;
            if (e.type == EventType.ScrollWheel)
            {
                brushRadius -= (e.delta.y * brushRadius) / 40f;
                brushRadius =
                    Mathf.Clamp(brushRadius, 0.1f, flowMapMono.areaSize);
            }

            var controlId = GUIUtility.GetControlID(FocusType.Passive);
            HandleUtility.AddDefaultControl(controlId);
            if (e.type == EventType.ScrollWheel)
                e.Use();

            var rayCastPos = FlowMapUtils.GetRayCastPos(flowMapMono.transform.position.y, e);
            if (float.IsInfinity(rayCastPos.x))
                return;

            var flowPos = new Vector3(-waterPos.x, 0, -waterPos.z) + (Vector3)rayCastPos;

            FlowMapUtils.CreatBrushHandle(e, controlId, rayCastPos, brushRadius);
            FlowMapUtils.CreatWaterHandle(flowData, flowMapMono);
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
                    flowMapLastPos = flowPos;
                }
                else
                {
                    var brushDir = (flowPos - flowMapLastPos);
                    flowMapLastPos = flowPos;
                    renderSetting.Draw(flowPos, brushDir, brushRadius,
                        1, e.control);
                }
            }
        }
        
    }
}