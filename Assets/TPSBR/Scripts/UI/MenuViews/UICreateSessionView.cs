using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using TMPro;
using Fusion;
using Unity.Services.Lobbies.Models;

namespace TPSBR.UI
{
	public class UICreateSessionView : UICloseView
	{
		// PROTECTED MEMBERS

		[SerializeField]
		private UIMapList _maps;
		[SerializeField]
		private UIMap _mapDetail;
		[SerializeField]
		private TMP_InputField _gameName;
		[SerializeField]
        private int _maxCharacters = 20;
		[SerializeField]
		private TMP_Dropdown _gameplay;
		[SerializeField]
		private TMP_InputField _maxPlayers;

		private int maxDefault = 8;

		[SerializeField]
		private UIToggle _isPrivate;
        //[SerializeField]
        //private GameObject _dedicatedServerWarning;
        [SerializeField]
        private TMP_Dropdown _serverRegion;
        [SerializeField]
		private UIButton _createButton;
		[SerializeField] private TextMeshProUGUI _errorText;
		private UIMatchmakerView _matchmakerView;
		[SerializeField]
		private GameManagerWebGL GameManager;

		private List<MapSetup> _mapSetups = new List<MapSetup>(8);

		// PRIVATE MEMBERS

		private bool _uiPrepared;

		// UIView INTERFACE

		private bool _isPreparing; 

		protected override void OnInitialize()
		{
			base.OnInitialize();

			_maps.UpdateContent += OnUpdateMapContent;
			_maps.SelectionChanged += OnMapSelectionChanged;

			_createButton.onClick.AddListener(OnCreateButton);

			PrepareMapData();
		}

		protected override void OnDeinitialize()
		{
			_maps.UpdateContent -= OnUpdateMapContent;
			_maps.SelectionChanged += OnMapSelectionChanged;

			_createButton.onClick.RemoveListener(OnCreateButton);

			base.OnDeinitialize();
		}

		protected override void OnOpen()
		{
			base.OnOpen();

            GameManager.LobbyCreated += OnLobbyCreated;

            if (_uiPrepared == false)
			{
				UpdateDropdowns();
				UpdateDropDownRegion();

                _maps.Refresh(_mapSetups.Count);
				_maps.Selection = 0;

				OnMapSelectionChanged(0);

				if (_gameName.text.Length < 5)
				{
					_gameName.text = $"{Context.PlayerData.Nickname}'s Arena";
				}

				_isPrivate.SetIsOnWithoutNotify(false);

				_uiPrepared = true;
			}
		}

        protected override void OnClose()
		{
            GameManager.LobbyCreated -= OnLobbyCreated;
        }


        protected override void OnTick()
		{
			base.OnTick();

			_createButton.interactable = CanCreateGame();
			//_dedicatedServerWarning.SetActive(_dedicatedServer.isOn);
		}

		// PRIVATE METHODS

		private void OnCreateButton()
		{
			_isPreparing = true;
			// 			var request = new SessionRequest
			// 			{
			// 				DisplayName  = _gameName.text,
			// 				GameMode     = _dedicatedServer.isOn == true ? GameMode.Server : GameMode.Host,
			// 				GameplayType = (EGameplayType) (_gameplay.value + 1),
			// 				MaxPlayers   = System.Int32.Parse(_maxPlayers.text),
			// 				ScenePath    = _mapSetups[_maps.Selection].ScenePath,
			// 			};
			// ;
			// 			Context.Matchmaking.CreateSession(request);
			//if (Context.PlayerData.UnityID.HasValue() == true)
			//{
			//	_errorText.text = string.Empty;
			//	_matchmakerView = Open<UIMatchmakerView>();
			//	await Context.Matchmaker.StartMatchmaker(Global.Settings.Network.GetCustomOrDefaultQueueName());
			//}
			//else
			//{
			//	var infoDialog = Open<UIInfoDialogView>();

			//	infoDialog.Title.text = "Unity Gaming Services";
			//	//infoDialog.Description.text = "For matchmaking functionality Unity Gaming Services need to be configured.\n\nPlease follow instructions in the Fusion BR documentation on how to add Multiplay support.";
			//	infoDialog.Description.text = "Matchmaking functionality need to be configured";
			//}
			string name = _gameName.text;
			//string map = _mapSetups[_maps.Selection].ID;
			bool isPrivate = _isPrivate.isOn;
            //int gameplayType = (_gameplay.value + 1);
			int maxPlayers =  maxDefault; //nt.Parse( _maxPlayers.text);
			PlayerData player = Context.PlayerData;

			Debug.Log(player.AgentID + " Agent ID before creatin lobby");

            Dictionary<string, string> data = new Dictionary<string, string>();
			data["Map"] = _mapSetups[_maps.Selection].ID;
            data["Region"] = Global.Settings.Network.Regions[_serverRegion.value].Region;
            data["GameplayType"] = "3"; //(_gameplay.value + 1).ToString();

			GameManager.CreateLobby(name, player, data, isPrivate, maxPlayers);
		}

