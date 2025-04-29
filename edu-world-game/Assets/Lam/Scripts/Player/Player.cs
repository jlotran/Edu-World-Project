using UnityEngine;
using Fusion;
using Fusion.Addons.SimpleKCC;
using System.Collections.Generic;
using System.Linq;
using StarterAssets;
using Rukha93.ModularAnimeCharacter.Customization;
using Fusion.Menu;
using System;
using Lam.GAMEPLAY;
using System.Collections;

namespace Lam.FUSION
{
	public enum PlayerState
	{
		synchronous,
		teleporting,
		free,
	}

	public class Player : NetworkBehaviour
	{
		#region Properties
		public static Player localPlayer;
		public static Dictionary<PlayerRef, Player> Players = new Dictionary<PlayerRef, Player>();
		public static Dictionary<PlayerRef, PlayerData> playerDatas = new Dictionary<PlayerRef, PlayerData>();

		[Header("Player")]
		[Tooltip("Move speed of the character in m/s")]
		public float MoveSpeed = 2.0f;

		[Tooltip("Sprint speed of the character in m/s")]
		public float SprintSpeed = 5.335f;

		[Tooltip("How fast the character turns to face movement direction")]
		[Range(0.0f, 0.3f)]
		public float RotationSmoothTime = 0.12f;

		[Tooltip("Acceleration and deceleration")]
		public float SpeedChangeRate = 10.0f;

		[Range(0, 1)] public float FootstepAudioVolume = 0.5f;

		[Space(10)]
		[Tooltip("The height the player can jump")]
		public float JumpHeight = 0.7f;

		[Tooltip("The character uses its own gravity value. The engine default is -9.81f")]
		public float Gravity = -15.0f;

		[Space(10)]
		[Tooltip("Time required to pass before being able to jump again. Set to 0f to instantly jump again")]
		public float JumpTimeout = 0.50f;

		[Tooltip("Time required to pass before entering the fall state. Useful for walking down stairs")]
		public float FallTimeout = 0.15f;



		[Header("Character customize equip")]
		public CustomizationDemo characterCustomize;

		private bool isEquip = false;

		[Header("Player Grounded")]
		[Tooltip("If the character is grounded or not. Not part of the CharacterController built in grounded check")]
		public bool Grounded = true;

		[Tooltip("Useful for rough ground")]
		public float GroundedOffset = -0.14f;

		[Tooltip("The radius of the grounded check. Should match the radius of the CharacterController")]
		public float GroundedRadius = 0.28f;

		[Tooltip("What layers the character uses as ground")]
		public LayerMask GroundLayers;

		[Header("Cinemachine")]
		[Tooltip("The follow target set in the Cinemachine Virtual Camera that the camera will follow")]
		public GameObject CinemachineCameraTarget;

		[Tooltip("How far in degrees can you move the camera up")]
		public float TopClamp = 70.0f;

		[Tooltip("How far in degrees can you move the camera down")]
		public float BottomClamp = -30.0f;

		[Tooltip("Additional degress to override the camera. Useful for fine tuning camera position when locked")]
		public float CameraAngleOverride = 0.0f;

		[Tooltip("For locking the camera position on all axis")]
		public bool LockCameraPosition = false;

		public Teleport teleport;

		// cinemachine
		private float _cinemachineTargetYaw;
		private float _cinemachineTargetPitch;

		private PlayerState state = PlayerState.free;

		// player
		private float _speed;

		[Networked]
		private float _animationBlend { get; set; }
		private float _targetRotation = 0.0f;
		private float _rotationVelocity;
		private float _verticalVelocity;
		private float _terminalVelocity = 53.0f;

		// timeout deltatime
		private float _jumpTimeoutDelta;
		private float _fallTimeoutDelta;

		private bool _isSetLayered = false;
		#endregion

#if ENABLE_INPUT_SYSTEM
		private UnityEngine.InputSystem.PlayerInput _playerInput;
#endif
		[SerializeField] private PlayerAnimator _animator;
		public Animator animator => _animator.animator;
		[SerializeField] private PlayerInput _inputNetwork;

		private const float _threshold = 0.01f;

		[SerializeField] private SimpleKCC _kcc;
		public SimpleKCC kcc => _kcc;

		private bool IsCurrentDeviceMouse
		{
			get
			{
#if ENABLE_INPUT_SYSTEM
				return _playerInput.currentControlScheme == "KeyboardMouse";
#else
                return false;
#endif
			}
		}

		void Start()
		{
			// Remove layer setting code from here
		}

		[ContextMenu("Free")]
		public void FreeKcc()
		{
			_animationBlend = 0;
			state = PlayerState.free;
		}


