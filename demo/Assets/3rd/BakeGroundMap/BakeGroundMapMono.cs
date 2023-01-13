using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Tools.BakeGroundMap
{
    public class BakeGroundMapMono : MonoBehaviour
    {
      
        public Vector4 uv;
        public Bounds bounds;
        public Vector3 boundSize;
        public Vector3 boundMin;
        public Vector3 boundMax;
        
        public Camera renderCam;
        public Texture2D groundTex;
        
        private void OnDrawGizmosSelected()
        {
            Color32 color = new Color(0f, 0.66f, 1f, 0.25f);
            Gizmos.color = color;
            Gizmos.DrawCube(bounds.center, bounds.size);

            color = new Color(0f, 0.66f, 1f, 1f);
            Gizmos.color = color;
            Gizmos.DrawWireCube(bounds.center, bounds.size);
        }

        public void SetBoundSize(Vector3 value)
        {
            boundSize = value;
        }

        public void SetBoundMax(Vector3 value)
        {
            boundMax = value;
        }

        public void SetBoundMin(Vector3 value)
        {
            boundMin = value;
        }
    }
}