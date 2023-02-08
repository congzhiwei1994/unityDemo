using System;
using UnityEngine;

namespace czw.FlowMapTool
{
    [ExecuteAlways]
    [Serializable]
    public class WaterSetting : MonoBehaviour
    {
        public FlowMapResolution resolution = FlowMapResolution._1024;
        public int areaSize = 200;
        public Vector3 areaPos = Vector3.zero;
        public float flowSpeed = 0.5f;
        public float brushStrength = 0.5f;
        public Texture flowMapTex;
    }
}