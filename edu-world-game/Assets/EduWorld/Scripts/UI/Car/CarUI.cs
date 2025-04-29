using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using ExitGames.Client.Photon.StructWrapping;
using StarterAssets;
using Lam.GAMEPLAY;

namespace Edu_World
{
    public class CarUI : MonoBehaviour, ICarView
    {
        // Singleton instance
        public static CarUI instance { get; private set; }

        // Core references and constants 
        [Header("Core References")]
        [Tooltip("Main container for all car UI elements")]
        [SerializeField] private GameObject GraphicsHolder;
        [SerializeField] private Button buttonClose;
        private PlayerInteract playerInteract;
        private const float MAX_STAT_VALUE = 1000f;

        [Header("Car List UI")]
        [Tooltip("Parent transform for spawning car items")]
        [SerializeField] private Transform parent;
        [SerializeField] private GameObject carUIPrefab;
        [SerializeField] private List<Transform> carModelParent;

        [Header("Car Stats Text Elements")]
        [SerializeField] private TextMeshProUGUI carNameText;
        [SerializeField] private TextMeshProUGUI topSeedText;
        [SerializeField] private TextMeshProUGUI handlingText;
        [SerializeField] private TextMeshProUGUI accelerationText;
        [SerializeField] private TextMeshProUGUI energyText;
        [SerializeField] private TextMeshProUGUI carPriceText;

        [Header("Car Stats Sliders")]
        [SerializeField] private Slider topSpeedSlider;
        [SerializeField] private Slider handlingSlider;
        [SerializeField] private Slider accelerationSlider;
        [SerializeField] private Slider energySlider;

        [Header("Slider Animation Components")]
        private TweenSlider topSpeedTween;
        private TweenSlider handlingTween;
        private TweenSlider accelerationTween;
        private TweenSlider energyTween;

        [Header("Car Purchase UI")]
        [SerializeField] private ButtonBuyUI buttonBuyUI;

        [Header("Success Panel")]
        [SerializeField] private GameObject successPanel;
        [SerializeField] private TextMeshProUGUI successText;
        [SerializeField] private Button buttonCloseSuccess;

        // State tracking
        private List<GameObject> spawnedCarModels = new List<GameObject>();
        private int currentCarIndex = 0;
        private int currentCarID;
        private CarPresenter presenter;

        private void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(gameObject);
                return;
            }
            instance = this;

