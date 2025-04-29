using System.Collections;
using System.Collections.Generic;
using Fusion.Menu;
using Fusion.Menu.Lam.FUSION;
using HathoraCloud.Models.Shared;
using LamFusion;
using TMPro;
using UnityEditor;
using UnityEngine;
using UnityEngine.UI;

namespace Lam.FUSION
{
     public class RoomItem : MonoBehaviour
     {
          [SerializeField] private TextMeshProUGUI roomID_text;
          [SerializeField] private Button join_btn;

          public void Init(LobbyV3 lobby)
          {
               HathoraRoomConfig roomConfig = JsonUtility.FromJson<HathoraRoomConfig>(lobby.RoomConfig);
               GameRoomType gameRoomType = roomConfig.roomType;
               string name = roomConfig.name;
               if (roomID_text)
                    roomID_text.text = gameRoomType == GameRoomType.city ? $"City-{name}" : $"{roomConfig.level} {name}";
               join_btn.onClick.AddListener(() => joinRoom(lobby.RoomId));
          }

          private void joinRoom(string roomID)
          {
               FusionMenuManager.instance.OnJoinRoom(roomID);
          }
     }
}
