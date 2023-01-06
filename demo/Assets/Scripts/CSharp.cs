using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSharp : MonoBehaviour
{
    // 定义委托
    private delegate string MyDelegate(string s);

    // 声明委托
    MyDelegate myDelegate;

    [ContextMenu("Test")]
    public void TestMain()
    {
        // string s ：相当于原本方法的参数
        myDelegate = s =>
        {
            // 相当与原本方法的方法体
            Debug.LogError(s);
            return s;
        };

        myDelegate?.Invoke("测试匿名方法");
    }

    // 原本的方法可以注释掉了
    // private string TestDelegate(string s)
    // {
    //     Debug.LogError(s);
    //     return s;
    // }
}