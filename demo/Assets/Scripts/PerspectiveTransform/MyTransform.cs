using System;
using UnityEngine;

namespace czw.Trans
{
    public class MyTransform
    {
        public static Matrix4x4 ModelMatrix(Vector4 position, Vector4 scale, Vector4 rotate, Vector3 viewPos,
            Vector3 viewTarget)
        {
            Matrix4x4 M = ViewMatrix(viewPos, viewTarget) * PositionTransform(position.x, position.y, position.z)
                                                          * ScaleTransform(scale.x, scale.y, scale.z)
                                                          * RotateTransform(rotate.x, rotate.y, rotate.z);
            return M;
        }


        private static Matrix4x4 PositionTransform(float x, float y, float z)
        {
            Matrix4x4 matrix4X4 = Matrix4x4.identity;
            matrix4X4.SetRow(0, new Vector4(1, 0, 0, x));
            matrix4X4.SetRow(1, new Vector4(0, 1, 0, y));
            matrix4X4.SetRow(2, new Vector4(0, 0, 1, z));
            matrix4X4.SetRow(3, new Vector4(0, 0, 0, 1));
            return matrix4X4;
        }


        private static Matrix4x4 ScaleTransform(float x, float y, float z)
        {
            Matrix4x4 matrix4X4 = Matrix4x4.identity;
            matrix4X4.SetRow(0, new Vector4(x, 0, 0, 0));
            matrix4X4.SetRow(1, new Vector4(0, y, 0, 0));
            matrix4X4.SetRow(2, new Vector4(0, 0, z, 0));
            matrix4X4.SetRow(3, new Vector4(0, 0, 0, 1));

            return matrix4X4;
        }

        private static Matrix4x4 RotateTransform(float x, float y, float z)
        {
            Matrix4x4 matrix4X4_x = Matrix4x4.identity;
            matrix4X4_x.SetRow(0, new Vector4(1, 0, 0, 0));
            matrix4X4_x.SetRow(1, new Vector4(0, GetCos(x), GetSin(x), 0));
            matrix4X4_x.SetRow(2, new Vector4(0, -GetSin(x), GetCos(x), 0));
            matrix4X4_x.SetRow(3, new Vector4(0, 0, 0, 1));

            Matrix4x4 matrix4X4_y = Matrix4x4.identity;
            matrix4X4_y.SetRow(0, new Vector4(GetCos(y), 0, GetSin(y), 0));
            matrix4X4_y.SetRow(1, new Vector4(0, 1, 0, 0));
            matrix4X4_y.SetRow(2, new Vector4(-GetSin(y), 0, GetCos(y), 0));
            matrix4X4_y.SetRow(3, new Vector4(0, 0, 0, 1));

            Matrix4x4 matrix4X4_z = Matrix4x4.identity;
            matrix4X4_z.SetRow(0, new Vector4(GetCos(z), GetSin(z), 0, 0));
            matrix4X4_z.SetRow(1, new Vector4(-GetSin(z), GetCos(z), 0, 0));
            matrix4X4_z.SetRow(2, new Vector4(0, 0, 1, 0));
            matrix4X4_z.SetRow(3, new Vector4(0, 0, 0, 1));

            Matrix4x4 matrix4X4 = matrix4X4_x * matrix4X4_y * matrix4X4_z;
            return matrix4X4;
        }


        private static float GetDegree()
        {
            return Mathf.PI / 180;
        }

        private static float GetSin(float value)
        {
            return Mathf.Sin(GetDegree() * value);
        }

        private static float GetCos(float value)
        {
            return Mathf.Cos(GetDegree() * value);
        }

        public static Matrix4x4 ViewMatrix(Vector3 viewPos, Vector3 viewTarget)
        {
            var view_Z = Vector3.Normalize(viewPos - viewTarget);
            var view_Y = new Vector3(0, 1, 0);
            var view_X = Vector3.Cross(view_Z, view_Y);

            Matrix4x4 matrix = Matrix4x4.identity;
            matrix.SetRow(0, new Vector4(view_X.x, view_X.y, view_X.z, 0));
            matrix.SetRow(1, new Vector4(view_Y.x, view_Y.y, view_Y.z, 0));
            matrix.SetRow(2, new Vector4(view_X.x, view_X.y, view_X.z, 0));
            matrix.SetRow(3, new Vector4(0, 0, 0, 1));

            Matrix4x4 matrix_T = Matrix4x4.identity;
            matrix_T.SetRow(0, new Vector4(1, 0, 0, -viewPos.x));
            matrix_T.SetRow(1, new Vector4(0, 1, 0, -viewPos.y));
            matrix_T.SetRow(2, new Vector4(0, 0, 1, -viewPos.z));
            matrix_T.SetRow(3, new Vector4(0, 0, 0, 1));

            var viewMatrix = matrix * matrix_T;
            viewMatrix = Matrix4x4.identity;
            return viewMatrix;
        }
    }
}