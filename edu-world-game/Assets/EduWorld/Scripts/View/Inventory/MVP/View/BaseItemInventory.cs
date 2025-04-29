using UnityEngine;
using TMPro;
using UnityEngine.EventSystems;
using System;

namespace Edu_World.Inventory.MVP
{
    public abstract class BaseItemInventory : MonoBehaviour, IPointerClickHandler
    {
        [SerializeField]
        protected TextMeshProUGUI itemNameText;
        [SerializeField] protected ItemCategory itemCategory;
        protected BaseItem currentItem;
        void Start()
        {
            InventoryManager.Instance.OnCategorySelected += OnCategorySelected;
            gameObject.SetActive(false);
        }

        public virtual void SetData(BaseItem item)
        {
            currentItem = item;
            if (itemNameText != null)
            {
                itemNameText.text = item.name;
            }
        }

        
        public virtual void OnCategorySelected(string name)
        {
            gameObject.SetActive(name.Equals(itemCategory.ToString(), StringComparison.OrdinalIgnoreCase));
        }

        public virtual void OnPointerClick(PointerEventData eventData)
        {
        }
    }
}
