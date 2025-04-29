using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ProgressingHandler : MonoBehaviour
{

    [Header("UI Components")]
    [SerializeField] private Slider barSlider;
    [SerializeField] private Image fillImage;
    [SerializeField] private TMP_Text[] progressText;
    
    public void UpdateProgress(float progress)
    {
        barSlider.value = progress;
        fillImage.fillAmount = progress;
        foreach (var text in progressText)
        {
            text.text = $"{(progress * 100).ToString("0")}%";
        }
    }
}

