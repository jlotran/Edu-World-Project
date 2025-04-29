using UnityEngine;
using UnityEditor;
using System;
using System.Collections.Generic;
using System.Linq;
using RGSK.Helpers;

namespace RGSK.Editor
{
    [InitializeOnLoad]
    public class RGSKWindow : EditorWindow, IHasCustomMenu
    {
        static string[] tabs = new string[]
        {
            "Welcome",
            "Setup",
            "General",
            "Scene",
            "Content",
            "Race",
            "AI",
            "UI",
            "Audio",
            "Input",
            "Vehicle",
            "Integrations"
        };

        static int tabIndex;
        int tabSize = 250;
        Vector2 scrollPosition;
        int supportButtonSize = 30;

        string[] contentTabs = new string[]
        {
            "Vehicles",
            "Tracks"
        };
        static int contentTabIndex;
        Vector2 contentTabScrollPosition;

        SerializedObject serializedCore;
        SerializedProperty generalSettings;
        SerializedProperty sceneSettings;
        SerializedProperty contentSettings;
        SerializedProperty raceSettings;
        SerializedProperty aiSettings;
        SerializedProperty uiSettings;
        SerializedProperty audioSettings;
        SerializedProperty inputSettings;
        SerializedProperty vehicleSettings;

        static RGSKWindow()
        {
            EditorApplication.delayCall += () =>
            {
                EditorHelper.LoadRGSKCoreSettings();
            };
        }

        void OnEnable()
        {
            if (serializedCore == null)
            {
                serializedCore = new SerializedObject(RGSKCore.Instance);
                generalSettings = serializedCore.FindProperty(nameof(generalSettings));
                sceneSettings = serializedCore.FindProperty(nameof(sceneSettings));
                contentSettings = serializedCore.FindProperty(nameof(contentSettings));
                raceSettings = serializedCore.FindProperty(nameof(raceSettings));
                aiSettings = serializedCore.FindProperty(nameof(aiSettings));
                uiSettings = serializedCore.FindProperty(nameof(uiSettings));
                audioSettings = serializedCore.FindProperty(nameof(audioSettings));
                inputSettings = serializedCore.FindProperty(nameof(inputSettings));
                vehicleSettings = serializedCore.FindProperty(nameof(vehicleSettings));
            }

            EditorHelper.LoadRGSKCoreSettings();
        }

        void OnFocus()
        {
            EditorHelper.LoadRGSKCoreSettings();
        }

        public void AddItemsToMenu(UnityEditor.GenericMenu menu)
        {
            var content = new GUIContent("Refresh");
            menu.AddItem(content, false, () => EditorHelper.LoadRGSKCoreSettings());
        }

        [MenuItem("Window/RGSK/Menu", false, 2500)]
        public static void ShowWindow()
        {
            var window = GetWindow<RGSKWindow>();
            window.titleContent = new GUIContent("RGSK", CustomEditorStyles.menuIconContent.image);
            window.Show();
        }

        public static void ShowSettingsTab()
        {
            tabIndex = tabs.ToList().IndexOf("Settings");
            ShowWindow();
        }

        void OnGUI()
        {
            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope("Box", GUILayout.MaxWidth(tabSize), GUILayout.ExpandHeight(true)))
                {
                    tabIndex = GUILayout.SelectionGrid(tabIndex, tabs, 1, CustomEditorStyles.verticalToolbarButton);
                }

                serializedCore?.Update();

