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
using System.Threading.Tasks;
using UnityEngine.SceneManagement;

namespace TPSBR.UI


{
	public class UITutorialGameOverView : UIView
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private UIBehaviour _winnerGroup;

		[SerializeField]
		private TextMeshProUGUI _username;


		[SerializeField]
		private TextMeshProUGUI _winner;

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
		private TMP_InputField _name;
		[SerializeField]
		private int _minCharacters = 5;


		[SerializeField]
		private GameObject _bloomieLogo;

		[SerializeField]
		private GameObject _counterPrefab;


		//public UIlevelInfoAnim levelInfoAnim;


		[SerializeField]
		private GameObject attractorBloomies;
		[SerializeField]
		private GameObject attractorXP;

		// UIView INTERFACE

		//


		[SerializeField]
		private GameObject _mask;
		[SerializeField]
		private CanvasGroup _background;

		private CryptoManager cryptoManager;



		// CANVAS SEQUENCE

		[SerializeField]
		private CanvasGroup _XpContainer;

		[SerializeField]
		private CanvasGroup _usernameContainer;
		[SerializeField]
		private Transform _pointsAnim;


		[SerializeField]
		private CanvasGroup _sec1;
		[SerializeField]
		private CanvasGroup _sec2;
		[SerializeField]
		private CanvasGroup _sec3;
		[SerializeField]
		private CanvasGroup _sec4;
		[SerializeField]
		private CanvasGroup _sec5;

		[SerializeField]
		private UIButton _sec1Bt;
		[SerializeField]
		private UIButton _sec2Bt;
		[SerializeField]
		private UIButton _sec3Bt;
		[SerializeField]
		private UIButton _sec4Bt;
		[SerializeField]
		private UIButton _sec5Bt;


		[SerializeField]
		private TextMeshProUGUI _text1;
		[SerializeField]
		private TextMeshProUGUI _text2;
		[SerializeField]
		private TextMeshProUGUI _text3;
		[SerializeField]
		private TextMeshProUGUI _text4;
		[SerializeField]
		private TextMeshProUGUI _text5;




		[SerializeField]
		private UIlevelInfoAnim _levelAnim;

		[SerializeField]
		private ParticleSystem _matchParticles;


		List<finalAnimation> finalAnimations2 = new List<finalAnimation>();

		private int bloomiesMatch;
		private int bloomiesExtra;
		private int XPMatch;
		private int XPKillPlace;


		protected override void OnInitialize()
		{
			base.OnInitialize();
			
		}



