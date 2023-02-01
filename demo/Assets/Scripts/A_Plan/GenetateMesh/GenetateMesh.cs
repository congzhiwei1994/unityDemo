using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


// 绑定组件
[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class GenetateMesh : MonoBehaviour
{
    private Mesh mesh;
    private Vector3[] vertices;
    public int X = 2;
    public int Y = 3;

    [ContextMenu("测试")]
    public void EditorTest()
    {
        TestMesh();
    }

    void Start()
    {
        TestMesh();
    }

    private void TestMesh()
    {
        vertices = new Vector3[(X + 1) * (Y + 1)];

        mesh = new Mesh();
        mesh.name = "TestMesh";
        mesh.triangles = GetTrianges();

        var filter = GetComponent<MeshFilter>();
        filter.mesh = mesh;

        StartCoroutine(GeneratePlane());
    }

    private IEnumerator GeneratePlane()
    {
        for (int i = 0, y = 0; y < Y + 1; y++)
        {
            for (int x = 0; x < X + 1; x++, i++)
            {
                vertices[i] = new Vector3(x, y, 0);
                yield return new WaitForSeconds(0.2f);
            }
        }
    }

    private int[] GetTrianges()
    {
        int[] trianges = new int[X * Y * 6];

        for (int i = 0; i <= X * Y; i++)
        {
            trianges[i] = i;
            trianges[i + 1] = X + 1 + i;
            trianges[i + 2] = 1 + i;

            trianges[i + 3] = X + 1 + i;
            trianges[i + 4] = X + 2 + i;
            trianges[i + 5] = 1 + i;

            Debug.LogError(i);
        }

        return trianges;
    }

    private void OnDrawGizmos()
    {
        if (vertices == null)
        {
            return;
        }

        for (int i = 0; i < vertices.Length; i++)
        {
            Gizmos.DrawSphere(vertices[i], 0.1f);
        }
    }
}