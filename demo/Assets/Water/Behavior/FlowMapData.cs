using System;
using UnityEngine;
using UnityEngine.Serialization;

namespace Water
{
    [ExecuteAlways]
    [Serializable]
    public class FlowMapData : MonoBehaviour
    {
        public Texture flowTex;
        public int texSize = 1024;
        public int areaSize = 200;
        public float flowSpeed;
        public float brushStrength;
        public Vector3 areaPos = Vector3.zero;
        public Bounds bounds;

        private Renderer renderer;

        private void Update()
        {
            bounds = CalculateBoundBox();
        }

        private void OnDrawGizmosSelected()
        {
            Color32 color = new Color(0f, 0.66f, 1f, 0.25f);
            color = new Color(1f, 0f, 1f, 0.25f);
            Gizmos.color = color;
            Gizmos.DrawCube(bounds.center, bounds.size);
            // color = new Color(0f, 0.66f, 1f, 1f);
            color = new Color(1f, 0f, 1f, 1f);
            Gizmos.color = color;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        private Bounds CalculateBoundBox()
        {
            if (renderer == null)
            {
                renderer = this.gameObject.GetComponent<Renderer>();
            }

            Bounds b = new Bounds(Vector3.zero, Vector3.zero);
            Vector3 minSum = Vector3.one * 4096;
            Vector3 maxSum = Vector3.zero;

            Vector3 min = renderer.bounds.min;
            Vector3 max = renderer.bounds.max;

            if (max.x > maxSum.x || max.y > maxSum.y || max.z > maxSum.z)
                maxSum = max;
            if (min.x < minSum.x || min.y < minSum.y || min.z < minSum.z)
                minSum = min;

            b.SetMinMax(minSum, maxSum);
            
            if (b.size.y < 2f)
            {
                b.Encapsulate(new Vector3(b.center.x, b.center.y + 10f, b.center.z));
                b.Encapsulate(new Vector3(b.center.x, b.center.y - 1f, b.center.z));
            }

            return b;
        }
    }
}