		protected override void OnOpen()
		{
			base.OnOpen();
			cryptoManager = GameObject.Find("CryptoManager").GetComponent<CryptoManager>();
			//_sec1Bt.onClick.AddListener(Section2);
			//CryptoManager.OnLoginCallback += Section2;


			_sec1Bt.onClick.AddListener(OnWalletButton);


			FindObjectOfType<MissionSystem>().SetActive(false);



			if(Context.PlayerData.WalletID==null){
				StartCoroutine(Section1());
			}else{
				Section2();
			}
			

			GameObject.Find("Rockets").SetActive(false);
			//Debug.Log("abriendo tutorial game over");

			var chatObj = GameObject.Find("ChatPrefab");
			if (chatObj != null)
			{
				chatObj.SetActive(false);
			}

			var userVal = "Player Bonus";

			if (Context.PlayerData.Nickname != "")
			{
				userVal = Context.PlayerData.Nickname;
			}

			var winnerStatistics = GetWinner();
			Player winner = null;

			if (Context != null)
			{
				//Context.Input.RequestCursorVisibility(true, ECursorStateSource.UI);
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


			Global.Networking.StopGameOnDisconnect();
		}


		private IEnumerator Section1()
		{
			textEffect(_text1);
			_sec1.SetActive(true);
			_sec1.DOFade(1, 1).SetEase(Ease.OutBack).SetDelay(0);

			TutorialManager.Instance.AnnouncerStep(6f, false);

			yield return new WaitForSeconds(8.6f);

			TutorialManager.Instance.AnnouncerStep(5f, false);
		}


		private void Section2()
		{
			// SET NICKNAME

				_sec1.SetActive(false);
			Debug.Log("Section 2 entro");
			if(Context.PlayerData.Nickname!=""){
				Section3();
				return;
			}

		
			_sec2.SetActive(true);
			_sec1.DOFade(0, 1).SetEase(Ease.OutBack).SetDelay(0);
			_sec2.DOFade(1, 1).SetEase(Ease.OutBack).SetDelay(0);

			TutorialManager.Instance.AnnouncerStep(5f, false);

			_sec2Bt.onClick.AddListener(validateName);

			textEffect(_text2);

			string currentNickname = Context.PlayerData.Nickname;
			if (currentNickname.HasValue() == false)
			{

			}
			else
			{
				_name.text = Context.PlayerData.Nickname;
			}
			//LoginController.
		}

		private void validateName()
		{
			if (_name.text.Length >= _minCharacters)
			{
				Section3();
			}
			else
			{
				var dialog = Open<UIYesNoDialogView>();

				dialog.Title.text = "Player Username";
				dialog.Description.text = "Please input a valid Username with at least 5 characters";

				dialog.HasClosed += (result) =>
				{
					if (result == true)
					{

					}
					else
					{

					}
				};
			}
		}
		private void Section3() => StartCoroutine(sec3_cor());

		IEnumerator sec3_cor()
		{

			_sec3Bt.onClick.AddListener(Section4);

			Context.PlayerData.Nickname = _name.text;
			if (PersistentStorage.GetString("token") != "")
			{
				_username.text = _name.text;
				cryptoManager.updateNickname();
			}

			_usernameContainer.DOFade(1, 1).SetEase(Ease.OutBack).SetDelay(0);

			_bloomieLogo.transform.DOScale(1.25f, 1).SetEase(Ease.OutBack).SetLoops(4, LoopType.Yoyo).SetDelay(0);

			// SET VALUES 


			var playerS = GetPlayerStatistics();
			var playerData = GetPlayer();

			// Match Exp Value
			var killsXP = 2000;
			var matchXPBase = 5000;
			// Bloomies Value
			var bonusBloomies = 50;
			var basebloomies = 50;

			// Base Bloomies & Position Bloomies & 
			bloomiesMatch = basebloomies;
			// Match Mechanics Extra & last Bonus
			bloomiesExtra = bonusBloomies;
			// Match Exp
			XPMatch = matchXPBase;
			//  Match Kills & Position
			XPKillPlace = killsXP;

			var XPtotal = XPMatch + XPKillPlace;

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


			StartCoroutine(animProc(finalAnimations));
			PlaySound(_openSound);
			//StartCoroutine(sec3_cor());

			TutorialManager.Instance.AnnouncerStep(5f, false);

			_sec2.SetActive(false);
			_sec3.SetActive(true);
			_sec2.DOFade(0, 1).SetEase(Ease.OutBack).SetDelay(0);
			_sec3.DOFade(1, 1).SetEase(Ease.OutBack).SetDelay(0);

			yield return null;
		}

		private void Section4() => StartCoroutine(sec4_cor());

		IEnumerator sec4_cor()
		{

			_sec4Bt.onClick.AddListener(Section5);

			_sec3.SetActive(false);
			_sec4.SetActive(true);
			_sec3.DOFade(0, 1).SetEase(Ease.OutBack).SetDelay(0);
			_sec4.DOFade(1, 0).SetEase(Ease.OutBack).SetDelay(0);

			//finalAnimations.Clear();

			var anim3 = new finalAnimation();
			anim3.description = "Level XP";
			anim3.amount = XPMatch;
			anim3.attractor = attractorXP;
			anim3.particleIndex = 1;
			anim3.particleName = "XPCoinFinal";
			finalAnimations2.Add(anim3);

			var anim4 = new finalAnimation();
			anim4.description = "Bonus Time XP";
			anim4.amount = XPKillPlace;
			anim4.attractor = attractorXP;
			anim4.particleIndex = 1;
			anim4.particleName = "XPCoinFinal";
			finalAnimations2.Add(anim4);

			_XpContainer.DOFade(1, 1).SetEase(Ease.OutBack).SetDelay(0);
			_usernameContainer.DOFade(1, 1).SetEase(Ease.OutBack).SetDelay(0);

			TutorialManager.Instance.AnnouncerStep(5f, false);

			StartCoroutine(animProc(finalAnimations2));

			_levelAnim.setLevelC(XPMatch + XPKillPlace);

			yield return new WaitForSeconds(5f);

			TutorialManager.Instance.AnnouncerStep(5f, false);
		}

		private void Section5() => StartCoroutine(sec5_cor());

		private IEnumerator sec5_cor()
		{

			_sec5Bt.onClick.AddListener(lobbyInvoke);

			_sec4.SetActive(false);
			_sec5.SetActive(true);
			_sec4.DOFade(0, 1).SetEase(Ease.OutBack).SetDelay(0);
			_sec5.DOFade(1, 0).SetEase(Ease.OutBack).SetDelay(0);

			TutorialManager.Instance.AnnouncerStep(5f, false);

			yield return new WaitForSeconds(6f);

			TutorialManager.Instance.AnnouncerStep(5f, false);

			yield return new WaitForSeconds(6f);

			TutorialManager.Instance.AnnouncerStep(7f, false);
		}

		private void lobbyInvoke()
		{
			_sec5.SetActive(false);
			_sec5.DOFade(0, 1).SetEase(Ease.OutBack).SetDelay(0);



			Global.Networking.StartGame(new SessionRequest
			{
				UserID = new Guid().ToString(),
				GameMode = Fusion.GameMode.Host,
				SessionName = Guid.NewGuid().ToString(),
				DisplayName = "Tutorial",
				ScenePath = "Assets/TPSBR/Scenes/lobby.unity",//SceneUtility.GetScenePathByBuildIndex(5),
				GameplayType = EGameplayType.Lobby,
				ExtraPeers = 0,
				MaxPlayers = 2,
				CustomLobby = "Tut." + Application.version,
			});

		}

		IEnumerator textEffect(TextMeshProUGUI m_textMeshPro)
		{

			Debug.Log("efecto text");

			int totalVisibleCharacters = m_textMeshPro.textInfo.characterCount; // Get # of Visible Character in text object
			int counter = 0;

			while (true)
			{
				int visibleCount = counter % (totalVisibleCharacters + 1);
				m_textMeshPro.maxVisibleCharacters = visibleCount; // How many characters should TextMeshPro display?

				if (visibleCount >= totalVisibleCharacters)
				{
					yield return new WaitForSeconds(1.0f);
				}

				counter += 1;

				yield return new WaitForSeconds(0.05f);
			}

		}



		private void titleAnim(int delay)
		{

			var bgMusic = GameObject.Find("SceneMusic");
			if (bgMusic != null)
			{
				bgMusic.GetComponent<AudioSource>().DOFade(0, 1f);
			}


			_audioEffect.Play();
			//_matchParticles.Emit(10);
			_audioMusic.Play();
			_audioSource.volume = 0.05f;

		}


		private void OnWalletButton()
		{

			CryptoManager.OnUserData += ParticleReturn;
			//CryptoManager.instance.LoginParticleClick();
			GameplayUI gameplayUI = GameObject.Find("GameplayUI").GetComponent<GameplayUI>();
			
            gameplayUI.OpenUserSignIn();
			//Open<UIWalletView>();
		}




		private float ParticleTotalTime = 1f;
		IEnumerator animProc(List<finalAnimation> finalAnimations)
		{

			Debug.Log("Creating animations");

			foreach (finalAnimation x in finalAnimations)
			{

				CreateAnim(x.description, x.amount, x.attractor, x.particleIndex, x.particleName);
				yield return new WaitForSeconds(ParticleTotalTime);
			}

		}

		void CreateAnim(string title, int amount, GameObject attractor, int particleIndex, string listenerName)
		{

			var InstancedNumbers = Instantiate(_counterPrefab, _pointsAnim.transform);

			var att1 = attractor.AddComponent<UIParticleAttractor>();
			att1.enabled = false;
			att1.maxSpeed = 10;
			if (particleIndex == 0)
			{
				att1.movement = UIParticleAttractor.Movement.Sphere;
			}
			else
			{
				att1.movement = UIParticleAttractor.Movement.Sphere;
			}

			Type type = att1.GetType(); // Or typeof(UIParticleAttractor)
			FieldInfo field = type.GetField("m_ParticleSystem", BindingFlags.NonPublic | BindingFlags.Instance);
			ParticleSystem coinSystem = InstancedNumbers.GetComponent<CounterAnimEnd>()._particles[particleIndex];
			var main = coinSystem.main;
			main.simulationSpace = ParticleSystemSimulationSpace.World;
			//main.simulationSpace = ParticleSystemSimulationSpace.Local;
			//main.customSimulationSpace = transform;
			InstancedNumbers.GetComponent<CounterAnimEnd>().ParticleIndex = particleIndex;
			ParticleSystem.Burst burst = coinSystem.emission.GetBurst(0);
			burst.count = 1;
			var amountTemp = (listenerName == "bloomieCoinBadge") ? amount : amount / 10;
			burst.cycleCount = amountTemp;
			burst.repeatInterval = (ParticleTotalTime / (float)amountTemp);
			coinSystem.emission.SetBurst(0, burst);
			field.SetValue(att1, coinSystem);


			//add Event 
			FieldInfo fieldEvent = type.GetField("m_OnAttracted", BindingFlags.NonPublic | BindingFlags.Instance);

			if (listenerName == "bloomieCoinBadge")
			{
				var coinparticles = GameObject.Find("bloomieCoinBadge").GetComponentInChildren<bloomieCoinFinal>();

				UnityEvent unityEvent = new UnityEvent();
				unityEvent.AddListener(coinparticles.shake);
				fieldEvent.SetValue(att1, unityEvent);
			}
			if (listenerName == "XPCoinFinal")
			{
				var coinparticles = GameObject.Find("XPCoinFinal").GetComponentInChildren<XPCoinFinal>();
				UnityEvent unityEvent = new UnityEvent();
				unityEvent.AddListener(coinparticles.shake);
				fieldEvent.SetValue(att1, unityEvent);
			}




			att1.enabled = true;
			InstancedNumbers.GetComponent<CounterAnimEnd>().startAnim(0, title, amount);
		}


		private void nextAnim()
		{
			Debug.Log("Termino anim");
		}

		protected override void OnDeinitialize()
		{
			_sec2Bt.onClick.RemoveListener(OnWalletButton);
			_sec3Bt.onClick.RemoveListener(Section3);
			_sec4Bt.onClick.RemoveListener(Section4);

			base.OnDeinitialize();
		}

		// PRIVATE MEMBERS

		private PlayerStatistics GetPlayerStatistics()
		{
			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;

				if (player.Player == Context.LocalPlayerRef)
				{
					var statistics = player.Statistics;
					return statistics;
				}
			}

			return default;
		}

