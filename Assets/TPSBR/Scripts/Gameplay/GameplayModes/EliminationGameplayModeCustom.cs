using Fusion;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Collections;


namespace TPSBR
{
	public class EliminationGameplayModeCustom : GameplayMode
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private int _extraLives = 4;

//		public bool HasStarted { get { return _state.IsBitSet(0); } set { _state = _state.SetBitNoRef(0, value); } }
//		public float WaitingCooldown => State == EState.Active && HasStarted == false ? _waitingForPlayersCooldown.RemainingTime(Runner).Value : 0f;
		public float BloomieTime => _bloomieTimer.IsRunning == true ? _bloomieTimer.RemainingTime(Runner).Value : 0f;
		public float BloomieCooldownTime => _bloomieCooldownTimer.IsRunning == true ? _bloomieCooldownTimer.RemainingTime(Runner).Value : 0f;
		public string playerOns;
		public Player playerInstance { get { return _playerInstance; } set { _playerInstance = value; } }
		public bool Midmode = true;
		//public Dictionary<Player,float>     ma_public    =>  ma_status;


		[Networked]
		private byte _state { get; set; }

		[Networked]
		private TickTimer _waitingForPlayersCooldown { get; set; }
		[Networked]
		public TickTimer _bloomieTimer { get; set; }
		[Networked]
		public TickTimer _bloomieCooldownTimer { get; set; }
		[Networked]
		public string middleStatus { get; set; }
		[Networked]
		public string _playerOn { get; set; }
		[Networked]
		public Player _playerInstance { get; set; }

	
		//[Networked]
		//public Dictionary<Player,float> ma_status { get; set; }


		[SerializeField]
		private float _waitingForPlayersTime = 120f;





		//public Dictionary<Player,float> ma_status = new Dictionary<Player, float>();

		// NUCLEOS ACTIVITY


		[SerializeField]
		private float ma_activationTime = 10f;
		//[SerializeField]
		//private float ma_respawnTime = 10f;
		[SerializeField]
		private int ma_bloomieBonus = 12;


		[SerializeField]
		private GameObject centerPrefab;


		private GameObject startEffect;
		private GameObject endEffect;
		private GameObject Core;
		private ParticleSystem CoreParticle;

		private EliminationComparer _playerComparer = new EliminationComparer();

	

		private float cooldownTime = 20f;


		public void StartBloomieActivity(Player player)
		{

			//RPC_AddBloomieActivity(player);


			Debug.Log(player + " player on start bloomie activity");
			if (player != null)
			{
				Debug.Log("Activando Bloomie Activity" + player.UnityID);
				//playerInstance = player;
				//_playerOn = player.UnityID;

				//playerOns = playerId;

				//Debug.Log(player + "----PLAYER ON ACTIVATED " );
			}
			else
			{
				//_playerOn = "";
				//RPC_AddBloomieActivity(player);
			}

			//RPC_AddBloomieActivity(player);
			//playerOns = playerId;

			//Debug.Log(player + "----PLAYER ON ACTIVATED " );
		}

		public event Action Cooling;
		public event Action CoreReady;
		public event Action Harvesting;
		public event Action HarvestFinish;


		private void controlBloomieCounter()
		{
			// HARVESTING

			if (_playerOn != "" && middleStatus != "ongoing" && middleStatus == "ready")
			{
				//Debug.Log("PLAYER ON-------------LALA " + playerOns + " middle " + middleStatus);
				//_playerOn = playerOns;
				_playerInstance = playerInstance;
				_bloomieTimer = TickTimer.CreateFromSeconds(Runner, 10);
				middleStatus = "ongoing";
				startEffect.SetActive(true);
				endEffect.SetActive(false);
				Harvesting?.Invoke();


			}

			// ABANDON
			if (_bloomieTimer.IsRunning && _playerOn == "")
			{
				_bloomieTimer = TickTimer.None;//  TickTimer.CreateFromSeconds(Runner,0);

				_bloomieCooldownTimer = TickTimer.CreateFromSeconds(Runner, cooldownTime);
				startEffect.SetActive(false);
				Core.SetActive(false);
				CoreParticle.Play();
				//endEffect.SetActive(false);
				middleStatus = "cooling";
				Cooling?.Invoke();
				//Debug.Log("LAST BLOOMIE TICK " + lastBloomieTick);
			}

			// ADD BLOOMIES TO PLAYER
			if (_bloomieTimer.Expired(Runner) == true && _playerOn != "" && middleStatus == "ongoing")
			{
				startEffect.SetActive(false);
				endEffect.SetActive(true);
				Core.SetActive(false);
				Debug.Log("Ended");
				_playerOn = "";
				HarvestFinish?.Invoke();

				_bloomieCooldownTimer = TickTimer.CreateFromSeconds(Runner, cooldownTime);
				CoreParticle.Play();
				middleStatus = "cooling";

				foreach (var player in Context.NetworkGame.ActivePlayers)
				{

					if (player != null && player.DBID != null)
					{
						Debug.Log(player.DBID + " USER DABTABASE ID " + playerInstance.DBID + " ");
						if (player.DBID == playerInstance.DBID)
						{
							Debug.Log("entregando bloomies" + ma_bloomieBonus + " " + player.Statistics.Deaths);
							var statistics = player.Statistics;
							statistics.BloomiesEarned += ma_bloomieBonus;
							player.UpdateStatistics(statistics);

						}
					}

				}


			}

			// Cooling Ready
			if (_bloomieCooldownTimer.Expired(Runner) && middleStatus == "cooling")
			{
				Core.SetActive(true);

				middleStatus = "ready";
				CoreParticle.Stop();

				CoreReady?.Invoke();

			}
			else
			{
				//Core.SetActive(false);
			}

		}





