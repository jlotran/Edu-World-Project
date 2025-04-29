using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion.Addons.Hathora;
using Fusion.Menu;
using HathoraCloud;
using UnityEngine;
using HathoraCloud.Models.Shared;
using Lam.FUSION;
using Lam.GAMEPLAY;

namespace SimpleFPS
{
	public class HathoraMenuConnection : FusionMenuConnection
	{
		private HathoraManager _manager;

		public HathoraMenuConnection(FusionMenuConnectionBehaviour connectionBehaviour, HathoraManager manager) : base(connectionBehaviour)
		{
			_manager = manager;
		}

		public override async Task<ConnectResult> ConnectAsync(IPhotonMenuConnectArgs connectionArgs)
		{

			HathoraClient hathoraClient = _manager.GetOrCreateClientInstance();
			await hathoraClient.Initialize(_manager.RunnerPrefab, connectionArgs.Region, connectionArgs.Session);

			if (hathoraClient.HasValidSession == true)
			{
			connectionArgs.Creating = false;
				connectionArgs.Session  = hathoraClient.SessionName;
				connectionArgs.Region   = hathoraClient.SessionRegion;
			}
			else
			{
				ConnectResult connectResult = new ConnectResult();
				connectResult.FailReason = ConnectFailReason.Disconnect;
				connectResult.DebugMessage = "Failed to connect to Hathora session.";
				return connectResult;
			}

			// Hathora + Fusion session found, using base implementation to connect.
			return await base.ConnectAsync(connectionArgs);
		}

		public override async Task<List<LobbyV3>> GetListLobby(IPhotonMenuConnectArgs connectionArgs, GameRoomType Roomtype, LevelType level)
		{
			HathoraClient hathoraClient = _manager.GetOrCreateClientInstance();
			return await hathoraClient.GetLobbyV3Asyncs(connectionArgs.PreferredRegion, Roomtype, level);
		}
	}
}
