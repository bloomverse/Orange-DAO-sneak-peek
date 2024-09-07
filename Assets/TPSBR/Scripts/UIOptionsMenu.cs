using UnityEngine;
using Unity.Services.Matchmaker.Models;
using UnityEngine.SceneManagement;
using System;
using Fusion.Photon.Realtime;


namespace TPSBR.UI
{
	public class UIOptionsMenu : UICloseView
	{
		// PRIVATE MEMBERS


		[SerializeField]
		private UIButton _closeOptionsButton;

		[SerializeField]
		private CanvasGroup _opt1;

		[SerializeField]
		private UIButton _optTournament;
		[SerializeField]
		//private UIButton _optArena;
		private CanvasGroup _optArena;
		[SerializeField]
		private UIButton _optLobby;
		[SerializeField]
		private UIButton _optTutorial;

		private UIMatchmakerView _matchmakerView;


		// UIView INTEFACE

		protected override void OnInitialize()
		{
			base.OnInitialize();
			_closeOptionsButton.onClick.AddListener(OnOptionsClose);
			

			//_optTournament.onClick.AddListener(tournamentsListView);
			//_optArena.onClick.AddListener(matchView);
			//_optLobby.onClick.AddListener(lobbyMode);
			//_optTutorial.onClick.AddListener(TutorialMode);

		}

		protected override void OnDeinitialize()
		{
			_closeOptionsButton.onClick.RemoveListener(OnOptionsClose);
			//_optTournament.onClick.RemoveListener(matchView);
			//_optArena.onClick.RemoveListener(lobbyMode);
			//_optLobby.onClick.RemoveListener(lobbyMode);
			//_optTutorial.onClick.RemoveListener(TutorialMode);
			base.OnDeinitialize();
		}

		private void OnOptionsClose()
		{
			//Close();
			Switch<UIMainMenuView>();
		}

		public void TutorialMode()
		{
			// Temporary send to US 
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

		public void tournamentsListView()
		{
			Switch<UITournament>();

		}

		private void tournamentsDetailView()
		{
			//Switch<UITournamentDetail>();

		}

		public void matchView()
		{
			Close();
			Switch<UIMultiplayerView>();
			//  Close();
			//multiplayView.BackView = this;

		}

		private void detailView()
		{
			//changeOptsView(false);
			//Switch<UITournamentDetail>();

		}

		private int levelByExp(float exp)
		{
			//return (int)Mathf.Round(0.3f * (exp*exp)) ;
			return (int)Mathf.Floor((0.03f * Mathf.Sqrt(exp)));
		}

		public async void lobbyMode()
		{
			// Lobby user restriction
			/*
			if (Context.PlayerData.userData == null)
			{
				var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Bloomverse Services";
				infoDialog.Description.text = "You need to sign in to access this mode";
				return;
			}

			var userLevel = levelByExp((float)Context.PlayerData.userData.userExp);
			if (userLevel < 0)
			{
				var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Bloomverse Services";
				infoDialog.Description.text = "You need to be level 10 to access this mode";
				return;
			}

			*/

			_matchmakerView = Open<UIMatchmakerView>();
			await Context.Matchmaker.StartMatchmaker("lobby");
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


		protected override void OnOpen()
		{
			base.OnOpen();

			//Global.PlayerService.PlayerDataChanged += OnPlayerDataChanged;
			Context.Matchmaker.MatchFound += OnMatchFound;
			Context.Matchmaker.MatchmakerFailed += OnMatchmakerFailed;
		}

		protected override void OnClose()
		{
			//Global.PlayerService.PlayerDataChanged -= OnPlayerDataChanged;

			Context.Matchmaker.MatchFound -= OnMatchFound;
			Context.Matchmaker.MatchmakerFailed -= OnMatchmakerFailed;
			//  Open<UIMainMenuView>();
			base.OnClose();
			//Context.PlayerPreview._animator.SetBool("Aim",false);
		}

		protected override bool OnBackAction()
		{
			if (IsInteractable == false)
				return false;



			//   Open<UIMainMenuView>();

			base.OnBackAction();

			return true;
		}

		// PRIVATE METHODS

		private void OnSettingsButton()
		{
			Open<UISettingsView>();
		}

		private void OnPlayButton()
		{
			//Open<UIMultiplayerView>();
			//changeOptsView(true);
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



	}
}
