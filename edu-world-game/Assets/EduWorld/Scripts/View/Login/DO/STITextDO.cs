using UnityEngine;
using DG.Tweening;

namespace EduWorld
{
    public class STITextDO : MonoBehaviour
    {
        private RectTransform rect;
        private Vector2 initPos;
        public Ease STITextEaseType;

        void Start()
        {
            rect = this.transform.GetComponent<RectTransform>();
            DOSTIText();
        }

        private void DOSTIText()
        {
            initPos = rect.anchoredPosition;
            rect.anchoredPosition = new Vector2(-Screen.width, rect.anchoredPosition.y);
            rect.DOAnchorPos(initPos, 2.0f).SetEase(STITextEaseType);
        }
    }
}