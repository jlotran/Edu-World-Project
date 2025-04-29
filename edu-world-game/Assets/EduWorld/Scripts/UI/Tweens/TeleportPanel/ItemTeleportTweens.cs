using DG.Tweening;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace EduWorld.UIComponents
{
    public class ItemTeleportTweens : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [Header("Components")]
        public CanvasGroup glowImage;
        public CanvasGroup gradientImage;
        public Image FlashImage;
        [Header("Tweens")]
        public Tweener glowTweener;
        public Tweener gradientTweener;
        public Sequence flashTweener;
        private void Start()
        {
            glowTweener = glowImage.DOFade(1, 0.5f).SetAutoKill(false).Pause();
            gradientTweener = gradientImage.DOFade(1, 0.5f).SetAutoKill(false).Pause();
            CanvasGroup cg = FlashImage.GetComponent<CanvasGroup>();
            Tweener flashTweener1 = cg.DOFade(0.5f, 0.2f);
            RectTransform rt = FlashImage.GetComponent<RectTransform>();
            Tweener flashTweener2 = rt.DOLocalMoveX(475f, 0.3f);
            Tweener flashTweener3 = cg.DOFade(0f, 0.2f);
            flashTweener = DOTween.Sequence().Append(flashTweener1).Append(flashTweener2).Append(flashTweener3).SetAutoKill(false).Pause();
        }
        public void OnPointerEnter(PointerEventData eventData)
        {
            flashTweener.Restart();
            gradientTweener.PlayForward();
            glowTweener.PlayForward();
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            glowTweener.PlayBackwards();
            gradientTweener.PlayBackwards();
        }

        private void OnDestroy()
        {
            glowTweener.Kill();
            gradientTweener.Kill();
            flashTweener.Kill();
        }
    }
}
