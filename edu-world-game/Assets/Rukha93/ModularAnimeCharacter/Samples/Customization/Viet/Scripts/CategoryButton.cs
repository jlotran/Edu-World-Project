using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class CategoryButton : MonoBehaviour
{
    public RectTransform categoryTransform;
    private Vector2 originalPos;
    
    void Start()
    {
        originalPos = categoryTransform.anchoredPosition;
    }

    public void OnCategorySelected()
    {
        // Đưa tất cả category về trạng thái ban đầu
        ResetAllCategories();

        // Di chuyển nhẹ sang phải (X + 20) và phóng to
        categoryTransform.DOAnchorPos(new Vector2(originalPos.x + 20, originalPos.y), 0.3f).SetEase(Ease.OutQuad);
        categoryTransform.DOScale(Vector3.one * 1.1f, 0.3f).SetEase(Ease.OutBack);
    }

    [System.Obsolete]
    void ResetAllCategories()
    {
        foreach (CategoryButton button in FindObjectsOfType<CategoryButton>())
        {
            button.categoryTransform.DOAnchorPos(button.originalPos, 0.3f);
            button.categoryTransform.DOScale(Vector3.one, 0.3f);
        }
    }
}
