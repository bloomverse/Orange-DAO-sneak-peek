using Fusion;
using System.Collections.Generic;
using UnityEngine;

namespace TPSBR
{
	using System.Collections.Generic;

	public class ActivityMode : GameplayMode
	{
		// PUBLIC MEMBERS

		public int   ScoreLimit;
		// GameplayMode INTERFACE

		protected override void AgentDeath(ref PlayerStatistics victimStatistics, ref PlayerStatistics killerStatistics)
		{
			base.AgentDeath(ref victimStatistics, ref killerStatistics);

			if (killerStatistics.IsValid == true && victimStatistics.PlayerRef != killerStatistics.PlayerRef)
			{
				if (killerStatistics.Score >= ScoreLimit)
				{
					FinishGameplay();
				}
			}
		}

		protected override void TrySpawnAgent(Player player)
        {
           
            var statistics = player.Statistics;
            var playerRef = player.Object.InputAuthority;
          
            statistics.IsAlive = true;
            player.UpdateStatistics(statistics);

            Transform spawnPoint = GetRandomSpawnPoint(100.0f);

            var spawnPosition = spawnPoint != null ? spawnPoint.position : Vector3.zero;
            var spawnRotation = spawnPoint != null ? spawnPoint.rotation : Quaternion.identity;

			Transform _Checkpoint = spawnPoint;

			 Agent _TutorialPlayer = SpawnAgent(player.Object.InputAuthority, _Checkpoint.transform.position, _Checkpoint.transform.rotation);
        
        }


		protected override void CheckWinCondition()
		{
			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;

				if (player.Statistics.Score >= ScoreLimit)
				{
					FinishGameplay();
					return;
				}
			}
		}

		
	}
}
