using System.Collections;
using UnityEngine;
using UnityEngine.Networking;
namespace Edu_World {
public class ChatService
{
    private const string API_URL = "https://67aaf3ce65ab088ea7e80b92.mockapi.io/api/shop/Message";

    public IEnumerator GetMessages(System.Action<Message[]> callback)
    {
        using (UnityWebRequest request = UnityWebRequest.Get(API_URL))
        {
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                MessageList messageList = JsonUtility.FromJson<MessageList>("{\"messages\":" + request.downloadHandler.text + "}");
                callback(messageList.messages);
            }
            else
            {
                Debug.LogError("Error: " + request.error);
                callback(new Message[0]);
            }
        }
    }

    public IEnumerator SendMessage(Message message, System.Action<bool> callback)
    {
        string json = JsonUtility.ToJson(message);
        using (UnityWebRequest request = UnityWebRequest.PostWwwForm(API_URL, json))
        {
            request.SetRequestHeader("Content-Type", "application/json");
            byte[] jsonBytes = System.Text.Encoding.UTF8.GetBytes(json);
            request.uploadHandler = new UploadHandlerRaw(jsonBytes);

            yield return request.SendWebRequest();

            callback(request.result == UnityWebRequest.Result.Success);
        }
    }
}
}