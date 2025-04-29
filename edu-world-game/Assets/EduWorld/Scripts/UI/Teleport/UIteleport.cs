using UnityEngine;
using UnityEngine.UI;
using Lam.FUSION;
using System.Collections;
using TMPro;
using DG.Tweening;
using System.Collections.Generic;

namespace LamFusion
{
    public class UIteleport : MonoBehaviour
    {
        public static bool IsTeleporting { get; private set; }

        [SerializeField] private List<GameObject> spawnPoint;
        [SerializeField] private List<Button> btn_teleport;

        [SerializeField] private GameObject PanelLoadingTeleport;
        private CanvasGroup panelLoadingGroup;

        [SerializeField] private string teleportMessage = "Teleport";
        [SerializeField] private TextMeshProUGUI messageText;

        private float fadeDuration = 2f;
        private Sequence fadeSequence;
        private Sequence dotsSequence;
        private string baseMessage;

        private void Start()
        {
            for (int i = 0; i < btn_teleport.Count; i++)
            {
                int index = i;
                btn_teleport[i].onClick.AddListener(() => OnTeleportClick(index));
            }
            panelLoadingGroup = PanelLoadingTeleport.GetComponent<CanvasGroup>();
            
            if (panelLoadingGroup == null)
            {
                panelLoadingGroup = PanelLoadingTeleport.AddComponent<CanvasGroup>();
            }
            
            panelLoadingGroup.alpha = 0;
            PanelLoadingTeleport.SetActive(false);
            
            if (messageText == null && PanelLoadingTeleport != null)
            {
                messageText = PanelLoadingTeleport.GetComponentInChildren<TextMeshProUGUI>();
            }

            baseMessage = teleportMessage;
        }

        private void OnTeleportClick(int buttonIndex)
        {
            var localPlayer = Player.localPlayer;
            if (localPlayer != null && buttonIndex < spawnPoint.Count)
            {
                foreach (var btn in btn_teleport){
                    btn.interactable = false;
                }
                localPlayer.SubcribeTelportCallBack(OnStartTeleport, OnFinishTeleport);
                localPlayer.Teleport(spawnPoint[buttonIndex].transform.position);
            }
        }

        private void FadeIn()
        {
            if (fadeSequence != null)
            {
                fadeSequence.Kill();
            }

            PanelLoadingTeleport.SetActive(true);
            
            if (messageText != null)
            {
                messageText.text = baseMessage;
                AnimateLoadingDots();
            }

            fadeSequence = DOTween.Sequence()
                .Append(panelLoadingGroup.DOFade(1, fadeDuration));
        }

        private void FadeOut()
        {
            if (fadeSequence != null)
            {
                fadeSequence.Kill();
            }

            fadeSequence = DOTween.Sequence()
                .Append(panelLoadingGroup.DOFade(0, fadeDuration))
                .OnComplete(() => PanelLoadingTeleport.SetActive(false));
        }

        private void AnimateLoadingDots()
        {
            if (dotsSequence != null)
            {
                dotsSequence.Kill();
            }

            dotsSequence = DOTween.Sequence()
                .AppendCallback(() => messageText.text = baseMessage)
                .AppendInterval(0.2f)
                .AppendCallback(() => messageText.text = baseMessage + ".")
                .AppendInterval(0.2f)
                .AppendCallback(() => messageText.text = baseMessage + "..")
                .AppendInterval(0.2f)
                .AppendCallback(() => messageText.text = baseMessage + "...")
                .AppendInterval(0.2f)
                .SetLoops(-1);
        }

        private void OnStartTeleport()
        {
            IsTeleporting = true;
            FadeIn();
        }

        private void OnFinishTeleport()
        {
            IsTeleporting = false;
            foreach (var btn in btn_teleport){
                btn.interactable = true;
            }
            
            if (dotsSequence != null)
            {
                dotsSequence.Kill();
                dotsSequence = null;
            }
            
            FadeOut();
        }
        
        private void OnDestroy()
        {
            if (dotsSequence != null)
            {
                dotsSequence.Kill();
                dotsSequence = null;
            }
            
            if (fadeSequence != null)
            {
                fadeSequence.Kill();
                fadeSequence = null;
            }
        }
    }
}
