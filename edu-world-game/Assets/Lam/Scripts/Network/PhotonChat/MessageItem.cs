using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Lam.FUSION
{
    public class MessageItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI content;
        [SerializeField] private TextMeshProUGUI  _name;

        public void SetContent(string userId, string content)
        {
            string message = $"{content}";
            this.content.text = message;
            this._name.text = userId;
        }
                public void SetContentPlayer(string content)
        {
            string message = $"{content}";
            this.content.text = message;
        }
    }
}
