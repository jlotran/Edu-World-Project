using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.UIElements;
using DG.Tweening;

namespace EduWorld.UIComponents
{
    [RequireComponent(typeof(ScrollRect))]
    public class SnapScrollView : MonoBehaviour, IEndDragHandler
    {
        public ScrollRect scrollRect;
        public RectTransform contentRect;
        public RectTransform sampleItemRect;
        public HorizontalLayoutGroup layoutGroup;
        public RectTransform scrollHandleRect;
        public GameObject prefabItemHandle;
        public RectTransform handleTranform;
        public int currentItem;

        [Header("Auto Scroll Settings")]
        [SerializeField] private bool enableAutoScroll = true;
        [SerializeField] private float scrollInterval = 3f;
        private float nextScrollTime;

        [Header("Scroll Settings")]
        [SerializeField] protected Vector3 pageStep = new Vector3(1f, 0f, 0f);

        protected virtual void Start()
        {
            scrollRect = GetComponent<ScrollRect>();
            contentRect = scrollRect.content;
            int itemCount = contentRect.childCount;
            for(int i = 0; i < itemCount - 1 ; i++)
            {
                Instantiate(prefabItemHandle, scrollHandleRect);
            }
            scrollRect.onValueChanged.AddListener(OnScroll);
            nextScrollTime = Time.time + scrollInterval;
        }

        void Update()
        {
            if (!enableAutoScroll) return;
            
            if (Time.time >= nextScrollTime)
            {
                nextScrollTime = Time.time + scrollInterval;
                ScrollToNext();
            }
        }

        private void OnScroll(Vector2 arg0)
        {
            int newCurrentItem = Mathf.RoundToInt(0 - contentRect.localPosition.x / ((sampleItemRect.rect.width + layoutGroup.spacing) * pageStep.x));
            if (newCurrentItem != currentItem)
            {
                if (scrollHandleRect != null && handleTranform != null)
                {
                    // Change the order of the child object
                    handleTranform.SetSiblingIndex(newCurrentItem);
                }
                currentItem = newCurrentItem;
            }
        }

        protected virtual void ScrollToNext()
        {
            int nextItem = (currentItem + 1) % contentRect.childCount;
            currentItem = nextItem;
            contentRect.DOLocalMoveX(-currentItem * (sampleItemRect.rect.width + layoutGroup.spacing) * pageStep.x, 0.5f)
                .SetEase(Ease.OutQuint);
            
            if (scrollHandleRect != null && handleTranform != null)
            {
                handleTranform.SetSiblingIndex(currentItem);
            }
        }

        public void SetAutoScroll(bool enable)
        {
            enableAutoScroll = enable;
            if (enable)
            {
                nextScrollTime = Time.time + scrollInterval;
            }
        }

        public void OnEndDrag(PointerEventData eventData)
        {
            contentRect.DOLocalMoveX(-currentItem * (sampleItemRect.rect.width + layoutGroup.spacing) * pageStep.x, 0.1f).SetEase(Ease.OutQuint);
        } 
    }
}
