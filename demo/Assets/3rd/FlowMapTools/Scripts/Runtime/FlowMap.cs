using System;
using UnityEngine;
using UnityEngine.Profiling;

namespace czw.FlowMapTool
{
    public enum FlowMapResolution
    {
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
    }

    [ExecuteAlways]
    [Serializable]
    public class FlowMap : MonoBehaviour
    {
        public FlowMapResolution resolution = FlowMapResolution._1024;
        public int areaSize = 200;
        public Vector3 areaPos = Vector3.zero;
        public float flowSpeed = 0.5f;
        public float brushStrength = 0.5f;
        public Texture flowMapTex;

        private WaterSetting waterSetting;

        public void Save(WaterSetting waterSetting)
        {
            waterSetting.resolution = resolution;
            waterSetting.areaPos = areaPos;
            waterSetting.areaSize = areaSize;
            waterSetting.flowSpeed = flowSpeed;
            waterSetting.brushStrength = brushStrength;
            waterSetting.flowMapTex = flowMapTex;
        }

        private void OnEnable()
        {
            SubscribeBeforeCameraRendering();
            SubscribeAfterCameraRendering();
        }

        private void OnDisable()
        {
            UnsubscribeBeforeCameraRendering();
            UnsubscribeAfterCameraRendering();
        }

        private void OnDestroy()
        {
        }

        void OnBeforeCameraRendering(Camera cam)
        {
        }

        void OnAfterCameraRendering(Camera cam)
        {
        }

        private void RenderWater()
        {
        }


        #region 事件

        void SubscribeBeforeCameraRendering()
        {
            Camera.onPreCull += OnBeforeCameraRendering;
        }

        void UnsubscribeBeforeCameraRendering()
        {
            Camera.onPreCull -= OnBeforeCameraRendering;
        }

        void SubscribeAfterCameraRendering()
        {
            Camera.onPostRender += OnAfterCameraRendering;
        }

        void UnsubscribeAfterCameraRendering()
        {
            Camera.onPostRender -= OnAfterCameraRendering;
        }

        #endregion
    }
}