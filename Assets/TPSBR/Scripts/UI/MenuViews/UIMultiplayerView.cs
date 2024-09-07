using Fusion;
using System.Collections.Generic;
using System.Collections;
using Fusion.Photon.Realtime;
using UnityEngine;
using TMPro;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Lobbies.Models;


#pragma warning disable 4014

namespace TPSBR.UI
{
	public class UIMultiplayerView : UICloseView
	{
		// PRIVATE MEMBERS

		//[SerializeField] private UISession _sessionDetail;
        [SerializeField] private UILobby _lobbyDetail;
        [SerializeField] private UIButton _createSessionButton;
		[SerializeField] private UIButton _quickPlayButton;
		[SerializeField] private UIButton _lobbyButton;
		[SerializeField] private UIButton _bloomiePlayButton;
		[SerializeField] private UIButton _cancelQuickPlayButton;
		[SerializeField] private UIButton _joinButton;
		[SerializeField] private UIButton _privateButton;
		[SerializeField] private UIButton _settingsButton;
		[SerializeField] private TMP_Dropdown _regionDropdown;

		[SerializeField] private UIButton _onBackButton;

		[SerializeField] private UIButton _refreshingButton;
		[SerializeField] private TextMeshProUGUI _refreshingTextAuto;
        [SerializeField] private SimpleAnimation _refreshingIcon;
		[SerializeField] private GameObject _refreshingText;
        [SerializeField] private UIBehaviour _noSessionsGroup;
		[SerializeField] private TextMeshProUGUI _errorText;
		[SerializeField] private GameManagerWebGL GameManagerWebGL;

		[SerializeField] private TextMeshProUGUI _applicationVersion;

		//private UISessionList _sessionList;
		private UILobbyList _lobbyList;
		private UIMatchmakerView _matchmakerView;

		private List<SessionInfo> _sessionInfo = new List<SessionInfo>(32);
        private List<Lobby> _lobbyInfo = new List<Lobby>(32);

        private SessionInfo _selectedSession;
        private Lobby _selectedLobby;

        private SimpleAnimation _joinButtonAnimation;

		// PUBLIC METHODS
		public void StartQuickPlay()
		{
			OnQuickPlayButton();
		}

		protected override void OnInitialize()
		{
			base.OnInitialize();

			Debug.Log("add listeners");
            _lobbyList = GetComponentInChildren<UILobbyList>();
            _lobbyList.SelectionChanged += OnLobbySelectionChanged;
            _lobbyList.UpdateContent += OnUpdateLobbyListContent;

			_quickPlayButton.onClick.AddListener(OnQuickPlayButton);
			_lobbyButton.onClick.AddListener(OnLobbyButton);

			_onBackButton.onClick.AddListener(OnBackScreen);
			_createSessionButton.onClick.AddListener(OnCreateGameButton);
			//_bloomiePlayButton.onClick.AddListener(OnQuickPlayButton);
			_cancelQuickPlayButton.onClick.AddListener(OnCancelQuickPlay);
			_joinButton.onClick.AddListener(OnJoinButton);
			_privateButton.onClick.AddListener(OnPrivateButton);
			_settingsButton.onClick.AddListener(OnSettingsButton);
			_regionDropdown.onValueChanged.AddListener(OnRegionChanged);
            _refreshingButton.onClick.AddListener(OnRefreshButton);

            //_sessionDetail.SetActive(false);
			_joinButtonAnimation = _joinButton.GetComponent<SimpleAnimation>();

			//PrepareRegionDropdown();
		}
		private void OnCreateGameButton()
		{		
				var session = Open<UICreateSessionView>();
				//session.BackView = this;
		}
		protected override void OnDeinitialize()
		{

			Debug.Log("remove listeners");
			  _lobbyList.SelectionChanged -= OnLobbySelectionChanged;
            _lobbyList.UpdateContent -= OnUpdateLobbyListContent;	
           

			_quickPlayButton.onClick.RemoveListener(OnQuickPlayButton);
			_cancelQuickPlayButton.onClick.RemoveListener(OnCancelQuickPlay);
			_joinButton.onClick.RemoveListener(OnJoinButton);
			_settingsButton.onClick.RemoveListener(OnSettingsButton);
			_regionDropdown.onValueChanged.RemoveListener(OnRegionChanged);
            _refreshingButton.onClick.RemoveListener(OnRefreshButton);
			_createSessionButton.onClick.RemoveListener(OnCreateGameButton);

            base.OnDeinitialize();
		}

		private void OnLobbyCreated(object seder, Lobby lobby){
			Close();
		}

		protected override bool OnBackAction(){
			
				Switch<UIMainMenuView>();	
				SceneUI.Get<UIJoinPrivateView>()?.Close();
				return true;
		}

		private void OnBackScreen(){
			//Close();
			Switch<UIMainMenuView>();
			SceneUI.Get<UIJoinPrivateView>()?.Close();
		}

