using UnityEngine;
using UnityEngine.UI;

namespace Edu_World.Inventory.UI
{
    using System;
    using System.Collections.Generic;
    using Edu_World.Inventory.MVP;
    using TMPro;

    public class CategoryButton : UIButtonSelection
    {
        [Header("Category Button")]
        [SerializeField] private Button button;
        [SerializeField] private TMP_Text textCategory;

        private string categoryName;
        public string CategoryName => categoryName;

        public void InitData(Category Data)
        {
            categoryName = Data.name;
            textCategory.text = Data.name;
            button.onClick.AddListener(() =>
            {
                OnButtonClick(Data.name);
            });
        }

        public void OnButtonClick(string name)
        {
            string type = $"{name}Model";
            Type modelType = Type.GetType(type);
            InventoryManager.Instance.CategorySelected(name);
        }
    }
}
