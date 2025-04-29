using System;
using System.Collections.Generic;
using UnityEngine;

namespace Edu_World
{
    public class CarPresenter
    {
        private readonly ICarView view;
        private List<CarData> carDataList;
        private int currentCarIndex = 0;
        PlayerInteract playerInteract;
        public CarPresenter(ICarView view)
        {
            this.view = view;
        }
        public CarPresenter(ICarView view, PlayerInteract playerInteract)
        {
            this.view = view;
            this.playerInteract = playerInteract;
            this.playerInteract.OnGetCarData.AddListener(OnGetCarData);
            
            // Load car data list from registry
            this.carDataList = CarSpawner.Instance?.GetAllCars();
            if (this.carDataList == null || this.carDataList.Count == 0)
            {
                Debug.LogError("Car data list is null or empty!");
            }
        }

        private void OnGetCarData(CarIdHolder carIdHolder)
        {
            if (carDataList == null)
            {
                Debug.LogError("Car data list is null when trying to get car data!");
                return;
            }
            this.view.ToggleUI(carIdHolder);
            CameraCar.instance.CameraOnOf();
        }


        public void Initialize(List<CarData> carDataList)
        {
            this.carDataList = carDataList;
            view.Initialize(carDataList);
            
            // if (carDataList != null && carDataList.Count > 0)
            // {
            //     CarData firstCar = carDataList[0];
            //     UpdateSelectedCar(firstCar.carName, firstCar.topSpeed, firstCar.handling, 
            //         firstCar.acceleration, firstCar.energy, firstCar.price);
            // }
        }

        public void UpdateSelectedCar(string carName, int topSpeed, int handling, 
            int acceleration, int energy, int price)
        {
            int newCarIndex = carDataList.FindIndex(car => car.carName == carName);
            if (newCarIndex != -1)
            {
                currentCarIndex = newCarIndex;
                view.UpdateCarDisplay(currentCarIndex);
                view.UpdateCarStats(carName, topSpeed, handling, acceleration, energy);
                CarData currentCar = carDataList[currentCarIndex];
                view.UpdatePriceInfo(price, currentCar);
            }
        }

        public void UpdateSelectedCarByID(int carID)
        {
            int newCarIndex = carDataList.FindIndex(car => car.carID == carID);
            if (newCarIndex != -1)
            {
                currentCarIndex = newCarIndex;
                view.UpdateCarDisplay(currentCarIndex);
                CarData currentCar = carDataList[currentCarIndex];
                view.UpdateCarStats(currentCar.carName, currentCar.topSpeed, currentCar.handling, 
                    currentCar.acceleration, currentCar.energy);
                view.UpdatePriceInfo(currentCar.price, currentCar);
            }
        }

        public void HandlePurchaseSuccess(string carName)
        {
            view.ShowSuccess(carName);
        }
    }
}
