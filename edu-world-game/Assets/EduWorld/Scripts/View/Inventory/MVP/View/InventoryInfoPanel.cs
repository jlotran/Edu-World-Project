using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Edu_World.Inventory.MVP.View
{
    using Edu_World.Inventory.MVP;
    using EduWorld;

    public class InventoryInfoPanel : BasePanel
    {
        [Header("Common")]
        [SerializeField] private Transform panelTransform;
        [SerializeField] private Button primaryButton;
        [SerializeField] private TextMeshProUGUI buttonText;

        [Header("Info Elements")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private GameObject infoElementsContainer;

        [Header("Image Elements")]
        [SerializeField] private RawImage displayImage;
        [SerializeField] private Texture textureImageCar;
        [SerializeField] private Texture textureImageOutfit;
        [SerializeField] private GameObject imageElementsContainer;

        [Header("Secondary Actions")]
        [SerializeField] private Button sellButton;
        [SerializeField] private GameObject sellButtonContainer;

        public event Action OnPrimaryButtonClicked;
        public event Action OnSellButtonClicked;

        private string currentCategory;

        void Start()
        {
            // primaryButton.onClick.AddListener(HandlePrimaryButtonClick);
            sellButton.onClick.AddListener(() => OnSellButtonClicked?.Invoke());

            InventoryManager.Instance.OnCategorySelected += OnCategorySelected;
            InventoryManager.Instance.OnCloseInventory += OnCloseInventory;
            InventoryManager.Instance.OnShowInventory += ShowInventory;
        }

        private void OnCategorySelected(string categoryName)
        {
            Show();
            SetCategory(categoryName);
            UpdatePanelLayout(categoryName);
        }

        private void UpdatePanelLayout(string categoryName)
        {
            bool isAccessory = categoryName.Equals(ItemCategory.Accessory.ToString(), StringComparison.OrdinalIgnoreCase);
            bool isVisual = categoryName.Equals(ItemCategory.Car.ToString(), StringComparison.OrdinalIgnoreCase) || categoryName.Equals(ItemCategory.Outfit.ToString(), StringComparison.OrdinalIgnoreCase);
            infoElementsContainer.SetActive(isAccessory);
            imageElementsContainer.SetActive(isVisual);
            sellButtonContainer.SetActive(isAccessory);

            if (isVisual)
            {
                UpdateDisplayImage();
            }
        }

        // private void HandlePrimaryButtonClick()
        // {
        //     switch (currentCategory)
        //     {
        //         case ItemCategory.Car.ToString():
        //             OnPrimaryButtonClicked?.Invoke();
        //             break;
        //         case ItemCategory.Outfit.ToString():
        //             InventoryManager.Instance.SaveOutfit();
        //             break;
        //         case ItemCategory.Accessories.ToString():
        //             OnPrimaryButtonClicked?.Invoke();
        //             break;
        //     }
        // }

        public void ShowItems(BaseItem item)
        {
            switch (item)
            {
                case CarModel carModel:
                    ShowCarInfo(carModel);
                    break;
                case OutfitModel _:
                    ShowOutfitInfo();
                    break;
                case AccessoryModel accessoryModel:
                    ShowAccessoryInfo(accessoryModel);
                    break;
            }
        }

        private void ShowCarInfo(CarModel carModel)
        {
            displayImage.texture = textureImageCar;
            nameText.text = carModel.name;
            buttonText.text = "View Details";
        }

        private void ShowOutfitInfo()
        {
            displayImage.texture = textureImageOutfit;
            buttonText.text = "Save Outfit";
        }

        private void ShowAccessoryInfo(AccessoryModel accessory)
        {
            nameText.text = accessory.name;
            descriptionText.text = accessory.description;
            moneyText.text = $"{accessory.price}";
            buttonText.text = "Use";
        }

        public void SetCategory(string category)
        {
            currentCategory = category;
        }

        private void UpdateDisplayImage()
        {
            if (displayImage == null) return;
            
            displayImage.texture = currentCategory.Equals(ItemCategory.Car.ToString(), StringComparison.OrdinalIgnoreCase)
                ? textureImageCar 
                : textureImageOutfit;
        }

        public void OnCloseInventory()
        {
            Hide();
        }

        public void ShowInventory()
        {
            Show();
            SetCategory("Car");
        }

        public override void Show()
        {
            panelTransform.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            panelTransform.gameObject.SetActive(false);
        }
    }
}
