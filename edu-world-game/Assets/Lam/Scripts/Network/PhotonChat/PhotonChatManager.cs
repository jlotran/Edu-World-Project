using EduWorld;
using ExitGames.Client.Photon;
using Photon.Chat;
using Photon.Chat.Demo;
using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lam.FUSION
{
    public class PhotonChatManager : MonoBehaviour, IChatClientListener
    {
        // Start is called once before the first execution of Update after the MonoBehaviour is created
        private ChatClient _chatClient;

        [Header("UI config")]
        [SerializeField] private Button btn_sendMessage;
        [SerializeField] private TMP_InputField input_Message;
        [SerializeField] private Transform textsContent;
        [SerializeField] private MessageItem playerMessageItem; // renamed from textItem
        [SerializeField] private MessageItem otherPlayerMessageItem; // new field

        protected internal ChatAppSettings chatAppSettings;
        private string _channelName = "PublicChat";

        void Start()
        {
            input_Message.onSubmit.AddListener(OnInputSubmit);
            btn_sendMessage.onClick.AddListener(OnSendMessage);
            SetUpPhotonChat();
        }

        private void OnInputSubmit(string arg0)
        {
            OnSendMessage();
        }

        //=================================================== End Handle the Enter key being pressed====================================================

        // Update is called once per frame
        void Update()
        {
            if (this._chatClient != null)
            {
                this._chatClient.Service();
            }
        }

        private void SetUpPhotonChat()
        {
            _chatClient = new ChatClient(this);
            string userId = Util.GenerateRandomString(8);

#if PHOTON_UNITY_NETWORKING
            this.chatAppSettings = PhotonNetwork.PhotonServerSettings.AppSettings.GetChatSettings();
#endif

#if !UNITY_WEBGL
            this._chatClient.UseBackgroundWorkerForSending = true;
#endif
            this._chatClient.AuthValues = new AuthenticationValues(userId);
            this._chatClient.ChatRegion = "DEMOEDU";

            this._chatClient.ConnectUsingSettings(this.chatAppSettings);
            Debug.Log("Connecting to photon chat server...");
        }

        public void OnConnected()
        {
            _chatClient.Subscribe(new string[] { _channelName });
            Debug.Log("Connected to photon chat server. Channel: " + _channelName);
        }

        public void OnSendMessage()
        {
            string message = input_Message.text;
            if (message != "")
            {
                _chatClient.PublishMessage(_channelName, message);
                input_Message.text = "";
                input_Message.ActivateInputField();
            }
        }

        public void OnGetMessages(string channelName, string[] senders, object[] messages)
        {
            for (int i = 0; i < senders.Length; i++)
            {
                string currentUserId = _chatClient.AuthValues.UserId;
                bool isPlayerMessage = senders[i] == currentUserId;
                MessageItem messageItemPrefab = isPlayerMessage ? playerMessageItem : otherPlayerMessageItem;

                MessageItem messageItem = Instantiate(messageItemPrefab, textsContent);
                if (isPlayerMessage)
                {
                    messageItem.SetContentPlayer(messages[i].ToString());
                }
                else
                {
                    messageItem.SetContent(senders[i], messages[i].ToString());
                }
            }
        }
        public void DebugReturn(ExitGames.Client.Photon.DebugLevel level, string message)
        {
            if (level == ExitGames.Client.Photon.DebugLevel.ERROR)
            {
                Debug.LogError(message);
            }
            else if (level == ExitGames.Client.Photon.DebugLevel.WARNING)
            {
                Debug.LogWarning(message);
            }
            else
            {
                Debug.Log(message);
            }
        }

        public void OnChatStateChange(ChatState state) { }

        public void OnDisconnected()
        {
            Debug.Log("On disconnected from photon chat server");
        }

        public void OnPrivateMessage(string sender, object message, string channelName) { }

        public void OnStatusUpdate(string user, int status, bool gotMessage, object message) { }

        public void OnSubscribed(string[] channels, bool[] results) { }

        public void OnUnsubscribed(string[] channels) { }

        public void OnUserSubscribed(string channel, string user) { }

        public void OnUserUnsubscribed(string channel, string user) { }
    }
}
