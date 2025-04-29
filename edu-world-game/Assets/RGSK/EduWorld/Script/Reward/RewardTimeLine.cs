using UnityEngine;
using UnityEngine.Playables;
using Cinemachine;

namespace RGSK
{
    public class RewardTimeLine : MonoBehaviour
    {
        public PlayableDirector director;
        public CinemachineVirtualCamera[] virtualCameras;

        void Start()
        {
            director = GetComponent<PlayableDirector>();
            virtualCameras = GetComponentsInChildren<CinemachineVirtualCamera>();
        }

        public void ToggleTimeline(bool isActive)
        {
            if (director == null)
            {
                Debug.LogWarning("PlayableDirector is missing.");
                return;
            }

            if (isActive)
            {
                foreach (var cam in virtualCameras)
                {
                    if (cam != null)
                    {
                        cam.gameObject.SetActive(true);
                        cam.Priority = 9999;
                    }
                }

                if (virtualCameras.Length > 0)
                {
                    var lastCam = virtualCameras[virtualCameras.Length - 1];
                    if (lastCam != null)
                    {
                        lastCam.Priority = 10000;
                    }
                }

                director.Play();
            }
            else
            {
                director.Stop();
            }
        }
    }
}
