using UnityEngine;

namespace EduWorld
{
    public class Manager : MonoBehaviour
    {
        public ManagerEvent managerEvent;

        void Start()
        {
            LoginScreen();
            LobbyScreen();
            CloseLoadingScreen();
        }

        private void LoginScreen()
        {
            managerEvent.OnOpenLoginMenuScreen?.Invoke();
            managerEvent.OnOpenLoginScreen?.Invoke();
            managerEvent.OnOpenLoginMethodScreen?.Invoke();
            managerEvent.OnCloseLoginScreen?.Invoke();
            managerEvent.OnCloseSignUpScreen?.Invoke();
            managerEvent.OnCloseCheckPassword?.Invoke();
        }

        private void LobbyScreen()
        {
            managerEvent.OnCloseLobbyScreen?.Invoke();
            managerEvent.OnOpenLobbyMainMenuScreen?.Invoke();
            managerEvent.OnCloseLobbyRoomScreen?.Invoke();
            managerEvent.OnOpenLobbyRoomScreenSeverBoard?.Invoke();
            managerEvent.OnCloseLobbyRoomScreenParty?.Invoke();
        }

        private void CloseLoadingScreen()
        {
            managerEvent.OnCloseLoadingScreen?.Invoke();
        }
    }
}