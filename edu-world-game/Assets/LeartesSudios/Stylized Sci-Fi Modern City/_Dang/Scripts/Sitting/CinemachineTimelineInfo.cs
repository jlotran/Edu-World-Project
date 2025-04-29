using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Timeline;
using Cinemachine;
using System.Collections.Generic;
using System.Collections;
using Lam.FUSION;

public class CinemachineTimelineInfo : MonoBehaviour
{
    private Player _playerController;
    public Transform playerPosTransform;

    [Header("Timeline Settings")]
    public PlayableDirector timeLine1;
    public List<CinemachineVirtualCameraBase> listCamVirtual1;

    public PlayableDirector timeLine2;
    public List<CinemachineVirtualCameraBase> listCamVirtual2;

    [Header("Camera Root")]
    public GameObject timelineCameraRoot;

    private bool isActive = false;

    void Start()
    {
        StartCoroutine(Initialize());
    }

    private IEnumerator WaitForPlayerController()
    {
        while (Player.localPlayer == null)
        {
            yield return null; // Chờ đến frame tiếp theo
        }
        _playerController = Player.localPlayer;
    }

    private IEnumerator Initialize()
    {
        // Chờ Player.localPlayer được gán
        yield return StartCoroutine(WaitForPlayerController());

        if (playerPosTransform == null)
        {
            Debug.LogError("PlayerPosTransform is not assigned in the Inspector.");
            yield break;
        }

        if (timeLine1 == null || timeLine2 == null)
        {
            Debug.LogError("Timeline not assigned in the inspector.");
            yield break;
        }

        if (listCamVirtual1 == null || listCamVirtual2 == null)
        {
            Debug.LogError("Camera list not assigned in the inspector.");
            yield break;
        }

        if (timelineCameraRoot != null)
        {
            timelineCameraRoot.SetActive(false);
        }
        else
        {
            Debug.LogError("TimeLine_Camera Game Object not assigned in the inspector.");
            yield break;
        }
    }

    private Player GetPlayerController()
    {
        if (_playerController == null)
        {
            _playerController = Player.localPlayer;
        }
        return _playerController;
    }

    private void DisableRootMotion()
    {
        var player = GetPlayerController();
        if (player != null && player.animator != null)
        {
            player.animator.applyRootMotion = false;
        }
    }

    private void EnableRootMotion()
    {
        var player = GetPlayerController();
        if (player != null && player.animator != null)
        {
            player.animator.applyRootMotion = true;
        }
    }

    private void RegisterValueToCameraShots()
    {
        ResetValue(timeLine1);
        ResetValue(timeLine2);

        SetValue(timeLine1, listCamVirtual1);
        SetValue(timeLine2, listCamVirtual2);
    }

    private void ResetValue(PlayableDirector timeLineDirector)
    {
        TimelineAsset timeline = timeLineDirector.playableAsset as TimelineAsset;
        if (timeline == null) return;

        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is CinemachineTrack)
            {
                foreach (TimelineClip clip in track.GetClips())
                {
                    CinemachineShot shot = clip.asset as CinemachineShot;
                    if (shot != null)
                    {
                        timeLineDirector.SetReferenceValue(shot.VirtualCamera.exposedName, null);
                    }
                }
            }
            else if (track is AnimationTrack animationTrack)
            {
                timeLineDirector.SetGenericBinding(animationTrack, null);
            }
        }
    }

    private void SetValue(PlayableDirector timeLineDirector, List<CinemachineVirtualCameraBase> listCamVirtual)
    {
        var player = GetPlayerController();
        TimelineAsset timeline = timeLineDirector.playableAsset as TimelineAsset;
        if (timeline == null) return;

        int camIndex = 0;
        foreach (var track in timeline.GetOutputTracks())
        {
            if (track is CinemachineTrack)
            {
                foreach (TimelineClip clip in track.GetClips())
                {
                    CinemachineShot shot = clip.asset as CinemachineShot;
                    if (shot == null) continue;

                    if (camIndex < listCamVirtual.Count)
                    {
                        timeLineDirector.SetReferenceValue(shot.VirtualCamera.exposedName, listCamVirtual[camIndex]);
                        camIndex++;
                    }
                    else
                    {
                        break;
                    }
                }
            }
            else if (track is AnimationTrack animationTrack)
            {
                if (player != null)
                {
                    timeLineDirector.SetGenericBinding(animationTrack, player.animator);
                }
            }
        }
        timeLineDirector.RebuildGraph();
    }

    private void OnTriggerEnter(Collider other)
    {
        var player = GetPlayerController();
        if (player == null)
        {
            Debug.LogError("PlayerController is null during OnTriggerEnter.");
            return;
        }

        player.FreeKcc();
        if (other.transform.parent.CompareTag("Player") && !isActive)
        {
            StartCoroutine(PlayTimelinesWithDelay());
        }
    }

    private IEnumerator PlayTimelinesWithDelay()
    {
        isActive = true;

        var player = GetPlayerController();

        // Bật timeline camera
        if (timelineCameraRoot != null)
        {
            timelineCameraRoot.SetActive(true);
        }

        player.FreeKcc();

        RegisterValueToCameraShots();

        yield return new WaitUntil(() => player.kcc != null);

        MovePlayerToPosition();

        yield return new WaitForEndOfFrame();

        DisableRootMotion();

        yield return new WaitForSeconds(0.05f);

        timeLine1.Play();
        yield return new WaitForSeconds((float)timeLine1.duration);

        timeLine2.Play();
        yield return new WaitForSeconds((float)timeLine2.duration);

        EnableRootMotion();
        isActive = false;

        // Bật timeline camera
        if (timelineCameraRoot != null)
        {
            timelineCameraRoot.SetActive(false);
        }
    }

    private void MovePlayerToPosition()
    {
        var player = GetPlayerController();
        if (player == null || playerPosTransform == null)
        {
            Debug.LogError("Cannot move player: PlayerController or PlayerPosTransform is null.");
            return;
        }

        var kcc = player.kcc;
        if (kcc != null)
        {
            kcc.SetPosition(playerPosTransform.position);
            kcc.SetLookRotation(playerPosTransform.rotation);
        }
        else
        {
            player.transform.position = playerPosTransform.position;
            player.transform.rotation = playerPosTransform.rotation;
        }
    }

}
