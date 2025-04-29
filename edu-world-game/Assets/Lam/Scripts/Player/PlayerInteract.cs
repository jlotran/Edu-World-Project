using System;
using System.Collections;
using Edu_World;
using Fusion;
using Lam.GAMEPLAY;
using Lam.UI;
using SimpleFPS;
using StarterAssets;
using UnityEngine;
using UnityEngine.Events;
namespace Edu_World
{

    public enum InteractionTag
    {
        None,
        Car_Interact,    // For showing button E
        Shop_Interact,
        CarTrigger,      // For showing UI Car
    }

    /// <summary>
    /// Handles player interactions with objects in the game world
    /// </summary>
    public class PlayerInteract : NetworkBehaviour
    {
        #region Singleton
        public static PlayerInteract LocalInstance; // Only local player instance
        #endregion

        #region Interaction Settings
        [Header("Interaction Configuration")]
        [Tooltip("Cooldown time between interactions")]
        [SerializeField] private float interactionCooldown = 1f;

        [Tooltip("Radius within which player can interact with objects")]
        [SerializeField] private float interactionRadius = 5f;
        #endregion

        #region State Variables
        private bool playerInRange = false;
        private IInteractable currentInteractable;
        #endregion

        #region Events
        [Header("Events")]
        public UnityEvent<CarIdHolder> OnGetCarData;
        public event Action OnActionGUI;

        #endregion

        #region Unity Lifecycle Methods
        private void Awake()
        {
            // Local player initialization
        }

        private void Start()
        {
            if (Object.HasInputAuthority)
            {
                if (LocalInstance == null)
                {
                    LocalInstance = this;
                    StartCoroutine(RegisterInput());
                }
                else
                    Destroy(gameObject);
            }
        }

        private void OnDisable()
        {
            if (Object != null && Object.HasInputAuthority)
            {
                if (InputManager.instance.input != null)
                {
                    InputManager.instance.input.OnActionPanel -= HandleTriggerInput;
                }
                else
                {
                    Debug.LogWarning("StarterAssetsInputs.instance is NULL in OnDisable. It might have been destroyed.");
                    return;
                }
            }

        }

        void OnTriggerEnter(Collider other)
        {
            if (!Object.HasInputAuthority) return;

            if (TryGetInteractionTag(other.tag, out InteractionTag tag))
            {
                if (tag == InteractionTag.Car_Interact || tag == InteractionTag.Shop_Interact)
                {
                    playerInRange = true;
                    UIInteract.instance.ActiveButtonE();

                    currentInteractable = other.GetComponent<IInteractable>();
                    currentInteractable?.OnInteract();
                }
            }
        }

        private void OnTriggerExit(Collider other)
        {
            if (!Object.HasInputAuthority) return;

            if (TryGetInteractionTag(other.tag, out InteractionTag tag))
            {
                if (tag == InteractionTag.Car_Interact || tag == InteractionTag.Shop_Interact)
                {
                    playerInRange = false;
                    UIInteract.instance.DeactiveButtonE();

                    if (tag == InteractionTag.Car_Interact)
                    {
                        CameraCar.instance.CameraCarOff();
                    }

                    currentInteractable?.OnInteractEnd();
                    currentInteractable = null;
                }
            }
        }
        #endregion

        #region Network Methods
        public void OnNetworkDespawn()
        {
            if (Object.HasInputAuthority && InputManager.instance.input != null)
            {
                InputManager.instance.input.OnActionPanel -= HandleTriggerInput;
            }

            if (LocalInstance == this)
            {
                LocalInstance = null;
            }
        }
        #endregion

        #region Input Handling
        private IEnumerator RegisterInput()
        {
            while (InputManager.instance.input == null)
            {
                yield return null;
            }

            // Register input events
            InputManager.instance.input.OnActionPanel += HandleTriggerInput;
        }

        /// <summary>
        /// Handles the trigger input for interactions and UI toggles
        /// </summary>
        protected virtual void HandleTriggerInput(bool pressed)
        {
            if (LamFusion.UIteleport.IsTeleporting ||
                (CarUI.instance != null && CarUI.instance.IsSuccessPanelActive()))
                return;

            if (pressed && Object.HasInputAuthority)
            {
                if (playerInRange && UIManager.Instance.CanInteractWithCarOrShop())
                {
                    UIInteract.instance.ToggleButtonE();
                    InputManager.instance.input.ToggleCursorState();
                    CheckInteraction();
                }
                else if (UIManager.Instance?.CanToggleUI() ?? false)
                {
                    OnOpenCloseGUI();
                }
            }
        }
        #endregion

        #region Interaction Methods
        /// <summary>
        /// Checks for interactive objects within the interaction radius
        /// </summary>
        private void CheckInteraction()
        {
            Vector3 checkPosition = transform.position + Vector3.up;
            var colliders = Physics.OverlapSphere(checkPosition, interactionRadius);

            foreach (var collider in colliders)
            {
                if (TryGetInteractionTag(collider.tag, out InteractionTag tag))
                {
                    switch (tag)
                    {
                        case InteractionTag.CarTrigger:
                            HandleCarInteraction(collider);
                            return;
                        case InteractionTag.Shop_Interact:
                            HandleShopInteraction(collider);
                            return;
                    }
                }
            }
        }

        private bool TryGetInteractionTag(string tag, out InteractionTag interactionTag)
        {
            interactionTag = InteractionTag.None;
            switch (tag)
            {
                case "Car_Interact":
                    interactionTag = InteractionTag.Car_Interact;
                    return true;
                case "Shop_Interact":
                    interactionTag = InteractionTag.Shop_Interact;
                    return true;
                case "CarTrigger":
                    interactionTag = InteractionTag.CarTrigger;
                    return true;
                default:
                    return false;
            }
        }

        /// <summary>
        /// Handles interaction with vehicles
        /// </summary>
        private void HandleCarInteraction(Collider collider)
        {
            if (collider.GetComponentInParent<CarIdHolder>() is CarIdHolder carIdHolder)
            {
                GetCarData(carIdHolder);
            }
        }

        /// <summary>
        /// Handles interaction with shop triggers
        /// </summary>
        private void HandleShopInteraction(Collider collider)
        {
            if (collider.GetComponent<ShopTrigger>() is ShopTrigger shop)
            {
                shop.OnTriggerActivated();
            }
        }
        private void GetCarData(CarIdHolder carIdHolder)
        {
            OnGetCarData.Invoke(carIdHolder);
        }

        public void OnOpenCloseGUI()
        {
            OnActionGUI?.Invoke();
        }
        #endregion

        #region Debug
        private void OnDrawGizmosSelected()
        {
            // Draw interaction radius
            Gizmos.color = Color.yellow;
            Gizmos.DrawWireSphere(transform.position + Vector3.up, interactionRadius);
        }
        #endregion
    }

}