using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace czw.GostShadow
{
    public class GostShadow : MonoBehaviour
    {
        public void Init(int id, Material material, Shader shader)
        {
        }

        public void SetActive(bool active)
        {
            this.gameObject.SetActive(active);
        }

        public void UpadeMesh(int id, Mesh mesh, Vector3 pos, Vector3 eular)
        {
        }
    }
}