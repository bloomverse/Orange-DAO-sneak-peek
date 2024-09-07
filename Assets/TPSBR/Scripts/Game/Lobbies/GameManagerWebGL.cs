using System;
using System.Collections;
using System.Collections.Generic;
using Newtonsoft.Json;
using Fusion.Photon.Realtime;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker.Models;
using Unity.Services.Matchmaker;
using UnityEngine;
using static Unity.Services.Matchmaker.Models.MultiplayAssignment;
using UnityEngine.Networking;

namespace TPSBR
{
    public class GameManagerWebGL : ContextBehaviour
    {

        string server_url =Constants.server_backend;
        //ENUMS
        //EGameplayType _gameplayType;

        //CALLBACKS
        //LobbyEventCallbacks m_LobbyEventCallbacks = new LobbyEventCallbacks();

        //VARS
        private LobbyManager LobbyManager;
        public List<Lobby> Lobbies { get; set; }
        public Lobby Lobby { get; set; }
        public MultiplayAssignment Assignment = null;
        internal bool isFetching = false;
        public bool gotAssignment = false;

        //INST
        static GameManagerWebGL m_GameManagerInstance;

        //EVENTS
        public event EventHandler<List<Lobby>> LobbiesListUpdated;
        public event EventHandler<Lobby> LobbyCreated;
        public event EventHandler<Lobby> LobbyJoined;
        public event EventHandler<Lobby> LobbyLeaved;
        public event EventHandler<Lobby>  LobbyDetailTournament;
        public event EventHandler<Lobby> LobbyDetailMatchmaker;
        public event EventHandler<Lobby> PlayerDataCahnged;
        public event EventHandler<Lobby> PlayerLeft;
        public event EventHandler<Lobby> LobbyDataCahnged;
        public event EventHandler<MultiplayAssignment> TicketStatus;
        public event EventHandler<string> LobbyInsideAlready;

        public TournamentMatches currentTournamentMatch;
        public TournamentRounds currentTournamentRound;

        protected virtual void OnLobbiesListUpdated()
        {
            LobbiesListUpdated?.Invoke(this, Lobbies);
        }

        protected virtual void OnLobbyCreated()
        {
            LobbyCreated?.Invoke(this, Lobby);
        }

        protected virtual void OnLobbyDetailTounament()
        {
            LobbyDetailTournament?.Invoke(this,Lobby);
        }
        protected virtual void OnLobbyDetailMatchMaker()
        {
            LobbyDetailMatchmaker?.Invoke(this,Lobby);
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

        // SINGLENTON
        public static GameManagerWebGL Instance
        {
            get
            {
                if (m_GameManagerInstance != null)
                    return m_GameManagerInstance;
                m_GameManagerInstance = FindObjectOfType<GameManagerWebGL>();
                return m_GameManagerInstance;
            }
        }


        protected virtual void OnTicketStatus()
        {
            //TicketStatus?.Invoke(this, Assignment);
             Dictionary<string, Dictionary<string, string>> data2 = new Dictionary<string, Dictionary<string, string>>();
            data2.Add("State", new Dictionary<string, string> { { "value", "Launching" }, { "visibility", "public" } });
            data2.Add("Map", new Dictionary<string, string> { { "value", Lobby.Data["Map"].Value }, { "visibility", "public" } });
            data2.Add("GameplayType", new Dictionary<string, string> { { "value", Lobby.Data["GameplayType"].Value }, { "visibility", "public" } });
            data2.Add("MatchId", new Dictionary<string, string> { { "value", Assignment.MatchId }, { "visibility", "public" } });
            //data2.Add("Ticket", new Dictionary<string, string> { { "value", ticket }, { "visibility", "member" } });
             StartCoroutine(UpdateLobbyData(Lobby.Id, data2, ""));   
        }

        public void QueryLobbies()
        {
            StartCoroutine(GetLobbies());
        }
        public void QueryLobbyTournament(string LobbyID)
        {
            StartCoroutine(GetLobby(LobbyID,"Tournament"));
        }
        public void QueryLobbyMatchmaker(string LobbyID)
        {
            StartCoroutine(GetLobby(LobbyID,"Matchmaker"));
        }

        IEnumerator GetLobby(string LobbyID, string gameType)
        {
            isFetching = true;
            string url = string.Format("{0}/lobbies/lobbyInfo/" + LobbyID, server_url);
            UnityWebRequest request = UnityWebRequest.Get(url);
       //     Debug.Log("Lobby query url: " + url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(request.downloadHandler.text);

                //QueryResponse data = JsonConvert.DeserializeObject<QueryResponse>(request.downloadHandler.text);

                //Lobbies = Converters.QueryToLobbies(data);

                //Debug.Log("Lobbies count: " + Lobbies.Count);
                Lobby = JsonConvert.DeserializeObject<Lobby>(request.downloadHandler.text);
                isFetching = false;
                if(gameType=="Tournament"){
                    OnLobbyDetailTounament();
                }
                if(gameType=="Matchmaker"){
                    OnLobbyDetailMatchMaker();
                }
                
                request.Dispose();
            }
            else
            {
                Debug.LogError(request.error);
                Lobby = null;
               if(gameType=="Tournament"){
                    OnLobbyDetailTounament();
                }
                if(gameType=="Matchmaker"){
                    OnLobbyDetailMatchMaker();
                }
            }
        }



