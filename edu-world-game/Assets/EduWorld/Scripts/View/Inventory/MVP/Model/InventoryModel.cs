using System.Collections.Generic;
using EduWorld;
using Unity.VisualScripting;
using UnityEngine;

namespace Edu_World.Inventory.MVP
{

    public enum ItemCategory
    {
        Car,
        Outfit,
        Accessory,
        Pets,
        Furniture
    }
    [System.Serializable]
    public class Category
    {
        public string name;
        public object details;
    }

    [System.Serializable]
    public class BaseItem
    {
        public string id;
        public string name;
    }

    [System.Serializable]
    public class CarModel : BaseItem
    {
        public float topSpeed;
        public float handling;
        public float acceleration;
        public float nitro;
        public string modelPath;
    }

    [System.Serializable]
    public class AccessoryModel : BaseItem
    {
        public string description;
        public float price;
        public int quantity;
    }

    [System.Serializable]
    public class OutfitModel : BaseItem
    {
    }

    public class InventoryModel
    {
        public List<Category> categories = new List<Category>();
    }

    public abstract class BaseCategory
    {
        public abstract void ShowData();
        public abstract void CreatePrefabs(Transform container);

        protected void SetupItemPrefab(BaseItemInventory prefab, BaseItem item)
        {
            var itemComponent = prefab.GetComponent<BaseItemInventory>();
            if (itemComponent != null)
            {
                itemComponent.SetData(item);
            }
        }
    }

    public class CarCategory : BaseCategory
    {
        public Transform carModelsContainer;
        public List<CarModel> items = new List<CarModel>();

        public override void ShowData()
        {
            InventoryManager.Instance.ShowInventory<CarModel>(items);
        }

        public override void CreatePrefabs(Transform container)
        {
            BaseItemInventory prefab = InventoryManager.Instance.GetBaseItemInventoryPref<CarInventoryItem>();

            if (carModelsContainer == null)
            {
                GameObject containerObj = new GameObject("CarModelsContainer");
                carModelsContainer = containerObj.transform;
                carModelsContainer.position = new Vector3(1000f, 1000f, 1000f);
            }

            foreach (CarModel car in items)
            {
                BaseItemInventory carItem = GameObject.Instantiate(prefab, container);

                SetupItemPrefab(carItem, car);

                GameObject carModelPrefab = Resources.Load<GameObject>($"{car.modelPath}");
                if (carModelPrefab != null)
                {
                    GameObject carModel = GameObject.Instantiate(carModelPrefab, carModelsContainer);
                    carModel.transform.localPosition = Vector3.zero;
                    carModel.transform.localRotation = Quaternion.identity;
                    carModel.SetActive(false);

                    if (carItem != null)
                    {
                        CarInventoryItem carInventoryItem = carItem as CarInventoryItem;
                        if (carInventoryItem != null)
                        {
                            carInventoryItem.SetCarModel(carModel);
                            InventoryManager.Instance.AddCarInventoryItem(carInventoryItem);
                        }
                    }
                }
            }
        }
    }

    public class AccessoryCategory : BaseCategory
    {
        public List<AccessoryModel> items = new List<AccessoryModel>();
        public override void ShowData()
        {
            InventoryManager.Instance.ShowInventory<AccessoryModel>(items);
        }
        public override void CreatePrefabs(Transform container)
        {
            AccessoryItemInventory accessoryPrefab = InventoryManager.Instance.GetBaseItemInventoryPref<AccessoryItemInventory>() as AccessoryItemInventory;

            foreach (AccessoryModel accessory in items)
            {
                AccessoryItemInventory accessoryItem = GameObject.Instantiate(accessoryPrefab, container);
                SetupItemPrefab(accessoryItem, accessory);
                InventoryManager.Instance.AddAccessoryInventoryItem(accessoryItem);
            }
        }
    }

    public class OutfitCategory : BaseCategory
    {
        public List<OutfitModel> subcategories = new List<OutfitModel>();

        public override void CreatePrefabs(Transform container)
        {

        }

        public override void ShowData()
        {
            return;
        }
    }
}
