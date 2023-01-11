using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace czw.GostShadow
{
    public enum SpwanState
    {
        ENABLE,
        DISABLE
    }

    /// <summary>
    /// 内存池用来管理多个Item
    /// </summary>
    public class GostShadowMgr : MonoBehaviour
    {
        // 活跃
        private List<GostShadow> _activeList;

        //非活跃
        private List<GostShadow> _inactiveList;
        private SkinnedMeshRenderer[] _renderers;

        private SpwanState spwanState;

        // 上一次时间
        private float lastTime;

        public void Init()
        {
            spwanState = SpwanState.DISABLE;
            _activeList = new List<GostShadow>();
            _inactiveList = new List<GostShadow>();

            _renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (_renderers == null)
                _renderers = new SkinnedMeshRenderer[0];
        }

        /// <summary>
        /// 外部调用，动态修改状态
        /// </summary>
        public void SetSpwanState(SpwanState state)
        {
            spwanState = state;
        }

        private bool JudgeState()
        {
            return spwanState == SpwanState.ENABLE;
        }

        private void Update()
        {
            if (JudgeState() && Time.time - lastTime > GostShadowConstData.SPAWN_INTERVAL_TIME)
            {
                lastTime = Time.time;
                Spwan();
            }
        }

        /// <summary>
        /// 生成
        /// </summary>
        public void Spwan()
        {
            GostShadow item = null;
            if (_inactiveList.Count > 0)
            {
                item = _inactiveList[0];
                _inactiveList.Remove(item);
            }
            else
            {
                item = SpwanNew();
                InitGostShadow(item);
            }

            item.SetActive(true);
            UpdateShadow(item);
            _activeList.Add(item);
        }

        /// <summary>
        /// 初始化 GostShadow，执行GostShadow。Init()。
        /// </summary>
        /// <param name="item"></param>
        private void InitGostShadow(GostShadow item)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                item.Init(i, _renderers[i].material, GetShader(), Despwan);
            }
        }

        private Shader GetShader()
        {
            return Shader.Find(GostShadowConstData.SHADER_NAME);
        }

        /// <summary>
        /// 回收
        /// </summary>
        public void Despwan(GostShadow item)
        {
            _activeList.Remove(item);
            _inactiveList.Add(item);
            item.SetActive(false);
        }

        /// <summary>
        /// 更新状态
        /// </summary>
        private void UpdateShadow(GostShadow item)
        {
            for (int i = 0; i < _renderers.Length; i++)
            {
                Mesh mesh = new Mesh();
                _renderers[i].BakeMesh(mesh);
                item.UpadeMesh(i, mesh, _renderers[i].transform.position, _renderers[i].transform.rotation);
            }
        }

        /// <summary>
        /// 初始化组件
        /// </summary>
        private GostShadow SpwanNew()
        {
            GameObject go = new GameObject("SpwanNew");
            return go.AddComponent<GostShadow>();
        }
    }
}