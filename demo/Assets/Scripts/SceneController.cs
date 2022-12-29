using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    #region 字段

    private int currentIndex;
    private Action<float> onPeogessChage;
    private Action onFinsh;
    private static SceneController _instance;

    #endregion

    public static SceneController Instance
    {
        get
        {
            if (_instance == null)
            {
                var obj = new GameObject();
                obj.AddComponent<SceneController>();
            }

            return _instance;
        }
    }

    private void Awake()
    {
        DontDestroyOnLoad(gameObject);

        if (_instance != null)
        {
            throw new Exception("场景中存在多个SceneController");
        }

        _instance = this;
    }


    public void LoadScene(int index, Action<float> onPeogessChage, Action onFinsh)
    {
        currentIndex = index;
        this.onPeogessChage = onPeogessChage;
        this.onFinsh = onFinsh;

        StartCoroutine(LoadScene());
    }

    private IEnumerator LoadScene()
    {
        yield return null;

        var asyncOption = SceneManager.LoadSceneAsync(currentIndex, LoadSceneMode.Single);

        while (!asyncOption.isDone)
        {
            onPeogessChage?.Invoke(asyncOption.progress);
            Debug.LogError(asyncOption.progress);
        }

        yield return new WaitForSeconds(1);
        onFinsh?.Invoke();
    }
}