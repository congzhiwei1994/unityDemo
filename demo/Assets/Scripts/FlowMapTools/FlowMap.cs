using System;
using UnityEngine;

namespace czw.FlowMapTool
{
    public enum FlowMapResolution
    {
        _512 = 512,
        _1024 = 1024,
        _2048 = 2048,
        _4096 = 4096,
    }

    public class FlowMap : MonoBehaviour
    {
        private FlowMapResolution _resolution = FlowMapResolution._1024;
        public int areaSize = 200;
        public Vector3 areaPos = Vector3.zero;
        

        private void OnEnable()
        {
        }

        private void OnDisable()
        {
        }

        private void OnDestroy()
        {
        }
    }
}