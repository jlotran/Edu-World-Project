using UnityEngine;
using Fusion;
using RGSK.Helpers;
using System.Linq;
using RGSK.Extensions;
using Unity.VisualScripting;
using Fusion.Menu;
using System.Collections.Generic;

namespace RGSK.Fusion
{
    public class Rider : NetworkBehaviour
    {
        public static Rider LocalRider { get; private set; }
        public RaceEntrant Entrant { get; set; }
        [Header("Required Components")]
        [SerializeField] private VehicleController _vehicleController;
        [SerializeField] private RiderInput _inputNetwork;
        [Header("Network Smoothing")]
        public float reconciliationThreshold = 10f;
        // Network properties
        [Networked] public int indexProfile { get; set; }

        [SerializeField] private Rigidbody _rb;

        // share
        public NetworkTimer timer;
        const float SERVER_TICK_RATE = 60;
        const int BUFFER_SIZE = 1024;

        //client
        CicularBuffer<StatePayload> clientStateBuffer;
        CicularBuffer<RacingInput> clientInputBuffer;
        StatePayload lastestServerState;
        StatePayload lastProcessedState;

        //server
        CicularBuffer<StatePayload> serverStateBuffer;
        Queue<RacingInput> serverInputQueue;

        //
        [Networked] public StatePayload networkState { get; set; }

        StatePayload previousNetworkState;


        public override void Spawned()
        {
            Debug.Log($"Spawned: {Object.InputAuthority}");
           

            if (Object.HasInputAuthority)
            {
                LocalRider = this;
                RGSKEntity entity = GetComponent<RGSKEntity>();
                if (entity != null)
                {
                    entity.IsPlayer = true;
                }
            }

            InitializeBuffer();
            InitializeProfile();
            InitializeComponent();
        }

        public void Update()
        {
            timer.Update(Time.deltaTime);
        }

        public void FixedUpdate()
        {
            while (timer.ShouldTick())
            {
                if (!Object.HasStateAuthority)
                {
                    lastestServerState = networkState;
                }
                HandleServerTick();
                HandleClientTick();
            }
        }

        private void HandleServerTick()
        {
            if (!Object.HasStateAuthority) return;

            int bufferIndex = -1;

            RacingInput inputPayload = _inputNetwork.NetworkInput;

            bufferIndex = inputPayload.tick % BUFFER_SIZE;

            StatePayload statePayload = SimulateMovement(inputPayload);
            serverStateBuffer.Add(statePayload, bufferIndex);

            if (bufferIndex == -1) return;

            networkState = serverStateBuffer.Get(bufferIndex);
        }

        private StatePayload SimulateMovement(RacingInput input)
        {
            Runvehicle(input);

            Debug.Log($"SimulateMovement: {_rb}");
            return new StatePayload()
            {
                tick = Runner.Tick,
                position = transform.position,
                rotation = transform.rotation,
                velocity = _rb.linearVelocity,
                angularVelocity = _rb.angularVelocity
            };
        }

        private void HandleClientTick()
        {
            if (Object.HasStateAuthority) return;

            var currentTick = Runner.Tick;
            var bufferIndex = currentTick % BUFFER_SIZE;

            RacingInput input = _inputNetwork.NetworkInput;

            clientInputBuffer.Add(input, bufferIndex);

            StatePayload statePayload = ProcessMovement(input);
            clientStateBuffer.Add(statePayload, bufferIndex);

            HandleServerReconcilation();
        }

        bool ShouldReconcile()
        {
            bool isNewSeverState = !lastestServerState.Equals(default);
            bool isLastestStateUndefineOrDifferent = lastProcessedState.Equals(default) || lastProcessedState.Equals(lastestServerState);

            return isNewSeverState && isLastestStateUndefineOrDifferent;
        }

        private void HandleServerReconcilation()
        {
            float positionError;
            float rotationError;
            int bufferIndex;

            StatePayload rewindState = default;

            bufferIndex = lastestServerState.tick % BUFFER_SIZE;
            if (bufferIndex - 1 < 0) return;

            rewindState = lastestServerState; // client
            positionError = Vector3.Distance(rewindState.position, clientStateBuffer.Get(bufferIndex).position);
            rotationError = Quaternion.Angle(rewindState.rotation, clientStateBuffer.Get(bufferIndex).rotation);

            if (positionError > reconciliationThreshold || rotationError > reconciliationThreshold)
            {
                ReconcileState(rewindState);
            }

            lastProcessedState = lastestServerState;
        }

