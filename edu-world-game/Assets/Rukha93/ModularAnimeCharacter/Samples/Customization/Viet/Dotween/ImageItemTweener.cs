using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ImageItemTweener
{
    private readonly float animationDuration;
    private readonly float checkIconAnimDuration;
    private readonly float hoverScale = 1.08f;
    private readonly float hoverDuration = 0.2f;
    private readonly float punchScale = 0.15f;

    public ImageItemTweener(float animDuration = 0.3f, float checkIconDuration = 0.2f)
    {
        animationDuration = animDuration;
        checkIconAnimDuration = checkIconDuration;
    }

    public void PlaySelectAnimation(Image backgroundImage, Sprite newSprite, Transform itemTransform, Image checkIcon = null)
    {
        // Kill any existing animations first
        DOTween.Kill(backgroundImage);
        DOTween.Kill(itemTransform);
        if (checkIcon != null) DOTween.Kill(checkIcon.transform);

        // Reset scale to 1 before starting new animation
        itemTransform.localScale = Vector3.one;

        // Background transition
        Sequence sequence = DOTween.Sequence();
        sequence.Join(backgroundImage.DOFade(0f, animationDuration / 2f))
                .AppendCallback(() => backgroundImage.sprite = newSprite)
                .Append(backgroundImage.DOFade(1f, animationDuration / 2f));

        // Punch effect
        itemTransform.DOPunchScale(Vector3.one * punchScale, animationDuration, 10, 0.5f)
                    .OnComplete(() => itemTransform.localScale = Vector3.one); // Ensure scale is reset after punch

        // Check icon animation if available
        if (checkIcon != null)
        {
            checkIcon.gameObject.SetActive(true);
            checkIcon.transform.localScale = Vector3.zero;
            checkIcon.transform.DOScale(1.2f, checkIconAnimDuration)
                    .SetEase(Ease.OutBounce)
                    .OnComplete(() => checkIcon.transform.DOScale(1f, 0.1f));
        }
    }

    public void PlayResetAnimation(Image backgroundImage, Sprite originalSprite, Image checkIcon = null)
    {
        // Kill any existing animations first
        DOTween.Kill(backgroundImage);
        if (checkIcon != null) DOTween.Kill(checkIcon.transform);

        // Background transition
        backgroundImage.DOFade(0f, animationDuration / 2f)
                      .OnComplete(() => {
                          backgroundImage.sprite = originalSprite;
                          backgroundImage.DOFade(1f, animationDuration / 2f);
                      });

        // Check icon hide animation if available
        if (checkIcon != null && checkIcon.gameObject.activeSelf)
        {
            checkIcon.transform.DOScale(0f, checkIconAnimDuration)
                    .SetEase(Ease.InBack)
                    .OnComplete(() => {
                        checkIcon.gameObject.SetActive(false);
                        checkIcon.transform.localScale = Vector3.zero; // Reset scale
                    });
        }
    }

    public void PlayHoverEnterAnimation(Transform itemTransform, Image backgroundImage)
    {
        // Kill any existing hover animations
        DOTween.Kill(itemTransform, true);
        DOTween.Kill(backgroundImage, true);

        itemTransform.DOScale(hoverScale, hoverDuration).SetEase(Ease.OutQuad);
        backgroundImage.DOColor(new Color(1f, 1f, 1f, 0.8f), hoverDuration);
    }

    public void PlayHoverExitAnimation(Transform itemTransform, Image backgroundImage)
    {
        // Kill any existing hover animations
        DOTween.Kill(itemTransform, true);
        DOTween.Kill(backgroundImage, true);

        itemTransform.DOScale(1f, hoverDuration).SetEase(Ease.OutQuad);
        backgroundImage.DOColor(Color.white, hoverDuration);
    }

    public void Cleanup(Transform itemTransform, Image backgroundImage, Image checkIcon = null)
    {
        DOTween.Kill(backgroundImage);
        DOTween.Kill(itemTransform);
        if (checkIcon != null)
        {
            DOTween.Kill(checkIcon.transform);
        }
    }
}
