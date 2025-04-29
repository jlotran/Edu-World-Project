using UnityEngine;
using Fusion;

namespace RGSK.Fusion
{
    public class RiderInput : NetworkBehaviour, IBeforeUpdate, IBeforeTick
    {
        [SerializeField] private Rider rider;
        [Networked] public RacingInput NetworkInput { get; set; }

        private RacingInput _accumulatedInput;
        private bool _resetAccumulatedInput;
        private RGSK.PlayerVehicleInput inputSystem => RGSK.PlayerVehicleInput.Instance;

        public override void Spawned()
        {
            if (HasInputAuthority)
            {
                Runner.GetComponent<NetworkEvents>().OnInput.AddListener(OnInput);

                if (!Application.isMobilePlatform || Application.isEditor)
                {
                    Cursor.lockState = CursorLockMode.Locked;
                    Cursor.visible = false;
                }
            }
        }

        void IBeforeUpdate.BeforeUpdate()
        {
            if (!HasInputAuthority) return;

            if (_resetAccumulatedInput)
            {
                _accumulatedInput = default;
                _resetAccumulatedInput = false;
            }

            _accumulatedInput.throttleInput = inputSystem.throttleInput;
            _accumulatedInput.steerInput = inputSystem.steerInput;
            _accumulatedInput.brakeInput = inputSystem.brakeInput;
            _accumulatedInput.handbrakeInput = inputSystem.handbrakeInput;
            _accumulatedInput.nitrousInput = inputSystem.nitrousInput;
            _accumulatedInput.tick = rider.timer.currentTick;
        }

        void IBeforeTick.BeforeTick()
        {
            if (HasStateAuthority && GetInput(out RacingInput input))
            {
                NetworkInput = input;
            }
        }

        private void OnInput(NetworkRunner runner, NetworkInput networkInput)
        {
            networkInput.Set(_accumulatedInput);
            _resetAccumulatedInput = true;
        }
    }
}
