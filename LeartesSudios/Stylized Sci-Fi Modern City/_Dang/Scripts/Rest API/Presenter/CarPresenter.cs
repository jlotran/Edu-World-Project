using System.Collections.Generic;
using UnityEngine;

namespace EduWorld
{
    public class CarPresenter
    {
        private CarView carView;
        private CarServices carServices;

        public CarPresenter(CarView view)
        {
            carView = view;
            carServices = new CarServices();
            Register();
        }

        void Register()
        {
            // Đăng ký sự kiện từ View
            carView.fetchButton.onClick.AddListener(FetchAPI);
            carView.fetchByIdButton.onClick.AddListener(FetchCarById);
            carView.createButton.onClick.AddListener(CreateCar);
            carView.updateButton.onClick.AddListener(UpdateCar);
            carView.deleteButton.onClick.AddListener(DeleteCar);
        }

        private async void FetchAPI()
        {
            List<Car> cars = await carServices.GetAllCars();
            if (cars == null || cars.Count == 0)
            {
                Debug.LogError("No cars found!");
                return;
            }

            foreach (var car in cars)
            {
                car.DisplayInfo();
                // carView.DisplayInfo(car);
            }
        }

        private async void FetchCarById()
        {
            if (int.TryParse(carView.carIdInput.text, out int carId))
            {
                Car car = await carServices.GetCarById(carId);
                if (car != null)
                    carView.DisplayInfo(car);
                else
                    Debug.LogError("Car not found!");
            }
            else Debug.LogError("Invalid Car ID");
        }

        private async void CreateCar()
        {
            if (!int.TryParse(carView.carIDInput.text, out int carId))
            {
                Debug.LogError("Invalid Car ID!");
                return;
            }

            Car newCar = new Car
            {
                id = carId,
                name = carView.carNameInput.text,
                speed = float.Parse(carView.carSpeedInput.text),
                handling = float.Parse(carView.carHandlingInput.text),
                acceleration = float.Parse(carView.carAccelerationInput.text),
                energy = float.Parse(carView.carEnergyInput.text),
                colorType = carView.carColorInput.text,
                lockState = carView.carLockStateToggle.isOn
            };

            Car createdCar = await carServices.CreateCar(newCar);
            if (createdCar != null)
                Debug.Log($"Created Car: {createdCar.id} - {createdCar.name}");
            else
                Debug.LogError("Failed to create car!");
        }

        private async void UpdateCar()
        {
            if (int.TryParse(carView.carIdInput.text, out int carId))
            {
                Car updatedCar = new Car
                {
                    id = carId,
                    name = carView.carNameInput.text,
                    speed = float.Parse(carView.carSpeedInput.text),
                    handling = float.Parse(carView.carHandlingInput.text),
                    acceleration = float.Parse(carView.carAccelerationInput.text),
                    energy = float.Parse(carView.carEnergyInput.text),
                    colorType = carView.carColorInput.text,
                    lockState = carView.carLockStateToggle.isOn
                };

                Car result = await carServices.UpdateCar(carId, updatedCar);
                if (result != null)
                    Debug.Log($"Updated Car: {result.name}");
                else
                    Debug.LogError("Failed to update car!");
            }
            else Debug.LogError("Invalid Car ID");
        }

        private async void DeleteCar()
        {
            if (int.TryParse(carView.carIdInput.text, out int carId))
            {
                bool success = await carServices.DeleteCar(carId);
                if (success)
                    Debug.Log("Car deleted successfully!");
                else
                    Debug.LogError("Failed to delete car!");
            }
            else Debug.LogError("Invalid Car ID");
        }
    }
}
