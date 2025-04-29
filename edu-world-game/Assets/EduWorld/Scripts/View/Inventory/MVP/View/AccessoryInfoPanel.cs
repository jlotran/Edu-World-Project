using System;
using Edu_World.Inventory.MVP.View;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Edu_World.Inventory.MVP
{
    public class AccessoryInfoPanel : BasePanel
    {
        [Header("Info Panel")]
        [SerializeField] private TextMeshProUGUI nameText;
        [SerializeField] private TextMeshProUGUI descriptionText;
        [SerializeField] private TextMeshProUGUI moneyText;
        [SerializeField] private Transform panelTransform;

        [SerializeField] private Button buttonUse;
        [SerializeField] private Button buttonSell;

        public event Action OnUseButtonClicked;
        public event Action OnSellButtonClicked;
        private void Start()
        {
            buttonUse.onClick.AddListener(
                () =>
                {
                    OnUseButtonClicked?.Invoke();
                }
            );
            buttonSell.onClick.AddListener(
                () =>
                {
                    OnSellButtonClicked?.Invoke();
                }
            );

            InventoryManager.Instance.OnCategorySelected += OnCategorySelected;
            InventoryManager.Instance.OnCloseInventory += CloseInventory;
        }

        private void OnCategorySelected(string categoryName)
        {
            if (categoryName.Equals(ItemCategory.Accessory.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                Show();
            } else
            {
                Hide();
            }
        }

        private void CloseInventory(){
            Hide();
        }

        public override void Show()
        {
            panelTransform.gameObject.SetActive(true);
        }

        public override void Hide()
        {
            panelTransform.gameObject.SetActive(false);
        }

        public void ShowInfo(AccessoryModel baseItem)
        {
            panelTransform.gameObject.SetActive(true);
            nameText.text = baseItem.name;
            descriptionText.text = baseItem.description;
            moneyText.text = $"{baseItem.price}";
        }
    }
}
