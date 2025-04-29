using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using Rukha93.ModularAnimeCharacter.Customization;
using Rukha93.ModularAnimeCharacter.Customization.UI;
using UnityEngine;
using TMPro; // Add this
using UnityEngine.UI;
using LamFusion;
using System.Collections;
using StarterAssets;
using Lam.GAMEPLAY; // Add this

namespace Edu_World
{
    public class UIShop : BaseUICustomization
    {
        #region Core Shop Components
        public Button btn_CloseShop;
        [SerializeField] private TextMeshProUGUI m_TotalPriceText;
        private int m_TotalPrice = 0;
        private Dictionary<string, int> selectedItemPrices = new Dictionary<string, int>();
        public Dictionary<string, (CustomizationItemAsset asset, string name)> selectedItemsBuy = new Dictionary<string, (CustomizationItemAsset asset, string name)>();
        #endregion

        #region Summary Panel Components
        [SerializeField] private GameObject m_SummaryPanel;
        [SerializeField] private Transform m_SummaryItemsContainer;
        [SerializeField] private Button m_ShowSummaryButton;
        [SerializeField] private Button m_CloseSummaryButton;
        [SerializeField] private UIImageItem m_SummaryItemPrefab;
        [SerializeField] private TextMeshProUGUI m_SummaryTotalPriceText;
        #endregion

        #region Shirt Panel Components
        [SerializeField] private GameObject m_ShirtPanel;
        [SerializeField] private Button btn_OpenShirtPanel;
        [SerializeField] private Button btn_OffShirtPanel;
        [SerializeField] private Button btn_ShirtOn;
        [SerializeField] private Button btn_ShirtOff;
        #endregion

        public override void Awake()
        {
            UIInteract.instance.SetShopState(true); // Add this
            InitializeUI();
            btn_CloseShop.onClick.AddListener(() => HideShop()); // Add this
            m_CategoryItemPrefab.gameObject.SetActive(false);
            m_ImageItemPrefab.gameObject.SetActive(false);
            UpdateTotalPriceText(); // Add this
            m_SummaryPanel.SetActive(false);
            m_ShowSummaryButton.onClick.AddListener(ShowSummaryPanel);
            m_CloseSummaryButton.onClick.AddListener(HideSummaryPanel);
            m_ShirtPanel.SetActive(false);
            btn_OpenShirtPanel.onClick.AddListener(ShowShirtPanel);
            btn_OffShirtPanel.onClick.AddListener(HideShirtPanel);
            // btn_ShirtOn.onClick.RemoveAllListeners();
            // btn_ShirtOff.onClick.RemoveAllListeners();
            btn_ShirtOn.onClick.AddListener(OnShirtOnClicked);
            btn_ShirtOff.onClick.AddListener(OnShirtOffClicked);
        }

        public void InitializeUI()
        {
            CustomizationDemo.localCustomizeCharacter.SetUI(this);
            OnClickCategory += CustomizationDemo.localCustomizeCharacter.OnSelectCategory;
            OnChangeItem += CustomizationDemo.localCustomizeCharacter.OnSwapItem;
            OnChangeColor += CustomizationDemo.localCustomizeCharacter.OnChangeColor;

            // Set categories like in Start()
            SetCategories(CustomizationDemo.m_Categories.ToArray());
            for (int i = 0; i < CustomizationDemo.m_Categories.Count; i++)
                SetCategoryValue(i, "");
            StartCoroutine(CustomizationDemo.localCustomizeCharacter.SelectAfterLoad("hairstyle"));
        }
        private void UpdateTotalPriceText()
        {
            if (m_TotalPriceText != null)
            {
                m_TotalPriceText.text = $"{m_TotalPrice} G";
                m_SummaryTotalPriceText.text = $"{m_TotalPrice} G"; // Update summary panel price
            }
        }
        #region Summary Panel
        private void ShowSummaryPanel()
        {
            m_SummaryPanel.SetActive(true);
            UpdateSummaryPanel();
        }

