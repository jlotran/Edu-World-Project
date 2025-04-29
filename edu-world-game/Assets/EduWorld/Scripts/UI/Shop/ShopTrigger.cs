using System.Linq;
using LamFusion;
using Rukha93.ModularAnimeCharacter.Customization;
using UnityEngine;
using Lam.FUSION;

namespace Edu_World
{
    public class ShopTrigger : MonoBehaviour
    {
        #region Singleton
        public static ShopTrigger instance { get; private set; }
        
        private void Awake()
        {
            if (instance == null)
                instance = this;
            else
                Destroy(gameObject);
        }
        #endregion

        #region Panel Setup
        [Header("Setup Prefab và Container")]
        [SerializeField] private UIShop shopPanelPrefab;
        [SerializeField] private Transform container;
        private static GameObject currentPanel;
        private UIShop CurrentUIShop => currentPanel?.GetComponent<UIShop>();
        #endregion

        public void OnTriggerActivated()
        {
            if (IsPanelActive())
            {
                HideShopPanel();
                LamFusion.Camera.instance.TurnCameraPlayer();
                return;
            }
            ShowShopPanel();
        }

        private bool IsPanelActive() => currentPanel != null && currentPanel.activeSelf;

        public void ShowShopPanel()
        {
            if (!shopPanelPrefab || !container) return;

            LamFusion.Camera.instance.SetShopCamera(true);
            UIInteract.instance.SetShopState(true);
            UIManager.Instance.SetUIState(UIType.Shop, true);

            if (currentPanel == null)
            {
                CreateNewPanel();
            }
            else
            {
                currentPanel.SetActive(true);
            }
        }

        private void CreateNewPanel()
        {
            currentPanel = Instantiate(shopPanelPrefab.gameObject, container);
            if (CurrentUIShop == null) return;
            
            CurrentUIShop.InitializeUI();
            CurrentUIShop.btn_CloseShop.onClick.AddListener(HideShopPanel);
        }

        public void HideShopPanel()
        {
            if (currentPanel == null) return;

            currentPanel.SetActive(false);
            UIInteract.instance.SetShopState(false);
            UIManager.Instance.SetUIState(UIType.Shop, false);
            
            CurrentUIShop?.OnShirtOffClicked();
        }
    }
}