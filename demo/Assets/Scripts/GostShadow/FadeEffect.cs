using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace czw.GostShadow
{
    /// <summary>
    /// 控制当前材质的Alpha，来控制当前物体的显示和隐藏
    /// </summary>
    public class FadeEffect : MonoBehaviour
    {
        private Action onComplete;
        private MeshRenderer renderer;
        private Material material;

        private void Update()
        {
        }

        public void StartEffect(Action complate)
        {
            onComplete = complate;

            if (renderer == null)
            {
                renderer = GetComponent<MeshRenderer>();
            }

            if (renderer == null || renderer.material == null)
            {
                gameObject.name = "Error GameObject";
                Debug.LogError("当前物体没有MeshRender或者没有材质，物体名称为：" + gameObject.name);
                return;
            }
            else
            {
                material = renderer.material;
            }
        }

        private void Hide()
        {
        }
    }
}