        private void HideSummaryPanel()
        {
            m_SummaryPanel.SetActive(false);
        }

        private void UpdateSummaryPanel()
        {
            var demo = FindAnyObjectByType<Rukha93.ModularAnimeCharacter.Customization.CustomizationDemo>();

            // Clear existing items
            foreach (Transform child in m_SummaryItemsContainer)
            {
                Destroy(child.gameObject);
            }

            // Add selected items
            foreach (var item in selectedItemsBuy)
            {
                UIImageItem summaryItem = Instantiate(m_SummaryItemPrefab, m_SummaryItemsContainer);
                summaryItem.SetTitle(item.Value.name); // Use the stored name instead of category
                summaryItem.Setup(item.Value.asset.icon);
                summaryItem.SetUpBackgroundImageItemShop(); // Add this

                // Create unequip button
                string category = item.Key; // Capture category for the lambda
                summaryItem.buttonUnEquipped.onClick.AddListener(() =>
                {
                    // Remove price from total
                    if (selectedItemPrices.ContainsKey(category))
                    {
                        m_TotalPrice -= selectedItemPrices[category];
                        selectedItemPrices.Remove(category);
                    }

                    // Unequip item
                    UnequipItemImage(category);

                    // Update UI
                    UpdateTotalPriceText();
                    UpdateSummaryPanel();

                    // Reset category display
                    // OnChangeItem?.Invoke(category, "");
                    demo.ReloadSavedEquipmentByCategory(category);
                });

                // Disable interaction components
                if (summaryItem.GetComponent<Button>() != null)
                    summaryItem.GetComponent<Button>().enabled = false;
                if (summaryItem.GetComponentInChildren<Image>() != null)
                    summaryItem.GetComponentInChildren<Image>().raycastTarget = false;
            }
            UpdateTotalPriceText();
        }
        #endregion


        #region Setup category and ImageItem
        public override void SetCategories(string[] categories)
        {
            int itemIndex = 0;
            for (int i = 0; i < categories.Length; i++)
            {
                // Skip skin category
                if (categories[i].ToLower() == "skin")
                    continue;
                if (categories[i].ToLower() == "head")
                    continue;

                UICategoryItem item;
                if (itemIndex < m_CategoryItems.Count)
                {
                    item = m_CategoryItems[itemIndex];
                }
                else
                {
                    item = Instantiate(m_CategoryItemPrefab, m_CategoryPanel.transform);
                    item.SetupBackgroundForShop(categories[i].ToLower());
                    if (categories[i].ToLower() == "hairstyle")
                    {
                        lastSelectedCategoryItem = item;
                        lastSelectedCategory = categories[i];
                    }
                    m_CategoryItems.Add(item);
                }

                item.gameObject.SetActive(true);
                item.Title = categories[i];

                if (itemIndex < m_CategoryIcons.Count)
                {
                    item.Icon = m_CategoryIcons[itemIndex];
                }

                string aux = categories[i];
                item.OnClick = () =>
                {
                    if (lastSelectedCategoryItem != null && lastSelectedCategoryItem != item)
                    {
                        lastSelectedCategoryItem.ResetToLastCategory();
                    }
                    lastSelectedCategoryItem = item;

                    if (lastSelectedCategory == aux)
                    {
                        bool isCurrentlyVisible = ScrollViewPanel.activeSelf;
                    }
                    else
                    {
                        OnClickCategory?.Invoke(aux);
                    }
                    lastSelectedCategory = aux;
                };

                itemIndex++;
            }

            for (int i = itemIndex; i < m_CategoryItems.Count; i++)
                m_CategoryItems[i].gameObject.SetActive(false);
        }

