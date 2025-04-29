using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

[RequireComponent(typeof(ScrollRect))]
public class SnappingScrollView : MonoBehaviour
{
    public float snapDuration = 0.25f;
    public Ease snapEase = Ease.OutCubic;

    private ScrollRect scrollRect;
    private RectTransform contentRect;
    private float itemWidth;
    private int totalItemCount;
    private int currentIndex = 0;

    public Button nextButton;
    public Button previousButton;

    public RectTransform scrollHandleRect;
    public GameObject prefabItemHandle;
    public RectTransform handleTranform;


    private List<GameObject> handleItems = new List<GameObject>();

    private void Awake()
    {
        scrollRect = GetComponent<ScrollRect>();
    }

    private void Start()
    {
        nextButton.onClick.AddListener(ScrollNext);
        previousButton.onClick.AddListener(ScrollPrevious);
        StartCoroutine(InitAfterDelay());
    }

    private IEnumerator InitAfterDelay()
    {
        yield return new WaitForSeconds(0.1f);

        contentRect = scrollRect.content;

        if (contentRect.childCount >= 2)
        {
            RectTransform first = contentRect.GetChild(0) as RectTransform;
            RectTransform second = contentRect.GetChild(1) as RectTransform;
            itemWidth = Mathf.Abs(second.localPosition.x - first.localPosition.x);
        }

        totalItemCount = contentRect.childCount;
        totalItemCount = 0;
        for (int i = 0; i < contentRect.childCount; i++)
        {
            if (contentRect.GetChild(i).gameObject.activeSelf)
            {
                totalItemCount++;
            }
        }
        CreateHandles();

        float initialX = -itemWidth * currentIndex;
        contentRect.anchoredPosition = new Vector2(initialX, contentRect.anchoredPosition.y);

        UpdateButtonInteractable();
    }

    private void ClearHandles()
    {
        foreach (var handle in handleItems)
        {
            if (handle != null)
                Destroy(handle);
        }
        handleItems.Clear();
    }

    private void CreateHandles()
    {
        ClearHandles();
        int handleCount = (totalItemCount / 3)+1;

        for (int i = 0; i < handleCount; i++)
        {
            GameObject handle = Instantiate(prefabItemHandle, scrollHandleRect);
            handleItems.Add(handle);
        }
    }

    public void SnapToIndex(int index)
    {
        currentIndex = Mathf.Clamp(index, 0, totalItemCount - 1);
        float targetX = -itemWidth * currentIndex;

        contentRect.DOAnchorPosX(targetX, snapDuration)
            .SetEase(snapEase)
            .OnComplete(() =>
            {
                if (scrollHandleRect != null && handleTranform != null)
                {
                    handleTranform.SetSiblingIndex(currentIndex);
                }
                UpdateButtonInteractable();
            });
    }

    public void ScrollNext()
    {
        SnapToIndex(currentIndex + 1);
    }

    public void ScrollPrevious()
    {
        SnapToIndex(currentIndex - 1);
    }
    public void UpdateTotalItemCount(int newCount)
    {
        totalItemCount = newCount;
        currentIndex = 0;
        float targetX = -itemWidth * currentIndex;
        contentRect.anchoredPosition = new Vector2(targetX, contentRect.anchoredPosition.y);
        CreateHandles();
        UpdateButtonInteractable();
    }

    private void OnDestroy()
    {
        ClearHandles();
    }

    private void UpdateButtonInteractable()
    {
        previousButton.interactable = currentIndex > 0;
        nextButton.interactable = currentIndex < (totalItemCount / 3) + 1;
    }

    public int GetCurrentIndex() => currentIndex;
}
