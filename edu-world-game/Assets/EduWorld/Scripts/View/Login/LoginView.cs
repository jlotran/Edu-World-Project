using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EduWorld
{
    public class LoginView : MonoBehaviour
    {
        public ManagerEvent loginMethodEvent;
        public TMP_InputField emailLoginInput;
        public TMP_InputField passwordLoginInput;
        public TMP_Text errorText;
        public Toggle rememberMeToggle;
        public AccountServices accountServices;

        private void Start()
        {
            accountServices = new AccountServices();
            LoadCredentials();
        }

        public void ResetTextField()
        {
            if (!rememberMeToggle.isOn)
            {
                emailLoginInput.text = string.Empty;
                passwordLoginInput.text = string.Empty;
            }
        }

        public void OnLoginClick()
        {
            if (string.IsNullOrEmpty(emailLoginInput.text) ||
                string.IsNullOrEmpty(passwordLoginInput.text) ||
                !IsValidEmail(emailLoginInput.text))
            {
                loginMethodEvent.OnOpenCheckPassword?.Invoke();
                string messError = string.IsNullOrEmpty(emailLoginInput.text) ? "Please enter email" : !IsValidEmail(emailLoginInput.text) ? "email is incorrect" : "Please enter password";
                errorText.text = messError;
            }
            else
            {
                StartCoroutine(HandleLogin());
            }
        }

        private IEnumerator HandleLogin()
        {
            loginMethodEvent.OnOpenCircleProgressBar?.Invoke();
            Account account = new Account(emailLoginInput.text, passwordLoginInput.text);
            yield return StartCoroutine(accountServices.LoginRequest(account, OnLoginResponse));
        }

        private void OnLoginResponse(bool success)
        {
            loginMethodEvent.OnCloseCircleProgressBar?.Invoke();
            if (success)
            {
                if (rememberMeToggle.isOn)
                {
                    SaveCredentials();
                }
                else
                {
                    ClearCredentials();
                }
                PlayerPrefs.SetString("LoggedInEmail", emailLoginInput.text);
                ResetTextField();
                StartCoroutine(ShowProgressBarAndChangeScene());
            }
            else
            {
                Debug.Log("Login failed.");
                errorText.text = "Account not exists. Please try again.";
                loginMethodEvent.OnOpenCheckPassword?.Invoke();
            }
        }

        private IEnumerator ShowProgressBarAndChangeScene()
        {
            loginMethodEvent.OnOpenCircleProgressBar?.Invoke();
            yield return null;
            loginMethodEvent.OnCloseCircleProgressBar?.Invoke();
            ChangeToLobbyScreen();
        }

        private void ChangeToLobbyScreen()
        {
            loginMethodEvent.OnCloseLoginMenuScreen?.Invoke();
            loginMethodEvent.OnOpenLobbyScreen?.Invoke();
            loginMethodEvent.OnOpenLobbyMainMenuScreen?.Invoke();
            ResetTextField();
        }

        public void OnSignupClick()
        {
            loginMethodEvent.OnCloseLoginScreen?.Invoke();
            loginMethodEvent.OnOpenSignUpScreen?.Invoke();
            ResetTextField();
        }

        public void OnTurnBackLoginClick()
        {
            loginMethodEvent.OnCloseLoginScreen?.Invoke();
            loginMethodEvent.OnOpenLoginMethodScreen?.Invoke();
            ResetTextField();
        }

        private bool IsValidEmail(string email)
        {
            return email.ToLower().EndsWith("@gmail.com");
        }

        private void SaveCredentials()
        {
            PlayerPrefs.SetString("Email", emailLoginInput.text);
            PlayerPrefs.SetString("Password", passwordLoginInput.text);
            PlayerPrefs.SetInt("RememberMe", rememberMeToggle.isOn ? 1 : 0);
            PlayerPrefs.Save();
        }

        private void LoadCredentials()
        {
            if (PlayerPrefs.GetInt("RememberMe", 0) == 1)
            {
                emailLoginInput.text = PlayerPrefs.GetString("Email", string.Empty);
                passwordLoginInput.text = PlayerPrefs.GetString("Password", string.Empty);
                rememberMeToggle.isOn = true;
            }
        }

        private void ClearCredentials()
        {
            PlayerPrefs.DeleteKey("Email");
            PlayerPrefs.DeleteKey("Password");
            PlayerPrefs.DeleteKey("RememberMe");
        }
    }
}
