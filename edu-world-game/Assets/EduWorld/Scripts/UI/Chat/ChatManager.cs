using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
namespace Edu_World
{
public class ChatManager : MonoBehaviour
{
    public Transform chatContent; // ScrollView content chứa tin nhắn
    public GameObject selfMessagePrefab; // Prefab tin nhắn của mình
    public GameObject otherMessagePrefab; // Prefab tin nhắn của người khác
    public TMP_InputField inputField;  // Reference to input field
    public Button sendButton;          // Reference to send button
    public ScrollRect scrollRect;


    private List<Message> chatHistory = new List<Message>();
    private ChatService chatService;

    void Start()
    {
        chatService = new ChatService();
        sendButton.onClick.AddListener(OnSendButtonClick);
        StartCoroutine(LoadMessages());
    }

    private IEnumerator LoadMessages()
    {
        yield return StartCoroutine(chatService.GetMessages((messages) =>
        {
            foreach (var msg in messages)
            {
                Message chatMessage = new Message(msg.sender, msg.message, msg.name);
                chatHistory.Add(chatMessage);
                DisplayMessage(chatMessage);
            }
        }));
    }

void DisplayMessage(Message message)
{
    GameObject messageObj = Instantiate(
        message.sender == "self" ? selfMessagePrefab : otherMessagePrefab
    );
    messageObj.transform.SetParent(chatContent, false);

    ChatMessageUI messageUI = messageObj.GetComponent<ChatMessageUI>();
    if(message.sender == "self"){
        messageUI.SetMessage(message.message);
    }
    else{
            messageUI.SetMessage(message.message);
    messageUI.SetName(message.name);
    }

    // Cập nhật lại layout để tránh lỗi vị trí
    // StartCoroutine(UpdateLayout());
    
    // StartCoroutine(ScrollToBottom());
}

// IEnumerator UpdateLayout()
// {
//     yield return new WaitForEndOfFrame();
//     LayoutRebuilder.ForceRebuildLayoutImmediate(chatContent.GetComponent<RectTransform>());
// }

    IEnumerator ScrollToBottom()
    {
        yield return new WaitForEndOfFrame();
        Canvas.ForceUpdateCanvases(); // Cập nhật UI
        scrollRect.verticalNormalizedPosition = 0f; // Cuộn xuống dưới

    }

    void OnSendButtonClick()
    {
        if (string.IsNullOrWhiteSpace(inputField.text))
            return;

        Message newMessage = new Message
        {
            sender = "self",
            message = inputField.text,
            name = "Viet"
        };

        StartCoroutine(chatService.SendMessage(newMessage, (success) =>
        {
            if (success)
            {
                Message chatMessage = new Message(newMessage.sender, newMessage.message, newMessage.name);
                chatHistory.Add(chatMessage);
                DisplayMessage(chatMessage);
                
                inputField.text = "";
                inputField.ActivateInputField();
                // StartCoroutine(ScrollToBottom());
            }
            else
            {
                Debug.LogError("Failed to send message");
            }
        }));
    }
}}