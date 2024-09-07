using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Collections;
using System;
using Coffee.UIExtensions;
using System.Reflection;
using UnityEngine.Events;
using DG.Tweening;

namespace TPSBR.UI


{
	public class UIGameOverView : UIView
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private UIBehaviour _winnerGroup;

		[SerializeField]
		private TextMeshProUGUI _username;


		[SerializeField]
		private TextMeshProUGUI _winner;
		[SerializeField]
		private UIButton _restartButton;
		[SerializeField]
		private AudioSetup _openSound;
		[SerializeField]
		private AudioEffect _audioEffect;
		[SerializeField]
		private AudioEffect _audioMusic;

		[SerializeField]
		private AudioClip _audioEf2;

		[SerializeField]
		private AudioSource _audioSource;

		[SerializeField]
		private GameObject _numbersContainer;
		[SerializeField]
		private GameObject _counterPrefab;


		[SerializeField]
		private TextMeshProUGUI nextBonusText;

		public UIlevelInfoAnim levelInfoAnim;

		
		/*[SerializeField]
		private UIParticleAttractor attractorBloomies;
		[SerializeField]
		private UIParticleAttractor attractorXP;*/

		[SerializeField]
		private GameObject attractorBloomies;
		[SerializeField]
		private GameObject attractorXP;

		// UIView INTERFACE

		//
		[SerializeField]
		private GameObject _leftHandle;
		[SerializeField]
		private GameObject _rightHandle;
		[SerializeField]
		private GameObject _mask;
		[SerializeField]
		private CanvasGroup _background;
		[SerializeField]
		private CanvasGroup _matchWord;
		[SerializeField]
		private CanvasGroup _resultsWord;


		[SerializeField]
		private UIlevelInfoAnim _levelAnim;

		[SerializeField]
		private ParticleSystem _matchParticles;

		protected override void OnInitialize()
		{
			base.OnInitialize();
			
			
		}

		private void titleAnim(int delay){

			var bgMusic = GameObject.Find("SceneMusic");
			if(bgMusic!=null){
				bgMusic.GetComponent<AudioSource>().DOFade(0,1f);
			}
			

			_leftHandle.transform.DOLocalMoveX(208, 0.5f).SetEase(Ease.OutBack).SetDelay(delay);
			_rightHandle.transform.DOLocalMoveX(-207, 0.5f).SetEase(Ease.OutBack).SetDelay(delay);
			_background.DOFade(1, 1).SetEase(Ease.OutBack).SetDelay(delay+ .3f);
			//_background.transform.DOLocalMoveX(0, 0.5f).SetEase(Ease.OutBack);
			_matchWord.DOFade(1,1).SetEase(Ease.OutBack).SetDelay(delay+.5f);
			_matchWord.transform.DOLocalMoveX(-87,0.5f).SetDelay(delay+.5f).SetEase(Ease.OutBack);
			_matchWord.transform.DOScale(1,0.5f).SetDelay(delay+.5f).SetEase(Ease.OutBounce).OnComplete(()=>{_audioSource.PlayOneShot(_audioEf2);});
			_resultsWord.DOFade(1, 1.3f).SetEase(Ease.OutBack).SetDelay(delay+1f);
			_resultsWord.transform.DOLocalMoveX(67,0.5f).SetEase(Ease.OutBack).SetDelay(delay+1f);
			_resultsWord.transform.DOScale(1,0.5f).SetDelay(delay+1f).SetEase(Ease.OutBounce);

			_audioEffect.Play();
			//_matchParticles.Emit(10);
			_audioMusic.Play();

		}

