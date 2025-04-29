using UnityEngine;
using Fusion.Menu;
using System.Threading.Tasks;
using Lam.GAMEPLAY;

namespace Lam.FUSION
{
    public class CompanyInteract : MonoBehaviour
    {
        [SerializeField] private GameRoomType gameRoomType;
        private bool isPlayerInTrigger = false;
        private bool isChangeScene = false;
        void Update()
        {
            if (isPlayerInTrigger)
            {
                if (InputManager.instance.input.shopInteract && !isChangeScene)
                {
                    isChangeScene = true;
                    _ = ChangeScene();
                }
            }
        }

        private async Task ChangeScene()
        {
            await FusionMenuManager.instance.JoinRandomRoom(gameRoomType);
        }

        void OnTriggerEnter(Collider other)
        {
            if (other.transform.root.CompareTag("Player"))
            {
                isPlayerInTrigger = true;
            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.transform.root.CompareTag("Player"))
            {
                isPlayerInTrigger = false;
            }
        }
    }
}
