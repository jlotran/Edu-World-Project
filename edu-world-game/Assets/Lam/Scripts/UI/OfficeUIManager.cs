using Fusion.Menu;
using Lam.FUSION;
using UnityEngine;
using UnityEngine.UI;

namespace Lam.UI
{
    public class OfficeUIManager : MonoBehaviour
    {
        [SerializeField] private Button back_btn;
        
        private void Start()
        {
            // back_btn.onClick.AddListener(OnBackBtnClick);
        }

        private async void OnBackBtnClick()
        {
            await FusionMenuManager.instance.JoinRandomRoom(GameRoomType.city);
        }
    }
}
