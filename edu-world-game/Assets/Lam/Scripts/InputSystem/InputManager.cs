using UnityEngine;
using StarterAssets;

namespace Lam.GAMEPLAY
{
    public enum InputMode
    {
        Gameplay,
        UI,
        Disabled
    }

    public class InputManager : Lam.FUSION.Singleton<InputManager>
    {
        [SerializeField] private StarterAssetsInputs _input;
        public StarterAssetsInputs input => _input;
        public InputMode inputMode { get; private set;}

        private void Start()
        {
            inputMode = InputMode.Gameplay;
            if (!input)
            {
                _input = GetComponent<StarterAssetsInputs>();
            }
        }

        public void DisableInput()
        {
            inputMode = InputMode.Disabled;
        }

        public void EnableInput()
        {
            inputMode = InputMode.Gameplay;
        }
    }
}