		private Player GetPlayer()
		{
			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null)
					continue;

				if (player.Player == Context.LocalPlayerRef)
				{
					return player;
				}
			}

			return default;
		}


		private void ParticleReturn(UserData data)
		{
			//Debug.Log(data + " Player database id"  + layerPrefs.GetString("TutorialComplete")!=true);
			Debug.Log("Regreso wallet sign in");

			if (data.solanaWallet != null && PlayerPrefs.GetString("TutorialComplete") != "true")
			{
				
				Debug.Log("Sengin Bloomies");
				SetList();
			}
			else
			{
				var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Bloomverse Services";
				infoDialog.Description.text = "Experience and Bloomie bonus are just added the first time you do the tutorial";

			}

			PlayerPrefs.SetString("TutorialComplete", "true");
			GameplayUI gameplayUI = GameObject.Find("GameplayUI").GetComponent<GameplayUI>();
			
            gameplayUI.CloseUserSignIn();

			Section2();
			CryptoManager.OnUserData -= ParticleReturn;

			//TODO SEND BLOOMIES
			//_matchParticles.Emit(10);
		}

		public void SetList()
		{

			List<UserToBase> rewardedPlayers = new List<UserToBase>(32);

			foreach (var player in Context.NetworkGame.ActivePlayers)
			{
				if (player == null || player.DBID == null)
					continue;

				var nplayer = new UserToBase();
				nplayer.UserID = player.UserID;
				nplayer.DBID = Context.PlayerData.userData._id;// player.DBID;
				//nplayer.WalletID = player.Context.PlayerData.userData.solanaWallet;

				//_lobbyId =  player.LobbyId;
				//_lobbyType = player.LobbyType;	

				// EXP
				nplayer.matchEXP = 5000;
				// BLoomies

				nplayer.bloomies = 100;

				var hours = (DateTime.UtcNow.Ticks - player.LastPlayed);

				var ts = TimeSpan.FromTicks(hours).TotalHours;

				//TimeSpan diff = (DateTime.UtcNow - nplayer.lastPlayed).Duration();


				nplayer.lastBloomieBonus = 0;


				nplayer.lastPlayed = DateTime.Now.Ticks;
				nplayer.SelectedChar = player.AgentID;

				Debug.Log("player DB ID " + player.DBID + "/ " + player.SelectedSkin + " / " + player.AgentID);

				rewardedPlayers.Add(nplayer);

			}

			Debug.Log("Sending rewards length" + Context.NetworkGame.ActivePlayerCount);


			Debug.Log("cerramos lista" + rewardedPlayers.Count);
			DataManager.instance.endIns(rewardedPlayers);

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