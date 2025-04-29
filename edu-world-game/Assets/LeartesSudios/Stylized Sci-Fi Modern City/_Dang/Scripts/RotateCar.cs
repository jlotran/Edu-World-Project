using UnityEngine;

namespace EduWorld
{
    public class RotateCar : MonoBehaviour
    {
        public float rotationSpeed = 100f;

        void Start()
        {
            float randomYRotation = Random.Range(0f, 360f);
            transform.rotation = Quaternion.Euler(0f, randomYRotation, 0f);
        }

        void Update()
        {
            transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);
        }
    }
}
