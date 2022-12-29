using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameMgr : SingletonMono<GameMgr>
{
    protected override void Awake()
    {
        Debug.Log("展示程序开始");
    }
}
