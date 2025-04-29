using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using DG.Tweening;

namespace Rukha93.ModularAnimeCharacter.Customization
{
    public class UIColorItem : MonoBehaviour
    {
        [SerializeField] private Button colorButtonPrefab;
        [SerializeField] private Transform buttonContainer;
        private List<Button> colorButtons = new List<Button>();
        public System.Action<Color, int> OnColorSelected;
        public System.Action OnColorButtonClicked;

        private Button currentSelectedButton;
        [SerializeField] private float highlightScale = 1.2f;
        [SerializeField] private float highlightDuration = 0.3f;
        [SerializeField] private Ease highlightEase = Ease.OutBack;

        [SerializeField] private Sprite selectedBackground; // Thêm sprite background
        [SerializeField] private GameObject backgroundPrefab; // Thêm reference tới background prefab
        private Dictionary<Button, Image> buttonBackgrounds = new Dictionary<Button, Image>(); // Lưu reference tới background images

        private ColorItemTweener tweener;

        // Define colors using RGBA values
        private readonly Color[] colors = new Color[]
        {
            new Color(0.46f, 0.47f, 0.47f, 1.0f),    // 767978 - Gray
            new Color(0.45f, 0.09f, 0.10f, 1.0f),    // 72161A - Dark Red
            new Color(0.07f, 0.37f, 0.45f, 1.0f),    // 135F72 - Deep Blue
            new Color(0.45f, 0.24f, 0.07f, 1.0f),    // 723E12 - Brown
            new Color(0.91f, 0.88f, 0.08f, 1.0f),    // E7E015 - Yellow
            new Color(0.05f, 0.93f, 0.29f, 1.0f),    // 0DEE4B - Green
            new Color(0.14f, 0.47f, 0.92f, 1.0f),    // 2377EA - Blue
            new Color(0.25f, 0.07f, 0.31f, 1.0f),    // 3F134E - Purple
            new Color(0.87f, 0.00f, 0.89f, 1.0f),    // DE00E2 - Magenta
            new Color(1.00f, 0.30f, 0.14f, 1.0f),    // FF4C24 - Orange Red
            new Color(0.17f, 0.24f, 0.60f, 1.0f),    // 2B3C99 - Navy Blue
            new Color(0.73f, 0.34f, 0.28f, 1.0f)     // BB5647 - Brick Red
        };

private readonly Color[] colorSkin = new Color[]
{
    // Tông sáng
    new Color(0.94f, 0.78f, 0.62f, 1.0f),     // F0C7A0 - Pale light skin trung gian
    new Color(0.96f, 0.75f, 0.58f, 1.0f),     // F4BF93 - Light skin ấm thiên vàng
    new Color(0.93f, 0.70f, 0.54f, 1.0f),     // EDAD88 - Light warm skin trung gian
    new Color(0.91f, 0.68f, 0.50f, 1.0f),     // E8AE80 - Fair skin đậm vàng nhẹ

    // Tông trung tính
    new Color(0.88f, 0.65f, 0.48f, 1.0f),     // E2A475 - Trung tính vàng ấm
    new Color(0.86f, 0.68f, 0.58f, 1.0f),     // DBAC94 - Medium light skin
    new Color(0.82f, 0.60f, 0.50f, 1.0f),     // D19A80 - Medium warm skin
    new Color(0.79f, 0.56f, 0.45f, 1.0f),     // CA8C73 - Medium da ấm nhẹ hơn

    // Tông tối
    new Color(0.76f, 0.54f, 0.44f, 1.0f),     // C28770 - Medium skin
    new Color(0.72f, 0.49f, 0.40f, 1.0f),     // B87C66 - Medium tan skin
    new Color(0.65f, 0.42f, 0.34f, 1.0f),     // A56957 - Medium dark skin
    new Color(0.58f, 0.38f, 0.30f, 1.0f),     // 94664D - Da nâu trung gian
    new Color(0.52f, 0.35f, 0.28f, 1.0f),     // 855842 - Dark skin
    new Color(0.44f, 0.30f, 0.24f, 1.0f),     // 704C3D - Deep brown skin
    new Color(0.38f, 0.26f, 0.22f, 1.0f),     // 61443A - Very dark skin
    new Color(0.30f, 0.22f, 0.18f, 1.0f)      // 4D392E - Darkest skin (tối nhất)
};


        public string colorItemId; // Unique identifier for this color item

        private void Start()
        {
            tweener = new ColorItemTweener(
                highlightScale,
                highlightDuration,
                highlightEase
            );
            CreateColorButtons();
            HideColorButtonPrefab();
        }

