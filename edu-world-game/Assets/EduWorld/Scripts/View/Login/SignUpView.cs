using System.Collections;
using TMPro;
using UnityEngine;

namespace EduWorld
{
    public class SignUpHandler : MonoBehaviour
    {
        public ManagerEvent loginMethodEvent;
        public TMP_InputField emailSignInInput;
        public TMP_InputField passwordSignInInput;
        public TMP_Text errorText;
        public AccountServices accountServices;

        private void Start()
        {
            accountServices = new AccountServices();
        }

        public void ResetInputFields()
        {
            emailSignInInput.text = string.Empty;
            passwordSignInInput.text = string.Empty;
            errorText.text = string.Empty;
        }

        public void OnSignUpClick()
        {
            if (string.IsNullOrEmpty(emailSignInInput.text) ||
                string.IsNullOrEmpty(passwordSignInInput.text) ||
                !IsValidEmail(emailSignInInput.text))
            {
                loginMethodEvent.OnOpenCheckPassword?.Invoke();
                string messError = string.IsNullOrEmpty(emailSignInInput.text) ? "Please enter email" : !IsValidEmail(emailSignInInput.text) ? "Email must have \"@gmail.com\" address" : "Please enter password";
                errorText.text = $"{messError}";
            }
            else
            {
                StartCoroutine(HandleSignUp());
            }
        }

        private IEnumerator HandleSignUp()
        {
            loginMethodEvent.OnOpenCircleProgressBar?.Invoke();
            Account account = new Account(emailSignInInput.text, passwordSignInInput.text);
            yield return StartCoroutine(accountServices.SignupRequest(account, OnSignUpResponse));
        }

        private void OnSignUpResponse(bool success)
        {
            loginMethodEvent.OnCloseCircleProgressBar?.Invoke();
            if (success)
            {
                ResetInputFields();
                loginMethodEvent.OnCloseSignUpScreen?.Invoke();
                loginMethodEvent.OnOpenLoginScreen?.Invoke();
            }
            else
            {
                errorText.text = "Signup failed. Please try again.";
            }
        }

        public void OnTurnBackSignupClick()
        {
            loginMethodEvent.OnCloseSignUpScreen?.Invoke();
            loginMethodEvent.OnOpenLoginScreen?.Invoke();
            ResetInputFields();
        }

        private bool IsValidEmail(string email)
        {
            return email.ToLower().EndsWith("@gmail.com");
        }
    }
}
