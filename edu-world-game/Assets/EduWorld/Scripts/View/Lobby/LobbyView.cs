using UnityEngine;

namespace EduWorld
{
    public class lobbyView : MonoBehaviour
    {
        public ManagerEvent ManagerEvent;

        public void OnServerSelectedClick()
        {
            ManagerEvent.OnCloseLobbyRoomScreenSeverBoard?.Invoke();
            ManagerEvent.OnOpenLobbyRoomScreenParty?.Invoke();
        }

        public void OnEnterRoom()
        {
            ManagerEvent.OnCloseLobbyScreen?.Invoke();
            ManagerEvent.OnOpenLoadingScreen?.Invoke();
        }
    }
}
