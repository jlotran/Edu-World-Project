using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace EduWorld
{
    public class WorldChatView : MonoBehaviour
    {
        public GameObject serverPrefab;
        public Transform serverListParent;
        public TMP_InputField messageInputField;
        private SeverServices severServices;
        private string lastMessageId;

        private void Start()
        {
            severServices = new SeverServices();
            StartCoroutine(severServices.FetchMessages(OnDataFetched));
        }

        private void OnDataFetched(List<Message> serverData)
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
                    serverText.text = $"{data.id}\n\n{data.message}";
                }
                lastMessageId = data.id;
            }
        }

        public void OnSubmitMessage()
        {
            string messageText = messageInputField.text;
            if (!string.IsNullOrEmpty(messageText))
            {
                Message newMessage = new Message { message = messageText };
                StartCoroutine(severServices.SendMessage(newMessage, OnMessageSent));
            }
        }
        private void OnMessageSent(bool success)
        {
            if (success)
            {
                messageInputField.text = string.Empty;
                StartCoroutine(severServices.FetchNewMessages(lastMessageId, OnNewDataFetched));
            }
            else
            {
                Debug.LogError("Failed to send message.");
            }
        }

        private void OnNewDataFetched(List<Message> newMessages)
        {
            if (newMessages == null || newMessages.Count == 0)
            {
                Debug.Log("No new messages.");
                return;
            }

            foreach (var data in newMessages)
            {
                GameObject serverInstance = Instantiate(serverPrefab, serverListParent);
                TMP_Text serverText = serverInstance.GetComponentInChildren<TMP_Text>();
                if (serverText != null)
                {
                    serverText.text = $"{data.id}\n{data.message}";
                }
            }
            lastMessageId = newMessages[newMessages.Count - 1].id;
        }

    }
}
