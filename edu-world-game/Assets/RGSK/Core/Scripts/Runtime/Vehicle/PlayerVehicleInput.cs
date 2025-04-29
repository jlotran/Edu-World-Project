using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace RGSK
{
    public class PlayerVehicleInput : Singleton<PlayerVehicleInput>
    {
        public IVehicle Vehicle => _vehicle;

        IVehicle _vehicle;
        Repositioner _repositioner;
        public float throttleInput { get; private set; }
        public float brakeInput { get; private set; }
        public float steerInput { get; private set; }
        public float handbrakeInput { get; private set; }
        public float nitrousInput { get; private set; }

        void OnEnable()
        {
            RGSKEvents.OnRaceInitialized.AddListener(OnRaceInitialized);
            InputManager.ThrottleEvent += OnThrottle;
            InputManager.BrakeEvent += OnBrake;
            InputManager.SteerEvent += OnSteer;
            InputManager.HandBrakeEvent += OnHandbrake;
            InputManager.BoostEvent += OnBoost;
            InputManager.ShiftUpEvent += OnShiftUp;
            InputManager.ShiftDownEvent += OnShiftDown;
            InputManager.RepositionPerformedEvent += OnReposition;
            InputManager.HornStartEvent += OnHornStart;
            InputManager.HornCancelEvent += OnHornEnd;
            InputManager.HeadlightEvent += OnToggleHeadlights;
            InputManager.EngineToggleEvent += OnToggleEngine;
            RGSKEvents.OnGamePaused.AddListener(OnGamePaused);
            RGSKEvents.OnGameUnpaused.AddListener(OnGameUnpaused);
        }

        void OnDisable()
        {
            InputManager.ThrottleEvent -= OnThrottle;
            InputManager.BrakeEvent -= OnBrake;
            InputManager.SteerEvent -= OnSteer;
            InputManager.HandBrakeEvent -= OnHandbrake;
            InputManager.BoostEvent -= OnBoost;
            InputManager.ShiftUpEvent -= OnShiftUp;
            InputManager.ShiftDownEvent -= OnShiftDown;
            InputManager.RepositionPerformedEvent -= OnReposition;
            InputManager.HornStartEvent -= OnHornStart;
            InputManager.HornCancelEvent -= OnHornEnd;
            InputManager.HeadlightEvent -= OnToggleHeadlights;
            InputManager.EngineToggleEvent -= OnToggleEngine;
            RGSKEvents.OnGamePaused.AddListener(OnGamePaused);
            RGSKEvents.OnGameUnpaused.RemoveListener(OnGameUnpaused);
        }

        private bool isFusionRacing = false;
        void OnRaceInitialized()
        {
            isFusionRacing = RaceManager.Instance.Session.isFusionRacing;
        }

        void Update()
        {
            if (_vehicle == null)
                return;
            if (!isFusionRacing)
            {
                _vehicle.ThrottleInput = throttleInput;
                _vehicle.BrakeInput = brakeInput;
                _vehicle.SteerInput = steerInput;
                _vehicle.HandbrakeInput = handbrakeInput;
                _vehicle.NitrousInput = nitrousInput;
            }
        }

        public void Bind(GameObject go)
        {
            if (go.TryGetComponent<IVehicle>(out var v))
            {
                _vehicle = v;
                _repositioner = go.GetComponent<Repositioner>();
                SetTransmission();
                ResetInputValues();
            }
        }

        public void Unbind(GameObject go)
        {
            if (_vehicle == null)
                return;

            if (go.TryGetComponent<IVehicle>(out var v))
            {
                if (_vehicle == v)
                {
                    ResetInputValues();
                    _vehicle = null;
                    _repositioner = null;
                    InputManager.Instance?.Rumble(0, 0, 0);
                }
            }
        }

        void ResetInputValues()
        {
            if (_vehicle == null)
                return;

            _vehicle.ThrottleInput = 0;
            _vehicle.BrakeInput = 0;
            _vehicle.SteerInput = 0;
            _vehicle.HandbrakeInput = 0;
            _vehicle.NitrousInput = 0;
            _vehicle.HornOn = false;
        }

        public void Rumble(IVehicle vehicle, float rumbleAmount, float duration)
        {
            if (!enabled || vehicle != _vehicle)
                return;

            InputManager.Instance?.Rumble(rumbleAmount * 0.5f, rumbleAmount, duration);
        }

        void OnThrottle(float value) => throttleInput = value;
        void OnBrake(float value) => brakeInput = value;
        void OnSteer(float value) => steerInput = value;
        void OnHandbrake(float value) => handbrakeInput = value;
        void OnBoost(float value) => nitrousInput = value;
        void OnReposition() => _repositioner?.Reposition();

        void OnToggleHeadlights()
        {
            if (_vehicle == null)
                return;

            _vehicle.HeadlightsOn = !_vehicle.HeadlightsOn;
        }

        void OnHornStart()
        {
            if (_vehicle == null)
                return;

            _vehicle.HornOn = true;
        }

        void OnHornEnd()
        {
            if (_vehicle == null)
                return;

            _vehicle.HornOn = false;
        }

        void OnToggleEngine()
        {
            if (_vehicle == null)
                return;

            if (!_vehicle.IsEngineOn)
            {
                _vehicle.StartEngine(-1);
            }
            else
            {
                _vehicle.StopEngine();
            }
        }

        void OnShiftUp()
        {
            if (!IsManual())
                return;

            _vehicle?.ShiftUp();
        }

        void OnShiftDown()
        {
            if (!IsManual())
                return;

            _vehicle?.ShiftDown();
        }

        void SetTransmission()
        {
            if (_vehicle == null)
                return;

            _vehicle.TransmissionType = RGSKCore.Instance.VehicleSettings.transmissionType;
        }

        bool IsManual() => _vehicle == null ? false : _vehicle.TransmissionType == TransmissionType.Manual;
        void OnGamePaused() => InputManager.Instance?.Rumble(0, 0, 0);
        void OnGameUnpaused() => SetTransmission();
    }
}