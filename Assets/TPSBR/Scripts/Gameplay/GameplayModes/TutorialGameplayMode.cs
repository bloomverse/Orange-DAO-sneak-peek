using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
    public class TutorialGameplayMode : GameplayMode
    {
        // PRIVATE MEMBERS

        [SerializeField]
        private int _extraLives = 1000;

        [Networked]
        private byte _state { get; set; }

        protected override void TrySpawnAgent(Player player)
        {
            Debug.Log("TRY SPAWN AGENT");

            PlayerPrefs.SetInt("HV1",0);

            var statistics = player.Statistics;
            var playerRef = player.Object.InputAuthority;
            //var agent = Runner.Spawn(playerRef, inputAuthority: playerRef);
            //Runner.SetPlayerAlwaysInterested(playerRef, agent.Object, true);
            statistics.IsAlive = true;
            player.UpdateStatistics(statistics);

            Transform spawnPoint = GetRandomSpawnPoint(100.0f);

            var spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            var spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

            Transform _Checkpoint = TutorialManager.Instance.GetCurrentCheckpoint();

            // _TutorialPlayer.Character.CharacterController.SetDynamicVelocity(Vector3.zero);
            // _TutorialPlayer.Character.CharacterController.SetExternalVelocity(Vector3.zero);
            // _TutorialPlayer.Character.CharacterController.SetPosition(_Checkpoint.transform.position);
            // _TutorialPlayer.Character.CharacterController.SetLookRotation(_Checkpoint.transform.rotation);

            Agent _TutorialPlayer = SpawnAgent(player.Object.InputAuthority, _Checkpoint.transform.position, _Checkpoint.transform.rotation);
            Debug.Log(_TutorialPlayer);
            if (_TutorialPlayer)
            {
                MissionSystem.Instance.Initialize(new List<Mission>() {
                    new Mission()
                    {
                        missionId = 1,
                        assigned = true,
                        //complete = false,
                        missionDescription = "Complete the tutorial",
                        missionTitle = "Welcome to Bloomverse",
                        objectives = new List<Objective>()
                        {
                            new Objective
                            {
                                objectiveId = 1,
                                objectiveDescription = "Move MOUSE to look around.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 1
                            },
                            new Objective
                            {
                                objectiveId = 2,
                                objectiveDescription = "Use WASD to move towards the ramp.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 2
                            },
                            new Objective
                            {
                                objectiveId = 3,
                                objectiveDescription = "Use the SPACEBAR to jump.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 3
                            },
                            new Objective
                            {
                                objectiveId = 4,
                                objectiveDescription = "Step on the elevator.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 4
                            },
                            new Objective
                            {
                                objectiveId = 5,
                                objectiveDescription = "Press LMB to shoot",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 5
                            },
                            new Objective
                            {
                                objectiveId = 777,
                                objectiveDescription = "Destroy Drones",
                                targetValue = 3,
                                currentValue = 0,
                                rank = 5
                            },
                            new Objective
                            {
                                objectiveId = 6,
                                objectiveDescription = "Press R to reload",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 6
                            },
                            new Objective
                            {
                                objectiveId = 777,
                                objectiveDescription = "Destroy Drones",
                                targetValue = 3,
                                currentValue = 0,
                                rank = 6
                            },
                            new Objective
                            {
                                objectiveId = 777,
                                objectiveDescription = "Destroy Drones",
                                targetValue = 3,
                                currentValue = 0,
                                rank = 7
                            },
                            new Objective
                            {
                                objectiveId = 8,
                                objectiveDescription = "Press SPACEBAR mid-jump to activate jetpack.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 8
                            },
                            new Objective
                            {
                                objectiveId = 9,
                                objectiveDescription = "Press H to use Spray on the back wall.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 9
                            },
                            new Objective
                            {
                                objectiveId = 10,
                                objectiveDescription = "Press LEFT SHIFT to boost.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 10
                            },
                            new Objective
                            {
                                objectiveId = 7,
                                objectiveDescription = "Hold RMB to aim down sights.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 11
                            },
                            new Objective
                            {
                                objectiveId = 777,
                                objectiveDescription = "Destroy drones in the ceiling corners.",
                                targetValue = 4,
                                currentValue = 0,
                                rank = 11
                            },
                            new Objective
                            {
                                objectiveId = 11,
                                objectiveDescription = "Press F to interact with items.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 12
                            },
                            new Objective
                            {
                                objectiveId = 12,
                                objectiveDescription = "Defeat the boss.",
                                targetValue = 1,
                                currentValue = 0,
                                rank = 13
                            },
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


    }
}
