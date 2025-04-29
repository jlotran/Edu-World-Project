using UnityEngine;

namespace EduWorld
{
    public class Car
    {
        public int id;
        public bool lockState;
        public string name;
        public float speed;
        public float handling;
        public float acceleration;
        public float energy;
        public string colorType;

        // Constructor
        public Car()
        {
        }

        public Car(int id, bool lockState, string name, float speed, float handling, float acceleration, float energy, string colorType)
        {
            this.id = id;
            this.lockState = lockState;
            this.name = name;
            this.speed = speed;
            this.handling = handling;
            this.acceleration = acceleration;
            this.energy = energy;
            this.colorType = colorType;
        }

        // Getters and Setters
        public int Id { get => id; set => id = value; }
        public string Name { get => name; set => name = value; }
        public float Speed { get => speed; set => speed = value; }
        public float Handling { get => handling; set => handling = value; }
        public float Acceleration { get => acceleration; set => acceleration = value; }
        public float Energy { get => energy; set => energy = value; }
        public string ColorType { get => colorType; set => colorType = value; }
        public bool LockState { get => lockState; set => lockState = value; }

        public void DisplayInfo()
        {
            Debug.Log($"Car Info: ID={Id}, Name={Name}, Speed={Speed}, Handling={Handling}, Acceleration={Acceleration}, Energy={Energy}, Color={ColorType}, Locked={LockState}");
        }

        public string DisplayInfo(Car car)
        {
            string info = $"ID: {car.Id}\nName: {car.Name}\nSpeed: {car.Speed}\nHandling: {car.Handling}\nAcceleration: {car.Acceleration}\nEnergy: {car.Energy}\nColor: {car.ColorType}\nLocked: {car.LockState}";
            return info;
        }
    }
}
