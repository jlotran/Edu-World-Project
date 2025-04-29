using UnityEngine;
using TMPro;
using UnityEngine.UI;

namespace EduWorld
{
    public class CarView : MonoBehaviour, ICarView
    {
        private CarPresenter carPresenter;

        [Header("Fetch All Car")]
        [Space(10)]
        public Button fetchButton;

        [Header("Fetch by ID")]
        [Space(10)]
        public Button fetchByIdButton;
        public TMP_InputField carIdInput;

        [Header("Car Operations")]
        [Space(10)]
        public Button createButton;
        public Button updateButton;
        public Button deleteButton;

        [Header("Car Info")]
        [Space(10)]
        public TMP_InputField carIDInput;
        public TMP_InputField carNameInput;
        public TMP_InputField carSpeedInput;
        public TMP_InputField carHandlingInput;
        public TMP_InputField carAccelerationInput;
        public TMP_InputField carEnergyInput;
        public TMP_InputField carColorInput;
        public Toggle carLockStateToggle;

        [Header("Display Car Info")]
        [Space(10)]
        public TMP_Text carInfoText;

        private void Start()
        {
            carPresenter = new CarPresenter(this);
        }

        public void DisplayInfo(Car car)
        {
            carInfoText.text = car.DisplayInfo(car);
        }
    }
}