		protected override void OnOpen()
		{
			base.OnOpen();
			_restartButton.onClick.AddListener(OnRestartButton);
			
			titleAnim(1);

			
			var chatObj = GameObject.Find("ChatPrefab");
			if(chatObj!=null){
				chatObj.SetActive(false);
			}

			var userVal = "Player Bonus";
			
			if(Context.PlayerData.Nickname!=""){
				userVal = Context.PlayerData.Nickname;
			}
			//_username.text = userVal;
			
			//an1.animDone += nextAnim;
			//
			//an1.startAnim(1);
			//an2.startAnim(4);
			//an3.startAnim(7);

			//var canvas = GameObject.Find("FX_canvas").GetComponent<Canvas>();
			//var uicam = GameObject.Find("ui_cameraFX").GetComponent<Camera>();
			//if(uicam != null){
			//	canvas.worldCamera = uicam;
				//uicam.transform.position = Camera.main.transform.position;	
		//	}
		
		
			Debug.Log("GAME OVER STATAS--------------");


			var winnerStatistics = GetWinner();
			Player winner = null;

			   if(Context!=null){
            	Context.Input.RequestCursorVisibility(true, ECursorStateSource.UI);
        	}

			if (winnerStatistics.IsValid == true)
			{
				winner = Context.NetworkGame.GetPlayer(winnerStatistics.PlayerRef);
			}

			if (winner != null)
			{
//				_winner.text = $"{winner.Nickname} wins";
				//_winnerGroup.SetActive(true);
			}
			else
			{
				_winnerGroup.SetActive(false);
			}

			// SET VALUES 

			var playerS = GetPlayerStatistics();
			var playerData = GetPlayer();

			// Match Exp Value
			var killsXP = playerS.Kills * 100;
			var matchXPBase = 1000;
			var placeXPc = 0;
			if(playerS.Position==1){
				placeXPc = 200;
			}
			if(playerS.Position==2){
				placeXPc = 100;
			}
			if(playerS.Position==3){
				placeXPc = 50;
			}
			// Bloomies Value
			var bonusBloomies = 0;
			var basebloomies = 10;
			if(playerS.Position==1){
				bonusBloomies = 10;
			}
			if(playerS.Position==2){
				bonusBloomies = 5;
			}
			if(playerS.Position==3){
				bonusBloomies = 2;
			}

			var hours = (DateTime.UtcNow.Ticks - playerData.LastPlayed);
			var ts = TimeSpan.FromTicks(hours).TotalHours;
			var lastBloomieBonus = playerData.LastBloomieBonus;
							if(ts<24f){
						
								lastBloomieBonus += 5;
							}else{
								lastBloomieBonus = 5;
							}

							if(lastBloomieBonus>15){
								lastBloomieBonus =15;
							}

		//	nextBonusText.SetText("+" + lastBloomieBonus + " bloomies");
	
			// Base Bloomies & Position Bloomies & 
			var bloomiesMatch = basebloomies +  bonusBloomies  ;
			// Match Mechanics Extra & last Bonus
			var bloomiesExtra = playerData.Statistics.ExtraBloomies + playerData.LastBloomieBonus ;
			// Match Exp
			var XPMatch = matchXPBase;
			//  Match Kills & Position
			var XPKillPlace = killsXP +  + placeXPc;

			var XPtotal = XPMatch + XPKillPlace;

			Debug.Log(bloomiesMatch + "bloomiesMatch");
			Debug.Log(bloomiesExtra	+ "bloomiesExtra");
			Debug.Log(XPMatch+ 		"XPMatch") ;
			Debug.Log(XPKillPlace+ 		"XPKillPlace") ;

			List<finalAnimation> finalAnimations = new List<finalAnimation>();

			

			var anim1 = new finalAnimation();
			anim1.description = "Match Bloomies";
			anim1.amount = bloomiesMatch;
			anim1.attractor = attractorBloomies;
			anim1.particleIndex = 0;
			anim1.particleName = "bloomieCoinBadge";
			finalAnimations.Add(anim1);	

			var anim2 = new finalAnimation();
			anim2.description = "Bonus Bloomies";
			anim2.amount = bloomiesExtra;
			anim2.attractor = attractorBloomies;
			anim2.particleIndex = 0;
			anim2.particleName = "bloomieCoinBadge";
			finalAnimations.Add(anim2);	

			var anim3 = new finalAnimation();
			anim3.description = "Match Experience";
			anim3.amount = XPMatch;
			anim3.attractor = attractorXP;
			anim3.particleIndex = 1;
			anim3.particleName = "XPCoinFinal";
			finalAnimations.Add(anim3);	

			var anim4 = new finalAnimation();
			anim4.description = "Bonus Experience";
			anim4.amount = XPKillPlace;
			anim4.attractor = attractorXP;
			anim4.particleIndex = 1;
			anim4.particleName = "XPCoinFinal";
			finalAnimations.Add(anim4);	

			StartCoroutine(animProc(finalAnimations));
			PlaySound(_openSound);


			_levelAnim.setLevelC(XPtotal);  // XPtotal

			Global.Networking.StopGameOnDisconnect();
		}


