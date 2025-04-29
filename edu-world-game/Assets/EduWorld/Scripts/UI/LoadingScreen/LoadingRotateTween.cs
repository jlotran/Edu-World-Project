using DG.Tweening;
using UnityEngine;

public class LoadingRotateTween : MonoBehaviour
{
    [SerializeField] private RectTransform rectTransform;

    Tweener Rotatetweener;

    private void Awake()
    {
        Rotatetweener = rectTransform.DOLocalRotate(new Vector3(0, 0, -360), 1f, RotateMode.FastBeyond360).SetLoops(-1, LoopType.Incremental).SetEase(Ease.Linear).SetAutoKill(false).Pause();
    }
    private void OnEnable()
    {
        Rotatetweener.Restart();
    }
    private void OnDisable()
    {
        Rotatetweener.Pause();
    }

    private void OnDestroy()
    {
        Rotatetweener.Kill();
    }
}

