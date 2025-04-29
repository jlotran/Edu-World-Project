using System;
using UnityEngine;

namespace Edu_World.Inventory.MVP.View
{
    public class PanelSubCategory : BasePanel
    {
        [SerializeField] private GameObject _panel;

        void Start()
        {
            InventoryManager.Instance.OnCategorySelected += OnCategorySelected;
            InventoryManager.Instance.OnCloseInventory += CloseInventory;
        }

        private void OnCategorySelected(string categoryName)
        {
            Action action = categoryName.Equals(ItemCategory.Outfit.ToString()) ? (Action)Show : Hide;
            action();
        }
        
        public void CloseInventory()
        {
            Hide();
        }
        public override void Show()
        {
            _panel.SetActive(true);
        }

        public override void Hide()
        {
            _panel.SetActive(false);
        }
    }
}
