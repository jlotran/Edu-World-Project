using System;
using System.Collections;
using System.Collections.Generic;
using HathoraCloud.Models.Shared;
using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
using Edu_World;
using Fusion.Menu.Lam.FUSION;
using Lam.FUSION;
using Lam.GAMEPLAY;
using Rukha93.ModularAnimeCharacter.Customization;
using Edu_World.Inventory;

namespace Lam.UI
{
    public class GameUIManager : MonoBehaviour
    {
        private Fusion.Menu.FusionMenuManager fusionMenuManager => Fusion.Menu.FusionMenuManager.instance;
        [SerializeField] private Button setting_btn;
        [SerializeField] private Button serverList_btn;
        [SerializeField] private ListRoom listRoomRacing;
        [SerializeField] private GameObject grapicsHolderPanel;
        [SerializeField] private Button Inventory_btn;
        [SerializeField] private InventoryManager _inventoryManager;
        [SerializeField] private Camera cameraCarInventory;
        public Camera cameraPlayerInventory;

        private UIInventoryCustomization currentUIInventory => GameObject.FindAnyObjectByType<UIInventoryCustomization>();

        private void Start()
        {
            setting_btn.onClick.AddListener(OnSettingBtn);
            serverList_btn.onClick.AddListener(OnGetListRoom);
            Inventory_btn.onClick.AddListener(() =>
            {
                if (currentUIInventory != null)
                {
                    _inventoryManager.OpenInventory();
                }
            });
            grapicsHolderPanel.SetActive(false);
            StartCoroutine(waitPlayerInteract());
            StartCoroutine(waitPlayer());
            cameraCarInventory.enabled = false;
            cameraPlayerInventory = null;
        }

        private IEnumerator waitPlayer()
        {
            while(Player.localPlayer == null)
            {
                yield return null;
            }
            if (cameraPlayerInventory == null)
            {
                var cameraObj = GameObject.FindGameObjectWithTag("PlayerCameraInventory");
                if (cameraObj != null)
                {
                    cameraPlayerInventory = cameraObj.GetComponent<Camera>();
                }
            }
        }

        private IEnumerator waitPlayerInteract()
        {
            while (PlayerInteract.LocalInstance == null)
            {
                yield return null;
            }
            PlayerInteract.LocalInstance.OnActionGUI += TogglePanel;
        }

        private async void OnSettingBtn()
        {
            await fusionMenuManager.OnQuitMatch();
        }

        private async void OnGetListRoom()
        {
            listRoomRacing.gameObject.SetActive(true);
            List<LobbyV3> listRoom = await fusionMenuManager.GetRooms(GameRoomType.racing); // it bua dua them level do
            listRoomRacing.SetDataRooms(listRoom);
        }

        public void ShowPanel()
        {
            grapicsHolderPanel.SetActive(true);
            UIManager.Instance.SetUIState(UIType.GUI, true);
            InputManager.instance.input.SetCursorState(false);
            cameraCarInventory.enabled = true;
            if (cameraPlayerInventory != null)
            cameraPlayerInventory.enabled = true;
            currentUIInventory?.InitializeUI();
        }

        public void HidePanel()
        {
            grapicsHolderPanel.SetActive(false);
            UIManager.Instance.SetUIState(UIType.GUI, false);
            InputManager.instance.input.SetCursorState(true);
            cameraCarInventory.enabled = false;
            if (cameraPlayerInventory != null)
            cameraPlayerInventory.enabled = false;
            _inventoryManager.CloseInventory();
        }

        public void TogglePanel()
        {
            if (grapicsHolderPanel.activeSelf)
            {
                HidePanel();
            }
            else
            {
                ShowPanel();
            }
        }

        private void OnDestroy()
        {
            if (PlayerInteract.LocalInstance != null)
            {
                PlayerInteract.LocalInstance.OnActionGUI -= TogglePanel;
            }
        }
    }
}
