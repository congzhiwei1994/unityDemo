using System;
using UnityEngine;

namespace Render.AfterImagPool
{
    /// <summary>
    /// 管理每一个Skin Mesh
    /// </summary>
    public class AfterImagItem : MonoBehaviour
    {
        private MeshFilter filter;
        private MeshRenderer renderer;
        private AfaterImageEffect _effect;

        public void Init(Material material, Shader shader)
        {
            if (filter == null)
                filter = this.gameObject.AddComponent<MeshFilter>();
            if (renderer == null)
                renderer = this.gameObject.AddComponent<MeshRenderer>();
            if (_effect == null)
                _effect = this.gameObject.AddComponent<AfaterImageEffect>();

            renderer.material = material;
            if (shader != null)
                renderer.material.shader = shader;
        }

        public void UpdateMesh(Mesh mesh, Vector3 pos, Quaternion rot, Action complete)
        {
            filter.mesh = mesh;
            this.transform.position = pos;
            this.transform.rotation = rot;
            _effect.StartEffect(complete);
        }
    }
}