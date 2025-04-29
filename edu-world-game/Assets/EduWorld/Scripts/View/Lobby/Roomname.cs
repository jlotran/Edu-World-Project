using TMPro;
using UnityEngine;

namespace EduWorld
{
    public class RoomName : MonoBehaviour
    {
        public TMP_Text usernameText;

        private void Start()
        {
            LoadUsername();
        }

        private void OnEnable()
        {
            LoadUsername();
        }

        private void LoadUsername()
        {
            if (PlayerPrefs.HasKey("LoggedInEmail"))
            {
                string email = PlayerPrefs.GetString("LoggedInEmail");
                usernameText.text = email.Replace("@gmail.com", "");
            }
            else
            {
                usernameText.text = "User name";
            }
        }
    }
}
