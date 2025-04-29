using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;
using UnityEngine.EventSystems;

namespace Edu_World
{
    public class UIButtonSelection : MonoBehaviour
    {
        // Dictionary to track currently selected button for each group
        public bool isSelected;
        [Header("Group Settings")]
        [SerializeField] private string buttonGroup = "Default";
        
        [SerializeField] private Image buttonImage;
        // [SerializeField] private Button button;
        
        [Header("Settings")]
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite selectedSprite;

        [Header("Animation Settings")]
        [SerializeField] private float transitionDuration = 0.4f;
        [SerializeField] private float scaleMultiplier = 1.1f;
        private Vector3 originalScale;

        private void Start()
        {
            // Find button component if not assigned
            // if (button == null)
            //     button = GetComponent<Button>();

            // if (button == null)
            // {
            //     Debug.LogError("UIButtonSelection: No Button component found!");
            //     return;
            // }

            // Find image component in children if not assigned
            if (buttonImage == null)
                buttonImage = GetComponentInChildren<Image>();

            if (buttonImage == null)
            {
                Debug.LogError("UIButtonSelection: No Image component found in children!");
                return;
            }

            // Set initial sprite
            buttonImage.sprite = normalSprite;

            // Add button click listener
            // button.onClick.AddListener(HandleSelection);

            originalScale = buttonImage.transform.localScale;
        }

        public void HandleSelection()
        {
            isSelected = true;
            TransitionToSelected();
        }

        public void TransitionToSelected()
        {
            Sequence sequence = DOTween.Sequence();

            // Scale down
            sequence.Append(buttonImage.transform.DOScale(originalScale * 0.9f, transitionDuration * 0.4f).SetEase(Ease.OutSine));

            // Change sprite
            sequence.AppendCallback(() => buttonImage.sprite = selectedSprite);

            // Scale up with bounce
            sequence.Append(buttonImage.transform.DOScale(originalScale * scaleMultiplier, transitionDuration * 0.6f).SetEase(Ease.OutBack));
        }

        public void TransitionToNormal()
        {
            isSelected = false;

            Sequence sequence = DOTween.Sequence();

            // Scale down
            sequence.Append(buttonImage.transform.DOScale(originalScale * 0.9f, transitionDuration * 0.4f).SetEase(Ease.OutSine));

            // Change sprite
            sequence.AppendCallback(() => buttonImage.sprite = normalSprite);

            // Scale back to normal
            sequence.Append(buttonImage.transform.DOScale(originalScale, transitionDuration * 0.6f).SetEase(Ease.OutBack));
        }

        private void OnDestroy()
        {            
            if (buttonImage != null)
                buttonImage.DOKill();
            buttonImage.transform.DOKill();
        }
    }
}
