using UnityEngine;
using System.Collections.Generic;

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{
    public class SelectionHistoryManager
    {
        private Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, int>>>> selectionHistory 
            = new Dictionary<string, Dictionary<int, Dictionary<string, Dictionary<string, int>>>>();

        public void TrackColorSelection(string category, int itemIndex, string materialName, string colorProperty, int buttonIndex)
        {
            // Initialize dictionaries if they don't exist
            if (!selectionHistory.ContainsKey(category))
            {
                selectionHistory[category] = new Dictionary<int, Dictionary<string, Dictionary<string, int>>>();
            }
            if (!selectionHistory[category].ContainsKey(itemIndex))
            {
                selectionHistory[category][itemIndex] = new Dictionary<string, Dictionary<string, int>>();
            }
            if (!selectionHistory[category][itemIndex].ContainsKey(materialName))
            {
                selectionHistory[category][itemIndex][materialName] = new Dictionary<string, int>();
            }

            // Store the color selection
            selectionHistory[category][itemIndex][materialName][colorProperty] = buttonIndex;

            // Debug log the selection
            // Debug.Log($"Updated selectionHistory: Category={category}, ItemIndex={itemIndex}, Material={materialName}, ColorProperty={colorProperty}, ButtonIndex={buttonIndex}");
        }

        public Dictionary<string, Dictionary<string, int>> GetMaterialColors(string category, int itemIndex)
        {
            if (selectionHistory.ContainsKey(category) && 
                selectionHistory[category].ContainsKey(itemIndex))
            {
                return selectionHistory[category][itemIndex];
            }
            return null;
        }
    }
}
