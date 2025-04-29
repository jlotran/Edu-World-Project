using UnityEngine;
using DG.Tweening;
using UnityEngine.UI;
public class InputFieldTweens : MonoBehaviour
{
    public Tweener alphaTweener;
    public Tweener scaleTweener;
    public Tweener moveTweener;
    public RectTransform placeHolder;
    public float placeHolderMoveValue = 25;
    public float placeHolderMoveDuration = 0.3f;
    public CanvasGroup filledImageCanvasGroup;

    private void Start()
    {
        moveTweener = placeHolder.DOLocalMoveY(placeHolderMoveValue, placeHolderMoveDuration).SetEase(Ease.OutBack).SetAutoKill(false).Pause();
        scaleTweener = placeHolder.DOScale(new Vector3(0.8f, 0.8f, 0.8f), placeHolderMoveDuration).SetEase(Ease.OutBack).SetAutoKill(false).Pause();
        alphaTweener = filledImageCanvasGroup.DOFade(1f,0.15f).SetAutoKill(false).Pause();
    }
    public void DoPlaceHolderMove()
    {
        if (moveTweener != null)
        {
            moveTweener.Restart();
        }
        if (scaleTweener != null) { 
            scaleTweener.Restart();
        }
    }

    public void DoPlaceHolderMoveBack()
    {
        moveTweener.Rewind(false);
        scaleTweener.Rewind(false);
    }
    public void DoHighLight()
    {
        if (alphaTweener != null)
        {
            alphaTweener.Restart();
        }
    }

    public void DoHighLightBack()
    {
        alphaTweener.Rewind(false);
    }

    private void OnDestroy()
    {
        alphaTweener.Kill();
        scaleTweener.Kill();
        moveTweener.Kill();
    }
}
