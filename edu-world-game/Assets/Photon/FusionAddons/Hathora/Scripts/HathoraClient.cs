using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using Hathora.Core.Scripts.Runtime.Client;
using HathoraCloud.Models.Shared;
using HathoraCloud.Models.Operations;
using Fusion.Photon.Realtime;
using Fusion.Sockets;
using Lam.FUSION;
using Lam.GAMEPLAY;
using Lam;

namespace Fusion.Addons.Hathora
{
	public class HathoraClient : HathoraClientMgr, INetworkRunnerCallbacks
	{
		public string SessionName => _sessionName;
		public string SessionRegion => _sessionRegion;
		public bool HasValidSession => string.IsNullOrEmpty(_sessionName) == false;

		[Header(nameof(Fusion))]
		[SerializeField]
		private bool _forceSinglePeerMode;
		[SerializeField]
		private bool _enableLogs;

		private string _sessionName;
		private string _sessionRegion;
		private float _sessionTimer;
		private float _initializeTime;
		private List<LobbyV3> _hathoraLobbies;

		public void ResetHathoraClient()
		{
			_sessionName = default;
			_sessionRegion = default;
			_hathoraLobbies = default;
			_sessionTimer = default;
		}

		public async Task<List<LobbyV3>> GetLobbyV3Asyncs(string preferredRegion, GameRoomType Roomtype, LevelType level)
		{
			PlayerTokenObject loginResponse = await AuthLoginAsync();
			if (string.IsNullOrEmpty(loginResponse.Token) == true)
			{
				LogError($"Hathora authentication login failed!");
				return null;
			}

			_sessionRegion = preferredRegion;
			if (string.IsNullOrEmpty(_sessionRegion) == true)
			{
				(bool bestRegionFound, Region bestHathoraRegion, double bestRegionPing) = await HathoraRegionUtility.FindBestRegion(HathoraSdk, Region.Frankfurt);
				_sessionRegion = HathoraRegionUtility.HathoraToPhoton(bestHathoraRegion);

				if (bestRegionFound == true)
				{
					LogInfo($"Found best Hathora region: {bestHathoraRegion}, Ping: {bestRegionPing:0}ms, Photon region: {_sessionRegion}");
				}
				else
				{
					LogWarning($"Best Hathora region not found! Using {bestHathoraRegion} (fallback), Photon region: {_sessionRegion}");
				}
			}

			Region hathoraRegion = HathoraRegionUtility.PhotonToHathora(_sessionRegion);
			LobbyV3 hathoraLobby = default;

			ListActivePublicLobbiesRequest publicLobbiesRequest = new ListActivePublicLobbiesRequest();
			publicLobbiesRequest.Region = hathoraRegion;
			_hathoraLobbies = await this.GetActivePublicLobbiesAsync(publicLobbiesRequest);
			
			List<LobbyV3> filteredLobbies = new List<LobbyV3>();
			if (_hathoraLobbies != null)
			{
				foreach (var lobby in _hathoraLobbies)
				{
					string jsonConfig = lobby.RoomConfig; 
					HathoraRoomConfig roomConfig = JsonUtility.FromJson<HathoraRoomConfig>(jsonConfig);

					if (roomConfig != null && roomConfig.roomType == Roomtype)
					{
						filteredLobbies.Add(lobby);
					}
				}
			}

			if (filteredLobbies == null || filteredLobbies.Count == 0)
			{

				hathoraLobby = await CreateLobbyByRoomType(Roomtype, hathoraRegion, level);
				if (hathoraLobby == null)
				{
					LogError($"Failed to create Hathora lobby! Region: {hathoraRegion}, RoomId: {null}");
					return null;
				}

				LogInfo($"Created new Hathora lobby. Region: {hathoraRegion}, RoomId: {hathoraLobby.RoomId}");

				filteredLobbies.Add(hathoraLobby);
			}
			return filteredLobbies;
		}

