using UnityEngine;
using UnityEngine.EventSystems;

namespace Edu_World.Inventory.MVP
{
    public class CarInventoryItem : BaseItemInventory
    {
        private GameObject carModel;

        public void SetCarModel(GameObject model)
        {
            carModel = model;
            if (carModel != null)
            {
                carModel.SetActive(false); // Hide by default
            }
        }
        public override void SetData(BaseItem item)
        {
            base.SetData(item);
            CarModel carItem = item as CarModel;
            // Debug.Log($"CarItem: {carItem.name}, Top Speed: {carItem.topSpeed}, Handling: {carItem.handling}, Acceleration: {carItem.acceleration}, Nitro: {carItem.nitro}");
        }

        public void SimulateClick()
        {
            var container = carModel.transform.parent;
            foreach (Transform child in container)
            {
                child.gameObject.SetActive(false);
            }
            if (carModel != null)
            {
                carModel.SetActive(true);
            }

            if (currentItem != null && currentItem is CarModel car)
            {
                InventoryManager.Instance.ShowCarInformation(car);
            }
        }


        public override void OnPointerClick(PointerEventData eventData)
        {
            base.OnPointerClick(eventData);

            // Hide all other car models in the container
            SimulateClick();
        }
    }
}
