using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Linq;
using Fusion.Menu;
using TMPro;
using System;
using Edu_World;

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{

    public class UICustomizationDemo : BaseUICustomization
    {
        public static UICustomizationDemo instance;
        public static event Action OnIdle;
        public static event Action OnLookAround;
        public override void Awake()
        {
            if (instance == null)
            {
                instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
            m_CategoryItemPrefab.gameObject.SetActive(false);
            m_MaterialItemPrefab.gameObject.SetActive(false);
            m_ImageItemPrefab.gameObject.SetActive(false);
            ScrollViewPanel.SetActive(false);
            m_SwapperItem.gameObject.SetActive(false);
            m_CustomizationPanel.SetActive(false);
            var existingMaterialItems = m_MaterialItemPrefab.GetComponentsInChildren<UIMaterialItem>(true);
            foreach (var item in existingMaterialItems)
            {
                if (item != m_MaterialItemPrefab)
                {
                    item.gameObject.SetActive(false);
                }
            }

            m_SwapperItem.OnClickLeft = _OnSwapPrevious;
            m_SwapperItem.OnClickRight = _OnSwapNext;

            // Initialize material swapper events
            m_SwapMaterial.OnClickLeft = () => _OnSwapPreviousMaterial();
            m_SwapMaterial.OnClickRight = () => _OnSwapNextMaterial();

            if (saveButton != null)
            {
                customizationDemo = FindAnyObjectByType<CustomizationDemo>();
                saveButton.onClick.AddListener(OnSaveButtonClicked);
            }
        }

        public override void Start()
        {
            ShowCustomization(false);
            customizationDemo.SetUI(this);
            m_ColorPicker.Close();
        }


        #region CATEGORY PANEL

        public override void ResetAndApplyStoredColorsForCategory(string category)
        {
            // Reset all color highlights first
            foreach (var matItem in m_MaterialItems)
            {
                matItem.ResetAllColorHighlights();
            }
            // Get the currently selected index for this category
            int currentIndex = GetSelectedIndexForCategory(category);
            if (currentIndex >= 0)
            {
                // Apply stored colors for the current category and item
                ApplyStoredColors(category, currentIndex);
            }
        }

        public override void SetCategories(string[] categories)
        {
            for (int i = 0; i < categories.Length; i++)
            {
                UICategoryItem item;
                if (i < m_CategoryItems.Count)
                {
                    item = m_CategoryItems[i];
                }
                else
                {
                    item = Instantiate(m_CategoryItemPrefab, m_CategoryPanel.transform);
                    item.SetupBackground();
                    m_CategoryItems.Add(item);
                }

                item.gameObject.SetActive(true);
                item.Title = categories[i];

                if (i < m_CategoryIcons.Count)
                {
                    item.Icon = m_CategoryIcons[i];
                }

                string aux = categories[i];
                item.OnClick = () =>
                {
                    // Reset previous category item if exists
                    if (lastSelectedCategoryItem != null && lastSelectedCategoryItem != item)
                    {
                        lastSelectedCategoryItem.ResetToLastCategory();
                    }
                    lastSelectedCategoryItem = item;

                    if (aux.ToLower() == "head" || aux.ToLower() == "hairstyle")
                    {
                        OnIdle?.Invoke();
                    }
                    else
                    {
                        OnLookAround?.Invoke();
                    }

                    // Reset all color highlights first
                    if (aux.ToLower() == "skin")
                    {
                        ScrollViewPanel.SetActive(false);
                        ShowCustomization(true);
                        m_CustomizationPanel.SetActive(true);  // Thêm dòng này
                        OnClickCategory?.Invoke(aux);
                        ResetAndApplyStoredColorsForCategory(aux);
                    }
                    else if (lastSelectedCategory == aux)
                    {
                        // If clicking the same category
                        bool isCurrentlyVisible = ScrollViewPanel.activeSelf;
                        ScrollViewPanel.SetActive(!isCurrentlyVisible);
                        m_CustomizationPanel.SetActive(!isCurrentlyVisible);
                        // m_ColorPanel.gameObject.SetActive(!isCurrentlyVisible);

                        if (isCurrentlyVisible == false)
                        {
                            ResetAndApplyStoredColorsForCategory(aux);
                        }
                    }
                    else
                    {
                        // If clicking a different category
                        ScrollViewPanel.SetActive(true);
                        // m_ColorPanel.gameObject.SetActive(true);
                        m_CustomizationPanel.SetActive(true);
                        OnClickCategory?.Invoke(aux);
                        ResetAndApplyStoredColorsForCategory(aux);
                    }
                    lastSelectedCategory = aux;
                };
            }

            for (int i = categories.Length; i < m_CategoryItems.Count; i++)
                m_CategoryItems[i].gameObject.SetActive(false);
        }

        public override void SetCategoryValue(int index, string value)
        {
            if (index < 0 || index >= m_CategoryItems.Count)
                return;
            UICategoryItem item = m_CategoryItems[index];
            // item.Value = value;
        }

        #endregion

        #region CUSTOMIZATION PANEL

        public override void ShowCustomization(bool value)
        {

            m_ColorPanel.gameObject.SetActive(true);
        }

        // Add these helper methods to manage selections
        public override int GetSelectedIndexForCategory(string category)
        {
            return CategorySelection.GetSelectedIndexForCategory(selectedItems, category);
        }

        public override void SetSelectedIndexForCategory(string category, int index)
        {
            CategorySelection.SetSelectedIndexForCategory(selectedItems, category, index);
        }

        // Add this helper method to check stored colors for an item
        private void ApplyStoredColors(string category, int itemIndex)
        {
            if (category.ToLower() == "head")
            {
                ApplyHeadColors();
                return;
            }

            var materialColors = selectionManager.GetMaterialColors(category, itemIndex);
            if (materialColors == null)
                return;

            foreach (var materialItem in m_MaterialItems)
            {
                string materialName = materialItem.Title;
                if (materialColors.ContainsKey(materialName))
                {
                    var colorProperties = materialColors[materialName];
                    foreach (var colorProp in colorProperties)
                    {
                        materialItem.HighlightStoredColor(colorProp.Key, colorProp.Value);
                    }
                }
            }
        }

        private void ApplyHeadColors()
        {
            // Get all stored head colors from any head item
            Dictionary<string, Dictionary<string, int>> headColors = null;
            for (int i = 0; headColors == null && i < m_ItemOptions.Count; i++)
            {
                headColors = selectionManager.GetMaterialColors("head", i);
            }

            if (headColors == null)
                return;

            // Apply these colors to all materials
            foreach (var materialItem in m_MaterialItems)
            {
                string materialName = materialItem.Title;
                if (headColors.ContainsKey(materialName))
                {
                    var colorProperties = headColors[materialName];
                    foreach (var colorProp in colorProperties)
                    {
                        materialItem.HighlightStoredColor(colorProp.Key, colorProp.Value);
                    }
                }
            }
        }

        private void TrackColorSelection(string category, int itemIndex, string materialName, string colorProperty, int buttonIndex)
        {
            if (category.ToLower() == "head")
            {
                // For head category, track color for all items
                for (int i = 0; i < m_ItemOptions.Count; i++)
                {
                    selectionManager.TrackColorSelection(category, i, materialName, colorProperty, buttonIndex);
                }
            }
            else
            {
                selectionManager.TrackColorSelection(category, itemIndex, materialName, colorProperty, buttonIndex);
            }

            foreach (var materialItem in m_MaterialItems)
            {
                if (materialItem.Title == materialName)
                {
                    materialItem.SaveHighlightState(materialItem.Title, colorProperty, buttonIndex);
                    break;
                }
            }
        }

        public override void SetCustomizationOptions(string category, string[] items, string currentItem, Dictionary<string, CustomizationItemAsset> itemAssets)
        {
            CurrentCategory = category;

            // Reset items except for the previously selected index
            int previouslySelectedIndex = GetSelectedIndexForCategory(category);

            // Reset items that are not the previously selected item
            for (int i = 0; i < m_ImageItems.Count; i++)
            {
                if (i != previouslySelectedIndex)
                {
                    m_ImageItems[i].ResetToLastImage();
                }
            }

            // Filter out index 0 for non-head categories
            if (category.ToLower() != "head")
            {
                m_ItemOptions = new List<string>(items.Skip(1));
            }
            else
            {
                m_ItemOptions = new List<string>(items);
            }

            m_CurrentItemIndex = Mathf.Max(m_ItemOptions.IndexOf(currentItem), 0);

            m_CustomizationTitle.text = category;

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

                string itemName = m_ItemOptions[i];
                item.SetUpBackgroundImageItem();
                item.SetTitle(itemName);

                // Set icon if available
                if (itemAssets != null && itemAssets.ContainsKey(itemPath))
                {
                    var asset = itemAssets[itemPath];
                    if (asset != null && asset.icon != null)
                    {
                        item.Setup(asset.icon);
                    }
                }

                // Auto-highlight the first item if no previous selection exists
                if (GetSelectedIndexForCategory(category) == -1 && i == 0 && category.ToLower() != "skin")
                {
                    item.ChangeImageChoose();
                    SetSelectedIndexForCategory(category, 0);
                }
                // Otherwise restore previously selected item for this category
                if (GetSelectedIndexForCategory(category) == i)
                {
                    item.ChangeImageChoose();
                }

                int currentIndex = i; // Capture the current index for the lambda
                item.OnClick = () =>
                {
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

                    m_CurrentItemIndex = m_ItemOptions.IndexOf(itemPath);
                    m_SwapperItem.Text = itemName;
                    OnChangeItem?.Invoke(CurrentCategory, itemPath);

                    // Turn off highlight colors
                    foreach (var matItem in m_MaterialItems)
                    {
                        matItem.ResetAllColorHighlights();
                    }

                    // Apply stored colors for the newly selected item
                    ApplyStoredColors(category, currentIndex);
                };
            }

            // Deactivate unused items
            for (int i = m_ItemOptions.Count; i < m_ImageItems.Count; i++)
            {
                m_ImageItems[i].gameObject.SetActive(false);
            }

            //item swapping
            if (m_CurrentItemIndex >= m_ItemOptions.Count)
            {
                m_SwapperItem.gameObject.SetActive(false);
                return;
            }

            m_SwapperItem.Text = m_ItemOptions[m_CurrentItemIndex];
            m_SwapperItem.gameObject.SetActive(false); // Always keep SwapperItem hidden
        }



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

                    // Skip if current category is head and material is in hidden list
                    if (CurrentCategory.ToLower() == "head" &&
                        hiddenHeadMaterials.Contains(sharedMaterials[j].name))
                        continue;

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
                if (i < m_MaterialItems.Count)
                {
                    item = m_MaterialItems[i];
                }
                else
                {
                    // Instantiate and handle nested prefab items
                    item = Instantiate(m_MaterialItemPrefab, m_ColorPanel.transform);

                    // Deactivate any child UIMaterialItems that might have been created from the prefab
                    var childMaterialItems = item.GetComponentsInChildren<UIMaterialItem>(true);
                    foreach (var childItem in childMaterialItems)
                    {
                        if (childItem != item)
                        {
                            childItem.gameObject.SetActive(false);
                        }
                    }

                    m_MaterialItems.Add(item);
                }
                item.gameObject.SetActive(i == 0);

                //setup item
                item.Title = materials[i].name;
                m_MaterialOptions.Add(materials[i].name); // Store material names
                var auxRenderer = renderersPerMaterial[i];
                var auxMatIndex = rendererMaterialIndex[i];
                var auxPropertyBlock = propertyBlock[i];

                // Skip color setup for head face materials
                if (CurrentCategory.ToLower() == "head" &&
                    (materials[i].name.Contains("mat_base_M_face") || materials[i].name.Contains("mat_base_F_face")))
                {
                    continue;
                }

                if (materials[i].HasProperty("_MaskRemap"))
                {
                    item.ResetColors();
                    string materialName = materials[i].name;

                    // Set fixed skin color for skin category
                    Color fixedSkinColor = new Color(
                        185f / 255f,  // B9 in hex
                        71f / 255f,   // 47 in hex
                        45f / 255f,   // 2D in hex
                        1f
                    );

                    if (CurrentCategory.ToLower() == "skin")
                    {
                        item.AddDoubleColor(
                            auxPropertyBlock.HasColor("_Color_A_2") ? auxPropertyBlock.GetColor("_Color_A_2") : materials[i].GetColor("_Color_A_2"),
                            fixedSkinColor,  // Use fixed color for skin category
                            (c, buttonIndex) =>
                            {
                                OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_A_2", c);
                                TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_A_2", buttonIndex);
                            },
                            (c, buttonIndex) =>
                            {
                                // Do nothing - color A1 is fixed for skin
                            }
                        );
                    }
                    else
                    {
                        item.AddDoubleColor(
                            auxPropertyBlock.HasColor("_Color_A_2") ? auxPropertyBlock.GetColor("_Color_A_2") : materials[i].GetColor("_Color_A_2"),
                            auxPropertyBlock.HasColor("_Color_A_1") ? auxPropertyBlock.GetColor("_Color_A_1") : materials[i].GetColor("_Color_A_1"),
                            (c, buttonIndex) =>
                            {
                                OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_A_2", c);
                                TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_A_2", buttonIndex);
                            },
                            (c, buttonIndex) =>
                            {
                                OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_A_1", c);
                                TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_A_1", buttonIndex);
                            }
                        );
                    }

                    if (CurrentCategory.ToLower() != "hairstyle" && CurrentCategory.ToLower() != "top" && CurrentCategory.ToLower() != "bottom" && CurrentCategory.ToLower() != "skin")
                    {
                        item.AddDoubleColor(
                            auxPropertyBlock.HasColor("_Color_B_2") ? auxPropertyBlock.GetColor("_Color_B_2") : materials[i].GetColor("_Color_B_2"),
                            auxPropertyBlock.HasColor("_Color_B_1") ? auxPropertyBlock.GetColor("_Color_B_1") : materials[i].GetColor("_Color_B_1"),
                            (c, buttonIndex) =>
                            {
                                OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_B_2", c);
                                TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_B_2", buttonIndex);
                            },
                            (c, buttonIndex) =>
                            {
                                OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_B_1", c);
                                TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_B_1", buttonIndex);
                            }
                        );
                    }

                    if (CurrentCategory.ToLower() != "skin" && CurrentCategory.ToLower() != "hairstyle")
                    {
                        item.AddDoubleColor(
                            auxPropertyBlock.HasColor("_Color_C_2") ? auxPropertyBlock.GetColor("_Color_C_2") : materials[i].GetColor("_Color_C_2"),
                            auxPropertyBlock.HasColor("_Color_C_1") ? auxPropertyBlock.GetColor("_Color_C_1") : materials[i].GetColor("_Color_C_1"),
                            (c, buttonIndex) =>
                            {
                                OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_C_2", c);
                                TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_C_2", buttonIndex);
                            },
                            (c, buttonIndex) =>
                            {
                                OnChangeColor?.Invoke(auxRenderer, auxMatIndex, "_Color_C_1", c);
                                TrackColorSelection(CurrentCategory, m_CurrentItemIndex, materialName, "Color_C_1", buttonIndex);
                            }
                        );
                    }
                }
            }

            if (m_MaterialOptions.Count > 0)
            {
                m_CurrentMaterialIndex = 0;
                m_SwapMaterial.Text = m_MaterialOptions[m_CurrentMaterialIndex];
                m_SwapMaterial.gameObject.SetActive(m_MaterialOptions.Count > 1);
            }
            else
            {
                m_SwapMaterial.gameObject.SetActive(false);
            }

            for (int i = materials.Count; i < m_MaterialItems.Count; i++)
                m_MaterialItems[i].gameObject.SetActive(false);
            m_ColorPicker.Close();

            ShowMaterialDetails(0);
        }

        private void _OnSwapPrevious()
        {
            m_CurrentItemIndex = m_CurrentItemIndex - 1;
            if (m_CurrentItemIndex < 0)
                m_CurrentItemIndex = m_ItemOptions.Count - 1;
            m_SwapperItem.Text = m_ItemOptions[m_CurrentItemIndex];

            OnChangeItem?.Invoke(CurrentCategory, m_ItemOptions[m_CurrentItemIndex]);
        }

        private void _OnSwapNext()
        {
            m_CurrentItemIndex = (m_CurrentItemIndex + 1) % m_ItemOptions.Count;
            m_SwapperItem.Text = m_ItemOptions[m_CurrentItemIndex];

            OnChangeItem?.Invoke(CurrentCategory, m_ItemOptions[m_CurrentItemIndex]);
        }

        private void _OnSwapPreviousMaterial()
        {
            if (m_MaterialOptions == null || m_MaterialOptions.Count == 0)
                return;

            m_CurrentMaterialIndex = m_CurrentMaterialIndex - 1;
            if (m_CurrentMaterialIndex < 0)
                m_CurrentMaterialIndex = m_MaterialOptions.Count - 1;

            m_SwapMaterial.Text = m_MaterialOptions[m_CurrentMaterialIndex];

            ShowMaterialDetails(m_CurrentMaterialIndex);
        }

        private void _OnSwapNextMaterial()
        {
            if (m_MaterialOptions == null || m_MaterialOptions.Count == 0)
                return;

            m_CurrentMaterialIndex = (m_CurrentMaterialIndex + 1) % m_MaterialOptions.Count;
            m_SwapMaterial.Text = m_MaterialOptions[m_CurrentMaterialIndex];

            // Display the corresponding material settings
            ShowMaterialDetails(m_CurrentMaterialIndex);
        }

        public override void ShowMaterialDetails(int materialIndex)
        {
            if (materialIndex < 0 || materialIndex >= m_MaterialItems.Count)
                return;
            foreach (var item in m_MaterialItems)
            {
                item.gameObject.SetActive(false);
            }
            m_MaterialItems[materialIndex].gameObject.SetActive(true);
        }

        protected virtual void OnSaveButtonClicked()
        {
            if (customizationDemo != null)
            {
                customizationDemo.SaveAllToPrefs();
            }
        }
        #endregion
    }
}