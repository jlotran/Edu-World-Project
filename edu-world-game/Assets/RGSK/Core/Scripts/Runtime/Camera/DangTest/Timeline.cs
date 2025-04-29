using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;

namespace RGSK
{
    [RequireComponent(typeof(PlayableDirector))]
    public class Timeline : MonoBehaviour
    {
        public PlayableDirector _director;
        private CinemachineVirtualCamera virtualCamera;

        void OnEnable()
        {
            _director = GetComponent<PlayableDirector>();
            virtualCamera = GetComponentInChildren<CinemachineVirtualCamera>();
            LookAtParent(virtualCamera);
            RGSKEvents.OnTimelineToggle.AddListener(ToggleTimeline);
        }

        void OnDisable()
        {
            RGSKEvents.OnTimelineToggle.RemoveListener(ToggleTimeline);
        }

        public void ToggleTimeline(bool isActive)
        {
            if (_director == null)
            {
                return;
            }

            if (isActive)
            {
                _director.Play();
            }
            else
            {
                _director.Stop();
            }
        }

        public void DisableUIInput()
        {
            InputManager.Instance?.SetInputMode(InputMode.Disabled);
        }

        public void EnableUIInput()
        {
            InputManager.Instance?.SetInputMode(InputMode.Gameplay);
        }

        public void LookAtParent(CinemachineVirtualCamera virtualCamera)
        {
            if (virtualCamera == null)
            {
                return;
            }

            Transform parent = virtualCamera.transform.parent;
            if (parent == null)
            {
                return;
            }

            virtualCamera.LookAt = parent;
        }

        public void OnTimelineStateChange(RaceState state)
        {
            if (_director == null)
            {
                return;
            }

            if (state == RaceState.PreRace)
            {
                _director.enabled = false;
            }
            else
            {
                _director.enabled = true;
            }
        }

        public void BindingCinemachineBrain(CinemachineBrain cinemachineBrain)
        {
            if (cinemachineBrain == null)
            {
                return;
            }

            foreach (var output in _director.playableAsset.outputs)
            {
                if (output.streamName == "Cinemachine Track")
                {
                    _director.SetGenericBinding(output.sourceObject, cinemachineBrain);
                }
            }
        }

        public void BindCinemachineVirtualCameraToShot(CinemachineVirtualCamera virtualCamera)
        {
            if (virtualCamera == null)
            {
                Debug.LogError("CinemachineVirtualCamera không được tìm thấy!");
                return;
            }

            var timelineAsset = _director.playableAsset as TimelineAsset;
            if (timelineAsset == null)
            {
                Debug.LogError("Không thể lấy TimelineAsset từ PlayableDirector!");
                return;
            }

            TimelineClip lastClip = null;

            foreach (var track in timelineAsset.GetOutputTracks())
            {
                if (track is CinemachineTrack)
                {
                    foreach (TimelineClip clip in track.GetClips())
                    {
                        lastClip = clip;  // Cứ gán, clip cuối cùng sẽ giữ lại
                    }

                    if (lastClip != null)
                    {
                        var shot = lastClip.asset as CinemachineShot;
                        if (shot != null)
                        {
                            // Debug.Log($"Binding VCam '{virtualCamera.name}' to LAST shot ({lastClip.displayName})");
                            _director.SetReferenceValue(shot.VirtualCamera.exposedName, virtualCamera);
                            _director.RebuildGraph(); // Cập nhật binding
                        }
                        else
                        {
                            Debug.LogError("Clip cuối cùng không phải CinemachineShot.");
                        }
                    }
                    else
                    {
                        Debug.LogError("Không có clip nào trong CinemachineTrack.");
                    }

                    return;  // Chỉ cần xử lý track đầu tiên có CinemachineTrack
                }
            }

            Debug.LogError("Không tìm thấy CinemachineTrack trong TimelineAsset.");
        }
    }
}
