using UnityEngine;
using Cinemachine;
using System.Collections;
using Unity.VisualScripting;
using UnityEngine.UI;

namespace Edu_World
{
    public class CameraCar : MonoBehaviour
    {
        private Camera mainCamera; // Kéo Main Camera vào đây
        public static CameraCar instance;
        [SerializeField] private RawImage controlArea; // Reference to your RawImage

        void Start()
        {
            instance = this;
            mainCamera = GameObject.Find("CameraCar_1")?.GetComponent<Camera>();
            mainCamera.gameObject.SetActive(false);
        }

        public void CameraOnOf()
        {
            if(mainCamera.gameObject.activeSelf == false)
            {
                mainCamera.gameObject.SetActive(true);
            }
            else
            {
                mainCamera.gameObject.SetActive(false);
            }
        }

        public void CameraCarOff()
        {
            mainCamera.gameObject.SetActive(false);
        }

        private bool IsMouseInControlArea()
        {
            if (controlArea == null) return false;
            RectTransform rect = controlArea.rectTransform;
            Vector2 localMousePosition;
            return RectTransformUtility.ScreenPointToLocalPointInRectangle(rect, Input.mousePosition, null, out localMousePosition) 
                && rect.rect.Contains(localMousePosition);
        }

        void LateUpdate()
        {
            if (mainCamera != null && IsMouseInControlArea())
            {
                mainCamera.transform.position = transform.position;
                mainCamera.transform.rotation = transform.rotation;
            }
        }
    }
}
