using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using System.Collections;
using System.Collections.Generic;

namespace Edu_World_Game
{
    public class TweenManagerUI : MonoBehaviour
    {
        public List<RectTransform> rectTransforms; // List of RectTransforms
        public List<RectTransform> buttonTransform;
        public float moveDuration = 1f;

        // public RectTransform rawImageTransform;
        // public float scaleDuration = 1f;
        // public float rotateDuration = 1f;
        // public float maxScale = 1f;

        private Sequence sequence;

        public List<RectTransform> background;
        public RectTransform textName;
        public Button[] buttons; // Array of buttons to disable/enable

        private PanelManager panelManager;
        private CheckName checkName;

        void Start()
        {
            panelManager = FindAnyObjectByType<PanelManager>();
            if (panelManager != null)
            {
                panelManager.OnShowPanel += ShowPanelName; // Subscribe to the event
                panelManager.OnHidePanel += HidePanelName; // Subscribe to the event
                panelManager.OnHideAllPanels += HideAllPanel; // Subscribe to the event
            }

            foreach (var rectTransform in rectTransforms)
            {
                rectTransform.anchoredPosition = new Vector2(rectTransform.anchoredPosition.x, 1500);
            }
            buttonTransform[0].anchoredPosition = new Vector2(buttonTransform[0].anchoredPosition.x,-1167.8f);
            buttonTransform[1].anchoredPosition = new Vector2(buttonTransform[1].anchoredPosition.x, -1167.8f);
            textName.anchoredPosition = new Vector2(textName.anchoredPosition.x, 1300);
            // rawImageTransform.localScale = Vector3.zero;
            foreach (var rectTransform in background)
            {
                rectTransform.localScale = Vector3.zero;
            }

            // Create a sequence for the animations
            sequence = DOTween.Sequence();
            // sequence.Append(rawImageTransform.DOScale(maxScale, scaleDuration).SetEase(Ease.OutBack))
                    sequence
                    .AppendCallback(() =>
                    {
                        foreach (var rectTransform in rectTransforms)
                        {
                            rectTransform.DOAnchorPosY(-61.52069f, moveDuration).SetEase(Ease.InOutSine);
                        }
                    })
                    .Append(buttonTransform[0].DOAnchorPosY(-581.666f, moveDuration).SetEase(Ease.InOutSine))
                    .OnKill(() =>
                    {
                        sequence = null;
                    });

            checkName = FindAnyObjectByType<CheckName>();
        }

        private void OnDestroy()
        {
            sequence?.Kill();

            if (panelManager != null)
            {
                panelManager.OnShowPanel -= ShowPanelName; // Unsubscribe from the event
                panelManager.OnHidePanel -= HidePanelName; // Unsubscribe from the event
                panelManager.OnHideAllPanels -= HideAllPanel; // Unsubscribe from the event
            }
        }

        private void ShowPanelName()
        {
            SetButtonsInteractable(false);
            // background[0].DOScale(maxScale, 1.2f).SetEase(Ease.OutBack).OnComplete(() =>
            // {
            //     background[1].DOScale(maxScale, 0f).SetEase(Ease.OutBack).OnComplete(() =>
            //     {
            //         SetButtonsInteractable(true);
            //     });
            // });
        }
        private void HidePanelName()
        {

            background[1].DOScale(0, 0f).SetEase(Ease.InBack).OnComplete(() =>
            {
                background[0].DOScale(0, 1.2f).SetEase(Ease.InBack);
            });

        }
        public void HideAllPanel()
        {
            // Disable buttons at the start of the animation
            SetButtonsInteractable(false);


            sequence = DOTween.Sequence();
            sequence.Append(background[1].DOScale(0, 0f).SetEase(Ease.InBack))
                    .Join(background[0].DOScale(0, 1.2f).SetEase(Ease.InBack))
                    .AppendCallback(() =>
                    {
                        foreach (var rectTransform in rectTransforms)
                        {
                            rectTransform.DOAnchorPosY(1500, moveDuration).SetEase(Ease.InOutSine);
                        }
                    })
                    .Append(buttonTransform[0].DOAnchorPosY(-150f, moveDuration).SetEase(Ease.InOutSine))
                    // .Join(rawImageTransform.DOAnchorPosX(168, moveDuration))
                    // .Join(rawImageTransform.DOScale(0.8f, scaleDuration).SetEase(Ease.InBack))

                    .Append(textName.DOAnchorPosY(-120f, moveDuration).SetEase(Ease.InOutSine))
                    .Join(buttonTransform[1].DOAnchorPosY(100f, moveDuration).SetEase(Ease.InOutSine))
                    .OnComplete(() =>
                    {
                        SetButtonsInteractable(true);
                    })
                    .OnKill(() =>
                    {
                        sequence = null;
                    });
        }

        private void SetButtonsInteractable(bool interactable)
        {
            foreach (var button in buttons)
            {
                button.interactable = interactable;
            }
        }
    }
}
