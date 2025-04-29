using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Edu_World
{
    public class CarItemTweening : MonoBehaviour
    {
        [SerializeField] private Image backgroundImage;
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite selectedSprite;
        [SerializeField] private float scaleDuration = 0.3f;
        [SerializeField] private float scaleMultiplier = 1.1f;
        
        private Vector3 originalScale;

        private void Awake()
        {
            originalScale = transform.localScale;
        }

        public void SetSelected()
        {
            backgroundImage.sprite = selectedSprite;
            transform.DOScale(originalScale * scaleMultiplier, scaleDuration)
                .SetEase(Ease.OutBack);
        }

        public void SetNormal()
        {
            backgroundImage.sprite = normalSprite;
            transform.DOScale(originalScale, scaleDuration)
                .SetEase(Ease.OutBack);
        }
    }
}
