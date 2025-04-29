using System.Collections.Generic;
using UnityEngine;

namespace Edu_World
{
    public enum UIType
    {
        Car,
        Shop,
        GUI,
        // Add more UI types as needed
    }

    public class UIManager : MonoBehaviour
    {
        public static UIManager Instance { get; private set; }

        // Dictionary to track active UI states
        private Dictionary<UIType, bool> activeUIStates = new Dictionary<UIType, bool>();

        private void Awake()
        {
            if (Instance == null)
                Instance = this;
            else
                Destroy(gameObject);
            
            // Initialize dictionary with default values
            foreach (UIType uiType in System.Enum.GetValues(typeof(UIType)))
            {
                activeUIStates[uiType] = false;
            }
        }

        public bool CanToggleUI()
        {
            return !(IsUIActive(UIType.Car) || IsUIActive(UIType.Shop));
        }

        public bool CanInteractWithCarOrShop()
        {
            return !IsUIActive(UIType.GUI);
        }

        public void SetUIState(UIType uiType, bool state)
        {
            if (state) // Nếu muốn bật UI
            {
                // Tắt tất cả UI khác trước
                foreach (UIType type in System.Enum.GetValues(typeof(UIType)))
                {
                    activeUIStates[type] = false;
                }
            }
            // Set trạng thái cho UI được chọn
            activeUIStates[uiType] = state;
        }

        public bool IsUIActive(UIType uiType)
        {
            return activeUIStates.TryGetValue(uiType, out bool isActive) && isActive;
        }
    }
}
