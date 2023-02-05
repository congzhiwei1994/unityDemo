using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// [ExecuteAlways]
public class SetScanCenterPosition : MonoBehaviour
{
    public GameObject target;
    private float time;
    private bool isEnable = false;
    private void OnEnable()
    {
        if (target == null)
        {
            return;
        }

        isEnable = false;
        time = 0;
        SetScanDistace(time);
    }


    void Update()
    {
        time += Time.deltaTime;
        Shader.SetGlobalVector("_ScanCenter", target.transform.position);
        if (Input.GetKeyDown(KeyCode.Q))
        {
            time = 0;
            isEnable = true;
        }

        if (isEnable)
        {
            SetScanDistace(time);
            if (time == 20)
            {
                isEnable = false;
                time = 0;
                SetScanDistace(time);
            }
        }
    }

    void SetScanDistace(float distance)
    {
        Shader.SetGlobalFloat("_ScanDistance", distance);
    }

    private void OnDisable()
    {
        time = 0;
        SetScanDistace(time);
    }
}