using UnityEngine;
using UnityEngine.EventSystems;

#if ENABLE_INPUT_SYSTEM
using UnityEngine.InputSystem;
#endif

namespace StarterAssets
{
	public class StarterAssetsInputs : MonoBehaviour
	{
		public UnityEngine.InputSystem.PlayerInput playerInput;

		[Header("Character Input Values")]
		public Vector2 move;
		public Vector2 look;
		public bool jump;
		public bool sprint;
		public bool shopInteract; // Add this new field

		[Header("Movement Settings")]
		public bool analogMovement;

		[Header("Mouse Cursor Settings")]
		public bool cursorLocked = true;
		public bool cursorInputForLook = true;

		private bool isEscPressed = false;

		public System.Action<bool> OnActionPanel;

#if ENABLE_INPUT_SYSTEM
		public void OnMove(InputValue value)
		{
			MoveInput(value.Get<Vector2>());
		}

		public void OnLook(InputValue value)
		{
			if (cursorInputForLook)
			{
				LookInput(value.Get<Vector2>());
			}
		}

		public void OnJump(InputValue value)
		{
			JumpInput(value.isPressed);
		}

		public void OnSprint(InputValue value)
		{
			SprintInput(value.isPressed);
		}

		public void OnEscape(InputValue value)
		{
			if (value.isPressed)
			{
				isEscPressed = !isEscPressed;
				SetCursorState(!isEscPressed);
				Debug.Log("Escape Pressed: " + isEscPressed);
			}
		}

		public void OnPanel(InputValue value)
		{
			shopInteract = value.isPressed;
			OnActionPanel?.Invoke(value.isPressed);
		}
#endif
		public void MoveInput(Vector2 newMoveDirection)
		{
			move = newMoveDirection;
		}

		public void LookInput(Vector2 newLookDirection)
		{
			look = newLookDirection;
		}

		public void SprintInput(bool newSprintState)
		{
			sprint = newSprintState;
		}

		private void OnApplicationFocus(bool hasFocus)
		{
			SetCursorState(cursorLocked);
		}

		public void SetCursorState(bool newState)
		{
			cursorLocked = newState;
			Cursor.lockState = newState ? CursorLockMode.Locked : CursorLockMode.None;
			Cursor.visible = !newState;
		}
		
		public void ToggleCursorState()
		{
			SetCursorState(!cursorLocked);
		}

		public void JumpInput(bool newJumpState)
		{
			jump = newJumpState;
		}
	}
}
