using System;
using System.Collections.Generic;

namespace Edu_World
{
    [Serializable]
    public class InventoryJsonModel
    {
        public List<Category> categories;
    }

    [Serializable]
    public class Category
    {
        public string categoryName;
        public List<Item> items;
        public List<Subcategory> subcategories;
    }

    [Serializable]
    public class Subcategory
    {
        public string name;
        public List<Item> items;
    }

    [Serializable]
    public class Item
    {
        public int id;
        public string name;
        public float price;
        public string description;
        // Car specific properties
        public float topSpeed;
        public float handling;
        public float acceleration;
        public float nitro;
        public string modelPath;

        //Accessories specific properties
        public int quantity;
    }
}
