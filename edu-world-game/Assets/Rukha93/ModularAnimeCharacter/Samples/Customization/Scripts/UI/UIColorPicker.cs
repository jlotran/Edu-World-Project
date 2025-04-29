using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{
    public class UIColorPicker : MonoBehaviour
    {
        [Space]
        [SerializeField] private UISlider m_HueSlider;    // Renamed from m_RedSlider
        [SerializeField] private UISlider m_SatSlider;    // Renamed from m_GreenSlider
        [SerializeField] private UISlider m_ValSlider;    // Renamed from m_BlueSlider

        [Space]
        [SerializeField] private Button m_CloseButton;

        public System.Action<Color> OnChangeColor;
        private Color m_Color;
        private float m_LockedHue = 0.1f; // Giữ nguyên giá trị Hue của màu da

        private void Awake()
        {
            m_HueSlider.title = "H";
            m_SatSlider.title = "S";
            m_ValSlider.title = "V";

            // Set min-max values for sliders to control range
            m_SatSlider.slider.minValue = 0.2f;  // Minimum saturation
            m_SatSlider.slider.maxValue = 0.8f;  // Maximum saturation
            
            m_ValSlider.slider.minValue = 0.3f;  // Minimum value/brightness
            m_ValSlider.slider.maxValue = 0.95f; // Maximum value/brightness

            m_CloseButton.onClick.AddListener(OnClickClose);
            m_HueSlider.gameObject.SetActive(false);
        }

        public void Show(Color color, System.Action<Color> onChangeColor)
        {
            if (gameObject.activeSelf)
                Close();

            m_SatSlider.slider.onValueChanged.RemoveAllListeners();
            m_ValSlider.slider.onValueChanged.RemoveAllListeners();

            m_Color = color;
            
            // Convert color to HSV and set sliders
            float h, s, v;
            Color.RGBToHSV(color, out h, out s, out v);
            
            // Keep the original hue
            m_LockedHue = h;
            
            m_SatSlider.slider.value = s;
            m_ValSlider.slider.value = v;
            
            OnChangeColor = onChangeColor;

            m_SatSlider.slider.onValueChanged.AddListener(OnSliderChange);
            m_ValSlider.slider.onValueChanged.AddListener(OnSliderChange);

            gameObject.SetActive(true);
        }

        public void Close()
        {
            m_SatSlider.slider.onValueChanged.RemoveAllListeners();
            m_ValSlider.slider.onValueChanged.RemoveAllListeners();

            OnChangeColor = null;
            gameObject.SetActive(false);
        }

        private void OnSliderChange(float v)
        {
            float a = m_Color.a;
            // Adjust hue slightly based on saturation and value for more natural variation
            float adjustedHue = m_LockedHue;
            if (m_SatSlider.slider.value > 0.6f)
            {
                adjustedHue += 0.02f; // Slightly warmer for more saturated colors
            }
            if (m_ValSlider.slider.value < 0.5f)
            {
                adjustedHue -= 0.02f; // Slightly cooler for darker colors
            }
            
            m_Color = Color.HSVToRGB(adjustedHue, m_SatSlider.slider.value, m_ValSlider.slider.value);
            m_Color.a = a;
            OnChangeColor?.Invoke(m_Color);
        }

        private void OnClickClose()
        {
            this.Close();
        }
    }
}