		private float ParticleTotalTime = 1f;
		IEnumerator animProc(List<finalAnimation> finalAnimations){
			

			foreach(finalAnimation x in finalAnimations){
				
				CreateAnim(x.description,x.amount,x.attractor,x.particleIndex,x.particleName);
				yield return new WaitForSeconds(ParticleTotalTime);
			}	
			
		}

		void CreateAnim(string title,int amount,GameObject attractor,int particleIndex,string listenerName){

			var InstancedNumbers = Instantiate(_counterPrefab, _numbersContainer.transform);

			var att1 = attractor.AddComponent<UIParticleAttractor>();
			att1.enabled = false;
			att1.maxSpeed = 10;
			if(particleIndex==0){
				att1.movement = UIParticleAttractor.Movement.Sphere;
			}else{
				att1.movement = UIParticleAttractor.Movement.Sphere;
			}
			
			Type type = att1.GetType(); // Or typeof(UIParticleAttractor)
			FieldInfo field = type.GetField("m_ParticleSystem", BindingFlags.NonPublic | BindingFlags.Instance);
			ParticleSystem coinSystem = InstancedNumbers.GetComponent<CounterAnimEnd>()._particles[particleIndex];
			InstancedNumbers.GetComponent<CounterAnimEnd>().ParticleIndex = particleIndex;
			ParticleSystem.Burst burst = coinSystem.emission.GetBurst(0);
			burst.count = 1;
			var amountTemp = (listenerName=="bloomieCoinBadge") ? amount : amount/10;
			burst.cycleCount = amountTemp ;
			burst.repeatInterval = (ParticleTotalTime / (float)amountTemp) ;
			coinSystem.emission.SetBurst(0, burst);
			field.SetValue(att1, coinSystem );
			
			
			//add Event 
			FieldInfo fieldEvent = type.GetField("m_OnAttracted", BindingFlags.NonPublic | BindingFlags.Instance);

			if(listenerName=="bloomieCoinBadge"){
				var coinparticles = GameObject.Find("bloomieCoinBadge").GetComponentInChildren<bloomieCoinFinal>();
				UnityEvent unityEvent = new UnityEvent();
				unityEvent.AddListener(coinparticles.shake);
				fieldEvent.SetValue(att1, unityEvent);
			}
			if(listenerName=="XPCoinFinal"){
				var coinparticles = GameObject.Find("XPCoinFinal").GetComponentInChildren<XPCoinFinal>();
				UnityEvent unityEvent = new UnityEvent();
				unityEvent.AddListener(coinparticles.shake);
				fieldEvent.SetValue(att1, unityEvent);
			}	
			
				

			
			att1.enabled = true;
			InstancedNumbers.GetComponent<CounterAnimEnd>().startAnim(0,title,amount);
		}


		private void nextAnim(){
			Debug.Log("Termino anim");
		}

		protected override void OnDeinitialize()
		{
			_restartButton.onClick.RemoveListener(OnRestartButton);

			base.OnDeinitialize();
		}

		// PRIVATE MEMBERS

		private PlayerStatistics GetPlayerStatistics(){
			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;

				if(player.Player == Context.LocalPlayerRef){
					var statistics = player.Statistics;
					return statistics;
				}
			}

			return default;
		}

		private Player GetPlayer(){
			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;
			
				if(player.Player == Context.LocalPlayerRef){
					return player;
				}
			}

			return default;
		}

		private PlayerStatistics GetWinner()
		{
			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;



				var statistics = player.Statistics;
				if (statistics.Position == 1)
				{
					return statistics;
				}
			}

			return default;
		}

		private void OnRestartButton()
		{
			Global.Networking.StopGame();
		}
	}
}


/*var children1 = InstancedNumbers.GetComponentInChildren<Transform>();
			foreach (Transform child in children1)
			{
				if(child.name == "CoinParticles"){
					 ParticleSystem coinparticles = child.GetComponentInChildren<ParticleSystem>();
					 field.SetValue(att1, coinparticles);
				}
			}*/