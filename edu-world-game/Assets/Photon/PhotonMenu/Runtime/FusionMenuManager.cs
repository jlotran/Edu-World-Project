using System.Threading.Tasks;
namespace Fusion.Menu
{
  using System;
  using System.Collections.Generic;
  using System.Threading.Tasks;
  using Lam.FUSION;
  using HathoraCloud.Models.Shared;
  using UnityEngine;
  using global::Lam.FUSION;
  using SimpleFPS;
    using global::Lam.GAMEPLAY;

    /// <summary>
    /// The main menu.
    /// </summary>
    public partial class FusionMenuManager : PhotonMenuUIScreen
  {

    public static FusionMenuManager instance;

    /// <summary>
    /// Callback fired before the connection is created.
    /// This can stop the connection attempt with an <see cref="ConnectResult"/>.
    /// </summary>

    public Func<IPhotonMenuConnectArgs, Task<ConnectResult>> OnBeforeConnection;

    partial void AwakeUser();
    partial void InitUser();
    partial void BeforeConnectUser();


    /// <summary>
    /// The Unity awake method. Calls partial method <see cref="AwakeUser"/> to be implemented on the SDK side.
    /// Applies the current selected graphics settings (loaded from PlayerPrefs)
    /// </summary>
    public override void Awake()
    {
      base.Awake();

      instance = this;

      new PhotonMenuGraphicsSettings().Apply();
      AwakeUser();
    }

    /// <summary>
    /// The screen init method. Calls partial method <see cref="InitUser"/> to be implemented on the SDK side.
    /// Initialized the default arguments.
    /// </summary>
    public override void Init()
    {
      base.Init();
      ConnectionArgs.SetDefaults(Config);

      InitUser();
    }
    /// <summary>
    /// Is called when the <see cref="_playButton"/> is pressed using SendMessage() from the UI object.
    /// Intitiates the connection and expects the connection object to set further screen states.
    /// </summary>
    protected virtual async void OnPlayButtonPressed()
    {
      ConnectionArgs.Session = null;
      ConnectionArgs.Creating = false;
      ConnectionArgs.Region = ConnectionArgs.PreferredRegion;

      // Debug.Log(ConnectionArgs.Scene.Name);

      BeforeConnectUser();

      Controller.Show<PhotonMenuUILoading>();

      var result = new ConnectResult { Success = true };
      if (OnBeforeConnection != null)
      {
        result = await OnBeforeConnection.Invoke(ConnectionArgs);
      }

      if (result.Success)
      {
        result = await Connection.ConnectAsync(ConnectionArgs);
      }

      if (result.CustomResultHandling == false)
      {
        if (result.Success)
        {
          // Controller.Show<PhotonMenuUIGameplay>();
          Debug.Log("Join game successfully");
        }
        else
        {
          var popup = Controller.PopupAsync(result.DebugMessage, "Connection Failed");
          if (result.WaitForCleanup != null)
          {
            await Task.WhenAll(result.WaitForCleanup, popup);
          }
          else
          {
            await popup;
          }
          Controller.Show<FusionMenuMainUI>();
        }
      }
    }

    public virtual async void OnJoinRoom(string session)
    {
      await Connection.DisconnectAsync(ConnectFailReason.UserRequest);

      ConnectionArgs.Session = null;
      ConnectionArgs.Creating = false;
      ConnectionArgs.Region = ConnectionArgs.PreferredRegion;

      // Debug.Log(ConnectionArgs.Scene.Name);

      BeforeConnectUser();

      Controller.Show<PhotonMenuUILoading>();

      var result = new ConnectResult { Success = true };
      if (OnBeforeConnection != null)
      {
        result = await OnBeforeConnection.Invoke(ConnectionArgs);
      }

      if (result.Success)
      {
        // disconnect with current room

        ConnectionArgs.Session = session;

        // connec to new room
        result = await Connection.ConnectAsync(ConnectionArgs);
      }

      if (result.CustomResultHandling == false)
      {
        if (result.Success)
        {
          Debug.Log("Join game successfully. SessionId = " + session);
        }
        else
        {
          var popup = Controller.PopupAsync(result.DebugMessage, "Connection Failed");
          if (result.WaitForCleanup != null)
          {
            await Task.WhenAll(result.WaitForCleanup, popup);
          }
          else
          {
            await popup;
          }
          Controller.Show<FusionMenuMainUI>();
        }
      }
    }

    public void OnDoneJoinRoom()
    {
      Controller.Show<PhotonMenuUIGameplay>();
    }

    public virtual void OnListRoomPressed()
    {
      _ = GetRooms(GameRoomType.city);

    }

    public async Task<List<LobbyV3>> GetRooms(GameRoomType roomType, LevelType level = LevelType.Intern)
    {
      return await GetListLobby(roomType, level);
    }

    // public async Task<List<LobbyV3>> GetRacingLobbies(LevelType level = LevelType.Intern)
    // {
    //   return await GetListLobby(GameRoomType.racing, level);
    // }

    public async Task<List<LobbyV3>> GetListLobby(GameRoomType roomType, LevelType level = LevelType.Intern)
    {
      return await Connection.GetListLobby(ConnectionArgs, roomType, level);
    }

    public async Task OnQuitMatch(bool isLoading = false)
    {
      await Connection.DisconnectAsync(ConnectFailReason.UserRequest);
      if (isLoading)
      {
        Controller.Show<PhotonMenuUILoading>();
      }
      else
      {
        Controller.Show<FusionMenuMainUI>();
      }
    }

    public async Task JoinRandomRoom(GameRoomType gameRoomType = GameRoomType.city)
    {
      await OnQuitMatch(true);

      // get random Room ID
      var rooms = await GetListLobby(gameRoomType, LevelType.Intern);
      if (rooms.Count < 0)
      {
        Debug.Log("No Room Available");
        return;
      }

      float _sessionTimer = 10.0f;
      float lastTime = Time.realtimeSinceStartup;
      while (_sessionTimer > 0.0f)
      {
      
        float currentTime = Time.realtimeSinceStartup;
        float deltaTime = currentTime - lastTime;
        lastTime = currentTime;

        _sessionTimer -= deltaTime;

        // Yield per frame on WebGL, else small delay
#if UNITY_WEBGL
        await Task.Yield();
#else
    await Task.Delay(100);
#endif
      }

      LobbyV3 randomRoom = rooms[UnityEngine.Random.Range(0, rooms.Count)];
      OnJoinRoom(randomRoom.RoomId);
    }
  }
}
