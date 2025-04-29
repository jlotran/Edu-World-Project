using UnityEngine;

namespace EduWorld
{
    public class LoginMethodHandler : MonoBehaviour
    {
        public ManagerEvent loginMethodEvent;

        public void OnAccountMethodClick()
        {
            loginMethodEvent.OnCloseLoginMethodScreen?.Invoke();
            loginMethodEvent.OnOpenLoginScreen?.Invoke();
        }

        public void OnFacebookMethodClick()
        {
            loginMethodEvent.OnCloseLoginMethodScreen?.Invoke();
            loginMethodEvent.OnOpenLoginScreen?.Invoke();
        }

        public void OnGoogleMethodClick()
        {
            loginMethodEvent.OnCloseLoginMethodScreen?.Invoke();
            loginMethodEvent.OnOpenLoginScreen?.Invoke();
        }

        public void OnGithubsMethodClick()
        {
            loginMethodEvent.OnCloseLoginMethodScreen?.Invoke();
            loginMethodEvent.OnOpenLoginScreen?.Invoke();
        }
    }
}
