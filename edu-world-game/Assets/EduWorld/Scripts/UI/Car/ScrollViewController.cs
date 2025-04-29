using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

namespace Edu_World
{
    public class ScrollViewController : MonoBehaviour
    {
        [SerializeField] private ScrollRect scrollRect;
        [SerializeField] private RectTransform contentPanel;
        [SerializeField] private Button previousButton;
        [SerializeField] private Button nextButton;
        
        private List<RectTransform> items = new List<RectTransform>();
        private int currentStartIndex = 0;
        private int visibleItemCount = 3; // Number of items visible at once
        
        private void Start()
        {
            // Get all immediate children of content panel
            for (int i = 0; i < contentPanel.childCount; i++)
            {
                if (contentPanel.GetChild(i).TryGetComponent(out RectTransform item))
                {
                    items.Add(item);
                }
            }
            
            previousButton.onClick.AddListener(OnPreviousClick);
            nextButton.onClick.AddListener(OnNextClick);
            UpdateButtonStates();
        }
        
        private void OnPreviousClick()
        {
            if (currentStartIndex > 0)
            {
                currentStartIndex--;
                ScrollToCurrentItems();
            }
        }
        
        private void OnNextClick()
        {
            if (currentStartIndex < items.Count - visibleItemCount)
            {
                currentStartIndex++;
                ScrollToCurrentItems();
            }
        }
        
        private void ScrollToCurrentItems()
        {
            // Calculate the target horizontal scroll position
            float scrollPosition = (float)currentStartIndex / (items.Count - visibleItemCount);
            scrollRect.horizontalNormalizedPosition = scrollPosition;
            
            UpdateButtonStates();
        }
        
        private void UpdateButtonStates()
        {
            previousButton.interactable = currentStartIndex > 0;
            nextButton.interactable = currentStartIndex < items.Count - visibleItemCount;
        }
    }
}

