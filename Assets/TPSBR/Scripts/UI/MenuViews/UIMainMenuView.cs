using UnityEngine;
using TMPro;
using Unity.Services.Matchmaker.Models;
using Fusion.Photon.Realtime;
using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using Unity.Services.Lobbies.Models;
using UnityEngine.SceneManagement;
using Fusion;

namespace TPSBR.UI
{
	public class UIMainMenuView : UIView
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private UIButton _playButton;

		[SerializeField]
		private bool _loginEnabled = true;


		[SerializeField]
		private UIButton _closeOptionsButton;

		[SerializeField]
		private CanvasGroup _canvasPromos;

		[SerializeField]
		private UIButton _settingsButton;
		[SerializeField]
		private UIButton _creditsButton;
		[SerializeField]
		private UIButton _walletButton;
		[SerializeField]
		private GameObject _walletContainer;
		[SerializeField]
		private UIButton _walletDeposit;

		[SerializeField]
		private UIButton _MarketButton;

		[SerializeField]
		private UIButton _tokenMarket;

		[SerializeField]
		private UIButton _bloomieBundles;


		[SerializeField]
		private UIButton _changeNicknameButton;
		[SerializeField]
		private UIButton _quitButton;
		[SerializeField]
		private UIButton _playerButton;
		[SerializeField]
		private UIPlayer _player;
		[SerializeField]
		private UIButton _changeAvatar;

		[SerializeField]
		private UIButton _tournamentMenu;
		[SerializeField]
		private UIButton _userButton;
		[SerializeField]
		private TextMeshProUGUI _agentName;

		[SerializeField]
		private CanvasGroup _loggedGroup;


		//private LobbyManager LobbyManager;

		[SerializeField]
		private CryptoManager cryptoManager;



		private UIMatchmakerView _matchmakerView;

	

		public void OnPlayerButtonPointerEnter()
		{
			Context.PlayerPreview.ShowOutline(true);
		}

		public void OnPlayerButtonPointerExit()
		{
			Context.PlayerPreview.ShowOutline(false);
		}

		void Awake()
		{
			//LobbyManager = new LobbyManager();
		}

		// UIView INTEFACE

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_settingsButton.onClick.AddListener(OnSettingsButton);
			_playButton.onClick.AddListener(OnPlayButton);
			_closeOptionsButton.onClick.AddListener(OnOptionsClose);

			_loggedGroup.alpha = 0;
			//_loggedGroup.SetActive(false);
			_walletContainer.SetActive(true);


			_MarketButton.onClick.AddListener(OnMarketButton);

			_tokenMarket.onClick.AddListener(OnTokenMarket);
			_bloomieBundles.onClick.AddListener(OnBloomieBundles);

			_creditsButton.onClick.AddListener(OnCreditsButton);
			_walletButton.onClick.AddListener(OnWalletButton);
			_walletDeposit.onClick.AddListener(OnDepositButton);
			_changeNicknameButton.onClick.AddListener(OnChangeNicknameButton);
			_quitButton.onClick.AddListener(OnQuitButton);
			_playerButton.onClick.AddListener(OnPlayerButton);
			_changeAvatar.onClick.AddListener(OnPlayerButton);
			_userButton.onClick.AddListener(OnWalletButton);

			_tournamentMenu.onClick.AddListener(tournamentsListView);

			CryptoManager.OnUserData += userDataRet;
			CryptoManager.OnWalletDetail += openWalletDetail;
			CryptoManager.OnUserDisconnect += userDisconnect;

			DataManager.OnHeartBeatTournament += matchInfo;

			GameManagerWebGL.Instance.LobbyDetailTournament += LobbyDetailTournament;
			GameManagerWebGL.Instance.LobbyDetailMatchmaker += LobbyDetailMatchmaker;
			GameManagerWebGL.Instance.LobbyInsideAlready += ReJoinAlert;

			GameManagerWebGL.Instance.LobbyJoined += ChangeSceneLobby;
			

			//AddressablesMain.instance.StartLoading();

			
			Debug.Log("Pasando aqui");
	
			Dictionary<string, string>  d = URLParameters.GetSearchParameters();
			string value = "";
			d.TryGetValue("partner",out value);


//			Debug.Log(value + " Value " + PlayerPrefs.GetString("LastActivity"));

