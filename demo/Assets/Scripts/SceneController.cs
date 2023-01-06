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
        asyncOption.allowSceneActivation = false;

        float cur = 0.00f;
        while (true)
        {
            if (asyncOption.progress < 0.90f)
            {
                //没加载完之前的进度条速度
                cur += 0.2f * Time.deltaTime;
                cur = Mathf.Min(cur, 0.90f);
            }
            else
            {
                //加载完之后的进度条速度
                cur += 0.4f * Time.deltaTime;
                cur = Mathf.Min(cur, 1.00f);
            }

            onPeogessChage?.Invoke(cur);
            Debug.LogError("Real Progress: " + asyncOption.progress + " : Fake Progress: " + cur);

            if (cur >= 1.0f)
            {
                asyncOption.allowSceneActivation = true;
                break;
            }
            yield return null;
        }

        //等待真的100%
        yield return new WaitUntil(() =>
        {
            return asyncOption.isDone;
        });

        Debug.LogError("Load Done: " + currentIndex);
        onFinsh?.Invoke();
    }
}