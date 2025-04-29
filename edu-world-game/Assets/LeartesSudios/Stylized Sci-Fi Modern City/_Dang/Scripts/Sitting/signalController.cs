using Lam.FUSION;
using UnityEngine;

namespace EduWorld
{
    public class signalController : MonoBehaviour
    {

        private Player _playerController;

        public Player playerController
        {
            get
            {
                if (_playerController == null)
                {
                    _playerController = Player.localPlayer;
                }

                return _playerController;
            }
        }

        public void DisablePlayerInput()
        {
            if (playerController == null) return;

            playerController.FreeKcc();
            playerController.DisableInput();
        }

        public void EnablePlayerInput()
        {
            if (playerController == null) return;

            Lam.GAMEPLAY.InputManager.instance.EnableInput();
            playerController.EnableSynchronous();
        }
    }
}
