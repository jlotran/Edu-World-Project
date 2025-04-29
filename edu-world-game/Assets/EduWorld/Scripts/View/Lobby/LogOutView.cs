using System.Collections;
using UnityEngine;

namespace EduWorld
{
    public class LogOutView : MonoBehaviour
    {
        public ManagerEvent ManagerEvent;

        public void OnLogOutClick()
        {
            StartCoroutine(ShowProgressBarAndLogOut());
        }

        private IEnumerator ShowProgressBarAndLogOut()
        {
            ManagerEvent.OnOpenCircleProgressBar?.Invoke();
            yield return new WaitForSeconds(2f);
            ManagerEvent.OnCloseCircleProgressBar?.Invoke();
            PlayerPrefs.DeleteKey("LoggedInEmail");
            ManagerEvent.OnCloseLobbyMainMenuScreen?.Invoke();
            ManagerEvent.OnCloseLobbyScreen?.Invoke();
            ManagerEvent.OnCloseLobbyRoomScreenParty?.Invoke();
            ManagerEvent.OnCloseLobbyRoomScreenSeverBoard?.Invoke();
            ManagerEvent.OnOpenLoginMenuScreen?.Invoke();
        }

    }
}