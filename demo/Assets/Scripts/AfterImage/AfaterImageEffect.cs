using System;
using UnityEngine;

namespace Render.AfterImagPool
{
    public class AfaterImageEffect : MonoBehaviour
    {
        private MeshRenderer renderer;
        private Material material;
        private float timer; // 计时器
        private float alpha;
        private bool isShow;
        private Action onComplete;
        private Color _color;
    
        public void StartEffect(Action onComplete)
        {
            this.onComplete = onComplete;
            timer = 0;
            if (renderer == null)
                renderer = GetComponent<MeshRenderer>();

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
                _color = new Color(1f, 0, 0, 1);
                SetColor(_color);
                renderer.material = material;
            }
        }

        private void Update()
        {
            if (!isShow)
            {
                return;
            }

            timer += Time.deltaTime;
            if (timer < AferImageData.EXIST_TIME)
            {
                alpha = (AferImageData.EXIST_TIME - timer) / AferImageData.EXIST_TIME;
                _color.a = alpha;
                SetColor(_color);
            }
            else
            {
                Hide();
            }
        }

        private void SetColor(Color color)
        {
            material.SetColor(AferImageData.SHADER_COLOR_NAME, color);
        }

        private void Hide()
        {
            isShow = false;
            onComplete?.Invoke();
            onComplete = null;
        }
    }
}