using System.Collections;
using System.Collections.Generic;
using Edu_World;
using UnityEngine;

namespace RGSK
{
    public class CarManager : MonoBehaviour
    {
        public List<CarData> carConfigs; // Đặt ScriptableObject sẵn trong Unity Inspector

        private void Start()
        {
            StartCoroutine(GetCarDataFromAPI());
        }

        private IEnumerator GetCarDataFromAPI()
        {
            yield return null;
            yield return StartCoroutine(new CarDataAPI().FetchCarData(UpdateCarStats));
        }

        private void UpdateCarStats(List<CarDataModel> carDataModels)
        {
            foreach (var model in carDataModels)
            {
                var matchingCar = carConfigs.Find(car => car.carID == model.carID);
                if (matchingCar != null)
                {
                    matchingCar.carName = model.carName;
                    matchingCar.topSpeed = model.topSpeed;
                    matchingCar.handling = model.handling;
                    matchingCar.acceleration = model.acceleration;
                    matchingCar.energy = model.energy;
                    matchingCar.price = model.price;
                    matchingCar.rank = model.rank;
                }
            }
            
        }
    }

}
