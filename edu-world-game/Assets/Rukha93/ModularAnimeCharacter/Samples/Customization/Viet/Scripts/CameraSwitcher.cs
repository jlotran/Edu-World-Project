using UnityEngine;
using Cinemachine;
using Rukha93.ModularAnimeCharacter.Customization.UI;
using System.Linq; // Add this

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{
    public class CameraSwitcher : MonoBehaviour
    {
        [SerializeField] private CinemachineVirtualCamera[] virtualCameras;
        [SerializeField] private UICustomizationDemo customizationUI;

        private void Start()
        {

            FindAllVirtualCameras();

            if (virtualCameras.Length > 0)
            {
                SwitchToCamera(0);
            }

            // Subscribe to category click events
            if (customizationUI != null)
            {
                customizationUI.OnClickCategory += HandleCategoryClick;
            }
        }


        void FindAllVirtualCameras()
        {
            string[] cameraNames = new string[] { "VirtualCameraFull", "VirtualCameraHead", "VirtualCameraHair", "VirtualCameraFull", "VirtualCameraFull", "VirtualCameraFull" };
            virtualCameras = new CinemachineVirtualCamera[cameraNames.Length];

            for (int i = 0; i < cameraNames.Length; i++)
            {
                GameObject cameraObj = GameObject.Find(cameraNames[i]);
                if (cameraObj != null)
                {
                    virtualCameras[i] = cameraObj.GetComponent<CinemachineVirtualCamera>();
                }
            }
        }


        private void HandleCategoryClick(string category)
        {
            // Find the index of the category in the UI's category list
            for (int i = 0; i < customizationUI.m_CategoryItems.Count; i++)
            {
                if (customizationUI.m_CategoryItems[i].Title == category)
                {
                    SwitchToCamera(i);
                    break;
                }
            }
        }

        private void SwitchToCamera(int index)
        {
            if (index < 0 || index >= virtualCameras.Length) return;

            // Turn off all cameras
            for (int i = 0; i < virtualCameras.Length; i++)
            {
                virtualCameras[i].Priority = 0;
            }

            // Turn on selected camera
            virtualCameras[index].Priority = 10;
        }

        private void OnDestroy()
        {
            if (customizationUI != null)
            {
                customizationUI.OnClickCategory -= HandleCategoryClick;
            }
        }
    }
}
