using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadView : ViewBase
{
    public Slider slider_Progress;
    public Text text_Progress;


    public void UpdateProgress(float progress)
    {
        slider_Progress.value = progress;
    }

    public void OnSliderProgressValueChange(float v)
    {
        text_Progress.text = string.Format("{0}%", Mathf.Round(v * 100));
    }
}