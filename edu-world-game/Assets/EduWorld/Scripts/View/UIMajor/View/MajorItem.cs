using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MajorItem : MonoBehaviour
{
    [SerializeField] private TMP_Text majorNameText;
    [SerializeField] private Button majorButton;

    public void Initialize(string majorName, System.Action<string> onMajorSelected)
    {
        majorNameText.text = majorName;
        majorButton.onClick.RemoveAllListeners();
        majorButton.onClick.AddListener(() => onMajorSelected?.Invoke(majorName));
    }

    private void Reset()
    {
        majorNameText = GetComponentInChildren<TMP_Text>();
        majorButton = GetComponent<Button>();
    }
}