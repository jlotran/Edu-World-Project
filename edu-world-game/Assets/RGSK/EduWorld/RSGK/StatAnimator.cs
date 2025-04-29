using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace RGSK
{
    public class StatAnimator : MonoBehaviour
    {
        public Image fillImage;
        public TextMeshProUGUI progressText;
        public TextMeshProUGUI correctAnswerText;
        public TextMeshProUGUI timeEfficiencyText;

        [Range(0, 1)] public float targetProgress = 0.8f;   // 80%
        public int correctAnswers = 8;
        public int totalQuestions = 10;
        public float avgTime = 6f; // giây

        public float animationDuration = 1f;

        private Coroutine animationCoroutine;
        private bool isAnimating = false;

        void Start()
        {
            StartAnimation();
        }

        void OnDisable()
        {
            StopAnimation();
        }

        public void StartAnimation()
        {
            StopAnimation();
            animationCoroutine = StartCoroutine(AnimateStats());
        }

        public void StopAnimation()
        {
            if (animationCoroutine != null)
            {
                StopCoroutine(animationCoroutine);
                animationCoroutine = null;
            }
            isAnimating = false;
        }

        IEnumerator AnimateStats()
        {
            isAnimating = true;
            float t = 0;
            float currentProgress = 0;
            float currentCorrect = 0;
            float currentTime = 0;

            while (t < animationDuration && isAnimating)
            {
                t += Time.deltaTime;
                float percent = Mathf.Clamp01(t / animationDuration);

                currentProgress = Mathf.Lerp(0, targetProgress, percent);
                currentCorrect = Mathf.Lerp(0, correctAnswers, percent);
                currentTime = Mathf.Lerp(0, avgTime, percent);

                UpdateUI(currentProgress, currentCorrect, currentTime);
                yield return null;
            }

            if (isAnimating)
            {
                UpdateUI(targetProgress, correctAnswers, avgTime);
            }
            isAnimating = false;
        }

        private void UpdateUI(float progress, float correct, float time)
        {
            if (fillImage != null)
                fillImage.fillAmount = progress;

            if (progressText != null)
                progressText.text = Mathf.RoundToInt(progress * 100f) + "%";

            if (correctAnswerText != null)
                correctAnswerText.text = Mathf.RoundToInt(correct) + "/" + totalQuestions;

            if (timeEfficiencyText != null)
                timeEfficiencyText.text = $"Avg: {Mathf.RoundToInt(time)}s/q";
        }
    }
}