        public void CreateLobby(string _name, PlayerData player, Dictionary<string, string> data, bool isPrivate = false, int maxPlayers = 8)
        {
            Debug.Log("Lobby ask created: " + _name +  " - " + data);
            StartCoroutine(CreateNewLobby(_name, player, data, isPrivate, maxPlayers));
        }

       public void JoinLobby(Lobby lobby, PlayerData player)
        {
            //Debug.Log("Joining lobby: " + lobby.Name + " -- " + lobby.Data["MatchId"].Value );
            StartCoroutine(JoinToLobbyId(lobby.Id, player));
        }

        public void JoinLobbySID(string lobbyId, PlayerData player)
        {
            StartCoroutine(JoinToLobbyId(lobbyId, player));
        }

        public void JoinPrivateLobby(string code, PlayerData player)
        {
            Debug.Log("Joining private lobby: " + code);
            StartCoroutine(JoinToLobbyCode(code, player));
        }

        public void ReConnectLobby(string lobbyId, PlayerData player)
        {
            Debug.Log("Reconnecting Lobby: " + lobbyId);
            StartCoroutine(ReconnectLobby(lobbyId,player));
        }

        public void LeaveLobby(PlayerData player)
        {
            Debug.Log("Levaing lobby: " + Lobby.Id);
            StartCoroutine(ToLeaveLobby(Lobby.Id, player.UnityID));
        }

        public void  JoinedLobbies(string playerId)
        {
            StartCoroutine(checkJoinedIE(playerId));
        }
         IEnumerator checkJoinedIE(string playerId)
        {
                string url = string.Format("{0}/lobbies/joined/{1}/", server_url, playerId);
                UnityWebRequest request = new UnityWebRequest(url, "GET");
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();
                if (request.result == UnityWebRequest.Result.Success){
                    Debug.Log("i am on lobby " + request.downloadHandler.text);
                    LobbyInsideAlready?.Invoke(this,request.downloadHandler.text.ToString());
                    request.Dispose();
                    
                }
                else{
                    Debug.LogError(request.error);
                    request.Dispose();
                }
        }


        public void Heartbeat(string playerId)
        {
            
            StartCoroutine(HeartbeatCR(playerId));
        }

         IEnumerator HeartbeatCR(string playerId)
        {
            while(Lobby.Data["State"].Value=="Lobby"){
                 string url = string.Format("{0}/lobbies/heartbeat/{1}/{2}", server_url, Lobby.Id, playerId);
                UnityWebRequest request = new UnityWebRequest(url, "POST");
///                Debug.Log("heartbeat... url: " + url);

                //request.uploadHandler = (UploadHandler)new UploadHandlerRaw({user_id});
                request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
                request.SetRequestHeader("Content-Type", "application/json");
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    request.Dispose();
                }
                else
                {
                    Debug.LogError(request.error);
                    request.Dispose();
                }

                yield return new WaitForSeconds(10);

            }
           

            
        }

