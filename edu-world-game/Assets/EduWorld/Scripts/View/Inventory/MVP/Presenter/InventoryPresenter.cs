using System;
using System.Collections.Generic;
using UnityEngine;

namespace Edu_World.Inventory.MVP
{
    public class InventoryPresenter
    {
        private readonly InventoryView _view;
        private readonly InventoryService _service;

        public InventoryModel _data;

        public InventoryPresenter(InventoryView view)
        {
            _view = view;
            _service = new InventoryService();
        }

        public void InitData()
        {
            _data = _service.LoadFromPath("Assets/EduWorld/Scripts/View/Inventory/Model/inventory.json");
            _view.InitListCategoryView(_data.categories);
        }

        // type la BaseCategory
        public object getData(string categoryName)
        {
            Category category = _data.categories.Find(c => c.name == categoryName);
            if (category == null)
                return null;
            if(categoryName.Equals(ItemCategory.Pets.ToString(), StringComparison.OrdinalIgnoreCase) || categoryName.Equals(ItemCategory.Furniture.ToString(), StringComparison.OrdinalIgnoreCase))
                return null;
            Type type = Type.GetType($"Edu_World.Inventory.MVP.{categoryName}Category");
            return JsonUtility.FromJson(category.details.ToString(), type);
        }
    }

}
