using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ColorItemTweener
{
    private readonly float highlightScale;
    private readonly float highlightDuration;
    private readonly Ease highlightEase;
    private readonly float fadeInDuration;
    private readonly float clickScale;
    private readonly float clickDuration;

    public ColorItemTweener(
        float highlightScale = 1.05f, // Nhẹ nhàng scale nhỏ hơn
        float highlightDuration = 0.25f,
        Ease highlightEase = Ease.InOutSine, // Easing mềm mại
        float fadeInDuration = 0.3f,
        float clickScale = 1.1f,
        float clickDuration = 0.15f)
    {
        this.highlightScale = highlightScale;
        this.highlightDuration = highlightDuration;
        this.highlightEase = highlightEase;
        this.fadeInDuration = fadeInDuration;
        this.clickScale = clickScale;
        this.clickDuration = clickDuration;
    }

    public void AnimateButtonCreation(Button button, Image buttonImage, int index)
    {
        button.transform.localScale = Vector3.zero;
        var buttonColor = buttonImage.color;
        buttonColor.a = 0;
        buttonImage.color = buttonColor;

        float delay = index * 0.05f; // Delay nhẹ
        button.transform.DOScale(1f, fadeInDuration).SetDelay(delay).SetEase(Ease.OutQuad);
        buttonImage.DOFade(1f, fadeInDuration).SetDelay(delay);
    }

    public void PlayHighlightAnimation(Button button)
    {
        button.transform.DOScale(highlightScale, highlightDuration).SetEase(highlightEase);
    }

    public void PlayClickAnimation(Button button)
    {
        button.transform.DOScale(clickScale, clickDuration).SetEase(Ease.OutQuad)
              .OnComplete(() => button.transform.DOScale(1f, 0.1f)); // Trả về size ban đầu
    }

    public void PlayResetAnimation(Button button)
    {
        button.transform.DOScale(1f, highlightDuration).SetEase(highlightEase);
    }

    public void AnimateButtonVisibility(Button button, bool show)
    {
        if (show)
        {
            button.gameObject.SetActive(true);
            button.transform.DOScale(1f, fadeInDuration).SetEase(Ease.OutQuad);
        }
        else
        {
            button.transform.DOScale(0f, fadeInDuration)
                  .OnComplete(() => button.gameObject.SetActive(false));
        }
    }

    public void CleanupButton(Button button)
    {
        if (button != null)
        {
            button.transform.DOKill();
        }
    }
}