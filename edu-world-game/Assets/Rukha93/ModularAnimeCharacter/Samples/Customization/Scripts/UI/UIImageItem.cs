using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using TMPro; // Add this

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{
    public class UIImageItem : MonoBehaviour
    {
        [SerializeField] private Image m_Icon;
        [SerializeField] private Button button;
        [SerializeField] private TextMeshProUGUI m_Text;
        [SerializeField] private TextMeshProUGUI m_PriceText; // Add this field

        [SerializeField] private Sprite sprite1;
        [SerializeField] private Sprite sprite2;
        [SerializeField] private Sprite sprite3; // Add this line
        [SerializeField] private float animationDuration = 0.3f; // Add animation duration
        [SerializeField] private Image checkIcon; // Add reference to check icon Image
        [SerializeField] private float checkIconAnimDuration = 0.2f; // Duration for check icon animation
        [SerializeField] public Button buttonUnEquipped; // Add this to position the unequip button
        public System.Action OnClick { get; set; }
        private Image backgroundImage;
        private ImageItemTweener tweener;
        private bool isHovered = false;
        private int currentPrice = 0;

        private void Awake()
        {
            backgroundImage = GetComponentInChildren<Image>();
            button.onClick.AddListener(() => OnClick?.Invoke());
            button.onClick.AddListener(ChangeImageChoose);
            
            // Initialize tweener
            tweener = new ImageItemTweener(animationDuration, checkIconAnimDuration);

            // Initialize check icon
            if (checkIcon != null)
            {
                checkIcon.gameObject.SetActive(false);
            }
        }

        void Start()
        {
            // backgroundImage.sprite = sprite1;
        }
        public void SetUpBackgroundImageItem()
        {
            if (backgroundImage != null)
            {
                backgroundImage.sprite = sprite1;
            }
        }
        public void SetUpBackgroundImageItemShop()
        {
            if (backgroundImage != null)
            {
                backgroundImage.sprite = sprite2;
            }
        }
        public void Setup(Sprite sprite)
        {
            if (m_Icon != null && sprite != null)
            {
                m_Icon.sprite = sprite;
            }
        }
        public void Setup(Sprite sprite, int price = 0)
        {
            if (m_Icon != null && sprite != null)
            {
                m_Icon.sprite = sprite;
            }
            if (m_PriceText != null)
            {
                currentPrice = price;
                SetPrice(price);
            }
        }
        public void SetTitle(string text)
        {
            if (m_Text != null && text != null)
            {
                m_Text.text = text;
            }
        }
        public void SetPrice(int price)
        {
            if (m_PriceText != null) // Only set price if m_PriceText exists
            {
                m_PriceText.text = price > 0 ? price.ToString() + " G" : "Free";
            }
        }
        public void ChangeImageChoose()
        {
            tweener.PlaySelectAnimation(backgroundImage, sprite2, transform, checkIcon);
            if (m_PriceText != null)
            {
                m_PriceText.text = "Equipped";
            }
        }

        public void ResetToLastImage()
        {
            tweener.PlayResetAnimation(backgroundImage, sprite1, checkIcon);
            if (m_PriceText != null)
            {
                SetPrice(currentPrice);
            }
        }

        public void OnPointerEnter()
        {
            if (!isHovered)
            {
                isHovered = true;
                tweener.PlayHoverEnterAnimation(transform, backgroundImage);
            }
        }

        public void OnPointerExit()
        {
            if (isHovered)
            {
                isHovered = false;
                tweener.PlayHoverExitAnimation(transform, backgroundImage);
            }
        }

        public void SetNormalSprite()
        {
            if (backgroundImage != null)
            {
                backgroundImage.sprite = sprite1;
                checkIcon.gameObject.SetActive(false);
            }
        }

        private void OnDestroy()
        {
            if(tweener != null)
            {
                tweener.Cleanup(transform, backgroundImage, checkIcon);
            }
        }
    }
}
