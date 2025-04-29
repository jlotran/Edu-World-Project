using UnityEngine;
using DG.Tweening;

namespace EduWorld
{
    public class LoginMethodDO : MonoBehaviour
    {
        private RectTransform rect;
        private Vector2 initPos;
        public Ease easeType;

        void Start()
        {
            rect = this.transform.GetComponent<RectTransform>();
            DOLoginMethod();
        }

        private void DOLoginMethod()
        {
            initPos = rect.anchoredPosition;
            rect.anchoredPosition = new Vector2(rect.anchoredPosition.x, -Screen.height);
            rect.DOAnchorPos(initPos, 1.3f).SetEase(easeType);
        }
    }
}