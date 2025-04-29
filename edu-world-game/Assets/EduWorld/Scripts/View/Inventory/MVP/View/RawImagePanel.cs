using System;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Edu_World.Inventory.MVP.View
{
    using Edu_World.Inventory.MVP;
    using EduWorld;

    public class RawImagePanel : BasePanel
    {
        [Header("Both Panels")]
        [SerializeField] private RawImage displayImage;
        [SerializeField] private Button button;
        [SerializeField] private Transform panelTransform;
        private TextMeshProUGUI buttonText;

        [Header("Raw Image Car")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private Texture textureImageCar;
        [SerializeField] private GameObject panelNameCar;

        [Header("Raw Image Outfit")]
        [SerializeField] private Texture textureImageOutfit;

        public event Action OnTransformButtonClicked;

        private string currentCategory;

        void Start()
        {
            button.onClick.AddListener(HandleButtonClick);
            buttonText = button.GetComponentInChildren<TextMeshProUGUI>();

            InventoryManager.Instance.OnCategorySelected += OnCategorySelected;
            InventoryManager.Instance.OnCloseInventory += OnCloseInventory;
            InventoryManager.Instance.OnShowInventory += showInventory;
        }

        private void OnCategorySelected(string categoryName)
        {
            if (categoryName.Equals(ItemCategory.Car.ToString(), StringComparison.OrdinalIgnoreCase) ||
                categoryName.Equals(ItemCategory.Outfit.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                Show();
                SetCategory(categoryName);
            }
            else
            {
                Hide();
            }
        }

        private void HandleButtonClick()
        {
            if (currentCategory.Equals(ItemCategory.Car.ToString()))
            {
                OnTransformButtonClicked?.Invoke();
            }
            else if (currentCategory.Equals(ItemCategory.Outfit.ToString()))
            {
                InventoryManager.Instance.SaveOutfit();
            }
        }

        public void ShowItems(BaseItem category)
        {
            switch (category)
            {
                case CarModel carModel:
                    ShowCarPanel(carModel);
                    break;
                case OutfitModel:
                    ShowOutfitPanel();
                    break;
            }
        }

        public void SetCategory(string category)
        {
            currentCategory = category;
            UpdateDisplayImage();
        }

        private void UpdateDisplayImage()
        {
            if (displayImage != null)
            {
                if (currentCategory.Equals(ItemCategory.Car.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    displayImage.texture = textureImageCar;
                    panelNameCar.SetActive(true);
                    buttonText.text = "View Details";
                }
                else if (currentCategory.Equals(ItemCategory.Outfit.ToString(), StringComparison.OrdinalIgnoreCase))
                {
                    displayImage.texture = textureImageOutfit;
                    panelNameCar.SetActive(false);
                    buttonText.text = "Save Outfit";
                }

            }
        }

        private void ShowCarPanel(CarModel carModel)
        {
            panelNameCar.SetActive(true);
            if (displayImage != null)
            {
                displayImage.texture = textureImageCar;
            }
            buttonText.text = "View Details";
            nameText.text = carModel.name; // Set the car name here, if available
        }

        private void ShowOutfitPanel()
        {
            panelNameCar.SetActive(false);
            if (displayImage != null)
            {
                displayImage.texture = textureImageOutfit;
            }
            buttonText.text = "Save Outfit";
        }

        public void OnCloseInventory()
        {
            Hide();
        }

        public void showInventory(){
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