                using (new GUILayout.VerticalScope())
                {
                    using (var scope = new GUILayout.ScrollViewScope(scrollPosition, false, false))
                    {
                        scrollPosition = scope.scrollPosition;
                        GUILayout.Space(10);
                        EditorGUILayout.LabelField(tabs[tabIndex], CustomEditorStyles.titleLabel, GUILayout.Height(25));
                        EditorHelper.DrawLine();

                        switch (tabs[tabIndex].ToLower(System.Globalization.CultureInfo.InvariantCulture))
                        {
                            case "welcome":
                                {
                                    DrawWelcomeUI();
                                    break;
                                }

                            case "setup":
                                {
                                    DrawSetupUI();
                                    break;
                                }

                            case "general":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: generalSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.GeneralSettings =
                                        (RGSKGeneralSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKGeneralSettings)generalSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.GeneralSettings = (RGSKGeneralSettings)generalSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "scene":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: sceneSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.SceneSettings =
                                        (RGSKSceneSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKSceneSettings)sceneSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.SceneSettings = (RGSKSceneSettings)sceneSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "content":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: contentSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.ContentSettings =
                                        (RGSKContentSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKContentSettings)contentSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.ContentSettings = (RGSKContentSettings)contentSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    GUILayout.Space(5);
                                    using (new GUILayout.VerticalScope())
                                    {
                                        using (var s = new GUILayout.ScrollViewScope(contentTabScrollPosition, false, false))
                                        {
                                            contentTabScrollPosition = s.scrollPosition;
                                            contentTabIndex = GUILayout.Toolbar(contentTabIndex, contentTabs, CustomEditorStyles.horizontalToolbarButton);
                                            DrawContentItems(contentTabIndex == 0);
                                        }
                                    }

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "race":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: raceSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.RaceSettings =
                                        (RGSKRaceSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKRaceSettings)raceSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.RaceSettings = (RGSKRaceSettings)raceSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "ai":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: aiSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.AISettings =
                                        (RGSKAISettings)EditorHelper.CloneScriptableObject
                                        ((RGSKAISettings)aiSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.AISettings = (RGSKAISettings)aiSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "ui":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: uiSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.UISettings =
                                        (RGSKUISettings)EditorHelper.CloneScriptableObject
                                        ((RGSKUISettings)uiSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.UISettings = (RGSKUISettings)uiSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "audio":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: audioSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.AudioSettings =
                                        (RGSKAudioSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKAudioSettings)audioSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.AudioSettings = (RGSKAudioSettings)audioSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "input":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: inputSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.InputSettings =
                                        (RGSKInputSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKInputSettings)inputSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.InputSettings = (RGSKInputSettings)inputSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "vehicle":
                                {
                                    EditorGUI.indentLevel = 1;

                                    DrawSettingsLayout(
                                    prop: vehicleSettings,
                                    onClone: () =>
                                    {
                                        RGSKCore.Instance.VehicleSettings =
                                        (RGSKVehicleSettings)EditorHelper.CloneScriptableObject
                                        ((RGSKVehicleSettings)vehicleSettings.objectReferenceValue);

                                        EditorHelper.SaveRGSKCoreSettings();
                                    },
                                    onChange: () =>
                                    {
                                        RGSKCore.Instance.VehicleSettings = (RGSKVehicleSettings)vehicleSettings.objectReferenceValue;
                                        EditorHelper.SaveRGSKCoreSettings();
                                    });

                                    EditorGUI.indentLevel = 0;
                                    break;
                                }

                            case "integrations":
                                {
                                    DrawIntegrationsUI();
                                    break;
                                }
                        }
                    }
                }
            }

            serializedCore?.ApplyModifiedProperties();
        }

        void DrawWelcomeUI()
        {
            EditorHelper.DrawHeader(position.width - 250, position.width - 300);
            GUILayout.Space(5);
            using (new GUILayout.VerticalScope())
            {
                if (GUILayout.Button("Online Documentation", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.DocumentationLink);
                }

                if (GUILayout.Button("Discord", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.DiscordLink);
                }

                if (GUILayout.Button("YouTube", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.YouTubeLink);
                }

                if (GUILayout.Button("Unity Forums", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.UnityForumsLink);
                }

                if (GUILayout.Button("Asset Store", GUILayout.Height(supportButtonSize)))
                {
                    Application.OpenURL(EditorHelper.AssetStoreLink);
                }
            }

            GUILayout.BeginArea(new Rect(position.width - (250 + 165), position.height - 25, 150, 50));
            {
                using (new GUILayout.VerticalScope())
                {
                    if (GUILayout.Button("Check For Updates"))
                    {
                        UpdateCheckerWindow.ShowWindow();
                    }
                }

                GUILayout.EndArea();
            }
        }

        void DrawSetupUI()
        {
            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Layers", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This will add the required layers to the project.", MessageType.Info);

                if (GUILayout.Button("Update Layers", GUILayout.Height(supportButtonSize)))
                {
                    EditorHelper.AddLayersToProjectSettings();
                }
            }

