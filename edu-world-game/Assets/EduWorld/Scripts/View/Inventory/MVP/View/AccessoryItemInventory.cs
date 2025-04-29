using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;

namespace Edu_World.Inventory.MVP
{
    public class AccessoryItemInventory : BaseItemInventory
    {
        [SerializeField]
        private TextMeshProUGUI quantityText;

        public override void SetData(BaseItem item)
        {
            base.SetData(item);
            if (item is AccessoryModel accessory && quantityText != null)
            {
                quantityText.text = $"x{accessory.quantity}";
            }
        }

        public void SimulateClick()
        {
            if (currentItem != null && currentItem is AccessoryModel accessory)
            {
                InventoryManager.Instance.ShowAccessoryInfo(accessory);
            }
        }

        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);
            SimulateClick();
        }
    }
}
