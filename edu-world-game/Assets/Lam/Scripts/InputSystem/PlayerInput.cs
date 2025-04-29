using UnityEngine;
using UnityEngine.InputSystem;
using Fusion;
using Fusion.Addons.SimpleKCC;
using StarterAssets;
using Lam.GAMEPLAY;

namespace Lam.FUSION
{
	/// <summary>
	/// Tracks player input.
	/// </summary>
	[DefaultExecutionOrder(-10)]
	public sealed class PlayerInput : NetworkBehaviour, IBeforeUpdate, IBeforeTick
	{
		public GameInput CurrentInput => _currentInput;
		public GameInput PreviousInput => _previousInput;

		[SerializeField]
		[Tooltip("Mouse delta multiplier.")]
		private Vector2 _lookSensitivity = Vector2.one;

		// We need to store current input to compare against previous input (to track actions activation/deactivation). It is also used if the input for current tick is not available.
		// This is not needed on proxies and will be replicated to input authority only.
		[Networked]
		private GameInput _currentInput { get; set; }

		private GameInput _previousInput;
		private GameInput _accumulatedInput;
		private bool _resetAccumulatedInput;
		private Vector2Accumulator _lookRotationAccumulator = new Vector2Accumulator(0.02f, true);

		private StarterAssetsInputs inputSystem => InputManager.instance.input;
		private GameObject _mainCamera;

		public override void Spawned()
		{
			// Reset to default state.
			_currentInput = default;
			_previousInput = default;
			_accumulatedInput = default;
			_resetAccumulatedInput = default;

			if (Object.HasInputAuthority == true)
			{
				// Register local player input polling.
				NetworkEvents networkEvents = Runner.GetComponent<NetworkEvents>();
				// inputSystem = gameObject.AddComponent<StarterAssetsInputs>();
				networkEvents.OnInput.AddListener(OnInput);

				if (Application.isMobilePlatform == false || Application.isEditor == true)
				{
					// Hide cursor
					Cursor.lockState = CursorLockMode.Locked;
					Cursor.visible = false;
				}

				_mainCamera = GameObject.FindGameObjectWithTag("MainCamera");
			}

			// Only local player needs networked properties (current input).
			// This saves network traffic by not synchronizing networked properties to other clients except local player.
			ReplicateToAll(false);
			ReplicateTo(Object.InputAuthority, true);
		}

		public override void Despawned(NetworkRunner runner, bool hasState)
		{
			if (runner == null)
				return;

			NetworkEvents networkEvents = runner.GetComponent<NetworkEvents>();
			if (networkEvents != null)
			{
				// Unregister local player input polling.
				networkEvents.OnInput.RemoveListener(OnInput);
			}
		}

		/// <summary>
		/// 1. Collect input from devices, can be executed multiple times between FixedUpdateNetwork() calls because of faster rendering speed.
		/// </summary>
		void IBeforeUpdate.BeforeUpdate()
		{
			if (HasInputAuthority == false)
				return;

			// Accumulated input was polled and explicit reset requested.
			if (_resetAccumulatedInput == true)
			{
				_resetAccumulatedInput = false;
				_accumulatedInput = default;
			}

			if (Application.isMobilePlatform == false || Application.isEditor == true)
			{
				// Input is tracked only if the cursor is locked.
				if (Cursor.lockState != CursorLockMode.Locked)
					return;
			}

			// Always use KeyControl.isPressed, Input.GetMouseButton() and Input.GetKey().
			// Never use KeyControl.wasPressedThisFrame, Input.GetMouseButtonDown() or Input.GetKeyDown() otherwise the action might be lost.

			Mouse mouse = Mouse.current;
			if (mouse != null)
			{
				Vector2 mouseDelta = mouse.delta.ReadValue();
				_lookRotationAccumulator.Accumulate(new Vector2(-mouseDelta.y, mouseDelta.x) * _lookSensitivity);
			}

			Keyboard keyboard = Keyboard.current;
			if (keyboard != null && InputManager.instance.inputMode == InputMode.Gameplay)
			{
				Vector2 moveDirection = inputSystem.move;

				_accumulatedInput.move = moveDirection.normalized;
				_accumulatedInput.sprint = inputSystem.sprint;
				_accumulatedInput.look = inputSystem.look;
				_accumulatedInput.camDirect = _mainCamera.transform.eulerAngles.y;
				_accumulatedInput.analogMovement = inputSystem.analogMovement;

				_accumulatedInput.Actions.Set(GameInput.JUMP_BUTTON, inputSystem.jump);
			}
		}

		/// <summary>
		/// 3. Read input from Fusion.
		/// </summary>
		void IBeforeTick.BeforeTick()
		{
			if (Object == null)
				return;

			// Set current in input as previous.
			_previousInput = _currentInput;

			// Clear all properties which should not propagate from last known input in case of missing new input. As example, following line will reset look rotation delta.
			// This results to the player not being incorrectly rotated (by using rotation delta from last known input) in case of missing input on state authority, followed by a correction on the input authority.
			GameInput currentInput = _currentInput;
			// currentInput.LookRotationDelta = default;
			_currentInput = currentInput;

			if (Object.InputAuthority != PlayerRef.None)
			{
				// If this fails, the current input won't be updated and input from previous tick will be reused.
				if (GetInput(out GameInput input) == true)
				{
					// New input received, we can store it as current.
					_currentInput = input;
				}
			}
		}

		/// <summary>
		/// 2. Push accumulated input and reset properties, can be executed multiple times within single Unity frame if the rendering speed is slower than Fusion simulation.
		/// This is usually executed multiple times if there is a performance spike, for example after expensive spawn which includes asset loading.
		/// </summary>
		private void OnInput(NetworkRunner runner, NetworkInput networkInput)
		{
			// Mouse movement (delta values) is aligned to engine update.
			// To get perfectly smooth interpolated look, we need to align the mouse input with Fusion ticks.
			// _accumulatedInput.LookRotationDelta = _lookRotationAccumulator.ConsumeTickAligned(runner);

			// Set accumulated input.
			networkInput.Set(_accumulatedInput);

			// Input is polled for single fixed update, but at this time we don't know how many times in a row OnInput() will be executed.
			// This is the reason to have a reset flag instead of resetting input immediately, otherwise we could lose input for next fixed updates (for example move direction).
			_resetAccumulatedInput = true;
			if (inputSystem.jump) inputSystem.jump = false;
		}
	}
}
