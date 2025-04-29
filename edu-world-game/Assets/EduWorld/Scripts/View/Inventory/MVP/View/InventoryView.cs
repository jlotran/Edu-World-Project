using System.Collections.Generic;
using UnityEngine;

namespace Edu_World.Inventory.MVP
{
    using System;
    using EduWorld;
    using UnityEngine.EventSystems;

    public class InventoryView : MonoBehaviour
    {
        private InventoryPresenter _presenter;

        [SerializeField] private Transform itemPrefabContainer;

        [SerializeField] private List<BaseItemInventory> itemInventoriesPref;

        public event Action<string> categorySelected;

        private void Start()
        {
            _presenter = new InventoryPresenter(this);
            _presenter.InitData();
        }


        public void InitListCategoryView(List<Category> categories)
        {
            InventoryManager.Instance.SetCategories(categories);

            foreach (Category category in categories)
            {
                string catName = category.name;
                object dataCategory = _presenter.getData(catName);
                if (dataCategory is BaseCategory categoryData)
                {
                    Debug.Log($"Category data of type {catName} found.");
                    categoryData.CreatePrefabs(itemPrefabContainer);
                }
                else
                {
                    Debug.LogWarning($"Category data of type {catName} not found.");
                }
            }
        }
        public BaseItemInventory GetPrefabByCategoryModel<T>() where T : BaseItemInventory
        {
            foreach (var item in itemInventoriesPref)
            {
                if (item is T)
                {
                    return item;
                }
            }
            Debug.LogWarning($"Prefab of type {typeof(T)} not found in itemInventoriesPref.");
            return null;
        }

        // public void ClearAllItems()
        // {
        //     foreach (var prefabs in categoryPrefabs.Values)
        //     {
        //         foreach (var prefab in prefabs)
        //         {
        //             prefab.SetActive(false);
        //         }
        //     }
        // }

        // private void OnDestroy()
        // {
        //     // Cleanup when scene is destroyed
        //     foreach (var prefabs in categoryPrefabs.Values)
        //     {
        //         foreach (var prefab in prefabs)
        //         {
        //             if (prefab != null)
        //                 Destroy(prefab);
        //         }
        //     }
        //     categoryPrefabs.Clear();
        // }
    }
}
