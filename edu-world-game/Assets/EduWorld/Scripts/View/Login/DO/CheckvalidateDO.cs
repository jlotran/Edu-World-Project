using UnityEngine;
using DG.Tweening;

namespace EduWorld
{
    public class CheckvalidateDO : MonoBehaviour
    {
        private RectTransform rect;
        public Ease easeType;

        void Awake()
        {
            rect = this.transform.GetComponent<RectTransform>();
        }

        void OnEnable()
        {
            DOLogo();
        }

        private void DOLogo()
        {
            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one, .5f).SetEase(easeType);
        }
    }
}