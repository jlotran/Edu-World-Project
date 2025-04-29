using DG.Tweening;
using UnityEngine;

namespace EduWorld
{
    public class PanelPrefabTween : MonoBehaviour
    {
        public RectTransform graphicHolder;
        Tweener enterTween;
        Tweener fadeTween;
        public Vector3 originPosition;
        private void Start()
        {
            originPosition = graphicHolder.localPosition;
            graphicHolder.localPosition = new Vector3(graphicHolder.rect.width, originPosition.y, 0);
            CanvasGroup canvasGroup = graphicHolder.GetComponent<CanvasGroup>();
            canvasGroup.alpha = 0;
            enterTween = graphicHolder.DOLocalMoveX(originPosition.x, 0.5f).SetEase(Ease.OutQuint).SetAutoKill(false).Pause();
            fadeTween = canvasGroup.DOFade(1, 0.5f).SetEase(Ease.OutQuint).SetAutoKill(false).Pause();
        }

        public void Enter()
        {
            enterTween.PlayForward();
            fadeTween.PlayForward();
        }

        public void Exit()
        {
            enterTween.PlayBackwards();
            fadeTween.PlayBackwards();
        }
        private void OnDestroy()
        {
            enterTween.Kill();
            fadeTween.Kill();
        }
    }
}