		protected override void OnOpen()
		{
			base.OnOpen();
			GameManagerWebGL.LobbiesListUpdated += OnLobbyListUpdated;
			GameManagerWebGL.LobbyCreated += OnLobbyCreated;
			GameManagerWebGL.LobbyLeaved += OnLobbyLeaved;
			GameManagerWebGL.LobbyJoined += OnLobbyJoined;

			Context.Matchmaker.MatchFound += OnMatchFound;
		    Context.Matchmaker.MatchmakerFailed += OnMatchmakerFailed;

			var currentRegion = Context.RuntimeSettings.Region;
			int regionIndex = System.Array.FindIndex(Context.Settings.Network.Regions, t => t.Region == currentRegion);
			_regionDropdown.SetValueWithoutNotify(regionIndex);

			GameManagerWebGL.QueryLobbies();
        }

		protected override void OnCloseButton(){
			Switch<UIMainMenuView>();
			SceneUI.Get<UIJoinPrivateView>()?.Close();
		}

		protected override void OnClose()
		{
            //Context.Matchmaking.SessionListUpdated -= OnSessionListUpdated;
            GameManagerWebGL.LobbiesListUpdated -= OnLobbyListUpdated;
            GameManagerWebGL.LobbyLeaved -= OnLobbyLeaved;
			GameManagerWebGL.LobbyCreated -= OnLobbyCreated;

			Context.Matchmaker.MatchFound -= OnMatchFound;
		    Context.Matchmaker.MatchmakerFailed -= OnMatchmakerFailed;
          
			base.OnClose();
			
		}

		protected override void OnTick()
		{
			base.OnTick();
			//_refreshingGroup.SetActive(GameManager.isFetching);
			if (GameManagerWebGL.isFetching)
			{
                _refreshingIcon.Play(0);
				_refreshingText.SetActive(false);
				_refreshingTextAuto.text = "Fetching lobbies...";
			}
			else
			{
				_refreshingText.SetActive(true);
				_refreshingIcon.Stop();
				_refreshingTextAuto.text = "Auto Refreshing in..." + refreshTimeout.ToString("0");
			}
			

            //bool canJoin = CanJoinLobby(_selectedLobby);
			//_joinButton.interactable = canJoin;
			//_joinButtonAnimation.enabled = canJoin;
			//_joinButton.SetActive(canJoin);
			refreshTimeout -= Time.deltaTime;
			OnRefreshButton();
		}

		// PRIVATE METHODS

        private void OnLobbyLeaved(object seder, Lobby lobby)
		{
			Open();
		}

        private void OnLobbyListUpdated(object seder, List<Lobby> lobbies)
        {
			//_refreshingButton.GetComponentInChildren<SimpleAnimation>().Stop();
//			Debug.Log("Lobby list updated");

			for (int i = 0; i < lobbies.Count; i++)
			{
                Lobby lobby = lobbies[i];

				Debug.Log(lobby.Name + " Lobby name");
				Debug.Log(lobby.Id + " Lobby Id");
				Debug.Log(lobby.Data["GameplayType"] + " Lobby Id");
				Debug.Log(lobby.Players + " Lobby Players");


				_lobbyInfo.Add(lobby);
			}

			_lobbyList.Refresh(lobbies.Count);

			UpdateLobbyDetail();
		}
		  private void OnLobbyJoined(object seder, Lobby lobby)
        {
			Debug.Log(seder.ToString() + " origen seeder");
			_errorText.text = string.Empty;
            Close();
        }


		private void OnLobbyJoinFailed(string region)
		{
			var regionInfo = Context.Settings.Network.GetRegionInfo(region);

			var regionText = regionInfo != null ? $"{regionInfo.DisplayName} ({regionInfo.Region})" : "Unknown";
			_errorText.text = $"Joining lobby in region {regionText} failed";
		}


		private float refreshTimeout = 10f;

		private void OnRefreshButton()
		{
			//_refreshingButton.GetComponentInChildren<SimpleAnimation>().Play(0);
			if(refreshTimeout < 0){
				refreshTimeout = 10f;
				GameManagerWebGL.QueryLobbies();
			}
			
		}

		private async void OnQuickPlayButton()
		{
			if (Context.PlayerData.UnityID.HasValue() == true)
			{
				_errorText.text = string.Empty;
				_matchmakerView = Open<UIMatchmakerView>();
				PhotonAppSettings.Global.AppSettings.FixedRegion = "us";
				_quickPlayButton.enabled = false;
				await Context.Matchmaker.StartMatchmaker("quickgame");  //Global.Settings.Network.GetCustomOrDefaultQueueName()
			}
			else
			{
				var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Unity Gaming Services";
				//infoDialog.Description.text = "For matchmaking functionality Unity Gaming Services need to be configured.\n\nPlease follow instructions in the Fusion BR documentation on how to add Multiplay support.";
				infoDialog.Description.text = "Matchmaking functionality need to be configured";
			}
		}
		
		private async void OnLobbyButton()
		{
			if (Context.PlayerData.UnityID.HasValue() == true)
			{
				_errorText.text = string.Empty;
				_matchmakerView = Open<UIMatchmakerView>();
				await Context.Matchmaker.StartMatchmaker("lobby");
			}
			else
			{
				var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Unity Gaming Services";
				infoDialog.Description.text = "Matchmaking functionality need to be configured";
			}
		}

