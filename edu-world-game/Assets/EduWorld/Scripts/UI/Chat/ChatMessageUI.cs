using UnityEngine;
using TMPro;
namespace Edu_World {
public class ChatMessageUI : MonoBehaviour
{
    [SerializeField] private TMP_Text messageText;
    [SerializeField] private TMP_Text nameText;


    public void SetMessage(string message)
    {
        messageText = messageText != null ? messageText : GetComponent<TMP_Text>();
        messageText.text = message;
    }
        public void SetName(string message)
    {
        nameText = nameText != null ? nameText : GetComponent<TMP_Text>();
        nameText.text = message;
    }
}
}