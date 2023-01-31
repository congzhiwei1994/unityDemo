using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Example : MonoBehaviour
{
    void Start()
    {
        Ray r = new Ray();
        print(r.GetPoint(10)); // a point 10 units along the ray
    }
}