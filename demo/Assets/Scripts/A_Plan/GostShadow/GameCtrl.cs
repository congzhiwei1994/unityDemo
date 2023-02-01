using System;
using System.Collections;
using UnityEngine;

namespace czw.GostShadow
{
    public class GameCtrl : MonoBehaviour
    {
        private GostShadowMgr _shadowMgr;

        private MovePath _movePath;

        private void Start()
        {
            _movePath = GetComponent<MovePath>();
            _shadowMgr = GetComponent<GostShadowMgr>();
            _shadowMgr.Init();
            StartCoroutine(ProcessCtrl());
        }

        private IEnumerator ProcessCtrl()
        {
            while (true)
            {
                Execute(HumanState.MOVE);
                yield return new WaitForSeconds(3);
                Execute(HumanState.IDLE);
                yield return new WaitForSeconds(2);
            }
        }

        private void Execute(HumanState state)
        {
            switch (state)
            {
                case HumanState.IDLE:
                    _shadowMgr.SetSpwanState(SpwanState.DISABLE);
                    _movePath.Pause();
                    break;

                case HumanState.MOVE:
                    _shadowMgr.SetSpwanState(SpwanState.ENABLE);
                    _movePath.Continue();
                    break;
            }
        }
    }

    public enum HumanState
    {
        IDLE,
        MOVE
    }
}