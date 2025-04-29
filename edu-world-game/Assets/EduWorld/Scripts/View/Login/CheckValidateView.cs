using UnityEngine;

namespace EduWorld
{
    public class ErrorHandler : MonoBehaviour
    {
        public ManagerEvent loginMethodEvent;

        public void OnCloseLoginErrorClick()
        {
            loginMethodEvent.OnCloseCheckPassword?.Invoke();
        }
    }
}
