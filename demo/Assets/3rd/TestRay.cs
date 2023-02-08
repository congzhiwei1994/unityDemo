using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TestRay : MonoBehaviour
{
    //需要移动的cube的预制
    public GameObject m_Cube;
    //距离射线原点的距离
    public float m_DistanceZ = 10.0f;
 
    Plane m_Plane;
    Vector3 m_DistanceFromCamera;
    

    void Start()
    {
        var camPos = Camera.main.transform.position;

        m_DistanceFromCamera = new Vector3(camPos.x, camPos.y, camPos.z + m_DistanceZ);
        //在距离m_DistanceFromCamera处，创建一个方向为forward的Plane
        m_Plane = new Plane(Vector3.forward, m_DistanceFromCamera);
 
    }

    void Update()
    {
        if (Input.GetMouseButton(0))
        {
 
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
 
            float enter = 0.0f;
 
            if (m_Plane.Raycast(ray, out enter))
            {
                Debug.DrawRay(ray.origin, ray.direction * enter, Color.green);
                //得到点击的点
                Vector3 hitPoint = ray.GetPoint(enter);
                //将你的方块游戏对象移动到你点击的地方
                m_Cube.transform.position = hitPoint;
            }
            else
            {
                Debug.Log($"enter{enter}");
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            }
        }
        
    }
    
    
    Plane plane = new Plane(Vector3.up, Vector3.zero);
    
    private void LateUpdate()
    {
        if (Input.GetMouseButton(0))
        {
            var ray = Camera.main.ScreenPointToRay(Input.mousePosition);
    
            float ent = 100.0f;
            if (plane.Raycast(ray, out ent))
            {
                Debug.Log("Plane Raycast hit at distance: " + ent);
                var hitPoint = ray.GetPoint(ent);
    
                var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
                go.transform.position = hitPoint;
                Debug.DrawRay(ray.origin, ray.direction * ent, Color.green);
            }
            else
            {
                Debug.DrawRay(ray.origin, ray.direction * 10, Color.red);
            }
    
        }
    }
}
