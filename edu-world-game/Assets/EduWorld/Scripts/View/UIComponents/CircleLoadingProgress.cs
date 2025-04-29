using UnityEngine;

namespace EduWorld
{
    public class CircleLoadingProgress : MonoBehaviour
    {

        RectTransform rect;
        public float speed = 400.0f;

        void Start()
        {
            rect = GetComponent<RectTransform>();
        }

        void Update()
        {
            rect?.Rotate(0, 0, -(speed * Time.deltaTime));
        }
    }
}
