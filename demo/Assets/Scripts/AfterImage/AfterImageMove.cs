using System.Collections.Generic;
using System.Linq;
using DG.Tweening;
using UnityEngine;

namespace Render.AfterImagPool
{
    public class AfterImageMove : MonoBehaviour
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

        public void Play()
        {
            DOTween.Play(MOVE_ID);
        }
    }
}