		protected override void OnActivate()
		{

			if (_middleEvents)
			{
				startEffect = GameObject.Find("startBloomie").gameObject;
				endEffect = GameObject.Find("endBloomie").gameObject;
				Core = GameObject.Find("coreBall").gameObject;
				CoreParticle = GameObject.Find("coreParticle").GetComponent<ParticleSystem>();
				startEffect.SetActive(false);
				endEffect.SetActive(false);
			}

			
		}

		protected override void TrySpawnAgent(Player player)
		{
		//	Debug.Log("TRY SPAWN AGENT");
			var statistics = player.Statistics;
			//var playerRef = player.Object.InputAuthority;
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

		private bool finishStarted;
		public bool _started;
		private bool startAnimState;

		public override void FixedUpdateNetwork()
		{
			base.FixedUpdateNetwork();
			if (State != EState.Active)
				return;

			if (Object.HasStateAuthority == false)
				return;

			if (_middleEvents)
			{
				//controlBloomieCounter();
			}


			if(!_started ){
				//StartSpawn();
				
			}

			if(_endTimer.RemainingTime(Runner) < TimeLimit-10 && !startAnimState){
				startAnimState = true;
				RPC_StartAnimation();
			}

		

			if (_endTimer.RemainingTime(Runner) < 60 && finishStarted == false)
			{
				RPC_FinishAnimation();
				finishStarted = true;
			}

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
			allStatistics.Sort(_playerComparer);
		}

		protected override void CheckWinCondition()
		{
			var alivePlayers = 0;
			var lastAlivePlayer = PlayerRef.None;


			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;

				var statistics = player.Statistics;

				if (statistics.Kills >= 20)
				{
					lastAlivePlayer = player.Object.InputAuthority;
					FinishGameplay();
					//return;
				}

				if (statistics.ExtraLives > 0 || statistics.IsAlive == true || statistics.RespawnTimer.IsRunning == true)
				{
					if (alivePlayers > 0)
						return;

					alivePlayers += 1;
					lastAlivePlayer = player.Object.InputAuthority;
				}
			}



			if (alivePlayers == 1)
			{
				//FinishGameplay();
				Log.Info($"Player {lastAlivePlayer} won the match!");
			}
			else if (alivePlayers == 0)
			{
				Log.Error("No player alive, this should not happen");
				//FinishGameplay();
			}
		}


		// PRIVATE METHODS
		[Rpc(RpcSources.Proxies, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
		private void RPC_AddWaitTime(float time)
		{
			AddWaitTime(time);
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
		private void RPC_StartAnimation()
		{
			ArenaManager.instance.StartCountDown();
		}

		[Rpc(RpcSources.StateAuthority, RpcTargets.All, Channel = RpcChannel.Reliable)]
		private void RPC_FinishAnimation()
		{
			ArenaManager.instance.finalSequence();
		}


		// PRIVATE METHODS
		[Rpc(RpcSources.Proxies, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
		public void RPC_AddBloomieActivity(Player player)
		{
			if (player != null)
			{
				_playerOn = player.UnityID;
				playerInstance = player;
			}
			else
			{
				_playerOn = "";
				playerInstance = null;
			}
		}

		[Rpc(RpcSources.Proxies, RpcTargets.StateAuthority, Channel = RpcChannel.Reliable)]
		private void RPC_StartAirdrop()
		{
			//StartSpawn();
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

		private class EliminationComparer : IComparer<PlayerStatistics>
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
