using System.Collections.Generic;
using UnityEngine;

namespace Edu_World
{
    public class CarSpawner : MonoBehaviour
    {
        public static CarSpawner Instance { get; private set; }
        [SerializeField] private List<CarData> carDataList;

        [SerializeField] public List<Transform> carModelParents = new List<Transform>();
        private List<GameObject> spawnedCarModels = new List<GameObject>();

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }
        void Start()
        {
            FindCarPosByTag();
            InitializeCarModels(carDataList);
        }
        void FindCarPosByTag()
        {
            GameObject[] carPosObjects = GameObject.FindGameObjectsWithTag("Car_Interact");

            foreach (GameObject obj in carPosObjects)
            {
                carModelParents.Add(obj.transform);
            }
        }
        public void InitializeCarModels(List<CarData> carDataList)
        {
            ClearExistingCars();

            for (int i = 0; i < carDataList.Count; i++)
            {
                GameObject carInstance = Instantiate(carDataList[i].carPrefab, carModelParents[i]);
                spawnedCarModels.Add(carInstance);
                carInstance.SetActive(true);
                carInstance.AddComponent<CarIdHolder>().carID = carDataList[i].carID;
                // carInstance.AddComponent<CarInteractable>();
            }
        }

        public List<CarData> GetAllCars()
        {
            return carDataList;
        }
        private void ClearExistingCars()
        {
            foreach (var carModel in spawnedCarModels)
            {
                if (carModel != null)
                    Destroy(carModel);
            }
            spawnedCarModels.Clear();
        }

        private void OnDestroy()
        {
            ClearExistingCars();
        }
    }
}