        private void ReconcileState(StatePayload rewindState)
        {
            transform.position = rewindState.position;
            transform.rotation = rewindState.rotation;
            _rb.linearVelocity = rewindState.velocity;
            _rb.angularVelocity = rewindState.angularVelocity;

            if (!rewindState.Equals(lastestServerState)) return;

            clientStateBuffer.Add(rewindState, rewindState.tick);

            int tickToRelay = lastestServerState.tick;
            while (tickToRelay < timer.currentTick)
            {
                int bufferIndex = tickToRelay % BUFFER_SIZE;
                StatePayload statePayload = ProcessMovement(clientInputBuffer.Get(bufferIndex));
                clientStateBuffer.Add(statePayload, bufferIndex);
                tickToRelay++;
            }
        }

        private void SendToServer(RacingInput input)
        {
            serverInputQueue.Enqueue(input);
        }

        StatePayload ProcessMovement(RacingInput input)
        {
            Runvehicle(input);

            return new StatePayload()
            {
                tick = Runner.Tick,
                position = transform.position,
                rotation = transform.rotation,
                velocity = _rb.linearVelocity,
                angularVelocity = _rb.angularVelocity
            };
        }

        #region Network
        public void StartRace()
        {
            if (Object.HasInputAuthority)
            {
                RPC_ServerStartRace();
            }
        }
        [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
        public void RPC_ServerStartRace()
        {
            if (Object.HasStateAuthority && !Object.HasInputAuthority)
            {
                LocalRider?.OnStartControl();
                RaceManager.Instance?.StartRace();
            }
            RPC_StartRace();
        }
        [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
        public void RPC_StartRace()
        {
            RaceManager.Instance?.StartRace();
            GeneralHelper.ToggleInputControl(gameObject, false);
            LocalRider.OnStartControl();
        }
        private void OnStartControl()
        {
        }
        #endregion


        private void Runvehicle(RacingInput input)
        {
            _vehicleController.ThrottleInput = input.throttleInput;
            _vehicleController.SteerInput = input.steerInput;
            _vehicleController.BrakeInput = input.brakeInput;
            _vehicleController.HandbrakeInput = input.handbrakeInput;
            _vehicleController.NitrousInput = input.nitrousInput;

            _vehicleController.FixedUpdateNetwork();
        }

        private void InitializeBuffer()
        {
            timer = new NetworkTimer(SERVER_TICK_RATE);
            clientStateBuffer = new CicularBuffer<StatePayload>(BUFFER_SIZE);
            clientInputBuffer = new CicularBuffer<RacingInput>(BUFFER_SIZE);

            serverStateBuffer = new CicularBuffer<StatePayload>(BUFFER_SIZE);
            serverInputQueue = new Queue<RacingInput>();
        }

        private void InitializeComponent()
        {
            // _rb = GetComponent<Rigidbody>();
            _inputNetwork = GetComponent<RiderInput>();
            _vehicleController = GetComponent<VehicleController>();
        }

        private void InitializeProfile()
        {
            if (Object.HasStateAuthority)
            {
                indexProfile = Random.Range(0, RGSKCore.Instance.AISettings.opponentProfiles.ToList().Count);
            }

            var entrant = PopulateEntrant();
            entrant.prefab = gameObject;
            Entrant = entrant;

            var profile = GetComponent<ProfileDefiner>();
            profile.definition = RGSKCore.Instance.AISettings.opponentProfiles.ToList()[indexProfile];
            gameObject.name = $"[{UIHelper.FormatNameText(profile.definition)}] {Object.InputAuthority}";
            RaceManager.Instance.SpawnEntrant(gameObject, Entrant, Object.HasInputAuthority, indexProfile);


            RGSK.Helpers.GeneralHelper.TogglePlayerInput(gameObject, true);
            RGSK.Helpers.GeneralHelper.ToggleInputControl(gameObject, true);
            RGSK.Helpers.GeneralHelper.ToggleVehicleCollision(gameObject, true);

            if (Object.HasInputAuthority)
            {
                LocalRider = this;
                RGSK.CameraManager.Instance?.SetTarget(transform);
                FusionMenuManager.instance?.OnDoneJoinRoom();
            }
        }

        private RaceEntrant PopulateEntrant()
        {
            var vehicles = RGSKCore.Instance.ContentSettings.vehicles.ToList();
            var entrants = RaceManager.Instance.Session?.entrants;
            var playerVehicleClass = GeneralHelper.GetPlayerEntrantVehicleClass(entrants);
            vehicles = RGSKCore.Instance.ContentSettings.vehicles.Where(x => x.vehicleClass == playerVehicleClass).ToList();
            if (vehicles.Count == 0)
            {
                Logger.LogWarning("Cannot auto populate opponents because no vehicles were found!");
                return null;
            }
            vehicles.Shuffle();
            var entrant = new RaceEntrant
            {
                prefab = vehicles[0].prefab,
                colorSelectMode = ColorSelectionMode.Random,
                isPlayer = false,
            };
            return entrant;
        }
    }
}
