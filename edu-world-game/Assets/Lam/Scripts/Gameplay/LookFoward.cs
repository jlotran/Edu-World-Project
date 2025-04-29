using UnityEngine;

namespace Lam.GAMEPLAY
{
    public class LookFoward : MonoBehaviour
    {
        [SerializeField] private Camera mainCamera;
        void Start()
        {
            mainCamera = Camera.main;
        }
        private void Update()
        {
            transform.rotation = Quaternion.Euler(13, mainCamera.transform.rotation.eulerAngles.y, 0);
        }
    }
}
