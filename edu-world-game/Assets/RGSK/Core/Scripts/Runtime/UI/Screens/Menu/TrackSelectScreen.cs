using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Events;
using RGSK.Extensions;
using RGSK.Helpers;
using DG.Tweening;

namespace RGSK
{
    public class TrackSelectScreen : UIScreen
    {
        [SerializeField] SelectionItemEntry entryPrefab;
        [SerializeField] ScrollRect scrollView;
        [SerializeField] TrackDefinitionUI trackDefinitionUI;
        [SerializeField] Button selectButton;
        [SerializeField] bool createLockedItemEntries = true;

        [SerializeField] GameObject requirementPref;
        [SerializeField] GameObject requirementContainer;

        [SerializeField] GameObject rewardPref;
        [SerializeField] GameObject rewardContainer;


        public UnityEvent OnSelected;

        TrackDefinition _selected;

        public override void Initialize()
        {
            PopulateTrackSelection();

            if (selectButton != null)
            {
                selectButton.onClick.AddListener(() => Select());
            }

            base.Initialize();
        }

        // public override void Open()
        // {
        //     base.Open();

        //     if (RGSKCore.Instance.ContentSettings.tracks.Count > 0)
        //     {
        //         var index = 0;

        //         if (RGSKCore.runtimeData.SelectedTrack != null)
        //         {
        //             index = RGSKCore.Instance.ContentSettings.tracks.IndexOf(RGSKCore.runtimeData.SelectedTrack);

        //             if (index < 0)
        //             {
        //                 index = 0;
        //             }
        //         }

        //         UpdateTrack(RGSKCore.Instance.ContentSettings.tracks[index]);
        //         StartCoroutine(scrollView?.SelectChild(index));
        //     }
        // }
        public override void Open()
        {
            base.Open();

            if (RGSKCore.Instance.ContentSettings.tracks.Count > 0)
            {
                // Always select track index 0
                var index = 0;

                UpdateTrack(RGSKCore.Instance.ContentSettings.tracks[index]);
                Select();
                StartCoroutine(scrollView?.SelectChild(index));
            }
        }

        public override void Close()
        {
            base.Close();
            _selected = null;
        }

        void UpdateTrack(TrackDefinition definition)
        {
            if (_selected == definition)
                return;

            _selected = definition;
            trackDefinitionUI?.UpdateUI(_selected);
            UpdateUIElements(_selected, requirementContainer, requirementPref, nameof(RequirementUI), _selected?.requirements);
            UpdateUIElements(_selected, rewardContainer, rewardPref, nameof(RewardUI), _selected?.rewards);
        }

        void UpdateUIElements<T>(TrackDefinition track, GameObject container, GameObject prefab, string componentName, IList<T> elements) where T : class
        {
            // Check if track is null
            if (track == null)
            {
                return;
            }

            // Check if container and prefab are assigned
            if (container == null || prefab == null)
            {
                return;
            }

            // Check if elements exist
            if (elements == null || elements.Count == 0)
            {
                container.DestroyAllChildren();
                return;
            }

            // Clear existing elements
            container.DestroyAllChildren();

            // Create new elements
            foreach (var element in elements)
            {
                if (element == null)
                {
                    continue;
                }

                var elementObj = Instantiate(prefab, container.transform);
                elementObj.SetActive(true);

                var uiComponent = elementObj.GetComponent(componentName);
                if (uiComponent != null)
                {
                    var method = uiComponent.GetType().GetMethod("Set" + componentName.Replace("UI", ""));
                    method?.Invoke(uiComponent, new object[] { element });
                    // Debug.Log($"[UpdateUIElements] Created {componentName}: {element}");
                }
                else
                {
                    Debug.LogError($"[UpdateUIElements] {componentName} component missing on prefab");
                }
            }
        }

        void Select()
        {
            if (!_selected.IsUnlocked())
            {
                GeneralHelper.PurchaseItem(
                    item: _selected,
                    OnSuccess: () =>
                    {
                        var current = _selected;
                        _selected = null;
                        UpdateTrack(current);
                    },
                    OnFail: () => { }
                    );

                return;
            }

            if (!_selected.IsUnlocked())
                return;

            RGSKCore.runtimeData.SelectedTrack = _selected;
            OnSelected.Invoke();
        }

        void PopulateTrackSelection()
        {
            if (scrollView == null)
                return;

            scrollView.content.gameObject.DestroyAllChildren();

            foreach (var item in RGSKCore.Instance.ContentSettings.tracks)
            {
                if (!createLockedItemEntries && !item.IsUnlocked())
                    continue;

                if (item != null)
                {
                    var e = Instantiate(entryPrefab, scrollView.content);
                    e.gameObject.SetActive(false);
                    e.Setup(
                    item: item,
                    col: Color.white,
                    onSelect: () =>
                    {
                        if (!GeneralHelper.IsMobilePlatform())
                        {
                            UpdateTrack(item);
                        }
                    },
                    onClick: () =>
                    {
                        if (GeneralHelper.IsMobilePlatform())
                        {
                            UpdateTrack(item);
                        }
                        else
                        {
                            Select();
                        }
                    });
                }
            }
        }
    }
}