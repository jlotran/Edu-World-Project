using UnityEngine;
using TMPro;

namespace RGSK
{
    public class FPSHandler : MonoBehaviour
    {
        public TextMeshProUGUI fpsText;
        private float deltaTime = 0.0f;

        void Start()
        {
            Application.targetFrameRate = -1;
        }

        void Update()
        {
            deltaTime += (Time.unscaledDeltaTime - deltaTime) * 0.1f;
            CalculateFPS();
        }

        private void CalculateFPS()
        {
            float fps = 1.0f / deltaTime;
            if (fpsText != null)
            {
                fpsText.text = "FPS: " + Mathf.Ceil(fps).ToString();
            }
            else
            {
                Debug.LogError("FPS Text is not assigned.");
            }
        }
    }
}
