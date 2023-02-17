using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode] //在编辑模式下也运行脚本
public class TC_Transform2 : MonoBehaviour
{
    [Header("TransformPara")] public Vector3 myScale = Vector3.one; // 缩放参数
    public Vector3 myRotation = Vector3.zero; // 旋转参数
    public Vector3 myPosition = Vector3.zero; // 位移参数

    public Vector4 myPerspectivePra = new Vector4(0, 0, 0, 1); //投影参数:0,0,0,1是不进投影变化

    //【其他变量】
    private Mesh myMesh; // 网格
    private Vector3[] myFirstSVertices; // 初始的位置

    ////-------------------------------------------------------------------------------
    void Start()
    {
        myMesh = this.GetComponent<MeshFilter>().sharedMesh;
        myFirstSVertices = myMesh.vertices; // 网格初始顶点位置
    }

    void Update()
    {
        TransformFun(); // 顶点变换
    }
    ////-------------------------------------------------------------------------------


    //// 顶点变换
    void TransformFun()
    {
        // 构造缩放、旋转、平移矩阵
        Matrix4x4 myScaleMatrix = SetScaleMatrixFun(myScale.x, myScale.y, myScale.z);
        Matrix4x4 myRotateMatrix = SetRotateMatrixFun(myRotation.x, myRotation.y, myRotation.z);
        Matrix4x4 myPositionMatrix = SetPositionMatrixFun(myPosition.x, myPosition.y, myPosition.z);
        // 仿射变换矩阵 =  位移 * 旋转 * 缩放  
        Matrix4x4 myAffineMatrix = myPositionMatrix * myRotateMatrix * myScaleMatrix;
        // 构造投影矩阵
        Matrix4x4 myPerspectiveMatrix = SetPerspectiveMatrix(myPerspectivePra);

        // 总变换矩阵
        Matrix4x4 myTranMatrix = myPerspectiveMatrix * myAffineMatrix;

        // 顶点变换
        SetVerticesFun(myTranMatrix, myMesh, myFirstSVertices);
    }


    //【函数：建立缩放矩阵】
    Matrix4x4 SetScaleMatrixFun(float scale_x, float scale_y, float scale_z)
    {
        // 缩放矩阵
        Vector4[] scaleMatrixPara =
        {
            new Vector4(scale_x, 0, 0, 0),
            new Vector4(0, scale_y, 0, 0),
            new Vector4(0, 0, scale_z, 0),
            new Vector4(0, 0, 0, 1)
        };
        Matrix4x4 scaleMatrix = new Matrix4x4(); //缩放矩阵
        for (int i = 0; i < 4; i++)
        {
            scaleMatrix.SetRow(i, scaleMatrixPara[i]); // 设置矩阵每一行的值
        }

        return scaleMatrix;
    }


