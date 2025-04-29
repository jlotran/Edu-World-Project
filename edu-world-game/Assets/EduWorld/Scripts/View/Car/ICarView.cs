using System.Collections.Generic;

namespace Edu_World
{
    public interface ICarView
    {
        void UpdateCarDisplay(int carIndex);
        void UpdateCarStats(string carName, int topSpeed, int handling, int acceleration, int energy);
        void UpdatePriceInfo(int price, CarData currentCar);
        void ShowSuccess(string carName);
        void Initialize(List<CarData> carDataList);
        void UpdateSelectedCarByID(CarIdHolder carID);

        void ToggleUI(CarIdHolder carHolder);
    }
}