		/*	if (PlayerPrefs.GetString("TutorialComplete") != "true" && !d.TryGetValue("partner",out value))
			{
				Debug.Log("Tutorial Loading........." + SceneUtility.GetScenePathByBuildIndex(7) + "NONANE");
				//PhotonAppSettings.Instance.AppSettings.FixedRegion = "us";
				PhotonAppSettings.Instance.AppSettings.FixedRegion = "us";
				Global.Networking.StartGame(new SessionRequest
				{
					UserID = new Guid().ToString(),
					GameMode = Fusion.GameMode.Host,
					SessionName = Guid.NewGuid().ToString(),
					DisplayName = "Tutorial",
					ScenePath = "Assets/TPSBR/Scenes/Tutorial.unity",
					GameplayType = EGameplayType.Tutorial,
					ExtraPeers = 0,
					MaxPlayers = 2,
					CustomLobby = "Tut." + Application.version,
				});
			}
*/
			// Check URL STring 
			
			//&& value != PlayerPrefs.GetString("LastActivity")
			//&& value.ToLower() != PlayerPrefs.GetString("LastActivity")
			if(d.TryGetValue("partner",out value)  ){
				PhotonAppSettings.Global.AppSettings.FixedRegion = "us";
				//Context.Matchmaker.MatchFound += OnMatchFound;
				//Open<UIMatchmakerView>();
				//Context.Matchmaker.StartMatchmaker("lobby");

				Debug.Log("Partner Loading........." + SceneUtility.GetScenePathByBuildIndex(8) + "NONANE");

				//NetworkRunner.SetActiveScene("Assets/TPSBR/Scenes/PartnerActivity.unity");

				//var buildIndex = SceneUtility.GetBuildIndexByScenePath("Assets/TPSBR/Scenes/PartnerActivity.unity");
				//Debug.Log(buildIndex + " buildIndex");


				Global.Networking.StartGame(new SessionRequest
				{
					UserID = new Guid().ToString(),
					GameMode = Fusion.GameMode.Host,
					SessionName = Guid.NewGuid().ToString(),
					DisplayName = "Activity",
					ScenePath = "Assets/TPSBR/Scenes/PartnerActivity.unity", //SceneUtility.GetScenePathByBuildIndex(8),
					GameplayType = EGameplayType.Activity,
					ExtraPeers = 0,
					MaxPlayers = 2,
					CustomLobby = "Activity." + Application.version // + UnityEngine.Random.Range(1,1000000)
				});

			}
			

		}

		protected override void OnDeinitialize()
		{
			_settingsButton.onClick.RemoveListener(OnSettingsButton);
			_playButton.onClick.RemoveListener(OnPlayButton);

			_MarketButton.onClick.RemoveListener(OnMarketButton);

			_tokenMarket.onClick.RemoveListener(OnTokenMarket);
			_bloomieBundles.onClick.RemoveListener(OnBloomieBundles);

			_walletButton.onClick.RemoveListener(OnWalletButton);
			_walletDeposit.onClick.AddListener(OnDepositButton);
			_changeNicknameButton.onClick.RemoveListener(OnChangeNicknameButton);
			_quitButton.onClick.RemoveListener(OnQuitButton);
			_playerButton.onClick.RemoveListener(OnPlayerButton);
			_changeAvatar.onClick.RemoveListener(OnPlayerButton);
			_userButton.onClick.RemoveListener(WalletClick);

			_tournamentMenu.onClick.RemoveListener(tournamentsListView);
			CryptoManager.OnUserDisconnect -= userDisconnect;

			CryptoManager.OnWalletDetail -= openWalletDetail;
			DataManager.OnHeartBeatTournament -= matchInfo;
			CryptoManager.OnUserData -= userDataRet;

			GameManagerWebGL.Instance.LobbyDetailTournament -= LobbyDetailTournament;
			GameManagerWebGL.Instance.LobbyDetailMatchmaker -= LobbyDetailMatchmaker;
			GameManagerWebGL.Instance.LobbyInsideAlready -= ReJoinAlert;

			base.OnDeinitialize();
		}

		public void AskTutorial(){
			if (PlayerPrefs.GetString("TutorialComplete") != "true"){
				Debug.Log("tuto not  complete");
				var dialog = Open<UITutDialogView>();
				dialog.Title.text = "PLAY TUTORIAL?";
				dialog.YesButtonText.SetText("Play Tutorial");
				dialog.NoButtonText.SetText("Maybe later");
				//dialog.Description.text = text;
				dialog.HasClosed += (result) =>
				{
					if (result == true){
						PhotonAppSettings.Global.AppSettings.FixedRegion = "us";
						Global.Networking.StartGame(new SessionRequest
						{
							UserID = new Guid().ToString(),
							GameMode = Fusion.GameMode.Host,
							SessionName = Guid.NewGuid().ToString(),
							DisplayName = "Tutorial",
							ScenePath = "Assets/TPSBR/Scenes/Tutorial.unity",
							GameplayType = EGameplayType.Tutorial,
							ExtraPeers = 0,
							MaxPlayers = 2,
							CustomLobby = "Tut." + Application.version,
						});
					}
					else{
						PlayerPrefs.SetString("TutorialComplete", "true");
					}
				};


			}
		}

		public void WalletClick()
		{
			cryptoManager.WalletClick();
		}

		private void OnMarketButton()
		{
			Application.OpenURL("https://market.bloomverse.io");
		}

		private void OnTokenMarket()
		{
			CryptoManager.instance.LauncheTokenMarket();
		}

		private void OnBloomieBundles()
		{
			Open<UIBloomieBundlesView>();
		}


		private void userDataRet(UserData userdata)
		{
			Debug.Log("USER DATA ----- main meni - ");
			heartBeatOn = true;
			_loggedGroup.alpha = 1;
			_loggedGroup.SetActive(true);
			_walletButton.SetActive(false);
			_walletContainer.SetActive(false);
			if(this.gameObject.activeSelf){
				//StartCoroutine(HeartRoutine(userdata));
			}
			//Debug.Log("Unity ID" + Context.PlayerData.UnityID);
			//GameManagerWebGL.Instance.JoinedLobbies(Context.PlayerData.UnityID);
		}

		private void userDisconnect(string origin)
		{
			Debug.Log("user disconnect");
			heartBeatOn = false;
			_loggedGroup.alpha = 0;
			_loggedGroup.SetActive(false);
			_walletButton.SetActive(true);
			_walletContainer.SetActive(true);
		}

		private bool heartBeatOn;
		IEnumerator HeartRoutine(UserData userdata)
		{
			while (heartBeatOn)
			{
//				Debug.Log("heartbeat tournaments");
				DataManager.instance.tournamentHearbeat(userdata._id);
				yield return new WaitForSeconds(10);
			};
		}

		private void matchInfo(TournamentMatchInfo matchInfo)
		{
			//Debug.Log(matchInfo._id + " hearthbeat");
			Rounds lastRound = null;
			for (int i = 0; i < matchInfo.rounds.Length; i++)
			{
				if (matchInfo.rounds[i].closed == "true")
				{
					lastRound = matchInfo.rounds[i];
				}
			}

			for (int i = 0; i < matchInfo.matches.Length; i++)
			{
				if (matchInfo.matches[i].round == lastRound.number)
				{
					//GameManagerWebGL.Instance.LobbyReadyID = matchInfo.matches[i].lobby;
					//GameManagerWebGL.Instance.LobbyReadyTournamentID = matchInfo._id;
					//GameManagerWebGL.Instance.tournamentRound = lastRound;
					//LoobyReadyAlert(matchInfo.name, matchInfo.matches[i].lobby);
					heartBeatOn = false;
				}
			}

		}

