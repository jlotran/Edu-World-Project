using System.Collections.Generic;
using UnityEngine;

namespace Edu_World.Inventory.UI
{
    using System;
    using Edu_World.Inventory.MVP;
    public class CategoryButtonsManager : MonoBehaviour
    {
        [Header("List Category")]
        [SerializeField] private CategoryButton buttonCategoyPrefab;
        [SerializeField] private Transform categoryButtonContainer;
        private List<CategoryButton> categoryButtons = new List<CategoryButton>();
        private CategoryButton currentSelectedButton;

        public void Start()
        {
            InventoryManager.Instance.OnCategorySelected += OnCategorySelected;
        }


        public void InitData(List<Category> categories)
        {
            // Clear existing buttons if any
            foreach (var button in categoryButtons)
            {
                if (button != null)
                    Destroy(button.gameObject);
            }
            categoryButtons.Clear();

            // Create new buttons
            foreach (var category in categories)
            {
                // Skip Pet and Furniture categories
                if (category.name.Equals(ItemCategory.Pets.ToString()) || category.name.Equals(ItemCategory.Furniture.ToString()))
                    continue;
                    
                CategoryButton button = Instantiate(buttonCategoyPrefab, categoryButtonContainer);
                button.InitData(category);
                categoryButtons.Add(button);
            }

            // // Delay the initial selection to ensure all components are initialized
        }
        private void OnCategorySelected(string model)
        {
            foreach (var button in categoryButtons)
            {
                if (button.CategoryName == model)
                {
                    // Deselect previous button if exists
                    if (currentSelectedButton != null && currentSelectedButton != button)
                    {
                        currentSelectedButton.TransitionToNormal();
                    }

                    // Select new button
                    currentSelectedButton = button;
                    button.HandleSelection();
                    break;
                }
            }
        }

        private void OnDestroy()
        {
            if (InventoryManager.Instance != null)
                InventoryManager.Instance.OnCategorySelected -= OnCategorySelected;
        }
    }
}