		private async void OnCancelQuickPlay()
		{
			await Context.Matchmaker.CancelMatchmaker();
		}

		private void OnMatchFound(MultiplayAssignment assignment)
		{
			Context.Matchmaking.JoinSession("mm-" + assignment.MatchId);
			if (_matchmakerView != null)
			{
				_matchmakerView.Close();
				_matchmakerView = null;
			}
		}

		private void OnMatchmakerFailed(string message)
		{
			_errorText.text = message;
			if (_matchmakerView != null)
			{
				_matchmakerView.Close();
				_matchmakerView = null;
			}
		}

		//private void OnSessionSelectionChanged(int index)
		//{
		//	_selectedSession = index >= 0 ? _sessionInfo[index] : null;
		//	UpdateSessionDetail();
		//}

		private void OnLobbySelectionChanged(int index)
		{
			_selectedLobby = index >= 0 ? _lobbyInfo[index] : null;
            UpdateLobbyDetail();
        }


  //      private void OnUpdateSessionListContent(int index, UISession content)
		//{
		//	content.SetData(_sessionInfo[index]);
		//}

		private void OnUpdateLobbyListContent(int index, UILobby content)
		{
            content.SetData(_lobbyInfo[index]);
        }
		

		 private bool canClick = true;
    	 private float clickDelay = 1.0f; 

        private void OnJoinButton()
		{
			Debug.Log("Clicking");
			//Context.Matchmaking.JoinSession(_selectedSession);

			// Para el boton join checa el estatus del lobby , si comenzo, si estoy , si es modo normal

			//if(canClick){
				if(_selectedLobby.Data.ContainsKey("Tournament") && _selectedLobby.Data["Tournament"].Value=="true"){
					Context.PlayerData.lobbyType = "Tournament";
				}
				GameManagerWebGL.JoinLobby(_selectedLobby, Context.PlayerData);
				//_joinButton.enabled = false;
				//StartCoroutine(ClickCooldown());
			//}
			
		}
		private IEnumerator ClickCooldown()
    	{
			canClick = false;
			
			yield return new WaitForSeconds(clickDelay);
			canClick = true;
			//_joinButton.enabled = true;
    	}

		private void OnSettingsButton()
		{
			Open<UISettingsView>();
		}
		private void OnPrivateButton()
		{
			Open<UIJoinPrivateView>();
		}

		private void OnRegionChanged(int regionIndex)
		{
			var region = Context.Settings.Network.Regions[regionIndex].Region;
			Context.RuntimeSettings.Region = region;
		
		}

		private void UpdateLobbyDetail()
		{
            if (_selectedLobby == null)
            {
                _lobbyDetail.SetActive(false);
            }
            else
            {
                _lobbyDetail.SetActive(true);
                _lobbyDetail.SetData(_selectedLobby);
				//Debug.Log("Is interactavle" + CanJoinLobby(_selectedLobby));
                _joinButton.SetActive(CanJoinLobby(_selectedLobby));
            }
        }
		private bool CanJoinLobby(Lobby lobby)
		{
			Debug.Log("State " +lobby.Data["State"].Value);
			Debug.Log("GameplayType " +lobby.Data["GameplayType"].Value);
			Debug.Log("Count " + lobby.Players.Count);
			Debug.Log("Max" +lobby.MaxPlayers);
			Debug.Log("Private" +lobby.IsPrivate);
			Debug.Log("Locked" + lobby.IsLocked);

            if (lobby == null){
				Debug.Log("Enter1");
                return false;
			}
				

			if (lobby.Data["State"].Value=="Finished" ){
				Debug.Log("Enter2");
                return false;	
			}


            if (lobby.Players.Count >= lobby.MaxPlayers){
				Debug.Log("Enter3");
                return false;	
			}
			

            if (lobby.IsPrivate == true || lobby.IsLocked == true){
				Debug.Log("Enter4");
                return false;
			}

			//Debug.Log(lobby.Data["Tournament"].Value + " eS TORNEO?" );
			if(lobby.Data.ContainsKey("Tournament") && lobby.Data["Tournament"].Value=="true"){
				foreach(var player in lobby.Players){
					Debug.Log(player.Id + " - " + Context.PlayerData.UnityID);
					if(player.Id==Context.PlayerData.UnityID){
						return true;
					}
					
				}
				//return false;
			}
				



            return true;
        }


        private void PrepareRegionDropdown()
		{
			var options = ListPool.Get<TMP_Dropdown.OptionData>(16);
			var regions = Context.Settings.Network.Regions;

			for (int i = 0; i < regions.Length; i++)
			{
				var regionInfo = regions[i];
				options.Add(new TMP_Dropdown.OptionData(regionInfo.DisplayName, regionInfo.Icon));
			}

			_regionDropdown.ClearOptions();
			_regionDropdown.AddOptions(options);

			ListPool.Return(options);
		}
	}
}
