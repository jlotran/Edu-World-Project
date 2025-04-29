using UnityEngine;
using System.Collections.Generic;
using System.Linq;

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{
    public struct CategorySelection
    {
        public string categoryName;
        public int selectedIndex;

        public CategorySelection(string category, int index)
        {
            categoryName = category;
            selectedIndex = index;
        }

        public static int GetSelectedIndexForCategory(List<CategorySelection> selectedItems, string category)
        {
            var selection = selectedItems.FirstOrDefault(x => x.categoryName == category);
            return selection.categoryName == null ? -1 : selection.selectedIndex;
        }

        public static void SetSelectedIndexForCategory(List<CategorySelection> selectedItems, string category, int index)
        {
            var existingIndex = selectedItems.FindIndex(x => x.categoryName == category);
            if (existingIndex != -1)
            {
                selectedItems[existingIndex] = new CategorySelection(category, index);
            }
            else
            {
                selectedItems.Add(new CategorySelection(category, index));
            }
        }
    }
}
