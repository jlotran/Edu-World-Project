namespace Fusion.Menu
{
    using UnityEngine;
    using DG.Tweening;
    using UnityEngine.UI;
    using Fusion.Menu;
    using System.Collections.Generic;
    using HathoraCloud;
    using HathoraCloud.Models.Shared;
    using System.Threading.Tasks;
    using Fusion.Menu.Lam.FUSION;
    using System;
    using System.Collections;
    using UnityEngine.SceneManagement;
    using Lam.FUSION;
    using global::Lam.FUSION;

    namespace Lam.FUSION
    {
        public class FusionMenuMainUI : PhotonMenuUIScreen
        {
            [Header("Panel")]
            [SerializeField] private RectTransform parentPanel;
            [SerializeField] private RectTransform chatPanel;
            [SerializeField] private ListRoom listRoom;

            [Header("Button")]
            [SerializeField] private Button showPanelButton;
            [SerializeField] private Button closePanelButton;
            [SerializeField] private Button Escape_btn;
            [SerializeField] private Button showServerSelectButton;
            [SerializeField] private Button quickJoinButton;

            private FusionMenuManager photonMenuUIMain => FusionMenuManager.instance;

            public override void Start()
            {
                quickJoinButton.onClick?.AddListener(OnQuickJoinButtonClicked);
                showServerSelectButton.onClick.AddListener(OnShowServerBtn);
                // Set initial position of child panel
                chatPanel.anchoredPosition = new Vector2(-800f, chatPanel.anchoredPosition.y);
                Escape_btn.onClick.AddListener(OnEscapeButtonClicked);
            }

            private async void OnEscapeButtonClicked()
            {
                await FusionMenuManager.instance.JoinRandomRoom(GameRoomType.racing);
            }

            private async void OnQuickJoinButtonClicked()
            {
                StartCoroutine(PreventSpamQuickJoinButton());
                await photonMenuUIMain.JoinRandomRoom();
            }

            private IEnumerator PreventSpamQuickJoinButton()
            {
                quickJoinButton.interactable = false;
                yield return new WaitForSeconds(3f);
                quickJoinButton.interactable = true;
            }

            private async void OnShowServerBtn()
            {
                listRoom.Show();
                List<LobbyV3> rooms = await photonMenuUIMain.GetRooms(GameRoomType.city);
                listRoom.SetDataRooms(rooms);
            }

            private void OnEnable()
            {
                showPanelButton?.onClick.AddListener(OnShowPanelButtonClicked);
                closePanelButton?.onClick.AddListener(OnClosePanelButtonClicked);
            }

            private void OnDisable()
            {
                showPanelButton?.onClick.RemoveListener(OnShowPanelButtonClicked);
                closePanelButton?.onClick.RemoveListener(OnClosePanelButtonClicked);
            }

            private void OnShowPanelButtonClicked()
            {
                ShowPanels();
            }

            private void OnClosePanelButtonClicked()
            {
                HidePanels();
            }

            public void ShowPanels()
            {
                // Create sequence for animations
                Sequence sequence = DOTween.Sequence();

                // Parent panel appears instantly (you can set alpha from 0 to 1 if needed)
                parentPanel.gameObject.SetActive(true);

                // Child panel slides in from left
                sequence.Append(chatPanel.DOAnchorPosX(0f, 0.5f).SetEase(Ease.OutBack));
            }

            public void HidePanels()
            {
                // Create sequence for hiding
                Sequence sequence = DOTween.Sequence();

                // Slide child panel out first
                sequence.Append(chatPanel.DOAnchorPosX(-800f, 0.5f).SetEase(Ease.InBack));

                // Hide parent panel after child panel animation
                sequence.OnComplete(() => parentPanel.gameObject.SetActive(false));
            }
        }
    }
}
