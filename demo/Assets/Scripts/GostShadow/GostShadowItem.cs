using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace czw.GostShadow
{
    public class GostShadowItem : MonoBehaviour
    {
        private MeshFilter filter;
        private MeshRenderer renderer;
        private FadeEffect fadeEffect;
        private Action onComplete;

        public void Init(Material material, Shader shader, Action complete)
        {
            if (filter == null)
                filter = this.gameObject.AddComponent<MeshFilter>();
            if (renderer == null)
                renderer = this.gameObject.AddComponent<MeshRenderer>();
            if (shader != null)
                material.shader = shader;
            if (fadeEffect == null)
                fadeEffect = this.gameObject.AddComponent<FadeEffect>();

            renderer.material = material;
            onComplete = complete;
        }

        public void UpadeMesh(Mesh mesh, Vector3 pos, Quaternion rot)
        {
            if (filter == null)
            {
                Debug.LogError("MeshFilter --" + "is null");
                return;
            }

            filter.mesh = mesh;
            transform.position = pos;
            transform.rotation = rot;
            fadeEffect.StartEffect(onComplete);
        }
    }
}