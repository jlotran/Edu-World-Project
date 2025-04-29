using UnityEngine;

namespace EduWorld
{
    public class MainMenuHandler : MonoBehaviour
    {
        public ManagerEvent ManagerEvent;
        public void OnSeverClick()
        {
            ManagerEvent.OnCloseLobbyMainMenuScreen?.Invoke();
            ManagerEvent.OnOpenLobbyRoomScreen?.Invoke();
            ManagerEvent.OnOpenLobbyRoomScreenSeverBoard?.Invoke();
        }
    }
}
