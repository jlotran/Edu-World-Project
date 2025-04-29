using System.Collections.Generic;
using Edu_World.Inventory.MVP;
using UnityEngine;

namespace Edu_World
{
    // public enum ItemCategory
    // {
    //     Car,
    //     Outfit,
    //     Accessories,
    //     Pets,
    //     Furniture
    // }

    public enum OutfitSubcategory
    {
        Hairstyle,
        Top,
        Bottom,
        Shoes,
    }
    // public interface IInventoryItem
    // {
    //     string Id { get; }
    //     string Name { get; }
    //     ItemCategory Category { get; }
    //     string Subcategory { get; } // optional
    // }
    [System.Serializable]
    public class InventoryModelBase
    {
        public string Id;
        public string Name;
        public ItemCategory Category;
        public string Subcategory;

        public InventoryModelBase(string id, string name, ItemCategory category, string subcategory)
        {
            Id = id;
            Name = name;
            Category = category;
            Subcategory = subcategory;
        }
    }
    [System.Serializable]
    public class CarModel : InventoryModelBase
    {
        public float Speed { get; set; }
        public float Handling { get; set; }
        public float Acceleration { get; set; }
        public float Nitro { get; set; }
        public string ModelPath { get; set; } // Path to the car model prefab

        public CarModel(string id, string name, float speed, float handling, float acceleration, float nitro, string modelPath) : base(id, name, ItemCategory.Car, null)
        {
            Speed = speed;
            Handling = handling;
            Acceleration = acceleration;
            Nitro = nitro;
            ModelPath = modelPath;
        }
    }
    [System.Serializable]
    public class OutfitModel : InventoryModelBase
    {
        public OutfitSubcategory SubcategoryType { get; set; }
        public OutfitModel(string id, string name, OutfitSubcategory subcategoryType) : base(id, name, ItemCategory.Outfit, subcategoryType.ToString())
        {
            SubcategoryType = subcategoryType;
        }
    }
    [System.Serializable]
    public class AccessoriesModel : InventoryModelBase
    {
        public string Description { get; set; }
        public float Price { get; set; }
        public int Quantity { get; set; }
        public AccessoriesModel(string id, string name, string description, float price, int Quantity) : base(id, name, ItemCategory.Accessory, null)
        {
            Description = description;
            Price = price;
            this.Quantity = Quantity;
        }

    }
    [System.Serializable]
    public class InventoryData
    {
        public List<CarModel> cars;
        public List<OutfitModel> outfits;
        public List<AccessoriesModel> accessories;

        public InventoryData()
        {
            cars = new List<CarModel>();
            outfits = new List<OutfitModel>();
            accessories = new List<AccessoriesModel>();
        }
    }
}
