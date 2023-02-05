using UnityEngine;
using UnityEngine.Rendering;

namespace czw.RenderFeature
{
    public class RenderFeatureUtils
    {
        /// <summary>
        /// 获取材质
        /// </summary>
        public static Material GetMaterial(Material materal, Shader shader, string shaderPath)
        {
            if (materal == null)
            {
                if (shader == null)
                {
                    shader = Shader.Find(shaderPath);
                }

                materal = CoreUtils.CreateEngineMaterial(shader);
            }

            return materal;
        }

        
        public static Matrix4x4 CaulateWorldPosMatrixByRay(Camera camera)
        {
            var cameraTrans = camera.transform;

            float fov = camera.fieldOfView;
            float near = camera.nearClipPlane;
            float aspect = camera.aspect;

            float halfHeight = near * Mathf.Tan(fov * 0.5f * Mathf.Deg2Rad);
            var toRight = cameraTrans.right * aspect * halfHeight;
            var toTop = cameraTrans.up * halfHeight;

            var topLeft = cameraTrans.forward * near + toTop - toRight;

            float scale = topLeft.magnitude / near; // |X|/Near
            topLeft.Normalize();
            topLeft *= scale;

            Vector3 topRight = cameraTrans.forward * near + toRight + toTop; //相机到近平面右上角的向量
            topRight.Normalize();
            topRight *= scale;

            Vector3 bottomLeft = cameraTrans.forward * near - toTop - toRight; //相机到近平面左下角的向量
            bottomLeft.Normalize();
            bottomLeft *= scale;

            Vector3 bottomRight = cameraTrans.forward * near + toRight - toTop; //相机到近平面右下角的向量
            bottomRight.Normalize();
            bottomRight *= scale;

            Matrix4x4 frustumCorners = Matrix4x4.identity; //4*4的单位矩阵
            frustumCorners.SetRow(0, bottomLeft);
            frustumCorners.SetRow(1, bottomRight);
            frustumCorners.SetRow(2, topRight);
            frustumCorners.SetRow(3, topLeft);
            return frustumCorners;
        }
    }
}