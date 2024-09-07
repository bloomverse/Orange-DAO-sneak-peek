using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
	public class LobbyGameplayMode : GameplayMode
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private int _extraLives = 0;

		public bool     HasStarted         { get { return _state.IsBitSet(0); } set { _state = _state.SetBitNoRef(0, value); } }
		public float    WaitingCooldown    => State == EState.Active && HasStarted == false ? _waitingForPlayersCooldown.RemainingTime(Runner).Value : 0f;

		[Networked]
		private byte _state { get; set; }
		[Networked]
		private TickTimer _waitingForPlayersCooldown { get; set; }

		[SerializeField]
		private float _waitingForPlayersTime = 120f;

		//private EliminationComparer _playerComparer = new EliminationComparer();



		public void StartImmediately()
		{
			ArenaManager.instance.StartCountDown();
			RPC_StartAirdrop();

			//StartSpawn();
			/*if (Object.HasStateAuthority == true)
			{
				StartSpawn();
			}
			else if (ApplicationSettings.IsModerator == true)
			{
				RPC_StartAirdrop();
			}*/

		}

		private void StartSpawn()
		{
			HasStarted = true;

			/*if (TimeLimit > 0f)
			{
				_endTimer = TickTimer.CreateFromSeconds(Runner, TimeLimit);
			}*/

			

			

			//TrySpawnAgent(player.Object.InputAuthority);
			//_airplane.ActivateDropWindow();
			//_dropCooldown = TickTimer.CreateFromSeconds(Runner, _playerDropTime);
		}

	

		protected override void OnActivate()
		{
			_waitingForPlayersCooldown = TickTimer.CreateFromSeconds(Runner, _waitingForPlayersTime);
			HasStarted = false;
		}
		protected override void TrySpawnAgent(Player player)
		{
				var statistics = player.Statistics;
				var playerRef = player.Object.InputAuthority;
				//var agent = Runner.Spawn(playerRef, inputAuthority: playerRef);
				//Runner.SetPlayerAlwaysInterested(playerRef, agent.Object, true);
				statistics.IsAlive = true;
				player.UpdateStatistics(statistics);
			
				Transform spawnPoint = GetRandomSpawnPoint(100.0f);

			var spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
			var spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

				SpawnAgent(player.Object.InputAuthority, spawnPosition, spawnRotation);

			

				// Too late, player is automatically eliminated
				/*
				statistics.IsEliminated = true;
				statistics.IsAlive = false;

				player.UpdateStatistics(statistics);

				SetSpectatorTargetToBestPlayer(player);*/
			
		}

		public void TryAddWaitTime(float time)
		{
			//AddWaitTime(time);
			RPC_AddWaitTime(time);
			/*if (Object.HasStateAuthority == true)
			{
				AddWaitTime(time);
			}
			else if (ApplicationSettings.IsModerator == true)
			{
				RPC_AddWaitTime(time);
			}*/
		}
		private void AddWaitTime(float time)
		{
			if (_waitingForPlayersCooldown.ExpiredOrNotRunning(Runner) == true)
				return;

			float remaining = _waitingForPlayersCooldown.RemainingTime(Runner).Value;
			_waitingForPlayersCooldown = TickTimer.CreateFromSeconds(Runner, remaining + time);
		}

		// GameplayMode INTERFACE

		protected override void PreparePlayerStatistics(ref PlayerStatistics playerStatistics)
		{
			base.PreparePlayerStatistics(ref playerStatistics);

			// Do not add extra lives after reconnect
			if (playerStatistics.Deaths == 0 && playerStatistics.IsEliminated == false && playerStatistics.ExtraLives == 0)
			{
				playerStatistics.ExtraLives = (short)GetExtraLivesForNewPlayer();
			}
		}

		public override void FixedUpdateNetwork()
		{
			base.FixedUpdateNetwork();

			if (State != EState.Active)
				return;

			if (Object.HasStateAuthority == false)
				return;

			if (HasStarted == false && _waitingForPlayersCooldown.ExpiredOrNotRunning(Runner) == true)
			{
				StartSpawn();
			}

			/*if (HasStarted == true && AirplaneActive == true && _dropCooldown.ExpiredOrNotRunning(Runner) == true)
			{
				StopAirdrop();
			}*/

			/*if (HasStarted == true && AirplaneActive == false && _airplaneAgents.Count > 0 && _forcedJumpCooldown.ExpiredOrNotRunning(Runner) == true)
			{
				foreach (var agentPair in _airplaneAgents)
				{
					RequestAirplaneJump(agentPair.Key, Vector3.down);
					break;
				}

				_forcedJumpCooldown = TickTimer.CreateFromSeconds(Runner, _forcedJumpDelay);

				if (_airplaneAgents.Count == 0)
				{
					_airplane.DeactivateDropWindow();
				}
			}
			// MATALO
			if (_airplane != null && _airplane.IsFinished == true)
			{
				Runner.Despawn(_airplane.Object);
				_airplane = null;
			}*/
		}

		protected override float GetRespawnTime(PlayerStatistics playerStatistics)
		{
			if (playerStatistics.ExtraLives > 0)
				return base.GetRespawnTime(playerStatistics);

			return -1f;
		}

		protected override void AgentDeath(ref PlayerStatistics victimStatistics, ref PlayerStatistics killerStatistics)
		{
			base.AgentDeath(ref victimStatistics, ref killerStatistics);

			if (victimStatistics.ExtraLives > 0)
			{
				victimStatistics.ExtraLives -= 1;
			}
		}

		protected override void SortPlayers(List<PlayerStatistics> allStatistics)
		{
			//allStatistics.Sort(_playerComparer);
		}

		protected override void CheckWinCondition()
		{
			//var alivePlayers    = 0;
			var lastAlivePlayer = PlayerRef.None;


			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;

				var statistics = player.Statistics;

				/*if(statistics.Kills>=20){
					lastAlivePlayer  = player.Object.InputAuthority;
					FinishGameplay();
					//return;
				}

				if (statistics.ExtraLives > 0 || statistics.IsAlive == true || statistics.RespawnTimer.IsRunning == true)
				{
					if (alivePlayers > 0)
						return;

					alivePlayers    += 1;
					lastAlivePlayer  = player.Object.InputAuthority;
				}*/
			}



			/*if (alivePlayers == 1)
			{
				FinishGameplay();
				Log.Info($"Player {lastAlivePlayer} won the match!");
			}
			else if (alivePlayers == 0)
			{
				Log.Error("No player alive, this should not happen");
				FinishGameplay();
			}*/
		}


		// PRIVATE METHODS
		[Rpc(RpcSources.Proxies, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
		private void RPC_AddWaitTime(float time)
		{
			AddWaitTime(time);
		}

		[Rpc(RpcSources.Proxies, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
		private void RPC_StartAirdrop()
		{
			StartSpawn();
		}

		private int GetExtraLivesForNewPlayer()
		{
			int minExtraLives = int.MaxValue;
			int maxExtraLives = int.MinValue;

			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;

				var playerStatistics = player.Statistics;

				if (playerStatistics.Deaths == 0 && playerStatistics.ExtraLives == 0 && playerStatistics.IsEliminated == false)
					continue; // Player not yet initialized properly

				minExtraLives = Mathf.Min(minExtraLives, playerStatistics.ExtraLives);
				maxExtraLives = Mathf.Max(maxExtraLives, playerStatistics.ExtraLives);
			}

			int extraLives = _extraLives;

			if (minExtraLives < int.MaxValue && maxExtraLives > int.MinValue)
			{
				extraLives = Mathf.Clamp(Mathf.RoundToInt((minExtraLives + maxExtraLives) * 0.5f), 0, _extraLives);
			}

			return extraLives;
		}

		// HELPERS

		private class LobbyComparer : IComparer<PlayerStatistics>
		{
			public int Compare(PlayerStatistics x, PlayerStatistics y)
			{
				var result = y.Kills.CompareTo(x.Kills);
				if (result != 0)
					return result;


				 result = x.IsEliminated.CompareTo(y.IsEliminated);
				if (result != 0)
					return result;

				result = y.ExtraLives.CompareTo(x.ExtraLives);
				if (result != 0)
					return result;

				

				return x.Deaths.CompareTo(y.Deaths);
			}
		}
	}
}
