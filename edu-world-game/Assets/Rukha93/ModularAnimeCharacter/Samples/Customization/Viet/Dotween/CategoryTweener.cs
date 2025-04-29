using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;

public class CategoryTweener
{
    private readonly Transform targetTransform;
    private readonly Image backgroundImage;
    private readonly float animationDuration;
    private readonly float scaleMultiplier;
    private readonly float rotationAngle;
    private readonly float hoverScale;
    private readonly float hoverDuration;
    private readonly Color hoverTintColor;
    private readonly Color originalColor;

    public CategoryTweener(Transform transform, Image background, float duration = 0.5f, 
        float scale = 1.1f, float rotation = 5f, float hoverScaleAmount = 1.05f, 
        float hoverDur = 0.2f, Color? hoverTint = null)
    {
        targetTransform = transform;
        backgroundImage = background;
        animationDuration = duration;
        scaleMultiplier = scale;
        rotationAngle = rotation;
        hoverScale = hoverScaleAmount;
        hoverDuration = hoverDur;
        originalColor = background.color;
        hoverTintColor = hoverTint ?? new Color(1f, 1f, 1f, 1.2f);
    }

    public void PlaySelectAnimation(Sprite newSprite)
    {
        Sequence sequence = DOTween.Sequence();

        // Background transition
        sequence.Join(backgroundImage.DOFade(0f, animationDuration * 0.3f))
               .AppendCallback(() => backgroundImage.sprite = newSprite)
               .Append(backgroundImage.DOFade(1f, animationDuration * 0.3f));

        // Transform animations
        sequence.Join(targetTransform.DOScale(scaleMultiplier, animationDuration * 0.4f)
               .SetEase(Ease.OutQuad));
        sequence.Join(targetTransform.DORotate(new Vector3(0, 0, rotationAngle), animationDuration * 0.2f)
               .SetEase(Ease.OutQuad));
        
        // Return to normal state
        sequence.Append(targetTransform.DOScale(1f, animationDuration * 0.3f)
               .SetEase(Ease.InOutQuad));
        sequence.Join(targetTransform.DORotate(Vector3.zero, animationDuration * 0.3f)
               .SetEase(Ease.InOutQuad));
    }

    public void PlayResetAnimation(Sprite originalSprite)
    {
        Sequence sequence = DOTween.Sequence();

        sequence.Join(backgroundImage.DOFade(0f, animationDuration * 0.3f))
               .AppendCallback(() => backgroundImage.sprite = originalSprite)
               .Append(backgroundImage.DOFade(1f, animationDuration * 0.3f));

        sequence.Join(targetTransform.DOShakeRotation(animationDuration * 0.5f, 3f, 5, 45f)
               .SetEase(Ease.OutQuad));
        sequence.Append(targetTransform.DORotate(Vector3.zero, animationDuration * 0.2f));
    }

    public void PlayHoverAnimation(bool isEnter)
    {
        DOTween.Kill(targetTransform, true);
        DOTween.Kill(backgroundImage, true);

        if (isEnter)
        {
            targetTransform.DOScale(hoverScale, hoverDuration).SetEase(Ease.OutQuad);
            backgroundImage.DOColor(hoverTintColor, hoverDuration);
        }
        else
        {
            targetTransform.DOScale(1f, hoverDuration).SetEase(Ease.OutQuad);
            backgroundImage.DOColor(originalColor, hoverDuration);
        }
    }

    public void Cleanup()
    {
        DOTween.Kill(backgroundImage);
        DOTween.Kill(targetTransform);
        targetTransform.rotation = Quaternion.identity;
        targetTransform.localScale = Vector3.one;
        if (backgroundImage != null)
            backgroundImage.color = originalColor;
    }
}