        public override void SetCategoryValue(int index, string value)
        {
            if (m_CategoryItems.Count > index)
            {
                UICategoryItem item = m_CategoryItems[index];
            }
        }
        public override void SetCustomizationOptions(string category, string[] items, string currentItem, Dictionary<string, CustomizationItemAsset> itemAssets)
        {
            CurrentCategory = category;
            int previouslySelectedIndex = GetSelectedIndexForCategory(category);

            for (int i = 0; i < m_ImageItems.Count; i++)
            {
                m_ImageItems[i].ResetToLastImage();
            }

            // Use all items without skipping
            m_ItemOptions = new List<string>(items.Skip(1));
            
            m_CurrentItemIndex = Mathf.Max(m_ItemOptions.IndexOf(currentItem), 0);

            // m_CustomizationTitle.text = category;

            // Generate ImageItems
            for (int i = 0; i < m_ItemOptions.Count; i++)
            {
                UIImageItem item;
                if (i < m_ImageItems.Count)
                {
                    item = m_ImageItems[i];
                }
                else
                {
                    item = Instantiate(m_ImageItemPrefab, m_ImageItemPanel.transform);
                    m_ImageItems.Add(item);
                }

                item.gameObject.SetActive(true);
                string itemPath = m_ItemOptions[i];

                // Get proper display name (remove path components)
                string itemName = m_ItemOptions[i];
                item.SetTitle(itemName);
                item.SetUpBackgroundImageItem();

                // Set icon if available
                if (itemAssets != null && itemAssets.ContainsKey(itemPath))
                {
                    var asset = itemAssets[itemPath];
                    if (asset != null)
                    {
                        item.Setup(asset.icon, asset.price); // Update to pass price
                    }
                }

                // Restore previously selected item for this category
                if (GetSelectedIndexForCategory(category) == i)
                {
                    item.ChangeImageChoose();
                }

                int currentIndex = i; // Capture the current index for the lambda
                item.OnClick = () =>
                {
                    // Check if clicking the same item that's already selected
                    if (GetSelectedIndexForCategory(category) == currentIndex)
                    {
                        return; // Do nothing if clicking the same item
                    }

                    // Reset previous selection for this category if exists
                    int prevIndex = GetSelectedIndexForCategory(category);
                    if (prevIndex != -1 && prevIndex != currentIndex)
                    {
                        if (prevIndex >= 0 && prevIndex < m_ImageItems.Count)
                        {
                            m_ImageItems[prevIndex].ResetToLastImage();
                        }
                    }

                    SetSelectedIndexForCategory(category, currentIndex);

                    // Update prices
                    if (itemAssets != null && itemAssets.ContainsKey(itemPath))
                    {
                        // Remove previous price for this category if exists
                        if (selectedItemPrices.ContainsKey(category))
                        {
                            m_TotalPrice -= selectedItemPrices[category];
                        }

                        // Add new price
                        int newPrice = itemAssets[itemPath].price;
                        selectedItemPrices[category] = newPrice;
                        m_TotalPrice += newPrice;
                    }

                    // Track selected item
                    if (itemAssets != null && itemAssets.ContainsKey(itemPath))
                    {
                        selectedItemsBuy[category] = (itemAssets[itemPath], itemName);
                    }

                    UpdateTotalPriceText();

                    m_CurrentItemIndex = m_ItemOptions.IndexOf(itemPath);
                    OnChangeItem?.Invoke(CurrentCategory, itemPath);
                };
            }

            // Deactivate unused items
            for (int i = m_ItemOptions.Count; i < m_ImageItems.Count; i++)
            {
                m_ImageItems[i].gameObject.SetActive(false);
            }

            // //item swapping
            // if (m_CurrentItemIndex >= m_ItemOptions.Count)
            // {
            //     m_SwapperItem.gameObject.SetActive(false);
            //     return;
            // }

            // m_SwapperItem.Text = m_ItemOptions[m_CurrentItemIndex];
            // m_SwapperItem.gameObject.SetActive(false); // Always keep SwapperItem hidden
        }
        // public override void SetCustomizationMaterials(Renderer[] renderers){
        //     return;
        // }
        public override void SetCustomizationMaterials(Renderer[] renderers)
        {
            if (renderers == null)
                renderers = new Renderer[0];

            //get all available materials
            List<Material> materials = new List<Material>();
            List<Renderer> renderersPerMaterial = new List<Renderer>();
            List<int> rendererMaterialIndex = new List<int>();
            List<MaterialPropertyBlock> propertyBlock = new List<MaterialPropertyBlock>();
            m_MaterialOptions = new List<string>(); // Initialize materials list

            for (int i = 0; i < renderers.Length; i++)
            {
                var renderer = renderers[i];
                var sharedMaterials = renderer.sharedMaterials;

                for (int j = 0; j < sharedMaterials.Length; j++)
                {
                    if (materials.Contains(sharedMaterials[j]))
                        continue;

                    // // Skip if current category is head and material is in hidden list
                    // if (CurrentCategory.ToLower() == "head" && 
                    //     hiddenHeadMaterials.Contains(sharedMaterials[j].name))
                    //     continue;

                    materials.Add(sharedMaterials[j]);
                    renderersPerMaterial.Add(renderer);
                    rendererMaterialIndex.Add(j);

                    var block = new MaterialPropertyBlock();
                    renderer.GetPropertyBlock(block, j);
                    propertyBlock.Add(block);
                }
            }

            for (int i = 0; i < materials.Count; i++)
            {
                //item from pool or instantiate
                UIMaterialItem item;
                // if (i < m_MaterialItems.Count)
                // {
                //     item = m_MaterialItems[i];
                // }
                // else
                // {
                //     // Instantiate and handle nested prefab items
                //     item = Instantiate(m_MaterialItemPrefab, m_ColorPanel.transform);

                //     // Deactivate any child UIMaterialItems that might have been created from the prefab
                //     var childMaterialItems = item.GetComponentsInChildren<UIMaterialItem>(true);
                //     foreach (var childItem in childMaterialItems)
                //     {
                //         if (childItem != item)
                //         {
                //             childItem.gameObject.SetActive(false);
                //         }
                //     }

                //     m_MaterialItems.Add(item);
                // }
                // item.gameObject.SetActive(i == 0);

                // //setup item
                // item.Title = materials[i].name;
                m_MaterialOptions.Add(materials[i].name); // Store material names

                var auxRenderer = renderersPerMaterial[i];
                var auxMatIndex = rendererMaterialIndex[i];
                var auxPropertyBlock = propertyBlock[i];

                if (materials[i].HasProperty("_MaskRemap"))
                {
                    // item.ResetColors();
                    // string materialName = materials[i].name;

                    // item.AddDoubleColor(
                    //     auxPropertyBlock.HasColor("_Color_A_2") ? auxPropertyBlock.GetColor("_Color_A_2") : materials[i].GetColor("_Color_A_2"),
                    //     auxPropertyBlock.HasColor("_Color_A_1") ? auxPropertyBlock.GetColor("_Color_A_1") : materials[i].GetColor("_Color_A_1"),
                    //     (c, buttonIndex) => {
                    //         OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_A_2", c);
                    //         TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_A_2", buttonIndex);
                    //     },
                    //     (c, buttonIndex) => {
                    //         OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_A_1", c);
                    //         TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_A_1", buttonIndex);
                    //     }
                    // );

                    // if (CurrentCategory.ToLower() != "hairstyle")
                    // {
                    //     item.AddDoubleColor(
                    //         auxPropertyBlock.HasColor("_Color_B_2") ? auxPropertyBlock.GetColor("_Color_B_2") : materials[i].GetColor("_Color_B_2"),
                    //         auxPropertyBlock.HasColor("_Color_B_1") ? auxPropertyBlock.GetColor("_Color_B_1") : materials[i].GetColor("_Color_B_1"),
                    //         (c, buttonIndex) => {
                    //             OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_B_2", c);
                    //             TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_B_2", buttonIndex);
                    //         },
                    //         (c, buttonIndex) => {
                    //             OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_B_1", c);
                    //             TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_B_1", buttonIndex);
                    //         }
                    //     );
                    // }

                    // if (CurrentCategory.ToLower() != "skin" && CurrentCategory.ToLower() != "hairstyle")
                    // {
                    //     item.AddDoubleColor(
                    //         auxPropertyBlock.HasColor("_Color_C_2") ? auxPropertyBlock.GetColor("_Color_C_2") : materials[i].GetColor("_Color_C_2"),
                    //         auxPropertyBlock.HasColor("_Color_C_1") ? auxPropertyBlock.GetColor("_Color_C_1") : materials[i].GetColor("_Color_C_1"),
                    //         (c, buttonIndex) => {
                    //             OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_C_2", c);
                    //             TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_C_2", buttonIndex);
                    //         },
                    //         (c, buttonIndex) => {
                    //             OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_C_1", c);
                    //             TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_C_1", buttonIndex);
                    //         }
                    //     );
                    // }
                }
            }

            // if (m_MaterialOptions.Count > 0)
            // {
            //     m_CurrentMaterialIndex = 0;
            //     m_SwapMaterial.Text = m_MaterialOptions[m_CurrentMaterialIndex];
            //     m_SwapMaterial.gameObject.SetActive(m_MaterialOptions.Count > 1);
            // }
            // else
            // {
            //     m_SwapMaterial.gameObject.SetActive(false);
            // }

            // for (int i = materials.Count; i < m_MaterialItems.Count; i++)
            //     m_MaterialItems[i].gameObject.SetActive(false);
            // m_ColorPicker.Close();

            ShowMaterialDetails(0);
        }
        #endregion

