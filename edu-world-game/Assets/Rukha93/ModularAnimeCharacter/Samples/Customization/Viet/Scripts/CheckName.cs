using UnityEngine;
using UnityEngine.UI;
using TMPro;
using DG.Tweening;

namespace Edu_World_Game
{
    public class CheckName : MonoBehaviour
    {
        PanelManager panelManager;
        [SerializeField] private TMP_InputField nameInput;
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI errorText; // Reference to the error text
        public RectTransform panelError;
        void Start()
        {
            panelManager = FindAnyObjectByType<PanelManager>();
            // errorText = FindAnyObjectByType<TextMeshProUGUI>();
            if (panelManager != null)
            {
                panelManager.OnHideAllPanels += ShowName; // Subscribe to the event
            }
            panelError.anchoredPosition = new Vector2(panelError.anchoredPosition.x, 280f); // Set the initial position of panelError
        }
        public void ShowName()
        {
            nameText.text = nameInput.text;
        }

        public bool ValidateName()
        {
            string inputText = nameInput.text;
            if (inputText.Length >= 5 && inputText.Length <= 20)
            {
                nameText.text = inputText;
                errorText.text = ""; // Clear any previous error message
                return true;
            }
            else
            {
                panelError.DOAnchorPosY(355f, 1f);
                errorText.text = "Name must be between 5 and 20 characters long"; // Display an error message
                return false;
            }
        }
    }
}
