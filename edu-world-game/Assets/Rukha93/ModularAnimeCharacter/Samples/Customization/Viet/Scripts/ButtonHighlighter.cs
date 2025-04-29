using UnityEngine;

namespace Edu_World_Game
{
using UnityEngine;
using UnityEngine.UI;

public class ButtonHighlighter : MonoBehaviour
{
    private Button button;
    private Image buttonImage;
    private static ButtonHighlighter lastSelectedButton; 

    private Color normalColor = Color.white;
    private Color highlightedColor = new Color(0.8f, 0.8f, 0.8f); 

    void Start()
    {
        button = GetComponent<Button>();
        buttonImage = GetComponent<Image>();

        button.onClick.AddListener(OnButtonClick);
    }

    void OnButtonClick()
    {
        if (lastSelectedButton != null)
        {
            lastSelectedButton.buttonImage.color = lastSelectedButton.normalColor;
        }

        buttonImage.color = highlightedColor;
        lastSelectedButton = this;
    }
}

}