		[ContextMenu("Synchronous")]
		public void EnableSynchronous()
		{
			_kcc.enabled = true;
			state = PlayerState.synchronous;
		}

		[ContextMenu("DisableInput")]
		public void DisableInput()
		{
			InputManager.instance.DisableInput();
		}

		public override void Spawned()
		{
			state = PlayerState.synchronous;
			PlayerRef playerRef = Object.InputAuthority;

			if (Players.ContainsKey(playerRef))
			{
				if (Players[playerRef] != null && Players[playerRef].Object != null)
				{
					Runner.Despawn(Players[playerRef].Object);
				}
				Players.Remove(playerRef);
			}

			gameObject.name = playerRef.ToString();
			Players.Add(playerRef, this);

			// Set layer for PlayerCity children
			if (Object.HasStateAuthority)
			{
				PlayerRef[] playerRefs = new PlayerRef[playerDatas.Count];
				string[] genders = new string[playerDatas.Count];
				string[] catsFlattened = new string[playerDatas.Count];
				string[] pathsFlattened = new string[playerDatas.Count];

				int index = 0;
				foreach (var entry in playerDatas)
				{
					playerRefs[index] = entry.Key;
					genders[index] = entry.Value.gender;
					catsFlattened[index] = string.Join(";", entry.Value.cats);
					pathsFlattened[index] = string.Join(";", entry.Value.paths);
					index++;
				}
				RPC_SendExistEquip(playerRef, playerRefs, genders, catsFlattened, pathsFlattened);
			}

			// Add delay before setting layer

			if (Object.HasInputAuthority)
			{
				SubcribeTelportCallBack(OnStartTeleport, OnFinishTeleport);
				Player.localPlayer = this;
				// set local customize to interact with UI
				this.characterCustomize.SetLocalPlayer();

				LamFusion.Camera.instance.SetFollowPoin(CinemachineCameraTarget.transform);
				_cinemachineTargetYaw = CinemachineCameraTarget.transform.rotation.eulerAngles.y;
				// CustomizeCharacterManager.Instance.LoadPathEquip(SaveDataEquip);
				Util.LoadSavedPath(out string gender, out List<string> listCat, out List<string> listPath);
				SaveDataEquip(gender, listCat, listPath);
				if (FusionMenuManager.instance != null)
					FusionMenuManager.instance.OnDoneJoinRoom();
			}
#if ENABLE_INPUT_SYSTEM
			_playerInput = InputManager.instance.input.playerInput;
#else
            Debug.LogError("Starter Assets package is missing dependencies. Please use Tools/Starter Assets/Reinstall Dependencies to fix it");
#endif
			_jumpTimeoutDelta = JumpTimeout;
			_fallTimeoutDelta = FallTimeout;
		}

		[ContextMenu("SetLayer")]
		public void SetLayer()
		{
			if (_isSetLayered) return;
			_isSetLayered = true;
			int layerToSet = LayerMask.NameToLayer("PlayerCity");
			if (layerToSet == -1)
			{
				Debug.LogError("Layer 'PlayerCity' chưa được tạo!");
			}

			SetLayerRecursively(gameObject, layerToSet);
		}
		// public void DelayedLayerSet()
		// {

		// 	if (_isSetLayered) return;
		// 	_isSetLayered = true;
		// 	int layerToSet = LayerMask.NameToLayer("PlayerCity");
		// 	if (layerToSet == -1)
		// 	{
		// 		Debug.LogError("Layer 'PlayerCity' chưa được tạo!");
		// 	}

		// 	SetLayerRecursively(gameObject, layerToSet);
		// }

		void SetLayerRecursively(GameObject obj, int layer)
		{
			obj.layer = layer;
			foreach (Transform child in obj.transform)
			{
				SetLayerRecursively(child.gameObject, layer);
			}
		}

		private void OnStartTeleport()
		{
			state = PlayerState.teleporting;
		}

		private void OnFinishTeleport()
		{
			state = PlayerState.synchronous;
		}

		public void SaveDataEquip(string gender, List<string> cat, List<string> path)
		{
			if (!Object.HasInputAuthority) return; // Chỉ client mới gửi yêu cầu

			string catsstr = string.Join(";", cat);
			string pathstr = string.Join(";", path);

			RPC_SendEquipData(Object.InputAuthority, gender, catsstr, pathstr);
		}