        #region Shirt Panel

        private void HideShop()
        {
            InputManager.instance.input.ToggleCursorState();
            InputManager.instance.input.SetCursorState(true);
            LamFusion.Camera.instance.TurnCameraPlayer();
            UIInteract.instance.SetShopState(false); // Add this
            UIInteract.instance.ActiveButtonE();
            gameObject.SetActive(false);
        }
        private void ShowShirtPanel()
        {
            m_ShirtPanel.SetActive(true);
            HideSummaryPanel();
        }

        private void HideShirtPanel()
        {
            m_ShirtPanel.SetActive(false);
        }

        private void OnShirtOnClicked()
        {
            // Clear selected items
            foreach (var item in selectedItemsBuy.Keys.ToList())
            {
                UnequipItemImage(item);
            }
            // Reset prices
            m_TotalPrice = 0;
            selectedItemPrices.Clear();
            UpdateTotalPriceText();
            // Save changes
            CustomizationDemo.localCustomizeCharacter.SaveAllToPrefs();
            // Just hide the shirt panel without showing button E
            HideShirtPanel();
        }

        public void OnShirtOffClicked()
        {
            foreach (var item in selectedItemsBuy.Keys.ToList())
            {
                UnequipItemImage(item);
            }
            m_TotalPrice = 0;
            selectedItemPrices.Clear();
            UpdateTotalPriceText();
            CustomizationDemo.localCustomizeCharacter.ReloadSavedEquipment();
            HideShirtPanel();
        }
        #endregion

        public void UnequipItemImage(string category)
        {
            selectedItemsBuy.Remove(category);

            // Find and reset the selected item in the category
            int selectedIndex = GetSelectedIndexForCategory(category);
            if (selectedIndex >= 0 && selectedIndex < m_ImageItems.Count)
            {
                m_ImageItems[selectedIndex].ResetToLastImage();
                // Reset the selected index
                SetSelectedIndexForCategory(category, -1);
            }

            // Reset the corresponding category item if needed
            var categoryItem = m_CategoryItems.FirstOrDefault(x => x.Title == category);
            if (categoryItem != null)
            {
                categoryItem.ResetToLastCategory();
            }
            
        }

                public override int GetSelectedIndexForCategory(string category)
        {
            return CategorySelection.GetSelectedIndexForCategory(selectedItems, category);
        }

        public override void SetSelectedIndexForCategory(string category, int index)
        {
            CategorySelection.SetSelectedIndexForCategory(selectedItems, category, index);
        }
    }
}
