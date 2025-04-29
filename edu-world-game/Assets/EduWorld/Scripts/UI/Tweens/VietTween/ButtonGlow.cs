using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;
using System.Collections.Generic;

namespace Edu_World
{
    [RequireComponent(typeof(Button))]
    public class ButtonGlow : MonoBehaviour
    {
        // Dictionary to track currently selected button for each group
        private static Dictionary<string, ButtonGlow> selectedButtons = new Dictionary<string, ButtonGlow>();
        
        [Header("Group Settings")]
        [SerializeField] private string buttonGroup = "Default";
        
        [Header("References")]
        [SerializeField] private Button button;
        [SerializeField] private Image buttonImage;
        
        [Header("Settings")]
        [SerializeField] private Sprite normalSprite;
        [SerializeField] private Sprite selectedSprite;

        [Header("Animation Settings")]
        [SerializeField] private float transitionDuration = 0.4f;
        [SerializeField] private float scaleMultiplier = 1.1f;
        private Vector3 originalScale;

        [Header("Initial State")]
        [SerializeField] private bool isInitiallySelected = false;

        private void Start()
        {
            // Get button component if not assigned
            if (button == null)
                button = GetComponent<Button>();

            // Find image component in children if not assigned
            if (buttonImage == null)
                buttonImage = GetComponentInChildren<Image>();

            if (buttonImage == null)
            {
                Debug.LogError("ButtonGlow: No Image component found in children!");
                return;
            }

            // Set initial sprite
            buttonImage.sprite = normalSprite;

            // Add button click listener
            button.onClick.AddListener(HandleSelection);

            originalScale = buttonImage.transform.localScale;

            // Handle initial selection
            if (isInitiallySelected)
            {
                HandleSelection();
            }
        }

        private void HandleSelection()
        {
            // If this button is already selected in its group, do nothing
            if (selectedButtons.ContainsKey(buttonGroup) && selectedButtons[buttonGroup] == this)
                return;

            // Deselect previous button in this group if exists
            if (selectedButtons.ContainsKey(buttonGroup) && selectedButtons[buttonGroup] != null)
            {
                selectedButtons[buttonGroup].TransitionToNormal();
            }

            // Set this as current selected for this group and transition
            selectedButtons[buttonGroup] = this;
            TransitionToSelected();
        }

        public void TransitionToSelected()
        {
            // Kill any ongoing tweens
            buttonImage.DOKill();
            buttonImage.transform.DOKill();

            Sequence sequence = DOTween.Sequence();

            // Fade and scale out
            sequence.Append(buttonImage.DOFade(0f, transitionDuration * 0.4f).SetEase(Ease.InSine));
            sequence.Join(buttonImage.transform.DOScale(originalScale * 0.9f, transitionDuration * 0.4f).SetEase(Ease.OutSine));

            // Change sprite at the midpoint
            sequence.AppendCallback(() => buttonImage.sprite = selectedSprite);

            // Fade and scale in with slight bounce
            sequence.Append(buttonImage.DOFade(1f, transitionDuration * 0.6f).SetEase(Ease.OutSine));
            sequence.Join(buttonImage.transform.DOScale(originalScale * scaleMultiplier, transitionDuration * 0.6f).SetEase(Ease.OutBack));
        }

        public void TransitionToNormal()
        {
            // Kill any ongoing tweens
            buttonImage.DOKill();
            buttonImage.transform.DOKill();

            Sequence sequence = DOTween.Sequence();

            // Fade and scale out
            sequence.Append(buttonImage.DOFade(0f, transitionDuration * 0.4f).SetEase(Ease.InSine));
            sequence.Join(buttonImage.transform.DOScale(originalScale * 0.9f, transitionDuration * 0.4f).SetEase(Ease.OutSine));

            // Change sprite at the midpoint
            sequence.AppendCallback(() => {
                buttonImage.sprite = normalSprite;
                if (selectedButtons.ContainsKey(buttonGroup) && selectedButtons[buttonGroup] == this)
                    selectedButtons.Remove(buttonGroup);
            });

            // Fade and scale in
            sequence.Append(buttonImage.DOFade(1f, transitionDuration * 0.6f).SetEase(Ease.OutSine));
            sequence.Join(buttonImage.transform.DOScale(originalScale, transitionDuration * 0.6f).SetEase(Ease.OutBack));
        }

        private void OnDestroy()
        {
            if (button != null)
                button.onClick.RemoveAllListeners();
            
            if (buttonImage != null)
                buttonImage.DOKill();
            
            // Clear group reference if this was the selected button
            if (selectedButtons.ContainsKey(buttonGroup) && selectedButtons[buttonGroup] == this)
                selectedButtons.Remove(buttonGroup);

            buttonImage.transform.DOKill();
        }

        // Optional: Method to get current selected button in a specific group
        public static ButtonGlow GetSelectedInGroup(string group)
        {
            return selectedButtons.ContainsKey(group) ? selectedButtons[group] : null;
        }
    }
}
