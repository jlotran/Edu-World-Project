namespace Fusion.Menu
{
  using System.Collections.Generic;
    using global::Lam;
    using global::Lam.FUSION;
    using HathoraCloud.Models.Shared;
  using UnityEngine;
  using UnityEngine.UI;

  namespace Lam.FUSION
  {
    /// <summary>
    /// The gameplay screen.
    /// </summary>
    public class ListRoom : MonoBehaviour
    {
      [SerializeField] private Button back_btn;

      [Header("Room")]
      [SerializeField] private Transform roomItemPref;
      [SerializeField] private Transform containerRoom;
      [SerializeField] private GameObject loading;

      public void Start()
      {
        back_btn.onClick.AddListener(OnBackBtnClick);
      }

      public void Show()
      {
        Util.ClearChildren(containerRoom);
        Instantiate(loading, containerRoom);
        gameObject.SetActive(true);
      }

      public void SetDataRooms(List<LobbyV3> listRoom)
      {
        Util.ClearChildren(containerRoom);
        foreach (LobbyV3 lobby in listRoom)
        {
          RoomItem roomItem = Instantiate(roomItemPref, containerRoom).GetComponent<RoomItem>();
          roomItem.Init(lobby);
        }
      }

      private void OnBackBtnClick()
      {
        this.gameObject.SetActive(false);
      }
    }
  }
}
