using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

namespace Lam.UI
{
    public enum ButtonEffect
    {
        None,
        Scale,
        HighlightBorder,
        ChangeColor
    }

    public class ButtonCustom : Button
    {
        [Header("Hover Settings")]
        [SerializeField] public ButtonEffect hoverEffect = ButtonEffect.None;
        [SerializeField] public float hoverZoomFactor = 1.2f;
        [SerializeField] public float hoverDuration = 0.3f;
        [SerializeField] public Color hoverHighlightColor = Color.yellow;
        [SerializeField] public Color hoverBorderColor = Color.cyan;

        [Header("Press Settings")]
        [SerializeField] public ButtonEffect pressEffect = ButtonEffect.None;
        [SerializeField] public float pressZoomFactor = 0.9f;
        [SerializeField] public float pressDuration = 0.2f;
        [SerializeField] public Color pressHighlightColor = Color.red;
        [SerializeField] public Color pressBorderColor = Color.blue;

        private Vector3 originalScale;
        private Color originalColor;
        private UnityEngine.UI.Outline outline;

        protected override void Start()
        {
            base.Start();

            originalScale = transform.localScale;

            if (targetGraphic != null)
            {
                originalColor = targetGraphic.color;
            }

            outline = gameObject.GetComponent<UnityEngine.UI.Outline>();
            if (outline == null)
            {
                outline = gameObject.AddComponent<UnityEngine.UI.Outline>();
                outline.enabled = false;
            }
        }

        public override void OnPointerEnter(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerEnter(eventData);

            // Gọi hiệu ứng hover
            ApplyEffect(hoverEffect, hoverZoomFactor, hoverHighlightColor, hoverBorderColor, hoverDuration);
        }

        public override void OnPointerExit(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerExit(eventData);

            // Reset trạng thái khi hover ra ngoài
            ResetEffect(hoverEffect, hoverZoomFactor, originalColor, hoverDuration);
        }

        public override void OnPointerDown(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerDown(eventData);

            // Gọi hiệu ứng khi nhấn
            ApplyEffect(pressEffect, pressZoomFactor, pressHighlightColor, pressBorderColor, pressDuration);
        }

        public override void OnPointerUp(UnityEngine.EventSystems.PointerEventData eventData)
        {
            base.OnPointerUp(eventData);

            // Reset trạng thái khi thả nút
            ResetEffect(pressEffect, pressZoomFactor, originalColor, pressDuration);
        }

        private void ApplyEffect(ButtonEffect effect, float zoomFactor, Color highlightColor, Color borderColor, float duration)
        {
            switch (effect)
            {
                case ButtonEffect.None:
                    break;

                case ButtonEffect.Scale:
                    transform.DOScale(originalScale * zoomFactor, duration).SetEase(Ease.OutBack);
                    break;
                case ButtonEffect.HighlightBorder:
                    if (outline != null)
                    {
                        outline.enabled = true;
                        outline.effectColor = borderColor;
                    }
                    break;

                case ButtonEffect.ChangeColor:
                    if (targetGraphic != null)
                    {
                        // targetGraphic.DOColor(highlightColor, duration).SetEase(Ease.OutQuad);
                    }
                    break;
            }
        }

        private void ResetEffect(ButtonEffect effect, float zoomFactor, Color originalColor, float duration)
        {
            switch (effect)
            {
                case ButtonEffect.None:
                    break;

                case ButtonEffect.Scale:
                    transform.DOScale(originalScale, duration).SetEase(Ease.InBack);
                    break;

                case ButtonEffect.HighlightBorder:
                    if (outline != null)
                    {
                        outline.enabled = false;
                    }
                    break;

                case ButtonEffect.ChangeColor:
                    if (targetGraphic != null)
                    {
                        // targetGraphic.DOColor(originalColor, duration).SetEase(Ease.InQuad);
                    }
                    break;
            }
        }
    }
}
