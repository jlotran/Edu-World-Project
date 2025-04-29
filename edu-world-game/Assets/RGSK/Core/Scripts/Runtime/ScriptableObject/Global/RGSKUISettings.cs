using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    [CreateAssetMenu(menuName = "RGSK/Core/Global Settings/UI")]
    public class RGSKUISettings : ScriptableObject
    {
        [System.Serializable]
        public class Screens
        {
            [Header("Common")]
            public UIScreenID loadingScreen;
            public UIScreenID pauseScreen;
            public UIScreenID replayScreeen;

            [Header("Race")]
            public UIScreenID preRaceScreen;
            public UIScreenID raceScreen;
            public UIScreenID postRaceScreen;

            [Header("Menu")]
            public UIScreenID vehicleSelectScreen;
            public UIScreenID trackSelectScreen;
            public UIScreenID raceSettingsScreen;
        }

        [Header("Screens")]
        public Screens screens;

        [Header("Race Type UI")]
        public RaceUILayout defaultRaceUILayout;
        public List<RaceUILayout> raceUILayouts = new List<RaceUILayout>();

        [Header("Race Boards")]
        public RaceBoardLayout defaultRaceBoardLayout;
        public List<RaceBoardLayout> raceBoardLayouts = new List<RaceBoardLayout>();

        [Header("Worldspace")]
        public Nameplate nameplate;
        public RaceWaypointArrow waypointArrow;
        public bool showNameplates = true;
        public bool showWaypointArrow;

        [Header("Minimap")]
        public string playerMinimapBlip = "player";
        public string opponentMinimapBlip = "opponent";

        [Header("Proximity Arrows")]
        public bool showProximityArrows;

        [Header("Target Score Icons")]
        public List<TargetScoreIcon> targetScoreIcons = new List<TargetScoreIcon>();

        [Header("Formats")]
        public TimeFormat raceTimerFormat = TimeFormat.MM_SS_FFF;
        public TimeFormat realtimeGapTimerFormat = TimeFormat.S_FFF;
        public SpeedUnit speedUnit;
        public DistanceUnit distanceUnit;
        public NumberDisplayMode raceBoardPositionFormat = NumberDisplayMode.Single;
        public NumberDisplayMode raceBoardLapFormat = NumberDisplayMode.Single;
        public VehicleNameDisplayMode raceBoardVehicleNameFormat = VehicleNameDisplayMode.FullName;
        public NameDisplayMode raceBoardNameFormat = NameDisplayMode.FullName;
        public string dnfString = "DNF";
        public string currencyFormat = "Cr. {0}";
        public string lockedItemFormat = "Unlocks at level {0}";

        [Header("Audio")]
        public string buttonHoverSound = "button_hover";
        public string buttonClickSound = "button_click";

        [Header("Modal Windows")]
        public List<ModalWindow> modalWindowPrefabs = new List<ModalWindow>();
        public ModalWindowProperties restartModal = new ModalWindowProperties
        {
            header = "Restart",
            message = "Are you sure you want to restart?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties exitModal = new ModalWindowProperties
        {
            header = "Exit",
            message = "Are you sure you want to exit?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties quitModal = new ModalWindowProperties
        {
            header = "Quit",
            message = "Are you sure you want to quit?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties purchasePromptModal = new ModalWindowProperties
        {
            header = "Purchase",
            message = "Do you want to purchase {0} for {1}?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties purchaseFailModal = new ModalWindowProperties
        {
            header = "Purchase",
            message = "You do not have enough currency!",
            confirmButtonText = "Ok",
            startSelection = 0
        };

        public ModalWindowProperties lockedItemModal = new ModalWindowProperties
        {
            header = "Locked",
            message = "This item will unlock at level {0}!",
            confirmButtonText = "Ok",
            startSelection = 0
        };

        public ModalWindowProperties lockedItemConditionalModal = new ModalWindowProperties
        {
            header = "Locked",
            message = "{0}",
            confirmButtonText = "Ok",
            startSelection = 0
        };

        public ModalWindowProperties deleteSaveModal = new ModalWindowProperties
        {
            header = "Delete Save Data",
            message = "Are you sure you want to delete the save data? This cannot be undone.",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };

        public ModalWindowProperties resetInputBindingsModal = new ModalWindowProperties
        {
            header = "Reset Input Bindings",
            message = "Are you sure you want to reset all input bindings?",
            confirmButtonText = "Yes",
            declineButtonText = "No",
            startSelection = 1
        };
    }
}