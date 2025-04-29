using System.Collections;
using Cinemachine;
using Edu_World;
using UnityEngine;

namespace LamFusion
{
    public class Camera : MonoBehaviour
    {
        public static Camera instance;
        [SerializeField] private float topRigXAxis = 0.5f; // Giữ góc nhìn ngang trung tâm
        [SerializeField] private float topRigYAxis = 1f; // Đưa lên Top Rig
        [SerializeField] private float defaultBlendTime = 1f;
        private CinemachineBrain cinemachineBrain;

        void Start()
        {
            instance = this;
            cinemachineBrain = UnityEngine.Camera.main.GetComponent<CinemachineBrain>();

            // Find and assign cameras
            cameraShop = GameObject.Find("ShopCamera")?.GetComponent<CinemachineVirtualCamera>();
            freeLookCamera = GameObject.Find("CameraCar")?.GetComponent<CinemachineFreeLook>();

            if (cameraShop)
            {
                cameraShop.gameObject.SetActive(false);
                freeLookCamera.gameObject.SetActive(false);
            }
        }

        public CinemachineVirtualCamera cinemachineVirtualCamera;
        public CinemachineVirtualCamera cameraShop;
        public CinemachineFreeLook freeLookCamera;

        private enum CameraType
        {
            Main,
            Shop, 
            FreeLook
        }

        private void SetActiveCamera(CameraType type)
        {
            cinemachineVirtualCamera.gameObject.SetActive(type == CameraType.Main);
            cameraShop.gameObject.SetActive(type == CameraType.Shop);
            freeLookCamera.gameObject.SetActive(type == CameraType.FreeLook);
        }

        public void SetFollowPoin(Transform target)
        {
            if (cinemachineVirtualCamera)
            {
                cinemachineVirtualCamera.Follow = target;
            }
        }
        public void SetLookAtPoin(Transform target)
        {
            cinemachineVirtualCamera.LookAt = target;
        }
        public void SetShopCamera(bool isShop)
        {
            SetActiveCamera(isShop ? CameraType.Shop : CameraType.Main);
        }

        // Camera Car
        public void SetMidRigOnly()
        {
            freeLookCamera.m_YAxis.Value = 0.5f; // Đặt ở Middle Rig (0.5 là giữa)
            freeLookCamera.m_XAxis.Value = topRigXAxis; // Giữ góc nhìn trung tâm

            // Khóa Top và Bottom Rig thành Middle Rig
            freeLookCamera.m_Orbits[0].m_Height = freeLookCamera.m_Orbits[1].m_Height;
            freeLookCamera.m_Orbits[0].m_Radius = freeLookCamera.m_Orbits[1].m_Radius;
            freeLookCamera.m_Orbits[2].m_Height = freeLookCamera.m_Orbits[1].m_Height;
            freeLookCamera.m_Orbits[2].m_Radius = freeLookCamera.m_Orbits[1].m_Radius;
        }

        public void SetCameraTarget(Transform target)
        {
            if (freeLookCamera == null || target == null) return;
            SetActiveCamera(CameraType.FreeLook);
            freeLookCamera.Follow = target;
            freeLookCamera.LookAt = target;
            SetMidRigOnly(); // Đảm bảo luôn nằm ở Middle Rig
        }
        public void TurnCameraPlayer()
        {
            SetActiveCamera(CameraType.Main);
        }
    }
}
