using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace czw.Trans
{
    [ExecuteInEditMode]
    public class PerspectiveTransform : MonoBehaviour
    {
        public Vector4 position = Vector4.zero;
        public Vector4 scale = Vector4.one;
        public Vector4 rotate = Vector4.zero;
        public Vector3 viewPos;
        public Vector3 targetPos;

        // public Camera camera;
        // public GameObject target;

        private Mesh myMesh;
        private Vector3[] oldPos;
        private List<Vector4> verticeList;
 

        void Start()
        {
            myMesh = this.gameObject.GetComponent<MeshFilter>().sharedMesh;
            oldPos = myMesh.vertices;
        }

        void Update()
        {
            if (myMesh == null)
            {
                myMesh = this.gameObject.GetComponent<MeshFilter>().sharedMesh;
            }

            
            var vertex = SetVertices();
            myMesh.SetVertices(vertex);
        }


        private List<Vector3> SetVertices()
        {
            List<Vector4> ver4D = new List<Vector4>();
            List<Vector3> ver3D = new List<Vector3>();

            for (int i = 0; i < oldPos.Length; i++)
            {
                // ÉèÖÃÆë´Î×ø±ê
                ver4D.Add(new Vector4(oldPos[i].x, oldPos[i].y, oldPos[i].z, 1));
                ver4D[i] = MyTransform.ModelMatrix(position, scale, rotate, viewPos, targetPos) * ver4D[i];

                ver3D.Add(new Vector3(ver4D[i].x, ver4D[i].y, ver4D[i].z));
            }

            return ver3D;
        }
    }
}