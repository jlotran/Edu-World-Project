using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
// using UnityEditor.SearchService;
using UnityEngine.SceneManagement;
using Rukha93.ModularAnimeCharacter.Customization.UI;

namespace Edu_World_Game
{
    public class PanelManager : MonoBehaviour
    {
        public Button[] buttons;
        public GameObject panel;
        public Action OnShowPanel, OnHidePanel, OnHideAllPanels; // Declare the events
        void Start()
        {
            for (int i = 0; i < buttons.Length; i++)
            {
                int index = i;
                buttons[0].onClick.AddListener(() => ShowPanel());
                buttons[1].onClick.AddListener(() => HidePanel());
                buttons[2].onClick.AddListener(() => HideAllPanels());
            }
        }

        void ShowPanel()
        {
            UICustomizationDemo.instance = null;
            SceneManager.LoadScene("StartHathora");
            return;
        }

        void HidePanel()
        {

            OnHidePanel?.Invoke(); // Invoke the event when the panel is hidden
            StartCoroutine(DelayedHidePanel());
        }

        bool ValidateName()
        {
            CheckName checkName = FindAnyObjectByType<CheckName>();
            if (checkName != null)
            {
                return checkName.ValidateName();
            }
            return false;
        }

        IEnumerator DelayedHidePanel()
        {
            yield return new WaitForSeconds(2f); // Delay for 3 seconds
            panel.SetActive(false);
        }

        public void HideAllPanels()
        {
            // Validate name before hiding all panels
            if (!ValidateName())
            {
                return;
            }

            HidePanel();
            OnHideAllPanels?.Invoke(); // Invoke the event when all panels are hidden
        }
    }
}
