using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
	public class LobbyTutorialGameplayMode : GameplayMode
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private int _extraLives = 0;

		[SerializeField]
		private GameObject tutorialObject;

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

			Transform spawnPoint = GetRandomSpawnPoint(100.0f);

			var spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
			var spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

			// _TutorialPlayer.Character.CharacterController.SetDynamicVelocity(Vector3.zero);
			// _TutorialPlayer.Character.CharacterController.SetExternalVelocity(Vector3.zero);
			// _TutorialPlayer.Character.CharacterController.SetPosition(_Checkpoint.transform.position);
			// _TutorialPlayer.Character.CharacterController.SetLookRotation(_Checkpoint.transform.rotation);

			//Instantiate(Resources.Load<GameObject>("Assets/TPSBR/Tutorial/Prefabs/LobbyTutorialManager.prefab"));
			Runner.Spawn(tutorialObject);
			//Instantiate(tutorialObject);

			Agent _TutorialPlayer = SpawnAgent(player.Object.InputAuthority, spawnPosition, spawnRotation);
			int _HVFirstGame = 0;

			if (!PlayerPrefs.HasKey("HV1")) PlayerPrefs.SetInt("HV1", 0);
			else _HVFirstGame = PlayerPrefs.GetInt("HV1");

			if (_TutorialPlayer)
			{
				MissionSystem.Instance.Initialize(new List<Mission>() {
					new Mission()
					{
						missionId = 1,
						assigned = true,
                        //complete = false,
                        missionDescription = "Complete the tutorial",
						missionTitle = "Bloomverse Lobby",
						objectives = _HVFirstGame.Equals(0) ?  new List<Objective>()
						{
							new Objective
							{
								objectiveId = 13,
								objectiveDescription = "Enter the lobby.",
								targetValue = 1,
								currentValue = 0,
								rank = 1
							},
							new Objective
							{
								objectiveId = 14,
								objectiveDescription = "Find and enter the Happy Vibes store.",
								targetValue = 1,
								currentValue = 0,
								rank = 2
							},
							new Objective
							{
								objectiveId = 15,
								objectiveDescription = "Interact with the central console.",
								targetValue = 1,
								currentValue = 0,
								rank = 3
							},
							new Objective
							{
								objectiveId = 16,
								objectiveDescription = "Play the Happy Vibes game.",
								targetValue = 1,
								currentValue = 0,
								rank = 4
							}
                        } :
						new List<Objective>()
						{
							new Objective
							{
                                objectiveId = 17,
                                objectiveDescription = "Confirm your email.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 1
                            }
						}
                    }});
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



		protected void FinishGameplay()
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
