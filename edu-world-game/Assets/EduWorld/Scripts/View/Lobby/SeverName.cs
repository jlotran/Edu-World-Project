using TMPro;
using UnityEngine;

namespace EduWorld
{
    public class SeverName : MonoBehaviour
    {
        public TMP_Text severnameText;

        private void LoadSeverName()
        {
            string serverName = PlayerPrefs.GetString("SelectedServerName", "Default Server");
            severnameText.text = serverName;
        }

        private void OnEnable()
        {
            LoadSeverName();
        }
    }
}
