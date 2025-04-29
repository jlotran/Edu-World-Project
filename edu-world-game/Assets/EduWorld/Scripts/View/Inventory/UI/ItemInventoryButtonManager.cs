using System.Collections.Generic;
using Edu_World.Inventory.MVP;
using UnityEngine;
using System.Collections;

namespace Edu_World.Inventory.UI
{
    public class ItemInventoryButtonManager : MonoBehaviour
    {
        private List<BaseItemInventory> itemButtons = new List<BaseItemInventory>();

        private List<CarInventoryItem> carInventoryItems = new List<CarInventoryItem>();
        private List<AccessoryItemInventory> accessoryInventoryItems = new List<AccessoryItemInventory>();

        public void AddCarInventoryItem(CarInventoryItem item)
        {
            if (item != null)
            {
                carInventoryItems.Add(item);
                itemButtons.Add(item);
            }
        }

        public void AddAccessoryInventoryItem(AccessoryItemInventory item)
        {
            if (item != null)
            {
                accessoryInventoryItems.Add(item);
                itemButtons.Add(item);
            }
        }

        public void OnCategorySelected(string categoryName)
        {
            if (categoryName.Equals(ItemCategory.Car.ToString()) && carInventoryItems.Count > 0)
            {
                carInventoryItems[0].SimulateClick();
            }
            else if (categoryName.Equals(ItemCategory.Accessory.ToString()) && accessoryInventoryItems.Count > 0)
            {
                accessoryInventoryItems[0].SimulateClick();
            }
        }
    }
}
