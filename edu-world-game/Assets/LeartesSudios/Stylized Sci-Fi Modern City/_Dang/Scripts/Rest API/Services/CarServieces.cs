using System.Collections.Generic;
using System.Threading.Tasks;
using Newtonsoft.Json;

namespace EduWorld
{
    public class CarServices
    {
        private readonly ApiServices apiServices;
        private const string endpoint = "/car"; // Định nghĩa endpoint cố định

        public CarServices()
        {
            string apiUrl = Config.GetApiUrl(); // Lấy URL từ Config
            apiServices = new ApiServices(apiUrl);
        }

        public void SetToken(string token)
        {
            apiServices.SetToken(token); // Cập nhật token cho API
        }

        public async Task<List<Car>> GetAllCars()
        {
            string json = await apiServices.Get(endpoint);
            return json != null ? JsonConvert.DeserializeObject<List<Car>>(json) : null;
        }

        public async Task<Car> GetCarById(int id)
        {
            string json = await apiServices.Get($"{endpoint}/{id}");
            return json != null ? JsonConvert.DeserializeObject<Car>(json) : null;
        }

        public async Task<Car> CreateCar(Car car)
        {
            string json = await apiServices.Post(endpoint, car);
            return json != null ? JsonConvert.DeserializeObject<Car>(json) : null;
        }

        public async Task<Car> UpdateCar(int id, Car car)
        {
            string json = await apiServices.Put($"{endpoint}/{id}", car);
            return json != null ? JsonConvert.DeserializeObject<Car>(json) : null;
        }

        public async Task<bool> DeleteCar(int id)
        {
            string response = await apiServices.Delete($"{endpoint}/{id}");
            return response != null;
        }
    }
}
