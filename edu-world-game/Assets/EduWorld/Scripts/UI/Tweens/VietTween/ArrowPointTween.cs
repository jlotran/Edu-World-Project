using UnityEngine;
using DG.Tweening;

namespace RGSK
{
    public class ArrowPointTween : MonoBehaviour
    {
        [SerializeField] private float rotationDuration = 1f;
        private Sequence bounceSequence;

        void Start()
        {            
            bounceSequence = DOTween.Sequence();
                        
            bounceSequence.Join(transform.DORotate(new Vector3(0, 360, 0), rotationDuration, RotateMode.FastBeyond360)
                                       .SetEase(Ease.Linear)
                                       .SetRelative());
            
            bounceSequence.SetLoops(-1);
        }

        void OnDestroy()
        { 
            bounceSequence?.Kill();
        }

    }
}
