using UnityEngine;
using UnityEngine.UI;

namespace Edu_World
{
    public class ButtonBuyUI : MonoBehaviour
    {
        public event System.Action<string> OnPurchaseSuccess;
        private string currentCarName;
        private int currentCarPrice;

        [Header("Money and Rank")]
        public int money;
        public int rank;
        [SerializeField] private Image moneyImage;
        [SerializeField] private Image iconMoneyImage;
        [SerializeField] private Sprite availableSprite;
        [SerializeField] private Sprite insufficientMoneySprite;
        [SerializeField] private Sprite insufficientRankSprite;
        [SerializeField] private Sprite iconMoney;
        [SerializeField] private Sprite iconLock;
        // [SerializeField] private Button button;

        private Button buyButton;

        private void Awake()
        {
        }
        void Start()
        {
            buyButton = GetComponent<Button>();
            buyButton.onClick.AddListener(OnBuyButtonClick);

        }
        private void OnDestroy()
        {
            if (buyButton != null)
                buyButton.onClick.RemoveListener(OnBuyButtonClick);
        }

        public void UpdateMoneyImage(int carPrice, int carRank)
        {
            currentCarPrice = carPrice;
            bool hasRequiredRank = rank >= carRank;
            bool hasEnoughMoney = money >= carPrice;

            iconMoneyImage.sprite = hasRequiredRank ? iconMoney : iconLock;
            moneyImage.sprite = hasRequiredRank
                ? (hasEnoughMoney ? availableSprite : insufficientMoneySprite)
                : insufficientRankSprite;
        }

        public void SetCurrentCarName(string carName)
        {
            currentCarName = carName;
        }

        public void OnBuyButtonClick()
        {
            if (money >= currentCarPrice)
            {
                money -= currentCarPrice; // Trừ tiền khi mua thành công
                OnSuccessfulPurchase(); // Gọi event thông báo mua thành công
            }
        }

        // Somewhere in your purchase success logic
        private void OnSuccessfulPurchase()
        {
            OnPurchaseSuccess?.Invoke(currentCarName);
        }
    }
}
