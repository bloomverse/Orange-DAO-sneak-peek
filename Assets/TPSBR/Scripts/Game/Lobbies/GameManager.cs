using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TPSBR.UI;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using static Unity.Services.Matchmaker.Models.MultiplayAssignment;
using System.Threading.Tasks;
using UnityEngine.Networking;
using System.Net;

namespace TPSBR
{
    public class GameManager : MonoBehaviour
    {
        //ENUMS
        EGameplayType _gameplayType;

        //CONTEXT
        //protected SceneUI SceneUI { get; private set; }
        //protected SceneContext Context { get { return SceneUI.Context; } }

        //CALLBACKS
        LobbyEventCallbacks m_LobbyEventCallbacks = new LobbyEventCallbacks();

        //VARS
        private LobbyManager LobbyManager;
        public List<Lobby> Lobbies { get; set; }
        public Lobby Lobby { get; set; }
        public MultiplayAssignment Assignment = null;
        internal bool isFetching = false;

        //INST
        static GameManager m_GameManagerInstance;

        //EVENTS
        public event EventHandler<List<Lobby>> LobbiesListUpdated;
        public event EventHandler<Lobby> LobbyCreated;
        public event EventHandler<Lobby> LobbyJoined;
        public event EventHandler<Lobby> LobbyLeaved;
        public event EventHandler<Lobby> PlayerDataCahnged;
        public event EventHandler<Lobby> PlayerLeft;
        public event EventHandler<Lobby> LobbyDataCahnged;
        public event EventHandler<MultiplayAssignment> TicketStatus;

        protected virtual void OnLobbiesListUpdated()
        {
            LobbiesListUpdated?.Invoke(this, Lobbies);
        }

        protected virtual void OnLobbyCreated()
        {
            LobbyCreated?.Invoke(this, Lobby);
        }

        protected virtual void OnLobbyJoined()
        {
            LobbyJoined?.Invoke(this, Lobby);
        }

        protected virtual void OnLobbyLeaved()
        {
            LobbyLeaved?.Invoke(this, Lobby);
        }

        protected virtual void OnPlayerDataCahnged()
        {
            PlayerDataCahnged?.Invoke(this, Lobby);
        }

        protected virtual void OnPlayerLeft()
        {
            PlayerLeft?.Invoke(this, Lobby);
        }

        protected virtual void OnLobbyDataChanged()
        {
            LobbyDataCahnged?.Invoke(this, Lobby);
        }

        protected virtual void OnTicketStatus()
        {
            TicketStatus?.Invoke(this, Assignment);
        }

        public static GameManager Instance
        {
            get
            {
                if (m_GameManagerInstance != null)
                    return m_GameManagerInstance;
                m_GameManagerInstance = FindObjectOfType<GameManager>();
                return m_GameManagerInstance;
            }
        }

        public virtual async void QueryLobbies()
        {
            Debug.Log("Lobby Quering");

            isFetching = true;
            var qr = await LobbyManager.RetrieveLobbyListAsync(_gameplayType);
            isFetching = false;

            if (qr == null)
            {
                return;
            }

            Debug.Log("Lobby service: " + qr);

            Lobbies = Converters.QueryToLobbies(qr);

            OnLobbiesListUpdated();
        }

        public virtual async void CreateLobby(string name, PlayerData player, Dictionary<string, string> data, bool isPrivate = false, int maxPlayers = 8)
        {
            Debug.Log("Lobby ask created: " + name);

            try
            {
                Debug.Log("Lobby creator: " + player.Nickname);

                Lobby = await LobbyManager.CreateLobbyAsync(
                    name,
                    data,
                    isPrivate,
                    maxPlayers,
                    player
                    );
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error creating lobby : {exception} ");
            }

            if (Lobby != null)
            {
                Debug.Log("Lobby created: " + Lobby.Name);

                OnLobbyCreated();
                OnLobbyJoined();
                SuscribeEvents();
            }
        }

        public virtual async void JoinLobby(Lobby lobby, PlayerData player)
        {
            try
            {
                Lobby = await LobbyManager.JoinLobbyAsync(lobby, player);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error joining lobby : {exception} ");
            }

            if (Lobby != null)
            {
                Debug.Log("Lobby joined: " + Lobby.Name);

                OnLobbyJoined();
                SuscribeEvents();
            }
        }

        public virtual async void JoinPrivateLobby(string code, PlayerData player)
        {
            try
            {
                Lobby = await LobbyManager.JoinPrivateLobbyAsync(code, player);
            }
            catch (Exception exception)
            {
                Debug.LogError($"Error joining lobby : {exception} ");
            }

            if (Lobby != null)
            {
                Debug.Log("Lobby joined: " + Lobby.Name);

                OnLobbyJoined();
                SuscribeEvents();
            }
        }

        void Awake()
        {
            Debug.Log("Lobby awake");
            //if(Application.platform == RuntimePlatform.WebGLPlayer)
            //{
            //    LobbyManager = new LobbyManagerWebGL();
            //}
            //else
            //{
            LobbyManager = new LobbyManager();
            //}
        }

        public virtual async void LeaveLobby(PlayerData player)
        {
            await LobbyManager.LeaveLobbyAsync(Lobby);

            Lobby = null;

            Debug.Log("Lobby leaved");

            m_LobbyEventCallbacks = new LobbyEventCallbacks();

            OnLobbyLeaved();
        }

