using UnityEngine;

namespace Edu_World_Game
{
using UnityEngine;
using UnityEngine.UI;

public class ImageBrightnessControl : MonoBehaviour
{
    public RawImage characterImage;  
    public Slider brightnessSlider;  

    private Color originalColor; 

    void Start()
    {
        originalColor = characterImage.color;

        brightnessSlider.onValueChanged.AddListener(AdjustBrightness);

        AdjustBrightness(brightnessSlider.value);
    }

    void AdjustBrightness(float value)
    {
        characterImage.color = new Color(originalColor.r * value, originalColor.g * value, originalColor.b * value, originalColor.a);
    }
}

}
