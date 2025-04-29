using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Cinemachine;
using System.Collections.Generic;

namespace Edu_World
{
    public class CarItem : MonoBehaviour, IPointerClickHandler
    {
        private static CarItem currentSelectedItem;
        private static List<CarItem> allCarItems = new List<CarItem>();
        [SerializeField] private Image carImage;
        [SerializeField] private CarItemTweening tweening;
        private CarData carData;
        public System.Action<string, int, int, int, int, int> OnCarSelected;

        public int CarID => carData.carID;

        void Awake()
        {
            allCarItems.Add(this);
        }

        void OnDestroy()
        {
            allCarItems.Remove(this);
        }

        public void Initialize(CarData data)
        {
            carData = data;
            carImage.sprite = carData.carSprite;
            SetNormalState();
            
            // Select first item if no item is currently selected
            if (currentSelectedItem == null)
            {
                OnPointerClick(null);
            }
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            if (currentSelectedItem != null && currentSelectedItem != this)
            {
                currentSelectedItem.SetNormalState();
            }
            currentSelectedItem = this;
            SetSelectedState();
            
            // OnCarSelected?.Invoke(carData.carName, carData.topSpeed, carData.handling, carData.acceleration, carData.energy, carData.price);
        }
        public void Deselect()
        {
            if (currentSelectedItem == this)
            {
                currentSelectedItem = null;
            }
            SetNormalState();
        }

        public void UpdateSelectedCarById(int id)
        {
            CarIdHolder carIdHolder = this.GetComponent<CarIdHolder>();
            if (carIdHolder.carID == id)
            {
                if (currentSelectedItem != null && currentSelectedItem != this)
                {
                    currentSelectedItem.SetNormalState();
                }
                currentSelectedItem = this;
                SetSelectedState();
                // OnCarSelected?.Invoke(carData.carName, carData.topSpeed, carData.handling, carData.acceleration, carData.energy, carData.price);
            }
            else
            {
                SetNormalState();
            }
        }

        public static void UpdateAllCarItemsById(int id)
        {
            foreach (var carItem in allCarItems)
            {
                carItem.UpdateSelectedCarById(id);
            }
        }

        private void SetNormalState()
        {
            tweening.SetNormal();
        }
        private void SetSelectedState()
        {
            tweening.SetSelected();
        }
    }
}
