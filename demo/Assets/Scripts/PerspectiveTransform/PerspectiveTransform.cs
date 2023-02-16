using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class PerspectiveTransform : MonoBehaviour
{
    public Vector4 position = Vector4.zero;
    public Vector4 scale = Vector4.one;
    public Vector4 rotate = Vector4.zero;

    public GameObject gameObject;

    void Start()
    {
    }

    void Update()
    {
        if (gameObject == null)
            return;

    }


    private Matrix4x4 Transform()
    {
        Matrix4x4 M = PositionTransform(position.x, position.y, position.z) *
                      ScaleTransform(scale.x, scale.y, scale.z) * RotateTransform(rotate.x, rotate.y, rotate.z);
        return M;
    }

    private Matrix4x4 PositionTransform(float x, float y, float z)
    {
        Matrix4x4 matrix4X4 = Matrix4x4.identity;
        matrix4X4.SetRow(0, new Vector4(0, 0, 0, x));
        matrix4X4.SetRow(1, new Vector4(0, 0, 0, y));
        matrix4X4.SetRow(2, new Vector4(0, 0, 0, z));
        matrix4X4.SetRow(3, new Vector4(0, 0, 0, 1));

        return matrix4X4;
    }


    private Matrix4x4 ScaleTransform(float x, float y, float z)
    {
        Matrix4x4 matrix4X4 = Matrix4x4.identity;
        matrix4X4.SetRow(0, new Vector4(x, 0, 0, 0));
        matrix4X4.SetRow(1, new Vector4(0, y, 0, 0));
        matrix4X4.SetRow(2, new Vector4(0, 0, z, 0));
        matrix4X4.SetRow(3, new Vector4(0, 0, 0, 1));

        return matrix4X4;
    }

    private Matrix4x4 RotateTransform(float x, float y, float z)
    {
        Matrix4x4 matrix4X4_x = Matrix4x4.identity;
        matrix4X4_x.SetRow(0, new Vector4(1, 0, 0, 0));
        matrix4X4_x.SetRow(1, new Vector4(0, GetCos(x), GetSin(x), 0));
        matrix4X4_x.SetRow(2, new Vector4(0, -GetSin(x), GetCos(x), 0));
        matrix4X4_x.SetRow(3, new Vector4(0, 0, 0, 1));

        Matrix4x4 matrix4X4_y = Matrix4x4.identity;
        matrix4X4_y.SetRow(0, new Vector4(GetCos(y), 0, GetSin(y), 0));
        matrix4X4_y.SetRow(1, new Vector4(0, 1, 0, 0));
        matrix4X4_y.SetRow(2, new Vector4(-GetSin(x), 0, GetCos(x), 0));
        matrix4X4_y.SetRow(3, new Vector4(0, 0, 0, 1));

        Matrix4x4 matrix4X4_z = Matrix4x4.identity;
        matrix4X4_z.SetRow(0, new Vector4(GetCos(z), GetSin(z), 0, 0));
        matrix4X4_z.SetRow(1, new Vector4(-GetSin(z), GetCos(z), 0, 0));
        matrix4X4_z.SetRow(2, new Vector4(0, 0, 1, 0));
        matrix4X4_z.SetRow(3, new Vector4(0, 0, 0, 1));

        Matrix4x4 matrix4X4 = matrix4X4_x * matrix4X4_y * matrix4X4_z;
        return matrix4X4;
    }


    private float GetSin(float value)
    {
        return Mathf.Sin(value);
    }

    private float GetCos(float value)
    {
        return Mathf.Cos(value);
    }
}