        private void CreateColorButtons()
        {
            // Create buttons without setting colors initially
            for (int i = 0; i < colors.Length; i++)
            {
                Button newButton = Instantiate(colorButtonPrefab, buttonContainer);
                SetupColorButton(newButton, Color.white, i); // Use placeholder color
                colorButtons.Add(newButton);
                tweener.AnimateButtonCreation(newButton, newButton.GetComponent<Image>(), i);
            }
            // Set initial colors based on category
            UpdateButtonColors();
        }

        private void UpdateButtonColors()
        {
            Color[] colorsToUse;
            // Check if this is a body material that should use skin colors
            if (colorItemId?.ToLower().Contains("mat_base") == true && 
                (colorItemId.ToLower().Contains("_m_body") || colorItemId.ToLower().Contains("_f_body")))
            {
                colorsToUse = colorSkin;
            }
            else
            {
                colorsToUse = colors;
            }

            for (int i = 0; i < colorButtons.Count && i < colorsToUse.Length; i++)
            {
                Image buttonImage = colorButtons[i].GetComponent<Image>();
                buttonImage.color = colorsToUse[i];
            }
        }

        private void SetupColorButton(Button button, Color color, int buttonIndex)
        {
            // Setup background
            GameObject bgObj = Instantiate(backgroundPrefab, button.transform);
            bgObj.transform.SetAsFirstSibling();

            Image bgImage = bgObj.GetComponent<Image>();
            if (bgImage == null) bgImage = bgObj.AddComponent<Image>();
            bgImage.sprite = selectedBackground;
            bgImage.gameObject.SetActive(false);

            buttonBackgrounds.Add(button, bgImage);

            // Setup button color
            Image buttonImage = button.GetComponent<Image>();
            buttonImage.color = color;

            // Setup click handler
            int index = buttonIndex; // Store index in local variable
            button.onClick.AddListener(() =>
            {
                Color currentColor = buttonImage.color; // Use current color from image
                SelectColor(currentColor, index);
                HighlightButton(button);
                tweener.PlayClickAnimation(button);
            });
        }

        // Add property setter for colorItemId that updates colors when changed
        public string ColorItemId
        {
            get => colorItemId;
            set
            {
                colorItemId = value;
                UpdateButtonColors();
            }
        }

        private void SelectColor(Color color, int buttonIndex)
        {
            OnColorSelected?.Invoke(color, buttonIndex);
            // The UICustomizationDemo will handle saving this through the OnColorSelected callback
        }

        private void HighlightButton(Button selectedButton)
        {
            // Reset previous selected button
            if (currentSelectedButton != null && currentSelectedButton != selectedButton)
            {
                tweener.PlayResetAnimation(currentSelectedButton);
                buttonBackgrounds[currentSelectedButton].gameObject.SetActive(false); // Ẩn background cũ
            }

            // Highlight new selected button
            tweener.PlayHighlightAnimation(selectedButton);
            buttonBackgrounds[selectedButton].gameObject.SetActive(true); // Hiện background mới
            currentSelectedButton = selectedButton;
        }

        public void HighlightButtonByIndex(int index)
        {
            if (index >= 0 && index < colorButtons.Count)
            {
                HighlightButton(colorButtons[index]);
            }
        }

        public void ResetHighlight()
        {
            if (currentSelectedButton != null)
            {
                currentSelectedButton.transform.DOScale(1f, highlightDuration).SetEase(highlightEase);
                buttonBackgrounds[currentSelectedButton].gameObject.SetActive(false);
                currentSelectedButton = null;
            }
        }

        private void OnDestroy()
        {
            // Kill all tweens when the object is destroyed
            if (currentSelectedButton != null)
            {
                tweener.CleanupButton(currentSelectedButton);
            }
            foreach (var button in colorButtons)
            {
                if (button != null)
                {
                    tweener.CleanupButton(button);
                }
            }
            buttonBackgrounds.Clear();
        }

        public void HideColorButtonPrefab()
        {
            if (colorButtonPrefab != null)
            {
                colorButtonPrefab.gameObject.SetActive(false);
            }
        }

        public void SetSingleColor(bool value)
        {
            // Hide the prefab first
            HideColorButtonPrefab();

            // Show/hide only the instantiated color buttons
            foreach (var button in colorButtons)
            {
                if (button != null)
                {
                    tweener.AnimateButtonVisibility(button, !value);
                }
            }
        }
    }
}