            InitializeSliders();
        }

        /// <summary>
        /// Initializes all slider components and sets their default states
        /// </summary>
        private void InitializeSliders()
        {
            // Add tween components
            topSpeedTween = topSpeedSlider.gameObject.AddComponent<TweenSlider>();
            handlingTween = handlingSlider.gameObject.AddComponent<TweenSlider>();
            accelerationTween = accelerationSlider.gameObject.AddComponent<TweenSlider>();
            energyTween = energySlider.gameObject.AddComponent<TweenSlider>();

            // Setup all sliders
            Slider[] sliders = { topSpeedSlider, handlingSlider, accelerationSlider, energySlider };
            foreach (var slider in sliders)
            {
                slider.interactable = false;
                slider.value = 0;
            }
        }

        /// <summary>
        /// Resets all sliders to zero value
        /// </summary>
        private void ResetSliders()
        {
            topSpeedSlider.value = handlingSlider.value =
            accelerationSlider.value = energySlider.value = 0;
        }

        private void Start()
        {
            successPanel.SetActive(false);
            buttonBuyUI.OnPurchaseSuccess += ShowSuccessPanel;
            GraphicsHolder.SetActive(false);
            buttonClose.onClick.AddListener(() => HideCarUI());
            buttonCloseSuccess.onClick.AddListener(() => HideSuccessCar());
            StartCoroutine(DelayCallPlayerInteract());
        }

        public IEnumerator DelayCallPlayerInteract()
        {
            while (playerInteract == null)
            {
                playerInteract = PlayerInteract.LocalInstance;
                yield return new WaitForSeconds(0.5f);
            }

            if (presenter == null)
            {
                presenter = new CarPresenter(this, playerInteract);
                Debug.Log("Presenter initialized with PlayerInteract");
            }
        }

        #region UI State Management
        public void ShowUI()
        {
            GraphicsHolder.SetActive(true);
            UIManager.Instance.SetUIState(UIType.Car, true);
        }

        public void HideUI()
        {
            GraphicsHolder.SetActive(false);
            UIManager.Instance.SetUIState(UIType.Car, false);
        }

        public void ToggleUI(CarIdHolder carHolder)
        {
            if (!GraphicsHolder) return;

            bool isActive = !GraphicsHolder.activeSelf;
            GraphicsHolder.SetActive(isActive);

            UpdateCarNameById(carHolder.carID);
            CarItem.UpdateAllCarItemsById(carHolder.carID);

            var camera = LamFusion.Camera.instance;
            if (!isActive)
            {
                camera.TurnCameraPlayer();
                return;
            }
            camera.SetCameraTarget(carHolder.transform);
        }

        public void HideCarUI()
        {
            InputManager.instance.input.ToggleCursorState();
            InputManager.instance.input.SetCursorState(true);
            GraphicsHolder.SetActive(false);
            UIManager.Instance.SetUIState(UIType.Car, false);
            if (successPanel.activeSelf) successPanel.SetActive(false);
            LamFusion.Camera.instance.TurnCameraPlayer();
            CameraCar.instance.CameraCarOff();
            UIInteract.instance.DeactiveButtonE();
            UIInteract.instance.ActiveButtonE();
        }

        public void HideSuccessCar()
        {
            successPanel.SetActive(false);
        }
        #endregion

        #region Car Data Updates
        // private void SpawnCarUI(CarData carData)
        // {
        //     GameObject carUIInstance = Instantiate(carUIPrefab, parent);
        //     CarItem carItem = carUIInstance.GetComponent<CarItem>();
        //     CarIdHolder carIdHolder = carUIInstance.AddComponent<CarIdHolder>();
        //     carIdHolder.carID = carData.carID;
        //     carItem.Initialize(carData);
        //     // carItem.OnCarSelected += UpdateCarName;
        // }

        // private void InitializeCarList()
        // {
        //     foreach (var carData in carDataList)
        //     {
        //         SpawnCarUI(carData);
        //     }
        // }

        // private void InitializeCarModels()
        // {
        //     CarSpawner.Instance.InitializeCarModels(carDataList);
        // }


        public void UpdateCarDisplay(int carIndex)
        {
            currentCarIndex = carIndex;
        }

        public void UpdateCarStats(string carName, int topSpeed, int handling, int acceleration, int energy)
        {
            carNameText.text = carName;

            // Update stat texts
            (topSeedText.text, handlingText.text, accelerationText.text, energyText.text) =
                (topSpeed.ToString(), handling.ToString(), acceleration.ToString(), energy.ToString());

            if (!GraphicsHolder.activeSelf) return;

            ResetSliders();
            StartCoroutine(DelayedTween(topSpeed, handling, acceleration, energy));
        }

        public void UpdatePriceInfo(int price, CarData currentCar)
        {
            carPriceText.text = buttonBuyUI.rank >= currentCar.rank ? price.ToString() : "LOCKED";
            buttonBuyUI.SetCurrentCarName(currentCar.carName);
            buttonBuyUI.UpdateMoneyImage(currentCar.price, currentCar.rank);
        }

        // private void UpdateCarName(string carName, int topSpeed, int handling, int acceleration, int energy, int price)
        // {
        //     presenter.UpdateSelectedCar(carName, topSpeed, handling, acceleration, energy, price);
        // }

        public void UpdateCarNameById(int id)
        {
            presenter.UpdateSelectedCarByID(id);
        }

        private IEnumerator DelayedTween(int topSpeed, int handling, int acceleration, int energy)
        {
            yield return null;

            // Tween sliders to new values
            topSpeedTween.TweenTo(topSpeed / MAX_STAT_VALUE);
            handlingTween.TweenTo(handling / MAX_STAT_VALUE);
            accelerationTween.TweenTo(acceleration / MAX_STAT_VALUE);
            energyTween.TweenTo(energy / MAX_STAT_VALUE);

            // Update text displays
            topSeedText.text = topSpeed.ToString();
            handlingText.text = handling.ToString();
            accelerationText.text = acceleration.ToString();
            energyText.text = energy.ToString();
        }

        public void ShowSuccess(string carName)
        {
            successText.text = $"{carName}";
            successPanel.SetActive(true);
        }

        private void ShowSuccessPanel(string carName)
        {
            presenter.HandlePurchaseSuccess(carName);
        }
        #endregion

        void ICarView.Initialize(List<CarData> carDataList)
        {
        }

        private void OnDestroy()
        {
            // Clean up event subscriptions
            // foreach (CarItem carItem in parent.GetComponentsInChildren<CarItem>())
            // {
            //     carItem.OnCarSelected -= UpdateCarName;
            // }
            buttonBuyUI.OnPurchaseSuccess -= ShowSuccessPanel;
        }

        public bool IsSuccessPanelActive()
        {
            return successPanel.activeSelf;
        }

        public void UpdateSelectedCarByID(CarIdHolder carID)
        {
            currentCarID = carID.carID;
        }
    }
}