		public async Task<bool> Initialize(NetworkRunner runnerPrefab, string preferredRegion, string roomId)
		{
			if (roomId == null) return false;

			if (_forceSinglePeerMode == true)
			{
				NetworkProjectConfig.Global.PeerMode = NetworkProjectConfig.PeerModes.Single;
			}

			_initializeTime = Time.realtimeSinceStartup;

			LogInfo($"Initializing. {nameof(preferredRegion)}: {preferredRegion}, {nameof(roomId)}: {roomId}");

			// Reset values to default.
			_sessionName = default;
			_sessionRegion = default;
			_hathoraLobbies = default;

			// 1. Login to Hathora cloud.
			PlayerTokenObject loginResponse = await AuthLoginAsync();
			if (string.IsNullOrEmpty(loginResponse.Token) == true)
			{
				LogError($"Hathora authentication login failed!");
				return false;
			}

			LogInfo($"Hathora authentication login success. {nameof(loginResponse.Token)} {loginResponse.Token}");

			// 2. Use preferred region or find a region with best ping.
			_sessionRegion = preferredRegion;
			if (string.IsNullOrEmpty(_sessionRegion) == true)
			{
				(bool bestRegionFound, Region bestHathoraRegion, double bestRegionPing) = await HathoraRegionUtility.FindBestRegion(HathoraSdk, Region.Frankfurt);
				_sessionRegion = HathoraRegionUtility.HathoraToPhoton(bestHathoraRegion);

				if (bestRegionFound == true)
				{
					LogInfo($"Found best Hathora region: {bestHathoraRegion}, Ping: {bestRegionPing:0}ms, Photon region: {_sessionRegion}");
				}
				else
				{
					LogWarning($"Best Hathora region not found! Using {bestHathoraRegion} (fallback), Photon region: {_sessionRegion}");
				}
			}

			Region hathoraRegion = HathoraRegionUtility.PhotonToHathora(_sessionRegion);
			LobbyV3 hathoraLobby = default;

			// 3. Get existing public lobbies for random join.
			// if (string.IsNullOrEmpty(roomId) == true)
			// {
			ListActivePublicLobbiesRequest publicLobbiesRequest = new ListActivePublicLobbiesRequest();
			publicLobbiesRequest.Region = hathoraRegion;
			_hathoraLobbies = await this.GetActivePublicLobbiesAsync(publicLobbiesRequest);
			LogInfo($"Found {(_hathoraLobbies != null ? _hathoraLobbies.Count : 0)} public Hathora lobbies for join.");
			// }

			if (_hathoraLobbies == null || _hathoraLobbies.Count == 0)
			{
				if (roomId == null)
				{
					hathoraLobby = await CreateLobbyByRoomType(GameRoomType.city, hathoraRegion);
					if (hathoraLobby == null)
					{
						LogError($"Failed to create Hathora lobby! Region: {hathoraRegion}, RoomId: {roomId}");
						return false;
					}

					LogInfo($"Created new Hathora lobby. Region: {hathoraRegion}, RoomId: {hathoraLobby.RoomId}");

					_hathoraLobbies = new List<LobbyV3>();
					_hathoraLobbies.Add(hathoraLobby);
				}
			}

			// 5. Connect to Photon lobby. We need to connect to one of public lobbies (3) or to the session created (4).
			FusionAppSettings appSettings = PhotonAppSettings.Global.AppSettings.GetCopy();
			appSettings.FixedRegion = _sessionRegion;
			NetworkRunner lobbyRunner = Instantiate(runnerPrefab);
			lobbyRunner.AddCallbacks(this);
			StartGameResult joinLobbyResult = await lobbyRunner.JoinSessionLobby(SessionLobby.ClientServer, customAppSettings: appSettings);
			if (joinLobbyResult.Ok == false)
			{
				LogError($"Failed to join Photon lobby! Region: {appSettings.FixedRegion}");
				lobbyRunner.RemoveCallbacks(this);
				await lobbyRunner.Shutdown(true);
				return false;
			}

			LogInfo($"Joined Photon lobby. Region: {appSettings.FixedRegion}");

			// 6. Set a timeout for join.
			_sessionTimer = 30.0f;

			float lastTime = Time.realtimeSinceStartup;
			while (_sessionTimer > 0.0f)
			{
				// 7. Time out if we can't join the lobby ==> return false.
				if (_sessionTimer < 20.0f && hathoraLobby == null)
				{
					LogError($"Failed to Join Hathora lobby! Region: {hathoraRegion}, RoomId: {roomId}");
					return false;
				}

				float currentTime = Time.realtimeSinceStartup;
				float deltaTime = currentTime - lastTime;
				lastTime = currentTime;

				_sessionTimer -= deltaTime;

#if UNITY_WEBGL
				await Task.Yield();
#else
				await Task.Delay(100);
#endif
			}

			lobbyRunner.RemoveCallbacks(this);
			await lobbyRunner.Shutdown(true);

			if (HasValidSession == true)
			{
				_sessionName = roomId;
				LogInfo($"Fusion server on Hathora found! Session: {_sessionName}, Region: {_sessionRegion}");
			}
			else
			{
				LogWarning($"Fusion server on Hathora not found!");
			}

			return HasValidSession;
		}