            using (new GUILayout.VerticalScope(EditorStyles.helpBox))
            {
                GUILayout.Label("Demo Scenes", EditorStyles.boldLabel);
                EditorGUILayout.HelpBox("This will add all demo scenes to the build settings.", MessageType.Info);

                if (GUILayout.Button("Add Demos to Build Settings", GUILayout.Height(supportButtonSize)))
                {
                    EditorHelper.AddDemoScenesToBuilds();
                }
            }
        }

        void DrawIntegrationsUI()
        {
            EditorGUILayout.HelpBox("Please ensure that the asset is already imported before adding support for it!", MessageType.Info);
            EditorHelper.DrawIntegrationUI("Edy's Vehicle Physics", "EVP_SUPPORT", EditorHelper.EVPSupport());
            EditorHelper.DrawIntegrationUI("Realistic Car Controller", "RCC_SUPPORT", EditorHelper.RCCSupport());
            EditorHelper.DrawIntegrationUI("Realistic Car Controller Pro", "RCC_PRO_SUPPORT", EditorHelper.RCCProSupport());
            EditorHelper.DrawIntegrationUI("Vehicle Physics Pro - Community Edition", "VPP_SUPPORT", EditorHelper.VPPSupport());
            EditorHelper.DrawIntegrationUI("NWH Vehicle Physics 2", "NWH2_SUPPORT", EditorHelper.NWH2Support());
            EditorHelper.DrawIntegrationUI("Highroad Solid Controller", "HSC_SUPPORT", EditorHelper.HSCSupport());
            EditorHelper.DrawIntegrationUI("Ash Vehicle Physics", "ASHVP_SUPPORT", EditorHelper.ASHVPSupport());
            EditorHelper.DrawIntegrationUI("Universal Vehicle Controller", "UVC_SUPPORT", EditorHelper.UVCSupport());
            EditorHelper.DrawIntegrationUI("Sim-Cade Vehicle Physics", "SCVP_SUPPORT", EditorHelper.SCVPSupport());
        }

        void DrawSettingsLayout(SerializedProperty prop, Action onClone, Action onChange = null)
        {
            var referenceValue = prop.objectReferenceValue;
            var cloneButtonRect = new Rect((position.width - tabSize) - 110, EditorGUIUtility.singleLineHeight * 3.15f, 100, EditorGUIUtility.singleLineHeight);
            var settingsWidth = (position.width - tabSize) - 120;

            EditorGUILayout.PropertyField(prop, new GUIContent("Settings"), GUILayout.Width(settingsWidth));

            if (referenceValue != null)
            {
                if (GUI.Button(cloneButtonRect, "Clone", EditorStyles.miniButton))
                {
                    onClone?.Invoke();
                }
            }

            if (referenceValue != prop.objectReferenceValue)
            {
                onChange?.Invoke();
            }
        }

        void DrawContentItems(bool vehicles)
        {
            var content = RGSKCore.Instance.ContentSettings;
            var max = vehicles ? content.vehicles.Count : content.tracks.Count;
            var columnCount = (position.width - (tabSize * 1.25f)) / 400;

            using (new GUILayout.HorizontalScope())
            {
                using (new GUILayout.VerticalScope())
                {
                    var i = 0;
                    while (i < max)
                    {
                        using (new GUILayout.HorizontalScope())
                        {
                            GUILayout.FlexibleSpace();

                            for (int j = 0; j < columnCount; j++)
                            {
                                if (i >= max)
                                    break;

                                if (vehicles)
                                {
                                    var item = content.vehicles[i];
                                    if (item != null)
                                    {
                                        EditorHelper.DrawItemUI(
                                            item.previewPhoto,
                                            UIHelper.FormatVehicleNameText(item),
                                            $"Class: {item.vehicleClass}",
                                            "Select",
                                            "Remove",
                                            () => EditorHelper.SelectObject(item),
                                            () =>
                                            {
                                                if (EditorUtility.DisplayDialog("",
                                                    $"Are you sure you want to remove this vehicle from the list?",
                                                    "Yes", "No"))
                                                {

                                                    content.vehicles.RemoveAt(i);
                                                    EditorHelper.MarkSettingsAsDirty();
                                                    i = max;
                                                }
                                            });
                                    }
                                }
                                else
                                {
                                    var item = content.tracks[i];
                                    if (item != null)
                                    {
                                        EditorHelper.DrawItemUI(
                                            item.previewPhoto,
                                            item.objectName,
                                            $"Layout: {item.layoutType}",
                                            "Select",
                                            "Remove",
                                            () => EditorHelper.SelectObject(item),
                                            () =>
                                            {
                                                if (EditorUtility.DisplayDialog("",
                                                    $"Are you sure you want to remove this track from the list?",
                                                    "Yes", "No"))
                                                {

                                                    content.tracks.RemoveAt(i);
                                                    EditorHelper.MarkSettingsAsDirty();
                                                    i = max;
                                                }
                                            });
                                    }
                                }

                                i++;
                            }

                            GUILayout.FlexibleSpace();
                        }
                    }
                }
            }
        }
    }
}