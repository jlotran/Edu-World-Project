using UnityEngine;
using DG.Tweening;

namespace EduWorld
{
    public class LogoDO : MonoBehaviour
    {
        private RectTransform rect;
        public Ease easeType;

        void Start()
        {
            rect = this.transform.GetComponent<RectTransform>();
            DOLogo();
        }

        private void DOLogo()
        {
            rect.localScale = Vector3.zero;
            rect.DOScale(Vector3.one, 2.0f).SetEase(easeType);
        }
    }
}