using UnityEngine;
using Fusion;
using System.Collections.Generic;

namespace RGSK
{
    using RGSK.Extensions;
    using RGSK.Fusion;
	public struct RiderData : INetworkStruct
	{
		[Networked, Capacity(24)]
		public string Nickname { get => default; set { } }
		public PlayerRef PlayerRef;
		public bool IsConnected;
	}

	public enum ERacingGameState
	{
		Skirmish = 0,
		Running = 1,
		Finished = 2,
	}
	public class RacingGameplay : NetworkBehaviour
	{
		[SerializeField] private Rider RiderPrefab;
		[SerializeField] private TimerFusion TimerPrefab;
		public float GameDuration = 2000f;

		[Networked]
		[Capacity(4)]
		[HideInInspector]
		public NetworkDictionary<PlayerRef, RiderData> RiderDatas { get; }
		[Networked]
		[HideInInspector]
		public ERacingGameState State { get; set; }

		private List<Transform> _recentSpawnPoints = new(4);

		private bool _isTimerSpawned = false;

		public override void Spawned()
		{
			if (Runner.Mode == SimulationModes.Server)
			{
				Application.targetFrameRate = TickRate.Resolve(Runner.Config.Simulation.TickRateSelection).Server;
			}

			if (Runner.GameMode == GameMode.Shared)
			{
				throw new System.NotSupportedException("This sample doesn't support Shared Mode, please start the game as Server, Host or Client.");
			}
		}

		public override void FixedUpdateNetwork()
		{
			if (HasStateAuthority == false)
				return;

			if (_isTimerSpawned == false)
			{
				Runner.Spawn(TimerPrefab);
				_isTimerSpawned = true;
			}

			RiderManager.UpdatePlayerConnections(Runner, SpawnPlayer, DespawnPlayer);
		}

		private void SpawnPlayer(PlayerRef playerRef)
		{
			if (RiderDatas.TryGet(playerRef, out var riderData) == false)
			{
				riderData = new RiderData();
				riderData.PlayerRef = playerRef;
				riderData.Nickname = playerRef.ToString();
				riderData.IsConnected = false;
			}

			if (riderData.IsConnected == true)
				return;

			Debug.LogWarning($"{playerRef} connected.");

			riderData.IsConnected = true;

			RiderDatas.Set(playerRef, riderData);

			var spawnPoint = GetSpawnPoint();
			RiderPrefab.gameObject.GetOrAddComponent<CompetitorFusion>();
			var rider = Runner.Spawn(RiderPrefab, spawnPoint.position, spawnPoint.rotation, playerRef);

			Runner.SetPlayerObject(playerRef, rider.Object);
		}

		private void DespawnPlayer(PlayerRef playerRef, Rider rider)
		{
			if (RiderDatas.TryGet(playerRef, out var riderData) == true)
			{
				if (riderData.IsConnected == true)
				{
					Debug.LogWarning($"{playerRef} disconnected.");
				}

				riderData.IsConnected = false;
				RiderDatas.Set(playerRef, riderData);
			}

			Runner.Despawn(rider.Object);
		}

		private Transform GetSpawnPoint()
		{
			return RaceManager.Instance.spawner.GetSpawnPoint();
		}
	}
}
