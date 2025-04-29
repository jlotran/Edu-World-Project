using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using Newtonsoft.Json;

namespace EduWorld
{
    public class SeverServices
    {
        public IEnumerator FetchData(System.Action<List<ServerData>> callback)
        {
            string url = "https://67b2ba58bc0165def8ce4fc6.mockapi.io/map";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                    callback(null);
                }
                else
                {
                    List<ServerData> serverData = JsonConvert.DeserializeObject<List<ServerData>>(www.downloadHandler.text);
                    callback(serverData);
                }
            }
        }

        public IEnumerator FetchMessages(System.Action<List<Message>> callback)
        {
            string url = "https://67b30c2fbc0165def8cfb241.mockapi.io/chatting";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                    callback(null);
                }
                else
                {
                    List<Message> messages = JsonConvert.DeserializeObject<List<Message>>(www.downloadHandler.text);
                    callback(messages);
                }
            }
        }

        public IEnumerator SendMessage(Message message, System.Action<bool> callback)
        {
            string url = "https://67b30c2fbc0165def8cfb241.mockapi.io/chatting";
            string jsonData = JsonConvert.SerializeObject(message);
            byte[] bodyRaw = System.Text.Encoding.UTF8.GetBytes(jsonData);

            using (UnityWebRequest www = new UnityWebRequest(url, "POST"))
            {
                www.uploadHandler = new UploadHandlerRaw(bodyRaw);
                www.downloadHandler = new DownloadHandlerBuffer();
                www.SetRequestHeader("Content-Type", "application/json");

                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                    callback(false);
                }
                else
                {
                    callback(true);
                }
            }
        }

        public IEnumerator FetchNewMessages(string lastMessageId, System.Action<List<Message>> callback)
        {
            string url = $"https://67b30c2fbc0165def8cfb241.mockapi.io/chatting";
            using (UnityWebRequest www = UnityWebRequest.Get(url))
            {
                yield return www.SendWebRequest();

                if (www.result == UnityWebRequest.Result.ConnectionError || www.result == UnityWebRequest.Result.ProtocolError)
                {
                    Debug.LogError(www.error);
                    callback(null);
                }
                else
                {
                    List<Message> allMessages = JsonConvert.DeserializeObject<List<Message>>(www.downloadHandler.text);

                    // Chỉ lấy những tin nhắn mới có ID lớn hơn lastMessageId
                    List<Message> newMessages = allMessages.FindAll(m => int.Parse(m.id) > int.Parse(lastMessageId));

                    callback(newMessages);
                }
            }
        }

    }
}
