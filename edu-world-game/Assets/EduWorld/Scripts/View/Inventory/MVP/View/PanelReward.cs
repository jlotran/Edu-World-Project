using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Edu_World.Inventory.MVP.View
{
    public class PanelReward : MonoBehaviour
    {
        #region UI Components
        [Header("Car Detail UI")]
        [SerializeField] private GameObject panelCarDetail;
        [SerializeField] private TextMeshProUGUI textCarName;
        [SerializeField] private TextMeshProUGUI textCarSpeed;
        [SerializeField] private TextMeshProUGUI textCarHandling;
        [SerializeField] private TextMeshProUGUI textCarAcceleration;
        [SerializeField] private TextMeshProUGUI textCarNitro;
        [SerializeField] private Button buttonCloseCarDetail;
        
        [Header("Car Stat Sliders")]
        [SerializeField] private Slider sliderSpeed;
        [SerializeField] private Slider sliderHandling;
        [SerializeField] private Slider sliderAcceleration;
        [SerializeField] private Slider sliderNitro;

        [Header("Use Panel UI")]
        [SerializeField] private GameObject panelUse;
        [SerializeField] private Button buttonCloseUse;

        [Header("Item Info UI")]
        [SerializeField] private TextMeshProUGUI textItemName;
        [SerializeField] private TextMeshProUGUI textItemQuantity;
        [SerializeField] private TextMeshProUGUI textItemPrice;
        [SerializeField] private TextMeshProUGUI textCurrentQuantity;
        [SerializeField] private TextMeshProUGUI textTotalPrice;

        [Header("Sell Panel UI")]
        [SerializeField] private Transform panelSell;
        [SerializeField] private Transform panelSellSuccess;
        [SerializeField] private Button buttonConfirmSell;
        [SerializeField] private Button buttonCloseSell;
        [SerializeField] private Button buttonCloseSellSuccess;
        [SerializeField] private Slider sliderSellQuantity;

        #endregion

        private int maxQuantity;
        private float pricePerItem;

        private void Start()
        {
            InitializeUI();
            SetupEventListeners();
        }

        private void InitializeUI()
        {
            panelCarDetail.SetActive(false);
            SetupSellSlider();
            SetupCarStatSliders();
        }

        private void SetupEventListeners()
        {
            buttonCloseCarDetail.onClick.AddListener(HideCarDetailPanel);
            buttonCloseUse.onClick.AddListener(HideUsePanel);
            buttonConfirmSell.onClick.AddListener(HandleSellConfirmation);
            buttonCloseSell.onClick.AddListener(HideSellPanel);
            buttonCloseSellSuccess.onClick.AddListener(HideSellSuccessPanel);
        }

        private void SetupSellSlider()
        {
            sliderSellQuantity.wholeNumbers = true;
            sliderSellQuantity.value = 0;
            sliderSellQuantity.onValueChanged.AddListener(UpdateSellQuantityUI);
        }

        private void SetupCarStatSliders()
        {
            // Set max value and make sliders non-interactive
            Slider[] carStatSliders = { sliderSpeed, sliderHandling, sliderAcceleration, sliderNitro };
            foreach (var slider in carStatSliders)
            {
                slider.maxValue = 200f;
                slider.minValue = 0f;
                slider.interactable = false;
            }
        }

        #region Car Detail Functions
        public void ShowCarDetails(CarModel car)
        {
            textCarName.text = car.name;
            textCarSpeed.text = car.topSpeed.ToString();
            textCarHandling.text = car.handling.ToString();
            textCarAcceleration.text = car.acceleration.ToString();
            textCarNitro.text = car.nitro.ToString();

            // Update sliders
            sliderSpeed.value = car.topSpeed;
            sliderHandling.value = car.handling;
            sliderAcceleration.value = car.acceleration;
            sliderNitro.value = car.nitro;
        }

        public void ShowPanelCarDetail()
        {
            panelCarDetail.SetActive(true);
        }

        private void HideCarDetailPanel() => panelCarDetail.SetActive(false);
        #endregion

        #region Sell Panel Functions
        public void ShowSellItemDetails(AccessoryModel item)
        {
            textItemName.text = item.name;
            textItemQuantity.text = $"You Have: {item.quantity}";
            textItemPrice.text = $"Price for each item: {item.price}";

            maxQuantity = item.quantity;
            pricePerItem = item.price;

            sliderSellQuantity.maxValue = maxQuantity;
            sliderSellQuantity.value = 0;
            UpdateSellQuantityUI(0);
        }

        private void UpdateSellQuantityUI(float quantity)
        {
            int currentQuantity = Mathf.RoundToInt(quantity);
            textCurrentQuantity.text = $"{currentQuantity}/{maxQuantity}";
            textTotalPrice.text = $"Total: {currentQuantity * pricePerItem}";
        }

        private void HandleSellConfirmation()
        {
            if (sliderSellQuantity.value > 0 && sliderSellQuantity.value <= maxQuantity)
            {
                HideSellPanel();
                ShowSellSuccessPanel();
            }
        }
        public void ShowUsePanel() => panelUse.SetActive(true);
        public void ShowSellPanel() => panelSell.gameObject.SetActive(true);
        public void HideSellPanel() => panelSell.gameObject.SetActive(false);
        private void ShowSellSuccessPanel() => panelSellSuccess.gameObject.SetActive(true);
        public void HideSellSuccessPanel() => panelSellSuccess.gameObject.SetActive(false);

        public void HideAllPanels()
        {
            HideCarDetailPanel();
            HideUsePanel();
            HideSellPanel();
            HideSellSuccessPanel();
        }
        #endregion

        #region Use Panel Functions
        private void HideUsePanel() => panelUse.SetActive(false);
        #endregion    
        }
}
