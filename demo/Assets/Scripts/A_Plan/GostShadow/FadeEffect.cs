using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal;

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

        // 计时器
        private float timer;
        private bool isShow;
        private float alpha;

        public void StartEffect(Action complate)
        {
            onComplete = complate;
            timer = 0;
            if (renderer == null)
            {
                renderer = GetComponent<MeshRenderer>();
            }

            if (renderer == null || renderer.material == null)
            {
                isShow = false;
                gameObject.name = "Error GameObject";
                Debug.LogError("当前物体没有MeshRender或者没有材质，物体名称为：" + gameObject.name);
                return;
            }
            else
            {
                isShow = true;
                material = renderer.material;
                SetAlpha(this.alpha);
            }
        }

        private void Update()
        {
            if (!isShow)
            {
                return;
            }

            timer += Time.deltaTime;
            if (timer < GostShadowConstData.EXIST_TIME)
            {
                alpha = (GostShadowConstData.EXIST_TIME - timer) / GostShadowConstData.EXIST_TIME;
            }
            else
            {
                Hide();
            }
        }

        private void SetAlpha(float alpha)
        {
            material.SetFloat("_Alpha", alpha);
        }

        private void Hide()
        {
            isShow = false;
            if (onComplete != null)
            {
                onComplete();
            }

            onComplete = null;
        }
    }
}