
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using Unity.Services.Lobbies.Models;

namespace TPSBR.UI
{
    public class UILobby : UIBehaviour
	{
        // PRIVATE MEMBERS
        private MapSettings MapSettings;

        [SerializeField]
        private TextMeshProUGUI _name;
        [SerializeField]
        private TextMeshProUGUI _playerCount;
        [SerializeField]
        private TextMeshProUGUI _map;
        [SerializeField]
        private TextMeshProUGUI _region;
        [SerializeField]
        private Image _mapImage;
        [SerializeField]
        private TextMeshProUGUI _gameplayType;
        [SerializeField]
        private TextMeshProUGUI _state;
        [SerializeField]
        private TextMeshProUGUI _lobbyCode;
        [SerializeField]
        private TextMeshProUGUI _lobbyCodeCaption;
        [SerializeField]
        private string _emptyField = "-";

        // PUBLIC METHODS

        public void SetData(Lobby lobby)
        {
            if (lobby == null)
                return;

            int playerCount = lobby.Players.Count;
            int maxPlayers = lobby.MaxPlayers;

            _name.text = lobby.Name;
            _playerCount.text = $"{playerCount}/{maxPlayers}";

            if (_lobbyCode)
            {
                _lobbyCode.text = lobby.LobbyCode;
                if (!lobby.IsPrivate)
                {
                    _lobbyCode.SetActive(false);
                    _lobbyCodeCaption.SetActive(false);
                }
            }

            string type = lobby.Data["GameplayType"].Value;
//            Debug.Log("Lobby gameplay type: " + type);

            // GAMEPLAY TYPE IS AN ID 
            EGameplayType gameplayType = (EGameplayType)int.Parse(type);
            _gameplayType.text = gameplayType != EGameplayType.None ? gameplayType.ToString() : _emptyField;

            string map_id = lobby.Data["Map"].Value;
//            Debug.Log("Lobby map id: " + map_id );

          

            //var map = MapSettings.GetMapSetup(map_id);
            MapSetup map = Global.Settings.Map.GetMapSetup(map_id);
            _map.text = map.DisplayName;
            if (_mapImage)
            {
                _mapImage.sprite = map.Image;
            }

            if (_region)
            {
                if (lobby.Data["Region"] != null)
                {
                    _region.text = lobby.Data["Region"].Value;
                }
            }

            // We do not have lobby state for now
            _state.text = lobby.Data["State"].Value;//sessionInfo.IsOpen == true ? "In Game" : "Finished";
        }
    }
}
