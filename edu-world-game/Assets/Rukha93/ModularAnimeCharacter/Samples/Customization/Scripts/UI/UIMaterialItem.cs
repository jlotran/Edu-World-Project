using Rukha93.ModularAnimeCharacter.Customization.UI;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Rukha93.ModularAnimeCharacter.Customization.UI
{
    public class UIMaterialItem : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI m_Title;
        [SerializeField] private LayoutGroup m_ColorPanel;
        [SerializeField] private UIColorItem m_ColorItemPrefab;
        private List<UIColorItem> m_ColorItems = new List<UIColorItem>();

        // Add dictionary to track colorItemIds for each channel
        private Dictionary<string, string> channelColorItemIds = new Dictionary<string, string>();

        // Thêm Dictionary để lưu trữ highlight state của từng colorItem
        private Dictionary<string, Dictionary<string, int>> colorHighlightStates = new Dictionary<string, Dictionary<string, int>>();

        public UIColorPicker ColorPicker { get; set; }

        public string Title
        {
            get => m_Title.text;
            set => m_Title.text = value;
        }

        private void Awake()
        {
            m_ColorItemPrefab.gameObject.SetActive(false);
        }

        public void ResetColors()
        {
            for (int i = 0; i < m_ColorItems.Count; i++)
                m_ColorItems[i].gameObject.SetActive(false);
        }

        // Modify ResetAllColorHighlights to restore saved states
        public void ResetAllColorHighlights()
        {
            foreach (var colorItem in m_ColorItems)
            {
                colorItem.ResetHighlight();
            }
            colorHighlightStates.Clear();
        }

        public void SetupSingleColor(Color currentColor, System.Action<Color, int> onChangeColor)
        {
            UIColorItem item;
            if (m_ColorItems.Count > 0)
            {
                item = m_ColorItems[0];
            }
            else
            {
                item = Instantiate(m_ColorItemPrefab, m_ColorPanel.transform);
                m_ColorItems.Add(item);
            }
            item.gameObject.SetActive(true);
            item.ColorItemId = Title.ToLower(); // This will trigger color update
            Debug.Log($"ColorItemId: {item.ColorItemId}");


            for (int i = 1; i < m_ColorItems.Count; i++)
                m_ColorItems[i].gameObject.SetActive(false);

            item.OnColorSelected = (selectedColor, buttonIndex) =>
            {
                onChangeColor?.Invoke(selectedColor, buttonIndex);
            };

            // item.OnColorButtonClicked = () =>
            // {
            //     ColorPicker.Show(currentColor, onChangeColor);
            // };
        }

        public void AddDoubleColor(Color color1, Color color2, System.Action<Color, int> onChangeColor1, System.Action<Color, int> onChangeColor2)
        {
            
            UIColorItem item = null;
            
            // Tìm item không active trong pool hoặc tạo mới
            if (m_ColorItems.Count > 0)
            {
                // Tìm item không active trong pool
                for (int i = 0; i < m_ColorItems.Count; i++)
                {
                    if (!m_ColorItems[i].gameObject.activeSelf)
                    {
                        item = m_ColorItems[i];
                        break;
                    }
                }
            }

            // Nếu không tìm thấy item trong pool, tạo mới
            if (item == null)
            {
                item = Instantiate(m_ColorItemPrefab, m_ColorPanel.transform);
                m_ColorItems.Add(item);
            }

            // Set colorItemId based on Title
            item.ColorItemId = Title.ToLower(); // This will trigger color update

            // Kích hoạt item và đặt callbacks
            item.gameObject.SetActive(true);
            
            string channelId = $"{Title}_{m_ColorItems.Count}";
            channelColorItemIds[channelId] = m_ColorItems.Count.ToString();
            

            item.OnColorSelected = (selectedColor, buttonIndex) =>
            {
                onChangeColor1?.Invoke(selectedColor, buttonIndex);
                onChangeColor2?.Invoke(selectedColor, buttonIndex);
            };
        }

        // Thêm method mới để lưu trạng thái highlight
        public void SaveHighlightState(string channelId, string colorProperty, int buttonIndex)
        {
            if (!colorHighlightStates.ContainsKey(channelId))
            {
                colorHighlightStates[channelId] = new Dictionary<string, int>();
            }
            colorHighlightStates[channelId][colorProperty] = buttonIndex;
        }

        // Modify HighlightStoredColor method to handle reversed channels
        public void HighlightStoredColor(string colorProperty, int buttonIndex)
        {
            string channelType = colorProperty.Contains("_A_") ? "A" : 
                                colorProperty.Contains("_B_") ? "B" : 
                                colorProperty.Contains("_C_") ? "C" : "";
            
            
            int channelIndex = 0;
            switch (channelType)
            {
                case "A": channelIndex = 0; break;
                case "B": channelIndex = 1; break;
                case "C": channelIndex = 2; break;
            }


            if (channelIndex < m_ColorItems.Count)
            {
                UIColorItem targetColorItem = m_ColorItems[channelIndex];
                if (targetColorItem != null)
                {
                    if (!targetColorItem.gameObject.activeSelf)
                    {
                        targetColorItem.gameObject.SetActive(true);
                    }
                    
                    string colorItemId = $"{Title}_{channelIndex + 1}";
                    SaveHighlightState(colorItemId, colorProperty, buttonIndex);
                    targetColorItem.HighlightButtonByIndex(buttonIndex);
                }
            }
        }

        // Add method to check if a channel has stored highlights
        public bool HasStoredHighlight(string colorItemId, string colorProperty)
        {
            return colorHighlightStates.ContainsKey(colorItemId) &&
                   colorHighlightStates[colorItemId].ContainsKey(colorProperty);
        }

        // Modify RestoreHighlights to handle multiple channels
        public void RestoreHighlights(string channelId)
        {
            if (!colorHighlightStates.ContainsKey(channelId))
                return;

            var channelStates = colorHighlightStates[channelId];
            
            // Group color properties by channel type (A, B, C)
            var channelGroups = channelStates
                .GroupBy(x => x.Key.Contains("_A_") ? "A" : 
                             x.Key.Contains("_B_") ? "B" : 
                             x.Key.Contains("_C_") ? "C" : "");

            foreach (var group in channelGroups)
            {
                foreach (var colorProperty in group)
                {
                    HighlightStoredColor(colorProperty.Key, colorProperty.Value);
                }
            }
        }
    }
}
