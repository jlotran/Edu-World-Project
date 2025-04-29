using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;

namespace RGSK
{
    [RequireComponent(typeof(RectTransform))]
    public class UIButtonFlex : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
    {
        [SerializeField] float scaleUp = 1.05f;
        [SerializeField] float transitionTime = 0.1f;

        private Vector3 originalScale;
        private Coroutine scaleRoutine;

        void Awake()
        {
            originalScale = transform.localScale;
        }

        public void OnPointerEnter(PointerEventData eventData)
        {
            StartScaleTween(originalScale * scaleUp);
        }

        public void OnPointerExit(PointerEventData eventData)
        {
            StartScaleTween(originalScale);
        }

        void StartScaleTween(Vector3 target)
        {
            if (scaleRoutine != null)
                StopCoroutine(scaleRoutine);

            scaleRoutine = StartCoroutine(ScaleTo(target));
        }

        IEnumerator ScaleTo(Vector3 target)
        {
            Vector3 start = transform.localScale;
            float time = 0;

            while (time < transitionTime)
            {
                transform.localScale = Vector3.Lerp(start, target, time / transitionTime);
                time += Time.unscaledDeltaTime;
                yield return null;
            }

            transform.localScale = target;
        }

    }
}