using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SceneChange : MonoBehaviour
{
    public LoadView loadView;

    public void Change(int index)
    {
        if (loadView != null)
        {
            loadView.Show();
        }

        this.gameObject.SetActive(false);

        SceneController.Instance.LoadScene(index, (prgress) =>
        {
            if (loadView)
                loadView.UpdateProgress(prgress);
        }, () =>
        {
            if (loadView)
                loadView.Hide();
        });
    }
}