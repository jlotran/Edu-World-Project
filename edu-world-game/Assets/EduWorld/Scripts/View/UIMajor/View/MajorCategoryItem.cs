using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class MajorCategoryItem : MonoBehaviour
{
    [SerializeField] private TMP_Text categoryNameText;
    [SerializeField] private Button categoryButton;

    private string majorName;

    public void Initialize(string major, string category, System.Action<string, string> onCategorySelected)
    {
        majorName = major;
        categoryNameText.text = category;
        categoryButton.onClick.RemoveAllListeners();
        categoryButton.onClick.AddListener(() => onCategorySelected?.Invoke(majorName, category));
    }

    private void Reset()
    {
        categoryNameText = GetComponentInChildren<TMP_Text>();
        categoryButton = GetComponent<Button>();
    }

    public string GetMajorName()
    {
        return majorName;
    }
}