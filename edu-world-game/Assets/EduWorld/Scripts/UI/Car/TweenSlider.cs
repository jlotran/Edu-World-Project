using UnityEngine;
using UnityEngine.UI;
using System.Collections;

namespace Edu_World
{
    public class TweenSlider : MonoBehaviour
    {
        private Slider slider;
        private Coroutine tweenCoroutine;

        private void Awake()
        {
            slider = GetComponent<Slider>();
        }

        public void TweenTo(float targetValue, float duration = 0.5f)
        {
            if (tweenCoroutine != null)
                StopCoroutine(tweenCoroutine);

            tweenCoroutine = StartCoroutine(TweenRoutine(targetValue, duration));
        }

        private IEnumerator TweenRoutine(float targetValue, float duration)
        {
            float startValue = slider.value;
            float elapsed = 0;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                float t = elapsed / duration;
                slider.value = Mathf.Lerp(startValue, targetValue, t);
                yield return null;
            }

            slider.value = targetValue;
        }
    }
}
