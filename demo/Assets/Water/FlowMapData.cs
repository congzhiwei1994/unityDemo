using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Water
{
    [Serializable]
    public class FlowMapData : MonoBehaviour
    {
        public Texture flowTex;
        public int texSize = 1024;
        public int areaSize = 200;
        public float flowSpeed;
        public float brushStrength;
        public Vector3 areaPos = Vector3.zero;
    }
}