        private async void SuscribeEvents()
        {
            m_LobbyEventCallbacks.LobbyChanged += async changes =>
            {
                if (changes.Data.Changed)
                    LobbyChanged();

                if (changes.PlayerData.Changed)
                    PlayerDataChanged();

                if (changes.PlayerLeft.Changed)
                    PlayersLeft();

                void LobbyChanged()
                {
                    Debug.Log("Lobby data changed");

                    foreach (var change in changes.Data.Value)
                    {
                        var changedValue = change.Value;
                        var changedKey = change.Key;

                        //if (changedValue.Removed)
                        //{
                        //    RemoveCustomLobbyData(changedKey);
                        //}

                        if (changedValue.Changed)
                        {
                            //ParseCustomLobbyData(changedKey, changedValue.Value);
                            //KeyValuePair<string, string> data = new KeyValuePair<string, string>("state", changedValue.Value.Value);
                            //DataObject dataObj = new DataObject(DataObject.VisibilityOptions.Public, data.Value);
                            Lobby.Data[changedKey] = new DataObject(
                                visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
                                value: changedValue.Value.Value
                            );

                            OnLobbyDataChanged();
                        }
                    }

                    //void RemoveCustomLobbyData(string changedKey)
                    //{
                    //    if (changedKey == key_RelayCode)
                    //        localLobby.RelayCode.Value = "";
                    //}

                    //void ParseCustomLobbyData(string changedKey, DataObject playerDataObject)
                    //{
                    //    if (changedKey == "state")
                    //        Lobby.Data["state"].Value = playerDataObject.Value
                    //        localLobby.RelayCode.Value = playerDataObject.Value;

                    //    if (changedKey == key_LobbyState)
                    //        localLobby.LocalLobbyState.Value = (LobbyState)int.Parse(playerDataObject.Value);

                    //    if (changedKey == key_LobbyColor)
                    //        localLobby.LocalLobbyColor.Value = (LobbyColor)int.Parse(playerDataObject.Value);
                    //}
                }

                void PlayerDataChanged()
                {
                    Debug.Log("Lobby player data changed");

                    foreach (var lobbyPlayerChanges in changes.PlayerData.Value)
                    {
                        var playerIndex = lobbyPlayerChanges.Key;
                        var playerChanges = lobbyPlayerChanges.Value;

                        //Debug.Log("Lobby player index: " + playerIndex);

                        if (playerChanges.ChangedData.Changed)
                        {
                            foreach (var playerChange in playerChanges.ChangedData.Value)
                            {
                                var changedValue = playerChange.Value;

                                if (changedValue.Changed)
                                {
                                    Lobby.Players[playerIndex].Data[playerChange.Key] = changedValue.Value;

                                    OnPlayerDataCahnged();
                                }
                            }
                        }
                    }
                }

                void PlayersLeft()
                {
                    foreach (var leftPlayerIndex in changes.PlayerLeft.Value)
                    {
                        Lobby.Players.RemoveAt(leftPlayerIndex);
                    }

                    OnPlayerLeft();
                }
            };

                m_LobbyEventCallbacks.LobbyEventConnectionStateChanged += lobbyEventConnectionState =>
            {
                Debug.Log($"Lobby ConnectionState Changed to {lobbyEventConnectionState}");
            };

            m_LobbyEventCallbacks.KickedFromLobby += () =>
            {
                Debug.Log("Left Lobby");

                Lobby = null;
            };

            await LobbyManager.SubscribeEventsAsync(Lobby, m_LobbyEventCallbacks);
        }

        virtual public async void SetPlayerStatus(PlayerData player, bool status)
        {
            Debug.Log("Lobby player change status");

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["Nickname"] = player.Nickname;

            string text_status = "Waiting";

            if (status)
            {
                text_status = "Ready";
            }

            data["Status"] = text_status;

            await LobbyManager.UpdatePlayerDataAsync(Lobby, data);
        }

        virtual public async void StartGame(string UnityID)
        {
            Debug.Log("Starting game");

            Dictionary<string, string> data = new Dictionary<string, string>();
            data["State"] = "Start";
            if(Lobby.Data["Map"] != null)
            {
                data["Map"] = Lobby.Data["Map"].Value;
            }
            if (Lobby.Data["GameplayType"] != null)
            {
                data["GameplayType"] = Lobby.Data["GameplayType"].Value;
            }

            string ticket = await LobbyManager.CreateTicketAsync(Lobby);

            data["Ticket"] = ticket;

            await LobbyManager.UpdateLobbyDataAsync(Lobby, data);
            gotAssignment = false;
            CheckTicketStatus(ticket); ;
        }

        public bool gotAssignment = false;

      
        virtual public void startTicketCheck(string ticket){
            
        }

        virtual public async void CheckTicketStatus(string ticket)
        {
           
                // Poll ticket
                var ticketStatus = await MatchmakerService.Instance.GetTicketAsync(ticket);
                if (ticketStatus == null)
                {
                    return;
                }

                //Convert to platform assignment data (IOneOf conversion)
                if (ticketStatus.Type == typeof(MultiplayAssignment))
                {
                    Assignment = ticketStatus.Value as MultiplayAssignment;
                }

                switch (Assignment.Status)
                {
                    case StatusOptions.Found:
                        gotAssignment = true;

                        OnTicketStatus();
                        break;
                    case StatusOptions.InProgress:
                        //...
                        break;
                    case StatusOptions.Failed:
                        gotAssignment = true;
                        Debug.LogError("Lobby failed to get ticket status. Error: " + Assignment.Message);
                        break;
                    case StatusOptions.Timeout:
                        gotAssignment = true;
                        Debug.LogError("Lobby failed to get ticket status. Ticket timed out.");
                        break;
                    default:
                        throw new InvalidOperationException();
                }

        
        }
    }
}
