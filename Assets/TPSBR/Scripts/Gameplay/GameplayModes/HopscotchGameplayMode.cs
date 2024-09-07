using System;
using Fusion;
using UnityEngine;
using System.Collections.Generic;

namespace TPSBR
{
	public class HopscotchGameplayMode : GameplayMode
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private int _extraLives = 0;

		private HopscotchComparer _playerComparer = new HopscotchComparer();

		[Networked]
		private byte _state { get; set; }
		
		protected override void TrySpawnAgent(Player player)
		{
			var statistics = player.Statistics;
			var playerRef = player.Object.InputAuthority;

			statistics.IsAlive = true;
			player.UpdateStatistics(statistics);

			Agent _HopscotchPlayer = SpawnAgent(player.Object.InputAuthority, Vector3.forward * -7.5f, Quaternion.identity);

			if (_HopscotchPlayer)
			{
                _HopscotchPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.Move ^ EGameplayInputActionStates.Jump ^ EGameplayInputActionStates.Look);
                _HopscotchPlayer.Jetpack.Deactivate();
				_HopscotchPlayer.Weapons.SwitchWeapon(0);
				_HopscotchPlayer.gameObject.tag = "Player";

				HopscotchManager.Instance.Begin(Runner, _HopscotchPlayer);
			}
		}

		public void restartGame(){
			
		}

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


		}

		protected override float GetRespawnTime(PlayerStatistics playerStatistics)
		{
			if (playerStatistics.ExtraLives > 0)
				return base.GetRespawnTime(playerStatistics);

			return -1f;
		}

		

		protected override void CheckWinCondition()
		{
			var alivePlayers    = 0;
			var lastAlivePlayer = PlayerRef.None;
			Debug.Log("hpoggameplay checking win");


			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;



				var statistics = player.Statistics;
				
				if (statistics.ExtraLives > 0 || statistics.IsAlive == true || statistics.RespawnTimer.IsRunning == true)
				{
					if (alivePlayers > 0)
						return;

					alivePlayers    += 1;
					lastAlivePlayer  = player.Object.InputAuthority;
				}
			}
			if (alivePlayers == 0)
			{
				FinishGameplay();
				Log.Info($"Player {lastAlivePlayer} won the match!");
			}
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

		private class HopscotchComparer : IComparer<PlayerStatistics>
		{
			public int Compare(PlayerStatistics x, PlayerStatistics y)
			{
				var result = x.IsEliminated.CompareTo(y.IsEliminated);
				if (result != 0)
					return result;

				result = y.ExtraLives.CompareTo(x.ExtraLives);
				if (result != 0)
					return result;

				result = y.Kills.CompareTo(x.Kills);
				if (result != 0)
					return result;

				return x.Deaths.CompareTo(y.Deaths);
			}
		}


	}
}