		[Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
		public void RPC_SendEquipData(PlayerRef playerRef, string gender, string newCats, string newPaths)
		{
			if (!Object.HasStateAuthority) return;

			// Tách danh sách từ string
			var cats = newCats.Split(';');
			var paths = newPaths.Split(';');

			if (cats.Length != paths.Length)
			{
				Debug.LogWarning("Mismatched cats and paths length!");
				return;
			}

			if (playerDatas.TryGetValue(playerRef, out var data))
			{
				var updatedCats = new List<string> { data.cats[0] };
				var updatedPaths = new List<string> { data.paths[0] };

				updatedCats.AddRange(cats);
				updatedPaths.AddRange(paths);

				data.gender = gender;
				data.cats = updatedCats;
				data.paths = updatedPaths;
			}
			else
			{
				playerDatas[playerRef] = new PlayerData(gender, cats.ToList(), paths.ToList());
			}

			RPC_EquipNewCharacter(gender, newCats, newPaths);
		}


		[Rpc(RpcSources.StateAuthority, RpcTargets.All)]
		public void RPC_EquipNewCharacter(string gender, string Cats, string Paths)
		{
			EquipCharacter(gender, Cats, Paths, this.characterCustomize);
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.InputAuthority)]
		public void RPC_SendExistEquip(PlayerRef targetPlayer, PlayerRef[] playerRefs, string[] genders, string[] catsFlattened, string[] pathsFlattened)
		{
			for (int i = 0; i < playerRefs.Length; i++)
			{
				if (Players.TryGetValue(playerRefs[i], out Player player))
				{
					EquipCharacter(genders[i], catsFlattened[i], pathsFlattened[i], player.characterCustomize);
				}
			}
		}

		private void EquipCharacter(string gender, string Cats, string Paths, CustomizationDemo characterCustomize)
		{
			string[] catsArray = Cats.Split(';');
			string[] pathsArray = Paths.Split(';');

			if (catsArray.Length != pathsArray.Length)
			{
				Debug.LogError("[EquipCharacter] Số lượng Cats và Paths không khớp. Kiểm tra dữ liệu!");
				return;
			}

			StartCoroutine(characterCustomize.EquipPath(gender, catsArray.ToList(), pathsArray.ToList()));
		}

		public override void FixedUpdateNetwork()
		{
			if (state == PlayerState.synchronous && InputManager.instance.inputMode == InputMode.Gameplay)
			{
				Move();
			}
			else if (state == PlayerState.free)
			{
				_kcc.enabled = false;
			}
		}

		private void LateUpdate()
		{
			if (Object.HasInputAuthority)
			{
				CameraRotation();
			}
		}

		private void CameraRotation()
		{
			if (Object.HasInputAuthority)
			{
				// if there is an input and camera position is not fixed
				if (_inputNetwork.CurrentInput.look.sqrMagnitude >= _threshold && !LockCameraPosition)
				{
					//Don't multiply mouse input by Time.deltaTime;
					float deltaTimeMultiplier = IsCurrentDeviceMouse ? 1.0f : Time.deltaTime;

					_cinemachineTargetYaw += _inputNetwork.CurrentInput.look.x * deltaTimeMultiplier;
					_cinemachineTargetPitch += _inputNetwork.CurrentInput.look.y * deltaTimeMultiplier;
				}

				// clamp our rotations so our values are limited 360 degrees
				_cinemachineTargetYaw = ClampAngle(_cinemachineTargetYaw, float.MinValue, float.MaxValue);
				_cinemachineTargetPitch = ClampAngle(_cinemachineTargetPitch, BottomClamp, TopClamp);

				// Cinemachine will follow this target
				CinemachineCameraTarget.transform.rotation = Quaternion.Euler(_cinemachineTargetPitch + CameraAngleOverride,
					_cinemachineTargetYaw, 0.0f);
			}
		}

		private void Move()
		{
			float targetSpeed = _inputNetwork.CurrentInput.sprint ? SprintSpeed : MoveSpeed;

			if (_inputNetwork.CurrentInput.move == Vector2.zero) targetSpeed = 0.0f;

			float inputMagnitude = _inputNetwork.CurrentInput.analogMovement ? _inputNetwork.CurrentInput.move.magnitude : 1f;
			_speed = Mathf.Lerp(_speed, targetSpeed * inputMagnitude, Time.deltaTime * SpeedChangeRate);
			_speed = Mathf.Round(_speed * 1000f) / 1000f;

			_animationBlend = Mathf.Lerp(_animationBlend, targetSpeed, Time.deltaTime * SpeedChangeRate);
			if (_animationBlend < 0.01f) _animationBlend = 0f;

			Vector3 inputDirection = new Vector3(_inputNetwork.CurrentInput.move.x, 0.0f, _inputNetwork.CurrentInput.move.y).normalized;

			if (_inputNetwork.CurrentInput.move != Vector2.zero)
			{
				float desiredRotation = Mathf.Atan2(inputDirection.x, inputDirection.z) * Mathf.Rad2Deg + _inputNetwork.CurrentInput.camDirect;

				float angleDifference = Mathf.Abs(Mathf.DeltaAngle(transform.eulerAngles.y, desiredRotation));

				if (angleDifference > 0.1f)
				{
					float rotationSpeedMultiplier = 1.0f / RotationSmoothTime;
					float rotation = Mathf.LerpAngle(transform.eulerAngles.y, desiredRotation, Time.deltaTime * rotationSpeedMultiplier);
					_kcc.AddLookRotation(new Vector2(0.0f, rotation - transform.eulerAngles.y));
				}

				inputDirection = Quaternion.Euler(0, desiredRotation, 0) * Vector3.forward;
			}

			Vector3 moveVelocity = inputDirection * _speed;

			// float jumpImpulse = HandleJumpAndGravity();
			float jumpImpulse = 0;

			_kcc.Move(moveVelocity, jumpImpulse);
		}