    //【函数：建立旋转矩阵(角度制)】
    Matrix4x4 SetRotateMatrixFun(float rotate_x, float rotate_y, float rotate_z)
    {
        // 三角函数(角度制)
        float CosDegreeFun(float degree) // Cos角度制
        {
            return Mathf.Cos(Mathf.PI / 180 * degree);
        }

        float SinDegreeFun(float degree) // Sin角度制
        {
            return Mathf.Sin(Mathf.PI / 180 * degree);
        }

        // 绕x轴旋转矩阵
        Vector4[] rotaionMatrixPara_x =
        {
            new Vector4(1, 0, 0, 0),
            new Vector4(0, CosDegreeFun(rotate_x), -SinDegreeFun(rotate_x), 0),
            new Vector4(0, SinDegreeFun(rotate_x), CosDegreeFun(rotate_x), 0),
            new Vector4(0, 0, 0, 1)
        };
        Matrix4x4 roationMatrix_x = new Matrix4x4(); // 绕x轴旋转矩阵
        for (int i = 0; i < 4; i++)
        {
            roationMatrix_x.SetRow(i, rotaionMatrixPara_x[i]); // 设置矩阵每一行的值
        }

        // 绕y轴旋转矩阵
        Vector4[] rotaionMatrixPara_y =
        {
            new Vector4(CosDegreeFun(rotate_y), 0, SinDegreeFun(rotate_y), 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(-SinDegreeFun(rotate_y), 0, CosDegreeFun(rotate_y), 0),
            new Vector4(0, 0, 0, 1)
        };
        Matrix4x4 roationMatrix_y = new Matrix4x4(); // 绕y轴旋转矩阵
        for (int i = 0; i < 4; i++)
        {
            roationMatrix_y.SetRow(i, rotaionMatrixPara_y[i]); // 设置矩阵每一行的值
        }

        // 绕z轴旋转矩阵
        Vector4[] rotaionMatrixPara_z =
        {
            new Vector4(CosDegreeFun(rotate_z), -SinDegreeFun(rotate_z), 0, 0),
            new Vector4(SinDegreeFun(rotate_z), CosDegreeFun(rotate_z), 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(0, 0, 0, 1)
        };
        Matrix4x4 roationMatrix_z = new Matrix4x4(); // 绕z轴旋转矩阵
        for (int i = 0; i < 4; i++)
        {
            roationMatrix_z.SetRow(i, rotaionMatrixPara_z[i]); // 设置矩阵每一行的值
        }

        // 总旋转矩阵
        Matrix4x4 roationMatrix = roationMatrix_x * roationMatrix_y * roationMatrix_z;

        return roationMatrix;
    }


    //【函数：建立平移矩阵】
    Matrix4x4 SetPositionMatrixFun(float pos_x, float pos_y, float pos_z)
    {
        //位移矩阵
        Vector4[] positionMatrixPara =
        {
            new Vector4(1, 0, 0, pos_x),
            new Vector4(0, 1, 0, pos_y),
            new Vector4(0, 0, 1, pos_z),
            new Vector4(0, 0, 0, 1)
        };
        Matrix4x4 positionMatrix = new Matrix4x4();
        for (int i = 0; i < 4; i++)
        {
            positionMatrix.SetRow(i, positionMatrixPara[i]); // 设置矩阵每一行的值
        }

        return positionMatrix;
    }


    ////【函数：建投影矩阵】,传入投影参数
    Matrix4x4 SetPerspectiveMatrix(Vector4 PerPara)
    {
        //投影矩阵
        Vector4[] perspectiveMatrixPara =
        {
            new Vector4(1, 0, 0, 0),
            new Vector4(0, 1, 0, 0),
            new Vector4(0, 0, 1, 0),
            new Vector4(PerPara.x, PerPara.y, PerPara.z, PerPara.w)
        };
        Matrix4x4 perspectiveMatrix = new Matrix4x4();
        for (int i = 0; i < 4; i++)
        {
            perspectiveMatrix.SetRow(i, perspectiveMatrixPara[i]); // 设置矩阵每一行的值
        }

        return perspectiveMatrix;
    }

    ////【顶点变换函数】,传入：变换矩阵、网格、网格每个顶点的初始位置、是否投影变换
    void SetVerticesFun(Matrix4x4 matrix, Mesh mesh, Vector3[] firstVertices)
    {
        // List,记录每个顶点的位置
        List<Vector3> meshList = new List<Vector3>();
        // 将初始顶点位置拓展为4维
        List<Vector4> meshFirstList4D = new List<Vector4>();
        // 遍历每个顶点，进行顶点变换
        for (var i = 0; i < mesh.vertexCount; i++)
        {
            // 将初始顶点位置拓展为4维
            meshFirstList4D.Add(new Vector4(firstVertices[i].x, firstVertices[i].y, firstVertices[i].z, 1.0f));
            //投影变换
            meshFirstList4D[i] = matrix * meshFirstList4D[i];
            meshFirstList4D[i] /= meshFirstList4D[i].w;
            // 更新每个顶点的位置
            meshList.Add((new Vector3(meshFirstList4D[i].x, meshFirstList4D[i].y, meshFirstList4D[i].z)));
        }

        mesh.SetVertices(meshList); // 给mesh的顶点赋值List的内容
    }
}