        private void OnLobbyCreated(object seder, Lobby lobby)
		{
			//  Close();
			//testing
			_isPreparing = false;
			Switch<UILobbyView>();
        }


        private bool CanCreateGame()
		{
			//if (Context.Matchmaking.Connecting == true)
			//	return false;

			if (_maps.Selection < 0)
				return false;

			var mapSetup = _mapSetups[_maps.Selection];

			if (mapSetup == null)
				return false;

			if (System.Int32.TryParse(_maxPlayers.text, out int maxPlayers) == false)
				return false;

			if (maxPlayers < 2 || maxPlayers > mapSetup.MaxPlayers)
				return false;

			if (_gameName.text.Length < 5)
				return false;
				            
			if(_gameName.text.Length > _maxCharacters)
                return false;

			if (_isPreparing == true)
				return false;

			return true;
		}

		private void UpdateDropdowns()
		{
			var options = ListPool.Get<string>(16);

			int defaultOption = 0;
			/*
			int i = 0;
			foreach (EGameplayType value in System.Enum.GetValues(typeof(EGameplayType)))
			{
				if (value == EGameplayType.None)
					continue;

				if (value == EGameplayType.BattleRoyale)
				{
					options.Add("Battle Royale");
				}
				else
				{
					options.Add(value.ToString());
				}

				if (value == EGameplayType.Deathmatch)
				{
					defaultOption = i;
				}

				i++;
			}*/

			options.Add("Elimination");

			_gameplay.ClearOptions();
			_gameplay.AddOptions(options);
			_gameplay.SetValueWithoutNotify(defaultOption);

			ListPool.Return(options);
		}

        private void UpdateDropDownRegion()
		{
            var options = ListPool.Get<string>(16);

            int defaultOption = 0;
            //int i = 0;
            foreach (RegionInfo region in Global.Settings.Network.Regions)
			{
				options.Add(region.DisplayName);
			}

            _serverRegion.ClearOptions();
            _serverRegion.AddOptions(options);
            _serverRegion.SetValueWithoutNotify(defaultOption);

            ListPool.Return(options);
        }

        private void OnMapSelectionChanged(int index)
		{
			if (index >= 0)
			{
				var mapSetup = _mapSetups[index];

				_mapDetail.SetData(mapSetup);
				_mapDetail.SetActive(true);

				_maxPlayers.text = mapSetup.RecommendedPlayers.ToString();
			}
			else
			{
				_mapDetail.SetActive(false);
			}
		}

		private void OnUpdateMapContent(int index, UIMap content)
		{
			content.SetData(_mapSetups[index]);
		}

		private void PrepareMapData()
		{
			_mapSetups.Clear();

			var allMapSetups = Context.Settings.Map.Maps;

			for (int i = 0; i < allMapSetups.Length; i++)
			{
				var mapSetup = allMapSetups[i];

				if (mapSetup.ShowInMapSelection == true)
				{
					_mapSetups.Add(mapSetup);
				}
			}
		}
	}
}
