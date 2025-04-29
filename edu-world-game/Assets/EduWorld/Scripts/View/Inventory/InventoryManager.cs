using System;
using UnityEngine;
using RGSK;
using Edu_World.Inventory.UI;
using System.Collections.Generic;
namespace Edu_World.Inventory
{
    using System.Collections;
    using Edu_World.Inventory.MVP;
    using Edu_World.Inventory.MVP.View;
    using Rukha93.ModularAnimeCharacter.Customization;
    using UnityEngine.UI;

    public class InventoryManager : Singleton<InventoryManager>
    {
        [SerializeField] private GameObject inventoryPanel;
        [SerializeField] private InventoryView _inventoryView;
        [SerializeField] private CategoryButtonsManager _inventoryCategoryButtonManager;
        [SerializeField] private AccessoryInfoPanel _accessoryInfoPanel;
        [SerializeField] private RawImagePanel _rawImagePanel;
        [SerializeField] private PanelReward _panelReward;
        [SerializeField] private PanelSubCategory _panelSubCategory;
        [SerializeField] private UIInventoryCustomization _uIInventoryCustomization;
        [SerializeField] private CategoryButtonsManager _categoryButtonsManager;
        [SerializeField] private ItemInventoryButtonManager itemInventoryButtonManager;

        public event Action<string> OnCategorySelected;

        public event Action OnCloseInventory;
        public event Action OnShowInventory;

        [SerializeField] private Button btn_closeInventory;


        void Start()
        {
            _rawImagePanel.OnTransformButtonClicked += () =>
             {
                 _panelReward.ShowPanelCarDetail();
             };

            _accessoryInfoPanel.OnUseButtonClicked += () =>
            {
                _panelReward.ShowUsePanel();
            };
            _accessoryInfoPanel.OnSellButtonClicked += () =>
            {
                _panelReward.ShowSellPanel();
            };

            btn_closeInventory.onClick.AddListener(() =>
            {

                CloseInventory();
            });
            // _uIInventoryCustomization.InitializeUI();
            inventoryPanel.SetActive(false);
        }

        public void SetCategories(List<Category> categories)
        {
            _inventoryCategoryButtonManager.InitData(categories);
        }

        public void CategorySelected(string categoryName)
        {
            OnCategorySelected.Invoke(categoryName);
            itemInventoryButtonManager.OnCategorySelected(categoryName);
        }

        public void ShowInventory<T>(List<T> items) where T : BaseItem
        {

        }

        public BaseItemInventory GetBaseItemInventoryPref<T>() where T : BaseItemInventory
        {
            return _inventoryView.GetPrefabByCategoryModel<T>();
        }

        public void ShowAccessoryInfo(AccessoryModel accessory)
        {
            if (_accessoryInfoPanel != null)
            {
                _accessoryInfoPanel.ShowInfo(accessory);
                _panelReward.ShowSellItemDetails(accessory);
            }
        }

        public void ShowCarInformation(CarModel carModel)
        {
            if (_accessoryInfoPanel != null)
            {
                _rawImagePanel.ShowItems(carModel);
                _panelReward.ShowCarDetails(carModel);
            }
        }
        public void OpenInventory()
        {
            OnShowInventory?.Invoke();
            inventoryPanel.SetActive(true);
            StartCoroutine(DelayCategorySelection(ItemCategory.Car.ToString()));
        }

        private IEnumerator DelayCategorySelection(string categoryName)
        {
            yield return new WaitForSeconds(0.2f);
            CategorySelected(categoryName); // This will trigger both category selection and first item selection
        }

        public void CloseInventory()
        {
            OnCloseInventory?.Invoke();
            inventoryPanel.SetActive(false);
            // _inventoryView.ClearAllItems();
            OnHideAllPanels();
        }
        public void SaveOutfit() => CustomizationDemo.localCustomizeCharacter.SaveAllToPrefs();


        private void OnHideAllPanels()
        {
            _panelReward?.HideAllPanels();
        }

        public void AddCarInventoryItem(CarInventoryItem item)
        {
            itemInventoryButtonManager.AddCarInventoryItem(item);
        }

        public void AddAccessoryInventoryItem(AccessoryItemInventory item)
        {
            itemInventoryButtonManager.AddAccessoryInventoryItem(item);
        }
    }
}
