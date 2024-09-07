using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
	public class GummyGambolGameplayMode : GameplayMode
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private int _extraLives = 0;

		[Networked]
		private byte _state { get; set; }

		protected override void TrySpawnAgent(Player player)
		{
			Debug.Log("TRY SPAWN AGENT");
			var statistics = player.Statistics;
			var playerRef = player.Object.InputAuthority;
			//var agent = Runner.Spawn(playerRef, inputAuthority: playerRef);
			//Runner.SetPlayerAlwaysInterested(playerRef, agent.Object, true);
			statistics.IsAlive = true;
			player.UpdateStatistics(statistics);

			// _TutorialPlayer.Character.CharacterController.SetDynamicVelocity(Vector3.zero);
			// _TutorialPlayer.Character.CharacterController.SetExternalVelocity(Vector3.zero);
			// _TutorialPlayer.Character.CharacterController.SetPosition(_Checkpoint.transform.position);
			// _TutorialPlayer.Character.CharacterController.SetLookRotation(_Checkpoint.transform.rotation);

			Agent _TutorialPlayer = SpawnAgent(player.Object.InputAuthority, Vector3.zero, Quaternion.identity);

			if (_TutorialPlayer)
			{
				_TutorialPlayer.AgentInput.SetActionStates(EGameplayInputActionStates.None);
				_TutorialPlayer.Jetpack.Deactivate();
				GummyGambolManager.Instance.Begin(_TutorialPlayer);
			}

			// Too late, player is automatically eliminated
			/*
			statistics.IsEliminated = true;
			statistics.IsAlive = false;

			player.UpdateStatistics(statistics);

			SetSpectatorTargetToBestPlayer(player);*/

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

		protected override void AgentDeath(ref PlayerStatistics victimStatistics, ref PlayerStatistics killerStatistics)
		{
			Debug.Log("Agent Death");


			base.AgentDeath(ref victimStatistics, ref killerStatistics);

			if (victimStatistics.ExtraLives > 0)
			{
				victimStatistics.ExtraLives -= 1;
			}
		}



		protected  void FinishGameplay()
		{


			Debug.Log("end boss fight");
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


	}
}