        public void SetPlayerStatus(PlayerData player, bool status)
        {
//            Debug.Log("Lobby player change status");
            string text_status = "Waiting";

            if (status)
            {
                text_status = "Ready";
            }

            Dictionary<string, Dictionary<string, string>> data = new Dictionary<string, Dictionary<string, string>>();
            data.Add("Status", new Dictionary<string, string> { { "value", text_status }, { "visibility", "public" } });

            StartCoroutine(UpdatePlayerData(Lobby.Id, player.UnityID, data));
        }

      

        IEnumerator UpdatePlayerData(string lobby_id, string player_id, Dictionary<string, Dictionary<string, string>> data)
        {
            string json = JsonConvert.SerializeObject(data);
//            Debug.Log("Lobby player data json: " + json);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

            string url = string.Format("{0}/lobbies/update/{1}/player/{2}", server_url, lobby_id, player_id);
            UnityWebRequest request = new UnityWebRequest(url, "POST");
//            Debug.Log("Lobby update player data url: " + url);

            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
              //  Debug.Log(request.downloadHandler.text);

                Lobby = JsonConvert.DeserializeObject<Lobby>(request.downloadHandler.text);

              //  Debug.Log("Lobby player updated: " + Lobby.Name);

                OnPlayerDataCahnged();
                request.Dispose();
            }
            else
            {
                Debug.LogError(request.error);
            }

