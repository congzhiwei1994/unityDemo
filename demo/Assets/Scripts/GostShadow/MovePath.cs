using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using DG.Tweening;

namespace czw.GostShadow
{
    public class MovePath : MonoBehaviour
    {
        public List<Transform> points;

        private const string MOVE_ID = "MOVE";

        // Start is called before the first frame update
        void Start()
        {
            var pos = points.Select(i => i.position).ToArray();
            transform
                .DOPath(pos, 2)
                .SetOptions(true)
                .SetLookAt(0)
                .SetLoops(-1)
                .SetEase(Ease.Linear)
                .SetId(MOVE_ID);
        }

        public void Pause()
        {
            DOTween.Pause(MOVE_ID);
        }

        public void Continue()
        {
            DOTween.Play(MOVE_ID);
        }
    }
}