using System;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace RGSK
{
    public class AnswerItemView : MonoBehaviour
    {
        [Header("View Component")]
        [SerializeField] Button _button;
        [SerializeField] private TMP_Text _answerText;

        public event Action<string> OnAnswerSelected;

        private void Awake()
        {
            _button = GetComponent<Button>();
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            OnAnswerSelected?.Invoke(_answerText.text);
        }


        public void Show(string answerText)
        {
            _answerText.text = answerText;
        }

    }
}
