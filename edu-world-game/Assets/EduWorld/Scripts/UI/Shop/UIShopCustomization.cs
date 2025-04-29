using System.Collections.Generic;
using System.Linq;
using Fusion.Menu;
using Rukha93.ModularAnimeCharacter.Customization;
using Rukha93.ModularAnimeCharacter.Customization.UI;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Edu_World
{
    public class UIShopCustomization : PhotonMenuUIScreen
    {
        private UICustomizationDemo uICustomizationDemo;
        [Header("category")]
        [SerializeField] private LayoutGroup m_CategoryPanel;
        [SerializeField] private UICategoryItem m_CategoryItemPrefab;
        [SerializeField] private List<Sprite> m_CategoryIcons;
        [SerializeField] private TextMeshProUGUI m_CustomizationTitle;


        [Header("Image Item")]
        [SerializeField] private UIImageItem m_ImageItemPrefab;
        [SerializeField] private LayoutGroup m_ImageItemPanel;
        [SerializeField] private GameObject ScrollViewPanel;


        private List<UIImageItem> m_ImageItems = new List<UIImageItem>();
        private List<UIMaterialItem> m_MaterialItems = new List<UIMaterialItem>();
        private List<string> m_ItemOptions;
        private int m_CurrentItemIndex;
        private int m_CurrentMaterialIndex;
        private List<string> m_MaterialOptions; 
        


        public System.Action<string> OnClickCategory;
        public System.Action<string, string> OnChangeItem;



        public string CurrentCategory { get; private set; }

        public List<UICategoryItem> m_CategoryItems = new List<UICategoryItem>();


        private string lastSelectedCategory;

        private List<CategorySelection> selectedItems = new List<CategorySelection>();

        private UICategoryItem lastSelectedCategoryItem;

        private SelectionHistoryManager selectionManager = new SelectionHistoryManager();


        void Awake()
        {
            m_CategoryItemPrefab.gameObject.SetActive(false);
            m_ImageItemPrefab.gameObject.SetActive(false);

        }

        public void SetCategories(string[] categories)
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
                    if (lastSelectedCategoryItem != null && lastSelectedCategoryItem != item)
                    {
                        lastSelectedCategoryItem.ResetToLastCategory();
                    }
                    lastSelectedCategoryItem = item;

                    if (aux.ToLower() == "skin")
                    {
                        // ScrollViewPanel.SetActive(false);
                        OnClickCategory?.Invoke(aux);
                    }
                    else if (lastSelectedCategory == aux)
                    {
                        bool isCurrentlyVisible = ScrollViewPanel.activeSelf;
                        // ScrollViewPanel.SetActive(!isCurrentlyVisible);

                    }
                    else
                    {
                        // ScrollViewPanel.SetActive(true);
                        OnClickCategory?.Invoke(aux);
                    }
                    lastSelectedCategory = aux;
                };
            }

            for (int i = categories.Length; i < m_CategoryItems.Count; i++)
                m_CategoryItems[i].gameObject.SetActive(false);
        }

        public void SetCategoryValue(int index, string value)
        {
            UICategoryItem item = m_CategoryItems[index];
            // item.Value = value;
        }


        public void SetCustomizationOptions(string category, string[] items, string currentItem, Dictionary<string, CustomizationItemAsset> itemAssets)
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

                // Get proper display name (remove path components)
                string itemName = m_ItemOptions[i];
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

                // Restore previously selected item for this category
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
                    // m_SwapperItem.Text = itemName;
                    OnChangeItem?.Invoke(CurrentCategory, itemPath);

                    // Turn off highlight colors
                    // foreach (var matItem in m_MaterialItems)
                    // {
                    //     matItem.ResetAllColorHighlights();
                    // }

                    // Apply stored colors for the newly selected item
                    // ApplyStoredColors(category, currentIndex);
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
        private int GetSelectedIndexForCategory(string category)
        {
            return CategorySelection.GetSelectedIndexForCategory(selectedItems, category);
        }

        private void SetSelectedIndexForCategory(string category, int index)
        {
            CategorySelection.SetSelectedIndexForCategory(selectedItems, category, index);
        }


                public void SetCustomizationMaterials(Renderer[] renderers)
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

            for (int i = materials.Count; i < m_MaterialItems.Count; i++)
                m_MaterialItems[i].gameObject.SetActive(false);
            // m_ColorPicker.Close();

            ShowMaterialDetails(0);
        }
                private void ShowMaterialDetails(int materialIndex)
        {
            if (materialIndex < 0 || materialIndex >= m_MaterialItems.Count)
                return;
            foreach (var item in m_MaterialItems)
            {
                item.gameObject.SetActive(false);
            }
             m_MaterialItems[materialIndex].gameObject.SetActive(true);
        }

    }
}
