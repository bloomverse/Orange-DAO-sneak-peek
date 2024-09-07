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
	public class UIHopscotchGameOverView : UIView
	{
		// PRIVATE MEMBERS
		[SerializeField]
		private UIButton _restartButton;
		[SerializeField]
		private UIButton _backLobby;

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
		[SerializeField]
		private TMP_Text m_HopscotchResults;


		[SerializeField]
		private TMP_Text m_coinRes;
		[SerializeField]
		private TMP_Text m_timeRes;
		[SerializeField]
		private TMP_Text m_finalScore;

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
			//_matchWord.DOFade(1,1).SetEase(Ease.OutBack).SetDelay(delay+.5f);
			//_matchWord.transform.DOLocalMoveX(-87,0.5f).SetDelay(delay+.5f).SetEase(Ease.OutBack);
			//_matchWord.transform.DOScale(1,0.5f).SetDelay(delay+.5f).SetEase(Ease.OutBounce).OnComplete(()=>{_audioSource.PlayOneShot(_audioEf2);});
			//_resultsWord.DOFade(1, 1.3f).SetEase(Ease.OutBack).SetDelay(delay+1f);
			//_resultsWord.transform.DOLocalMoveX(67,0.5f).SetEase(Ease.OutBack).SetDelay(delay+1f);
			//_resultsWord.transform.DOScale(1,0.5f).SetDelay(delay+1f).SetEase(Ease.OutBounce);

			

			_audioEffect.Play();
			//_matchParticles.Emit(10);
			_audioMusic.Play();

		}

		protected override void OnOpen()
		{
			base.OnOpen();
			_restartButton.onClick.AddListener(OnRestartButton);
			_backLobby.onClick.AddListener(OnLobbyButton);
			
			titleAnim(1);

			var userVal = "Player Bonus";
			
			if(Context.PlayerData.Nickname!=""){
				userVal = Context.PlayerData.Nickname;
			}
		
			Debug.Log("GAME OVER STATS--------------");

			if(Context!=null){
            	Context.Input.RequestCursorVisibility(true, ECursorStateSource.UI);
        	}

			// SET VALUES 

			HopscotchResults _HopscotchResults = HopscotchManager.Instance.GetGameResults();

			//m_HopscotchResults.text = "Coins Collected: " + _HopscotchResults.m_CoinsCollected;

			m_coinRes.text = _HopscotchResults.m_CoinsCollected.ToString() + "  x10";
			m_timeRes.text = _HopscotchResults.m_finalTime.ToString();
			m_finalScore.text = _HopscotchResults.m_Score.ToString();

           List<finalAnimation> finalAnimations = new List<finalAnimation>();
			var anim1 = new finalAnimation();
			anim1.description = "Game Bloomies";
			anim1.amount = 30;
			anim1.attractor = attractorBloomies;
			anim1.particleIndex = 0;
			anim1.particleName = "bloomieCoinBadge";
			finalAnimations.Add(anim1);	

			var anim2 = new finalAnimation();
			anim2.description = "Bonus Bloomies";
			anim2.amount = 20;
			anim2.attractor = attractorBloomies;
			anim2.particleIndex = 0;
			anim2.particleName = "bloomieCoinBadge";
			finalAnimations.Add(anim2);	

			var anim3 = new finalAnimation();
			anim3.description = "Game Experience";
			anim3.amount = 100;
			anim3.attractor = attractorXP;
			anim3.particleIndex = 1;
			anim3.particleName = "XPCoinFinal";
			finalAnimations.Add(anim3);	

			var anim4 = new finalAnimation();
			anim4.description = "Bonus Experience";
			anim4.amount = 50;
			anim4.attractor = attractorXP;
			anim4.particleIndex = 1;
			anim4.particleName = "XPCoinFinal";
			finalAnimations.Add(anim4);	

			StartCoroutine(animProc(finalAnimations));
			PlaySound(_openSound);


			_levelAnim.setLevelC(5);  // XPtotal


			// _levelAnim.setLevelC(XPtotal);  // XPtotal

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
		public void OnRestartButton()
		{
			 HopscotchManager.Instance.restartGame();
		}
		public void OnLobbyButton()
		{
			Debug.Log("Click");
			Global.Networking.StopGame();

			bool _BackToTutorial = HopscotchManager.Instance.m_Tutorial;

            Global.Networking.StartGame(new SessionRequest
            {
                UserID = new Guid().ToString(),
                GameMode = Fusion.GameMode.Client,
                SessionName = Guid.NewGuid().ToString(),
                DisplayName = _BackToTutorial ? "Tutorial" : "Lobby",
                ScenePath = "Assets/TPSBR/Scenes/lobby.unity",//SceneUtility.GetScenePathByBuildIndex(5),
                GameplayType = _BackToTutorial ? EGameplayType.Tutorial : EGameplayType.Lobby,
                ExtraPeers = 0,
                MaxPlayers = 2,
                CustomLobby = _BackToTutorial ? "Tut." : "Lobby" + Application.version,
            });
        }
	}
}