		public async Task<LobbyV3> CreateLobbyByRoomType(GameRoomType roomType, Region region, LevelType level = LevelType.Intern, string name = "")
		{
			PlayerTokenObject loginResponse = await AuthLoginAsync();
			if (string.IsNullOrEmpty(loginResponse.Token) == true)
			{
				LogError($"Hathora authentication login failed!");
				return null;
			}

			if (string.IsNullOrEmpty(name))
			{
				name = Util.GenerateRandomString(8);
			}
			HathoraRoomConfig roomConfig = new HathoraRoomConfig()
			{
				roomType = roomType,
				level = level,
				name = name,
			};

			LobbyV3 lobby = await this.CreateLobbyAsync(roomConfig, region, null);
			if (lobby == null)
			{
				LogError($"Failed to create Hathora lobby! Region: {region}, RoomId: {null}");
				return null;
			}

			return lobby;
		}

		void INetworkRunnerCallbacks.OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
		{
			foreach (SessionInfo session in sessionList)
			{
				if (session.IsOpen == false)
					continue;

				foreach (LobbyV3 hathoraLobby in _hathoraLobbies)
				{
					if (session.Name == hathoraLobby.RoomId && (session.PlayerCount < session.MaxPlayers || session.MaxPlayers <= 0))
					{
						_sessionName = session.Name;
						_sessionTimer = default;
						return;
					}
				}
			}
		}

		void INetworkRunnerCallbacks.OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
		void INetworkRunnerCallbacks.OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
		void INetworkRunnerCallbacks.OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
		void INetworkRunnerCallbacks.OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
		void INetworkRunnerCallbacks.OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
		void INetworkRunnerCallbacks.OnInput(NetworkRunner runner, NetworkInput input) { }
		void INetworkRunnerCallbacks.OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
		void INetworkRunnerCallbacks.OnConnectedToServer(NetworkRunner runner) { }
		void INetworkRunnerCallbacks.OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
		void INetworkRunnerCallbacks.OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
		void INetworkRunnerCallbacks.OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
		void INetworkRunnerCallbacks.OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
		void INetworkRunnerCallbacks.OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
		void INetworkRunnerCallbacks.OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
		void INetworkRunnerCallbacks.OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
		void INetworkRunnerCallbacks.OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
		void INetworkRunnerCallbacks.OnSceneLoadStart(NetworkRunner runner) { }
		void INetworkRunnerCallbacks.OnSceneLoadDone(NetworkRunner runner) { }

		private void LogInfo(object message) { if (_enableLogs == true) Debug.Log($"[{nameof(HathoraClient)}][{(Time.realtimeSinceStartup - _initializeTime):F3}] {message}", gameObject); }
		private void LogWarning(object message) { if (_enableLogs == true) Debug.LogWarning($"[{nameof(HathoraClient)}][{(Time.realtimeSinceStartup - _initializeTime):F3}] {message}", gameObject); }
		private void LogError(object message) { if (_enableLogs == true) Debug.LogError($"[{nameof(HathoraClient)}][{(Time.realtimeSinceStartup - _initializeTime):F3}] {message}", gameObject); }
	}
}
