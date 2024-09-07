using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;


namespace TPSBR.UI
{
    public class UIJoinPrivateView : UICloseView
    {
        [SerializeField]
        private TMP_InputField _lobbyCode;
        [SerializeField]
        private UIButton _joinButton;
        [SerializeField]
        private GameManagerWebGL GameManager;

        protected override void OnInitialize()
        {
            base.OnInitialize();

            _joinButton.onClick.AddListener(OnJoinButton);
        }

        protected override void OnDeinitialize()
        {

            _joinButton.onClick.RemoveListener(OnJoinButton);

            base.OnDeinitialize();
        }

        protected override void OnOpen()
        {
            base.OnOpen();

            GameManager.LobbyJoined += OnLobbyJoined;
        }

        private async void OnJoinButton()
        {
            string lobby_code = _lobbyCode.text;
            PlayerData player = Context.PlayerData;

            GameManager.JoinPrivateLobby(lobby_code, player);
        }

        private void OnLobbyJoined(object seder, Lobby lobby)
        {
            Close();
        }
    }
}