		private bool _isJumping = false;

		private float HandleJumpAndGravity()
		{
			float jumpImpulse = 0;

			if (_kcc.IsGrounded)
			{
				// Reset thời gian rơi
				_fallTimeoutDelta = FallTimeout;

				_animator.SetJump(false);
				_animator.SetFall(false);
				_animator.SetIsGround(true);
				_isJumping = false; // Reset trạng thái nhảy khi chạm đất

				// Dừng vận tốc rơi nếu đang chạm đất
				if (_verticalVelocity < 0.0f)
				{
					_verticalVelocity = -2f;
				}

				// Xử lý Jump
				if (!_isJumping && _inputNetwork.CurrentInput.Actions.WasPressed(_inputNetwork.PreviousInput.Actions, GameInput.JUMP_BUTTON) && _jumpTimeoutDelta <= 0.0f)
				{
					// Nhảy với lực nhất định
					jumpImpulse = Mathf.Sqrt(JumpHeight * -2f * Gravity);
					_animator.SetJump(true);
					_isJumping = true; // Đánh dấu trạng thái đang nhảy
				}

				// Đếm thời gian chờ trước khi có thể nhảy tiếp
				if (_jumpTimeoutDelta >= 0.0f)
				{
					_jumpTimeoutDelta -= Runner.DeltaTime;
				}
			}
			else
			{
				_animator.SetIsGround(false);
				// Reset bộ đếm thời gian nhảy
				_jumpTimeoutDelta = JumpTimeout;

				// Đếm thời gian rơi
				if (_fallTimeoutDelta >= 0.0f)
				{
					_fallTimeoutDelta -= Runner.DeltaTime;
				}
				else
				{
					_animator.SetFall(false);
				}

				// Áp dụng trọng lực
				if (_verticalVelocity < _terminalVelocity)
				{
					_verticalVelocity += Gravity * Runner.DeltaTime;
				}
			}

			return jumpImpulse;
		}

		// Cập nhật animation trong Render()
		public override void Render()
		{
			if (_animator)
			{
				float inputMagnitude = _inputNetwork.CurrentInput.analogMovement ? _inputNetwork.CurrentInput.move.magnitude : 1f;
				_animator.SetSpeed(_animationBlend);
				_animator.SetMotionSpeed(inputMagnitude);
			}
		}

		private static float ClampAngle(float lfAngle, float lfMin, float lfMax)
		{
			if (lfAngle < -360f) lfAngle += 360f;
			if (lfAngle > 360f) lfAngle -= 360f;
			return Mathf.Clamp(lfAngle, lfMin, lfMax);
		}

		private void OnDrawGizmosSelected()
		{
			Color transparentGreen = new Color(0.0f, 1.0f, 0.0f, 0.35f);
			Color transparentRed = new Color(1.0f, 0.0f, 0.0f, 0.35f);

			if (Grounded) Gizmos.color = transparentGreen;
			else Gizmos.color = transparentRed;

			// when selected, draw a gizmo in the position of, and matching radius of, the grounded collider
			Gizmos.DrawSphere(
				new Vector3(transform.position.x, transform.position.y - GroundedOffset, transform.position.z),
				GroundedRadius);
		}

		public void SubcribeTelportCallBack(Action callbackOnStart, Action callbackOnFinish)
		{
			teleport.Subcribe(callbackOnStart, callbackOnFinish);
		}

		public void SetState(PlayerState newState)
		{
			state = newState;
		}

		public void Teleport(Vector3 targetPosition)
		{
			if (teleport && Object.HasInputAuthority)
			{
				// Set state before starting teleport process
				state = PlayerState.teleporting;

				// Force position update in KCC to prevent initial resistance
				if (_kcc)
				{
					_kcc.SetPosition(targetPosition);
				}

				StartCoroutine(teleport.TriggerTeleport(targetPosition));
			}
		}
	}
}
