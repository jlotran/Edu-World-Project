using UnityEngine;

namespace Edu_World
{
    [CreateAssetMenu(fileName = "CarData", menuName = "ScriptableObjects/CarData")]

    public class CarData : ScriptableObject
    {
        public int carID;
        public Sprite carSprite;
        public string carName;
        public int topSpeed;
        public int handling;
        public int acceleration;
        public int energy;
        public int price;
        public GameObject carPrefab;
        public int rank;
    }
}