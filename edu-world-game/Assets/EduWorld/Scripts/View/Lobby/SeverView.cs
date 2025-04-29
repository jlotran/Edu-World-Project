using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace EduWorld
{
    public class SeverView : MonoBehaviour
    {
        public GameObject serverPrefab;
        public Transform serverListParent;
        public lobbyView partyHandler;
        private SeverServices severServices;

        private void Start()
        {
            severServices = new SeverServices();
            StartCoroutine(severServices.FetchData(OnDataFetched));
        }

        private void OnDataFetched(List<ServerData> serverData)
        {
            if (serverData == null)
            {
                Debug.LogError("Failed to fetch server data.");
                return;
            }

            foreach (var data in serverData)
            {
                GameObject serverInstance = Instantiate(serverPrefab, serverListParent);
                TMP_Text serverText = serverInstance.GetComponentInChildren<TMP_Text>();
                if (serverText != null)
                {
                    serverText.text = $"{data.id}\n{data.name}";
                }

                Button serverButton = serverInstance.GetComponentInChildren<Button>();
                if (serverButton != null)
                {
                    serverButton.onClick.AddListener(() => OnServerSelected(data));
                }
            }
        }

        private void OnServerSelected(ServerData data)
        {
            PlayerPrefs.SetString("SelectedServerName", $"{data.id}\n{data.name}");
            PlayerPrefs.Save();
            partyHandler.OnServerSelectedClick();
        }
    }
}
