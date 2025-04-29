using UnityEngine;
using UnityEngine.Events;

namespace EduWorld
{
    public class ManagerEvent : MonoBehaviour
    {
        [Header("Game Object")]
        // Game Object: Login Screen
        public GameObject MethodScreen;
        public GameObject LoginScreen;
        public GameObject SignUpScreen;
        public GameObject CheckPassword;
        public GameObject LoginScreenMenu;
        public GameObject CircleProgressBar;

        // Game Object: Lobby Screen
        public GameObject LobbyScreen;
        public GameObject LobbyMainMenuScreen;
        public GameObject lobbyRoomScreen;
        public GameObject lobbyRoomScreenSeverBoard;
        public GameObject lobbyRoomScreenParty;

        // Game Object: Loading Screen
        public GameObject LoadingScreen;

        [Header("Unity Event")]
        //login
        public UnityEvent OnOpenLoginScreen;
        public UnityEvent OnCloseLoginScreen;
        public UnityEvent OnOpenLoginMethodScreen;
        public UnityEvent OnCloseLoginMethodScreen;
        public UnityEvent OnOpenSignUpScreen;
        public UnityEvent OnCloseSignUpScreen;
        public UnityEvent OnOpenCheckPassword;
        public UnityEvent OnCloseCheckPassword;
        public UnityEvent OnOpenLoginMenuScreen;
        public UnityEvent OnCloseLoginMenuScreen;
        public UnityEvent OnOpenCircleProgressBar;
        public UnityEvent OnCloseCircleProgressBar;

        // lobby
        public UnityEvent OnOpenLobbyScreen;
        public UnityEvent OnCloseLobbyScreen;
        public UnityEvent OnOpenLobbyMainMenuScreen;
        public UnityEvent OnCloseLobbyMainMenuScreen;
        public UnityEvent OnOpenLobbyRoomScreen;
        public UnityEvent OnCloseLobbyRoomScreen;
        public UnityEvent OnOpenLobbyRoomScreenSeverBoard;
        public UnityEvent OnCloseLobbyRoomScreenSeverBoard;
        public UnityEvent OnOpenLobbyRoomScreenParty;
        public UnityEvent OnCloseLobbyRoomScreenParty;

        //
        public UnityEvent OnOpenLoadingScreen;
        public UnityEvent OnCloseLoadingScreen;

        private void Awake()
        {
            //
            OnOpenLoginScreen.AddListener(OpenLoginScreen);
            OnCloseLoginScreen.AddListener(CloseLoginScreen);

            //
            OnOpenLoginMethodScreen.AddListener(OpenLoginMethodScreen);
            OnCloseLoginMethodScreen.AddListener(CloseLoginMethodScreen);

            //
            OnOpenSignUpScreen.AddListener(OpenSignUpScreen);
            OnCloseSignUpScreen.AddListener(CloseSignUpScreen);

            //
            OnOpenCheckPassword.AddListener(OpenCheckPassword);
            OnCloseCheckPassword.AddListener(CloseCheckPassword);

            //
            OnOpenLoginMenuScreen.AddListener(OpenLoginMenuScreen);
            OnCloseLoginMenuScreen.AddListener(CloseLoginMenuScreen);

            //
            OnOpenLobbyScreen.AddListener(OpenLobbyScreen);
            OnCloseLobbyScreen.AddListener(CloseLobbyScreen);

            //
            OnOpenLobbyMainMenuScreen.AddListener(OpenLobbyMainMenuScreen);
            OnCloseLobbyMainMenuScreen.AddListener(CloseLobbyMainMenuScreen);

            //
            OnOpenLobbyRoomScreen.AddListener(OpenLobbyRoomScreen);
            OnCloseLobbyRoomScreen.AddListener(CloseLobbyRoomScreen);

            //
            OnOpenLobbyRoomScreenSeverBoard.AddListener(OpenLobbyRoomScreenSeverBoard);
            OnCloseLobbyRoomScreenSeverBoard.AddListener(CloseLobbyRoomScreenSeverBoard);

            //
            OnOpenLobbyRoomScreenParty.AddListener(OpenLobbyRoomScreenParty);
            OnCloseLobbyRoomScreenParty.AddListener(CloseLobbyRoomScreenParty);

            //
            OnOpenCircleProgressBar.AddListener(OpenCircleProgressBar);
            OnCloseCircleProgressBar.AddListener(CloseCircleProgressBar);

            //
            OnOpenLoadingScreen.AddListener(OpenLoadingScreen);
            OnCloseLoadingScreen.AddListener(CloseLoadingScreen);
        }

        // OPEN CLOSE Event
        public void OpenLoginScreen()
        {
            LoginScreen.SetActive(true);
        }

        public void CloseLoginScreen()
        {
            LoginScreen.SetActive(false);
        }

        public void OpenLoginMethodScreen()
        {
            MethodScreen.SetActive(true);
        }

        public void CloseLoginMethodScreen()
        {
            MethodScreen.SetActive(false);
        }

        public void OpenSignUpScreen()
        {
            SignUpScreen.SetActive(true);
        }

        public void CloseSignUpScreen()
        {
            SignUpScreen.SetActive(false);
        }

        public void OpenCheckPassword()
        {
            CheckPassword.SetActive(true);
        }

        public void CloseCheckPassword()
        {
            CheckPassword.SetActive(false);
        }

        public void OpenLoginMenuScreen()
        {
            LoginScreenMenu.SetActive(true);
        }

        public void CloseLoginMenuScreen()
        {
            LoginScreenMenu.SetActive(false);
        }

        public void OpenLobbyScreen()
        {
            LobbyScreen.SetActive(true);
        }

        public void CloseLobbyScreen()
        {
            LobbyScreen.SetActive(false);
        }

        public void OpenLobbyMainMenuScreen()
        {
            LobbyMainMenuScreen.SetActive(true);
        }

        public void CloseLobbyMainMenuScreen()
        {
            LobbyMainMenuScreen.SetActive(false);
        }

        public void OpenLobbyRoomScreen()
        {
            lobbyRoomScreen.SetActive(true);
        }

        public void CloseLobbyRoomScreen()
        {
            lobbyRoomScreen.SetActive(false);
        }

        public void OpenLobbyRoomScreenSeverBoard()
        {
            lobbyRoomScreenSeverBoard.SetActive(true);
        }

        public void CloseLobbyRoomScreenSeverBoard()
        {
            lobbyRoomScreenSeverBoard.SetActive(false);
        }

        public void OpenLobbyRoomScreenParty()
        {
            lobbyRoomScreenParty.SetActive(true);
        }

        public void CloseLobbyRoomScreenParty()
        {
            lobbyRoomScreenParty.SetActive(false);
        }

        public void OpenCircleProgressBar()
        {
            CircleProgressBar.SetActive(true);
        }

        public void CloseCircleProgressBar()
        {
            CircleProgressBar.SetActive(false);
        }

        public void OpenLoadingScreen()
        {
            LoadingScreen.SetActive(true);
        }

        public void CloseLoadingScreen()
        {
            LoadingScreen.SetActive(false);
        }
    }
}