using System;
using System.Collections.Generic;
using Fusion.Menu;
using Rukha93.ModularAnimeCharacter.Customization;
using Rukha93.ModularAnimeCharacter.Customization.UI;
using UnityEngine;
using UnityEngine.UI;

namespace Edu_World
{
    public abstract class BaseUICustomization : PhotonMenuUIScreen
    {
        [Header("category")]
        [SerializeField] protected LayoutGroup m_CategoryPanel;
        [SerializeField] protected UICategoryItem m_CategoryItemPrefab;
        [SerializeField] protected List<Sprite> m_CategoryIcons;

        [Header("customization")]
        [SerializeField] protected Text m_CustomizationTitle;
        [SerializeField] protected GameObject m_CustomizationPanel;
        [SerializeField] protected LayoutGroup m_ColorPanel;
        [SerializeField] protected UISwapperItem m_SwapperItem;
        [SerializeField] protected UISwapMaterial m_SwapMaterial;
        [SerializeField] protected UIMaterialItem m_MaterialItemPrefab;



        [Header("Image Item")]
        [SerializeField] protected UIImageItem m_ImageItemPrefab;
        [SerializeField] protected LayoutGroup m_ImageItemPanel;
        [SerializeField] protected GameObject ScrollViewPanel;

        [Space]
        [SerializeField] protected UIColorPicker m_ColorPicker;

        [Header("Save Button")]
        [SerializeField] protected Button saveButton;
        protected CustomizationDemo customizationDemo;

        public string CurrentCategory { get; set; }


        public List<UICategoryItem> m_CategoryItems = new List<UICategoryItem>();
        protected List<UIMaterialItem> m_MaterialItems = new List<UIMaterialItem>();
        protected List<UIImageItem> m_ImageItems = new List<UIImageItem>();

        protected List<string> m_ItemOptions;
        protected int m_CurrentItemIndex;
        protected int m_CurrentMaterialIndex;
        protected List<string> m_MaterialOptions;

        public System.Action<string> OnClickCategory;
        public System.Action<string, string> OnChangeItem;
        public System.Action<Renderer, int, string, Color> OnChangeColor;



        [SerializeField] private FakeLoader m_FakeLoader; // FakeLoader để lấy ảnh


        protected readonly string[] hiddenHeadMaterials = new string[]
        {
            "mat_base_M_face",
            "mat_base_F_face",
            "mat_eyelashes",
            "mat_eye_highlight",
            "mat_eyebrows",
            "mat_base_mouth"
        };

        protected string lastSelectedCategory;

        protected List<CategorySelection> selectedItems = new List<CategorySelection>();

        protected UICategoryItem lastSelectedCategoryItem;

        protected SelectionHistoryManager selectionManager = new SelectionHistoryManager();

        public bool IsCustomizationOpen => m_ColorPanel.gameObject.activeSelf;




        public virtual void ResetAndApplyStoredColorsForCategory(string category) { }

        public virtual void SetCategories(string[] categories) { }
        public virtual void SetCategoryValue(int category, string value) { }

        public virtual void SetCustomizationOptions(string category, string[] items, string currentItem, Dictionary<string, CustomizationItemAsset> itemAssets) { }

        public virtual void SetCustomizationMaterials(Renderer[] renderers) { }

        public virtual int GetSelectedIndexForCategory(string category)
        {
            return 0;
        }
        public virtual void SetSelectedIndexForCategory(string category, int index) { }

        public virtual void ShowMaterialDetails(int materialIndex) { }

        public virtual void ShowCustomization(bool show){}
    }
}
