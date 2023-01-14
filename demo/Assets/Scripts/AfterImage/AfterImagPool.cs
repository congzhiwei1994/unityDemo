using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Render.AfterImagPool
{
    public enum SpwanState
    {
        ENABLE,
        DISABLE
    }

    public class AfterImagPool : MonoBehaviour
    {
        private List<AfterImag> activeList;
        private List<AfterImag> inactiveList;
        private SkinnedMeshRenderer[] skinRenders;
        private SpwanState _state;

        // 上一次时间
        private float lastTime;
        
        public void Init()
        {
            activeList = new List<AfterImag>();
            inactiveList = new List<AfterImag>();
            _state = SpwanState.DISABLE;

            skinRenders = GetComponentsInChildren<SkinnedMeshRenderer>();
            if (skinRenders == null)
                skinRenders = new SkinnedMeshRenderer[0];

            Spwan();
        }

        private void Update()
        {
            if (JudgeState() && Time.time - lastTime > AferImageData.SPAWN_INTERVAL_TIME)
            {
                Spwan();
                lastTime = Time.time;
            }
        }

        private void Spwan()
        {
            AfterImag afterImag = null;
            if (inactiveList.Count > 0)
            {
                afterImag = inactiveList[0];
                inactiveList.Remove(inactiveList[0]);
            }
            else
            {
                afterImag = CreatAfterImag();
                InitAfterImag(afterImag);
            }

            afterImag.SetActive(true);
            UpdateMesh(afterImag, Despwan);

            activeList.Add(afterImag);
        }

        public void SetSpwanState(SpwanState state)
        {
            this._state = state;
        }

        public bool JudgeState()
        {
            return this._state == SpwanState.ENABLE;
        }


        private void Despwan(AfterImag afterImag)
        {
            activeList.Remove(afterImag);
            inactiveList.Add(afterImag);
            afterImag.SetActive(false);
        }

        private AfterImag CreatAfterImag()
        {
            GameObject go = new GameObject(this.name + "AfterImag");
            var ai = go.AddComponent<AfterImag>();
            return ai;
        }

        private void InitAfterImag(AfterImag afterImag)
        {
            for (int i = 0; i < skinRenders.Length; i++)
            {
                afterImag.Init(i, skinRenders[i].material, GetShader());
            }
        }

        private Shader GetShader()
        {
            return Shader.Find(AferImageData.SHADER_NAME);
        }

        /// <summary>
        /// 更新mesh
        /// </summary>
        private void UpdateMesh(AfterImag afterImag, Action<AfterImag> complete)
        {
            for (int i = 0; i < skinRenders.Length; i++)
            {
                Mesh mesh = new Mesh();
                skinRenders[i].BakeMesh(mesh);
                var trans = skinRenders[i].transform;
                afterImag.UpdateMesh(i, mesh, trans.position, trans.rotation, () => complete(afterImag));
            }
        }
    }
}