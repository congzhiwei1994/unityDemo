using System;
using System.Collections;
using UnityEngine;

namespace Render.AfterImagPool
{
    public enum HumanState
    {
        IDLE,
        MOVE
    }

    public class AfterImageGameCtrl : MonoBehaviour
    {
        private AfterImagPool _pool;
        private AfterImageMove _move;
        private HumanState _state;

        private void Start()
        {
            _pool = GetComponent<AfterImagPool>();
            _move = GetComponent<AfterImageMove>();

            _pool.Init();
            StartCoroutine(ProcessCtrl());
        }

        private IEnumerator ProcessCtrl()
        {
            while (true)
            {
                Execute(HumanState.MOVE);
                yield return new WaitForSeconds(2);
                Execute(HumanState.IDLE);
                yield return new WaitForSeconds(2);
            }
        }

        private void Execute(HumanState state)
        {
            switch (state)
            {
                case HumanState.MOVE:

                    _pool.SetSpwanState(SpwanState.ENABLE);
                    _move.Play();

                    break;
                case HumanState.IDLE:

                    _pool.SetSpwanState(SpwanState.DISABLE);
                    _move.Pause();
                    break;
            }
        }
    }
}