            request.Dispose();
            //GameManagerWebGL.Instance.JoinedLobbies(Context.PlayerData.UnityID);
        }

        IEnumerator ToLeaveLobby(string lobby_code, string player_id)
        {
            string url = string.Format("{0}/lobbies/leave/{1}/player/{2}", server_url, lobby_code, player_id);
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            Debug.Log("Lobby leave url: " + url);

            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);

                Lobby = null;

                Debug.Log("Player leaved lobby");

                OnLobbyLeaved();
                request.Dispose();
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        IEnumerator JoinToLobbyCode(string lobby_code, PlayerData player)
        {
            LobbyPlayer lobby_player = new LobbyPlayer();
            lobby_player.id = player.UnityID;

            lobby_player.data = new Dictionary<string, Dictionary<string, string>>();
            lobby_player.data.Add("Nickname", new Dictionary<string, string> { { "value", player.Nickname }, { "visibility", "public" } });
            lobby_player.data.Add("Status", new Dictionary<string, string> { { "value", "Waiting" }, { "visibility", "public" } });
            lobby_player.data.Add("Skin", new Dictionary<string, string> { { "value", player.SelectedSkin }, { "visibility", "public" } });
            lobby_player.data.Add("Agent", new Dictionary<string, string> { { "value", player.AgentID }, { "visibility", "public" } });

            string json = JsonConvert.SerializeObject(lobby_player);
            Debug.Log("Player json: " + json);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

            string url = string.Format("{0}/lobbies/join?code={1}&user={2}", server_url, lobby_code, player.UnityID);
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            Debug.Log("Lobby join url: " + url);

            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);

                Lobby = JsonConvert.DeserializeObject<Lobby>(request.downloadHandler.text);
                PhotonAppSettings.Global.AppSettings.FixedRegion = Lobby.Data["Region"].Value;
                Debug.Log("Player joined: " + Lobby.Name);

                OnLobbyJoined();
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        IEnumerator JoinToLobbyId(string lobby_id, PlayerData player)
        {
            LobbyPlayer lobby_player = new LobbyPlayer();
            lobby_player.id = player.UnityID;

            lobby_player.data = new Dictionary<string, Dictionary<string, string>>();
            lobby_player.data.Add("Nickname", new Dictionary<string, string> { { "value", player.Nickname }, { "visibility", "public" } });
            lobby_player.data.Add("Status", new Dictionary<string, string> { { "value", "Waiting" }, { "visibility", "public" } });
            lobby_player.data.Add("Skin", new Dictionary<string, string> { { "value", player.SelectedSkin }, { "visibility", "public" } });
            lobby_player.data.Add("Agent", new Dictionary<string, string> { { "value", player.AgentID }, { "visibility", "public" } });

            string json = JsonConvert.SerializeObject(lobby_player);
            Debug.Log("Player json: " + json);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

            string url = string.Format("{0}/lobbies/join/{1}?user={2}", server_url, lobby_id, player.UnityID);
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            Debug.Log("Lobby join url: " + url);

            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);

                Lobby = JsonConvert.DeserializeObject<Lobby>(request.downloadHandler.text);
                 PhotonAppSettings.Global.AppSettings.FixedRegion = Lobby.Data["Region"].Value;
                Debug.Log("Player joined: " + Lobby.Name);

                OnLobbyJoined();
                request.Dispose();
            }
            else
            {

                Debug.LogError(request.error + " Trying to reconnect");
                StartCoroutine(ReconnectLobby(lobby_id,player));
            }
        }

        IEnumerator CreateNewLobby(string _Name, PlayerData player, Dictionary<string, string> data, bool isPrivate, int maxPlayers)
        {
            LobbyData lobby = new LobbyData();
            lobby.Name = _Name;
            lobby.isPrivate = isPrivate;
            lobby.maxPlayers = maxPlayers;

            lobby.data = new Dictionary<string, Dictionary<string, string>>();
            lobby.data.Add("Region", new Dictionary<string, string> { { "value", data["Region"] }, { "visibility", "public" } });
            lobby.data.Add("Map", new Dictionary<string, string> { { "value", data["Map"] }, { "visibility", "public" } });
            lobby.data.Add("GameplayType", new Dictionary<string, string> { { "value", data["GameplayType"] }, { "visibility", "public" } });
            lobby.data.Add("State", new Dictionary<string, string> { { "value", "Lobby" }, { "visibility", "public" } });
            lobby.data.Add("Ticket", new Dictionary<string, string> { { "value", null }, { "visibility", "member" } });

            

            lobby.player = new LobbyPlayer();
            lobby.player.id = player.UnityID;

            Debug.Log("Player selected Skin on lobby creation " + player.SelectedSkin);

            lobby.player.data = new Dictionary<string, Dictionary<string, string>>();
            lobby.player.data.Add("Nickname", new Dictionary<string, string> { { "value", player.Nickname }, { "visibility", "public" } });
            lobby.player.data.Add("Status", new Dictionary<string, string> { { "value", "Waiting" }, { "visibility", "public" } });
            lobby.player.data.Add("Skin", new Dictionary<string, string> { { "value", player.SelectedSkin }, { "visibility", "public" } });
            lobby.player.data.Add("Agent", new Dictionary<string, string> { { "value", player.AgentID }, { "visibility", "public" } });

            string json = JsonConvert.SerializeObject(lobby);
            Debug.Log("Lobby json: " + json);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

            string url = string.Format("{0}/lobbies/create?user={1}", server_url, player.UnityID);
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            Debug.Log("Lobby create url: " + url);

            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(request.downloadHandler.text);

                Lobby = JsonConvert.DeserializeObject<Lobby>(request.downloadHandler.text);
                PhotonAppSettings.Global.AppSettings.FixedRegion = Lobby.Data["Region"].Value;

                Debug.Log("Lobby created: " + Lobby.Name);
                Heartbeat(player.UnityID);
                OnLobbyCreated();
                request.Dispose();
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        IEnumerator GetLobbies()
        {

            isFetching = true;
            string url = string.Format("{0}/lobbies/query", server_url);
            UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "");
//            Debug.Log("Lobby query url: " + url);

            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(request.downloadHandler.text);

                QueryResponse data = JsonConvert.DeserializeObject<QueryResponse>(request.downloadHandler.text);

                Lobbies = Converters.QueryToLobbies(data);

//                Debug.Log("Lobbies count: " + Lobbies.Count);
                isFetching = false;
                OnLobbiesListUpdated();
                request.Dispose();
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        IEnumerator ReconnectLobby(string lobby_id, PlayerData player)
        {
            Debug.Log(lobby_id + " " );
            string url = string.Format("{0}/lobbies/{1}/reconnect?user={2}", server_url,lobby_id,player.UnityID);
            Debug.Log(url + " Url query");
            UnityWebRequest request = UnityWebRequest.PostWwwForm(url, "POST");
            
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                //Debug.Log(request.downloadHandler.text);
                Lobby = JsonConvert.DeserializeObject<Lobby>(request.downloadHandler.text);
                OnLobbyJoined();
                request.Dispose();
            }
            else
            {
                Debug.LogError(request.error);
            }
        }

        

      

        void Awake()
        {
            LobbyManager = new LobbyManager();
        }

        IEnumerator UpdateLobbyData(string lobby_id, Dictionary<string, Dictionary<string, string>> data,string UnityID)
        {
            string json = JsonConvert.SerializeObject(data);
            Debug.Log("Lobby data json: " + json);
            byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);

            string url = string.Format("{0}/lobbies/update/{1}/?user={2}", server_url, lobby_id, UnityID);
            UnityWebRequest request = new UnityWebRequest(url, "POST");
            Debug.Log("Lobby update data url: " + url);

            request.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("Content-Type", "application/json");
            yield return request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                Debug.Log(request.downloadHandler.text);

                Lobby = JsonConvert.DeserializeObject<Lobby>(request.downloadHandler.text);

                Debug.Log("Lobby updated: " + Lobby.Name);

                OnLobbyDataChanged();
                request.Dispose();
               
            }
            else
            {
                Debug.LogError(request.error);
            }

            
        }

        public async void StartGame(string UnityID)
        {
            Debug.Log("Lobby Match start game.....");
           // Dictionary<string, string> data = new Dictionary<string, string>();
           // data["State"] = "Start";
           /* if(Lobby.Data["Map"] != null)
            {
                data["Map"] = Lobby.Data["Map"].Value;
            }
            if (Lobby.Data["GameplayType"] != null)
            {
                data["GameplayType"] = Lobby.Data["GameplayType"].Value;
            }
             
            data["Ticket"] = ticket;*/
            Debug.Log("Region " + Lobby.Data["Region"].Value + " -- " + PhotonAppSettings.Global.AppSettings.FixedRegion);
            PhotonAppSettings.Global.AppSettings.FixedRegion = Lobby.Data["Region"].Value;

            Debug.Log("Creating matchmaking with players..." + Lobby.Players.Count);

            string ticket = await LobbyManager.CreateTicketAsync(Lobby);

            Dictionary<string, Dictionary<string, string>> data2 = new Dictionary<string, Dictionary<string, string>>();
            data2.Add("State", new Dictionary<string, string> { { "value", "Start" }, { "visibility", "public" } });
            data2.Add("Map", new Dictionary<string, string> { { "value", Lobby.Data["Map"].Value }, { "visibility", "public" } });
            data2.Add("GameplayType", new Dictionary<string, string> { { "value", Lobby.Data["GameplayType"].Value }, { "visibility", "public" } });
            data2.Add("Ticket", new Dictionary<string, string> { { "value", ticket }, { "visibility", "member" } });
            

             StartCoroutine(UpdateLobbyData(Lobby.Id, data2, UnityID));        
             StartCoroutine(TicketTick(ticket)) ;
        }


       /* public void startTicketCheck(string ticket){
            Debug.Log("Starting checkticket multiplaya");
            StartCoroutine(TicketTick(ticket)) ;
        }*/


         IEnumerator TicketTick(string ticket){
            while(!gotAssignment){
                Debug.Log("passing Ienumerator");
                CheckTicketStatus(ticket);

                yield return new WaitForSeconds(2);
            }
            
        }

        public async void CheckTicketStatus(string ticket)
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
