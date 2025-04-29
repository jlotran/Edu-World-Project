using UnityEngine;
using System.IO;
using System;
using Newtonsoft.Json;

namespace Edu_World.Inventory.MVP
{
    public class InventoryService
    {
        public InventoryModel LoadFromPath(string fullPath)
        {
            Debug.Log($"Attempting to load inventory from: {fullPath}");
            if (!File.Exists(fullPath))
            {
                Debug.LogError($"Không tìm thấy file tại đường dẫn: {fullPath}");
                return null;
            }

            string json = File.ReadAllText(fullPath);
            var jsonModel = JsonConvert.DeserializeObject<InventoryModel>(json);

            if (jsonModel == null)
            {
                Debug.LogError("Failed to parse JSON data");
                return null;
            }

            // foreach (var category in jsonModel.categories)
            // {
            //     Debug.Log($"Category: {category.name}");
            //     Debug.Log($"Details: {category.details}");
            // }

            return jsonModel;
        }
    }
}
