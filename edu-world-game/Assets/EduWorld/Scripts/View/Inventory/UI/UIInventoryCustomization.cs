using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Edu_World.Inventory;
using Edu_World.Inventory.MVP;
using Lam.FUSION;
using Rukha93.ModularAnimeCharacter.Customization;
using Rukha93.ModularAnimeCharacter.Customization.UI;
using UnityEngine;

namespace Edu_World
{
    public class UIInventoryCustomization : BaseUICustomization
    {
        private bool shouldShowImageItems = false;


        public override void Start()
        {
            InventoryManager.Instance.OnCategorySelected += OnCategorySelected;
            InventoryManager.Instance.OnCloseInventory += CloseInventory;
            return;
        }

        private void OnCategorySelected(string category)
        {
            SetImageItemsVisibility(category == ItemCategory.Outfit.ToString());
        }

        private void CloseInventory(){
            SetImageItemsVisibility(false);
            ResetAllCategories();
            ClearSelectionHistory();
            CustomizationDemo.localCustomizeCharacter.ReloadSavedEquipment();
            
            // Select hairstyle category by default
            string defaultCategory = "hairstyle";
            foreach (var categoryItem in m_CategoryItems)
            {
                if (categoryItem.Title.ToLower().Equals(defaultCategory.ToLower()))
                {
                    categoryItem.OnClick?.Invoke();
                    lastSelectedCategoryItem = categoryItem;
                    lastSelectedCategory = defaultCategory;
                }
                // else
                // {
                //     categoryItem.setNormal();
                // }
                // {
                //     categoryItem.OnClick?.Invoke();
                //     break;
                // }
            }
            
            CustomizationDemo.localCustomizeCharacter.SelectAfterLoad(defaultCategory);
        } 

        public void InitializeUI()
        {
            CustomizationDemo.localCustomizeCharacter.SetUI(this);
            // Set up the UI elements and event listeners
            OnClickCategory += CustomizationDemo.localCustomizeCharacter.OnSelectCategory;
            OnChangeItem += CustomizationDemo.localCustomizeCharacter.OnSwapItem;
            OnChangeColor += CustomizationDemo.localCustomizeCharacter.OnChangeColor;

            // Set categories like in Start()
            SetCategories(CustomizationDemo.m_Categories.ToArray());
            for (int i = 0; i < CustomizationDemo.m_Categories.Count; i++)
                SetCategoryValue(i, "");
            StartCoroutine(CustomizationDemo.localCustomizeCharacter.SelectAfterLoad("hairstyle"));
        }

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
                    // item.SetupBackgroundForShop(categories[i].ToLower());
                    // if (categories[i].ToLower() == "hairstyle")
                    // {
                    //     lastSelectedCategoryItem = item;
                    //     lastSelectedCategory = categories[i];
                    // }
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

            // Reset existing image items
            for (int i = 0; i < m_ImageItems.Count; i++)
            {
                m_ImageItems[i].ResetToLastImage();
            }

            // Use all items without skipping
            m_ItemOptions = new List<string>(items.Skip(1));


            m_CurrentItemIndex = Mathf.Max(m_ItemOptions.IndexOf(currentItem), 0);

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

                // Set initial visibility based on shouldShowImageItems
                item.gameObject.SetActive(shouldShowImageItems);

                string itemPath = m_ItemOptions[i];
                string itemName = m_ItemOptions[i];

                item.SetTitle(itemName);
                item.SetUpBackgroundImageItem();

                // Set icon if available
                if (itemAssets != null && itemAssets.ContainsKey(itemPath))
                {
                    var asset = itemAssets[itemPath];
                    if (asset != null)
                    {
                        item.Setup(asset.icon, asset.price);
                    }
                }

                // Restore previously selected item for this category
                if (GetSelectedIndexForCategory(category) == i)
                {
                    item.ChangeImageChoose();
                }

                int currentIndex = i;
                item.OnClick = () =>
                {
                    // Check if clicking the same item
                    if (GetSelectedIndexForCategory(category) == currentIndex)
                    {
                        return;
                    }

                    // Reset previous selection
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
                    OnChangeItem?.Invoke(CurrentCategory, itemPath);
                };
            }

            // Deactivate unused items
            for (int i = m_ItemOptions.Count; i < m_ImageItems.Count; i++)
            {
                m_ImageItems[i].gameObject.SetActive(false);
            }
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

        public void SetImageItemsVisibility(bool visible)
        {
            shouldShowImageItems = visible;
            if (m_ImageItems != null)
            {
                for (int i = 0; i < m_ImageItems.Count; i++)
                {
                    if (m_ImageItems[i] != null)
                    {
                        // Only show items up to the number of options in current category
                        bool shouldShow = visible && (m_ItemOptions != null && i < m_ItemOptions.Count);
                        m_ImageItems[i].gameObject.SetActive(shouldShow);
                    }
                }
            }
        }

        public void ResetAllCategories()
        {
            foreach (var categoryItem in m_CategoryItems)
            {
                if (categoryItem != null)
                {
                    categoryItem.setNormal();
                }
            }
            foreach (var imageItem in m_ImageItems)
            {
                if (imageItem != null)
                {
                    imageItem.SetNormalSprite();
                }
            }
        }

        public void ClearSelectionHistory()
        {
            selectedItems.Clear(); // Clear the selection history
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
    #endregion
}