// JOIN ---------------------------------------------------
		private void LoobyReadyAlert(string tournamentName, string lobbyId)
		{
			var dialog = Open<UIYesNoDialogView>();
			dialog.Title.text = "TOURNAMENT MATCH READY";
			dialog.Description.text = "The Lobby for Tournament Match   " + tournamentName + " is ready, do you want to join?";
			dialog.HasClosed += (result) =>
			{
				if (result == true){
					GameManagerWebGL.Instance.QueryLobbyTournament(lobbyId);
				}
				else{
					heartBeatOn = true;
				}
			};
		}

		private void LobbyDetailTournament(object seder,Lobby lobbyInfo)
		{
/*
			Debug.Log(lobbyInfo + "lobby info");
			if (lobbyInfo != null){
				Context.PlayerData.lobbyType = "Tournament";
				Switch<UILobbyView>();
				GameManagerWebGL.Instance.JoinLobbySID(lobbyInfo.Id, Context.PlayerData);
			}

			else{
				var infoDialog = Open<UIInfoDialogView>();
				infoDialog.Title.text = "Bloomverse Matchmaker";
				infoDialog.Description.text = "Lobby not available anymore";
			}
			*/
		}

		private void ReJoinAlert(object seder, string lobbyId)
		{
			Debug.Log(lobbyId + "lobbyId in");
			if(lobbyId!=""){
					GameManagerWebGL.Instance.QueryLobbyMatchmaker(lobbyId);
			};
		}

		private void LobbyDetailMatchmaker(object seder, Lobby lobbyInfo){
			
			if (lobbyInfo != null){

				var dialog = Open<UIYesNoDialogView>();
				dialog.Title.text = "REJOIN GAME";
				dialog.Description.text = "You have a game in progress , do you want to reconnect?";

			dialog.HasClosed += (result) =>
			{
				if (result == true)
				{
					//GameManagerWebGL.Instance.QueryLobby(lobbyId);
					//GameManagerWebGL.Instance.ReConnectLobby(lobbyInfo.Id,);
					Close();

					//GameManagerWebGL.JoinLobby(lobbyId, Context.PlayerData);
				}
				else
				{
					heartBeatOn = true;
				}
			};

			}
		}

		private void Rejoin(object seder, Lobby lobbyInfo){
			
			if (lobbyInfo!= null)
			{
				Switch<UILobbyView>();
				GameManagerWebGL.Instance.JoinLobbySID(lobbyInfo.Id, Context.PlayerData);
			}
			else
			{
				var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Bloomverse Matchmaker";
				infoDialog.Description.text = "Lobby not available anymore";
			}
		}

		private void ChangeSceneLobby(object seder, Lobby lobbyInfo){
				Switch<UILobbyView>();
		}

		public void infoDialogGS(){
			  var infoDialog = Open<UIInfoDialogView>();
			    infoDialog.Title.text = "Info";
			    infoDialog.Description.text = "Powered by Gameshift and Solana Labs, your Bloom account allows top security protocols to protect your account while making it retrievable if you ever lose access to it. It will also allow you to use 2FA in the near future to prevent unauthorized transactions from happening. Use a secure email that you trust. ";
		}

		public void infoDialog(string desc){
			  var infoDialog = Open<UIInfoDialogView>();
			    infoDialog.Title.text = "Info";
			    infoDialog.Description.text = desc;
		}

		private void OnOptionsClose()
		{
			//changeOptsView(false);
		}
		/*
				private void changeOptsView(bool visible){
					if(visible){
						_opt1.SetActive(true);
						 _canvasPromos.DOFade(0f,.5f);

						//_opt1.alpha = 1;
					//_opt2.alpha = 1;
					}else{
						_opt1.SetActive(false);
						_canvasPromos.DOFade(1f,.5f);
					//	_opt1.alpha = 0;
					//_opt2.alpha = 0;
					}

				}

		*/



		private void tournamentsListView()
		{
			//changeOptsView(false);
			Tooltip.HideToolTip_Static();
			Switch<UITournament>();

		}

		private void tournamentsDetailView()
		{
			//changeOptsView(false);
			//Switch<UITournamentDetail>();

		}

		private void matchView()
		{
			//changeOptsView(false);
			Switch<UIMultiplayerView>();

		}

		private void detailView()
		{
			//changeOptsView(false);
			//Switch<UITournamentDetail>();

		}


		private async void lobbyMode()
		{
			Debug.Log("lobby requesting");
			_matchmakerView = Open<UIMatchmakerView>();
			await Context.Matchmaker.StartMatchmaker("lobby");
		}

		private void OnMatchFound(MultiplayAssignment assignment)
		{
			Debug.Log("Joining session " +  assignment.MatchId);
			Context.Matchmaking.JoinSession("mm-" + assignment.MatchId);
			Context.Matchmaker.MatchFound -= OnMatchFound;
			if (_matchmakerView != null)
			{
				_matchmakerView.Close();
				_matchmakerView = null;
			}
		}

		private void OnMatchmakerFailed(string message)
		{
			//_errorText.text = message;
			if (_matchmakerView != null)
			{
				_matchmakerView.Close();
				_matchmakerView = null;
				var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Bloomverse Services";
				infoDialog.Description.text = "Matchmaking failed err  B-12";
			}
		}

		private void openWalletDetail()
		{
			Open<UIWalletView>();
		}
		public void closeWallet(){
			
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			cryptoManager.startManager();
			UpdatePlayer();

			Global.PlayerService.PlayerDataChanged += OnPlayerDataChanged;
			//Context.Matchmaker.MatchFound += OnMatchFound;
		    //Context.Matchmaker.MatchmakerFailed += OnMatchmakerFailed;

			Context.PlayerPreview.ShowAgent(Context.PlayerData.AgentID);


//			Debug.Log("Main menu view ");
			Context.PlayerPreview.ShowOutline(false);

			if (PlayerPrefs.GetString("TutorialComplete") != "true" ){
				AskTutorial();
			}

			
		}

		protected override void OnClose()
		{
			Global.PlayerService.PlayerDataChanged -= OnPlayerDataChanged;

			//Context.Matchmaker.MatchFound -= OnMatchFound;
			//Context.Matchmaker.MatchmakerFailed -= OnMatchmakerFailed;

			Context.PlayerPreview.ShowOutline(false);

			base.OnClose();
			//Context.PlayerPreview._animator.SetBool("Aim",false);
		}

		protected override bool OnBackAction()
		{
			if (IsInteractable == false)
				return false;

#if !UNITY_WEBGL && !UNITY_EDITOR
			OnQuitButton();
#endif

			return true;
		}

		// PRIVATE METHODS

		private void OnSettingsButton()
		{
			Open<UISettingsView>();
		}


		private void tournamentHeartbeat()
		{

		}
		private bool matchDone;
		private IEnumerator Heartbeat()
		{


			while (!matchDone)
			{


			}

			yield return new WaitForSeconds(10);
		}

		private void OnPlayButton()
		{
			//Close();
			var cas = Switch<UIOptionsMenu>();
			cas.BackView = this;


		}

		private void OnCreditsButton()
		{
			Open<UICreditsView>();
		}
		private void OnWalletButton()
		{
			_loggedGroup.SetActive(true);
			//CryptoManager.instance.LoginParticleClick();
			Open<UIWalletView>();
		}
		private void OnDepositButton()
		{
			Debug.Log("invoking JS");
			//CryptoManager.AskbloomieTransaction();
		}

		private void OnChangeNicknameButton()
		{
			var changeNicknameView = Open<UIChangeNicknameView>();
			changeNicknameView.SetData("CHANGE NICKNAME", false);
		}

		private void OnQuitButton()
		{
			var dialog = Open<UIYesNoDialogView>();

			dialog.Title.text = "EXIT GAME";
			dialog.Description.text = "Are you sure you want to exit the game?";

			dialog.YesButtonText.text = "EXIT";
			dialog.NoButtonText.text = "CANCEL";

			dialog.HasClosed += (result) =>
			{
				if (result == true)
				{
					SceneUI.Scene.Quit();
				}
			};
		}

		private void OnPlayerButton()
		{
			Debug.Log("player enter selecion");

			var agentSelection = Open<UIAgentSelectionView>();
			//var alevel= Open<UIlevelInfo>();


			agentSelection.BackView = this;

			Close();

			if (Context.PlayerPreview._animator != null)
			{
				Context.PlayerPreview._animator.CrossFade("Pistol", .04f);
			}

		}

		private void OnPlayerDataChanged(PlayerData playerData)
		{
			UpdatePlayer();
		}

		private void UpdatePlayer()
		{
			_player.SetData(Context, Context.PlayerData);
			Context.PlayerPreview.ShowAgent(Context.PlayerData.AgentID);

			var setup = Context.Settings.Agent.GetAgentSetup(Context.PlayerData.AgentID);
			_agentName.text = setup != null ? $"Playing as {setup.DisplayName}" : string.Empty;
		}
	}
}
