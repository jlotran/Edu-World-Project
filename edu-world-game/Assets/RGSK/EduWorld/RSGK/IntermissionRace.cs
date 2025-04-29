using StarterAssets;
using UnityEngine;
using UnityEngine.UI;
namespace RGSK
{
    using Lam.GAMEPLAY;
    public class IntermissionRace : MonoBehaviour
    {
        public static IntermissionRace Instance { get; private set; }

        [SerializeField] UIScreenID screen;

        [SerializeField] Button playRaceButton;

        [SerializeField] CanvasGroup canvasGroup;



        void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
            }
            else
            {
                Destroy(gameObject);
            }
        }

        void Start()
        {
            playRaceButton.onClick.AddListener(() => { InitializationLoader.Execute(); });
            playRaceButton.onClick.AddListener(() => { screen.Open(); });
            playRaceButton.onClick.AddListener(() => { Close(); });
        }

        public void Open()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 1f;
                canvasGroup.interactable = true;
                canvasGroup.blocksRaycasts = true;

                if(Lam.GAMEPLAY.InputManager.instance.input != null)
                {
                    Lam.GAMEPLAY.InputManager.instance.DisableInput();
                }
                else
                {
                    Debug.LogError("InputManager Instance is null or input is not set.");
                }
                Lam.GAMEPLAY.InputManager.instance.input.SetCursorState(false);

            }
        }

        public void Close()
        {
            if (canvasGroup != null)
            {
                canvasGroup.alpha = 0f;
                canvasGroup.interactable = false;
                canvasGroup.blocksRaycasts = false;
                if(Lam.GAMEPLAY.InputManager.instance.input != null)
                {
                    // Lam.GAMEPLAY.InputManager.instance.EnableInput();
                }
                else
                {
                    Debug.LogError("InputManager Instance is null or input is not set.");
                }
                Lam.GAMEPLAY.InputManager.instance.input.SetCursorState(true);
            }
        }

    }
}
