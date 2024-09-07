using System;
using System.Collections;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using Newtonsoft.Json;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;

using UnityEngine;
using UnityEngine.Networking;
using Unity.Services.Authentication;
//using System;
using Unity.Services.Matchmaker;

namespace TPSBR
{
    public class LobbyManagerWebGL : IDisposable
    {
        ////API SERVER
         Lobby m_CurrentLobby;
        static readonly HttpClient client = new HttpClient();


    public bool InLobby()
        {
            if (m_CurrentLobby == null)
            {
                Debug.LogWarning("LobbyManager not currently in a lobby. Did you CreateLobbyAsync or JoinLobbyAsync?");
                return false;
            }

            return true;
        }

    public async Task<QueryResponse> RetrieveLobbyListAsync(EGameplayType gameplayType = EGameplayType.None)
    {
            Debug.Log("New lobby query");
            string url = String.Format("{0}/lobbies/query", Constants.server_backend);
            HttpContent filters = new StringContent("");
            Task<HttpResponseMessage> response = client.PostAsync(url, new StringContent(""));
            response.Wait();

            if (response.Result.IsSuccessStatusCode)
            {
                string body = await response.Result.Content.ReadAsStringAsync();
                Debug.Log("Lobby response body: " + body);

                QueryResponse data = JsonConvert.DeserializeObject<QueryResponse>(body);
                Debug.Log("Lobbies count: " + data.Results.Count);

                return data;
            }
            else
            {
                Debug.Log("No llego respuesta ");
                return null;
            }
                
    }
    

    public async Task<Lobby> CreateLobbyAsync(string name, Dictionary<string, string> data, bool isPrivate, int maxPlayers, PlayerData player)
    {
            Debug.Log("Creating lobby async" + player.AgentID + " - " + isPrivate);

           // if (m_CreateCooldown.IsCoolingDown)
           // {
           //     Debug.LogWarning("Create Lobby hit the rate limit.");
           //     return null;
          //  }

            //await m_CreateCooldown.QueueUntilCooldown();

            try
            {
                Debug.Log("Lobby building...");
                //string uasId = AuthenticationService.Instance.PlayerId;

                Dictionary<string, DataObject> lobby_data = new Dictionary<string, DataObject>();
                 lobby_data["Name"] = new DataObject(
                    visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
                    value: name
                //index: DataObject.IndexOptions.S1
                );
                 lobby_data["MaxPlayers"] = new DataObject(
                    visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
                    value: maxPlayers.ToString()
                //index: DataObject.IndexOptions.S1
                );
                lobby_data["Region"] = new DataObject(
                    visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
                    value: data["Region"]
                //index: DataObject.IndexOptions.S1
                );
                lobby_data["Map"] = new DataObject(
                    visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
                    value: data["Map"]
                    //index: DataObject.IndexOptions.S1
                );
                lobby_data["GameplayType"] = new DataObject(
                    visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
                    value: data["GameplayType"]
                //index: DataObject.IndexOptions.N1
                );
                lobby_data["State"] = new DataObject(
                    visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
                    value: "Lobby"
                //index: DataObject.IndexOptions.N1
                );
                lobby_data["Ticket"] = new DataObject(
                    visibility: DataObject.VisibilityOptions.Public, // Visible publicly.
                    value: null
                //index: DataObject.IndexOptions.N1
                );

                Debug.Log("Lobby data: " + lobby_data.ToString());

                CreateLobbyOptions options = new CreateLobbyOptions
                {
                    IsPrivate = isPrivate,
                    Data = lobby_data
                };

            

                Debug.Log("Lobby options: " + options.ToString());
                
                options.Player = new Unity.Services.Lobbies.Models.Player(
                    id: AuthenticationService.Instance.PlayerId,
                    data: new Dictionary<string, PlayerDataObject>()
                    {
                        {
                            "Nickname", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                            value: player.Nickname)
                        },
                        {
                            "Status", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                            value: "Waiting")
                        },
                        {
                            "Skin", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                            value: player.SelectedSkin)
                        },
                         {
                            "Agent", new PlayerDataObject(
                            visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                            value: player.AgentID)
                        }
                    }
                );

                Debug.Log("Lobby player: " + options.Player.Data["Nickname"]);

               
                //m_CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers);
                //StartHeartBeat();

               // var data2 = new Dictionary<string, string>();
                 //   data2.Add("name", name);
                 //   data2.Add("maxp", maxPlayers.ToString());

                  //  var content = new FormUrlEncodedContent(data2);

                   var content = new StringContent( JsonConvert.SerializeObject(options), System.Text.Encoding.UTF8, "application/json");
                   
            
                // CRATE NODE LOBBY 
                //string user_id = Context.PlayerData.UnityID;
                string url = String.Format("{0}/lobbies/create/", Constants.server_backend);
                Debug.Log(url);
                Debug.Log(content.ToString());
            
            var response = await client.PostAsync(url, content);

           // response.Wait();

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                Debug.Log("Lobby response body: " + body);

                Lobby dataS = JsonConvert.DeserializeObject<Lobby>(body);
                Debug.Log("slots count: " + dataS.AvailableSlots);

                m_CurrentLobby = dataS;
                return dataS;
            }
            else
            {
                Debug.Log("No llego respuesta ");
                return null;
            }

              //  return null;//m_CurrentLobby;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Lobby Create failed:\n{ex}");
                return null;
            }
    }

    public async Task<Lobby> JoinLobbyAsync(Lobby lobby, PlayerData player)
    {
        var player_data = new Unity.Services.Lobbies.Models.Player(
                id: AuthenticationService.Instance.PlayerId,
                data: new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "Nickname", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: player.Nickname)
                    },
                    {
                        "Status", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: "Waiting")
                    },
                    {
                        "Skin", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: player.SelectedSkin)
                    },
                    {
                        "Agent", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: player.AgentID)
                    }
                }
            );

            JoinLobbyByIdOptions joinOptions = new JoinLobbyByIdOptions
            { Player = player_data };
            
            var content = new StringContent( JsonConvert.SerializeObject(joinOptions), System.Text.Encoding.UTF8, "application/json");
                   
            string url = String.Format(Constants.server_backend + "/lobbies/joinById/{0}/?user={1}",lobby.Id, player.UnityID);
            Debug.Log(url);
                
            var response = await client.PostAsync(url, content);

           // response.Wait();

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                Debug.Log("Lobby response body: " + body);

                Lobby dataS = JsonConvert.DeserializeObject<Lobby>(body);
                Debug.Log("slots count: " + dataS.AvailableSlots);

                m_CurrentLobby = dataS;
                return dataS;
            }
            else
            {
                Debug.Log("No llego respuesta ");
                return null;
            }
            //m_CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, joinOptions);
            
    }

    public async Task<Lobby> JoinPrivateLobbyAsync(string code, PlayerData player)
    {
        var player_data = new Unity.Services.Lobbies.Models.Player(
                id: AuthenticationService.Instance.PlayerId,
                data: new Dictionary<string, PlayerDataObject>()
                {
                    {
                        "Nickname", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: player.Nickname)
                    },
                    {
                        "Status", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: "Waiting")
                    },
                    {
                        "Skin", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: player.SelectedSkin)
                    },
                    {
                        "Agent", new PlayerDataObject(
                        visibility: PlayerDataObject.VisibilityOptions.Member, // Visible only to members of the lobby.
                        value: player.AgentID)
                    }
                }
            );

            JoinLobbyByCodeOptions joinOptions = new JoinLobbyByCodeOptions
            { Player = player_data };
            
            var content = new StringContent( JsonConvert.SerializeObject(joinOptions), System.Text.Encoding.UTF8, "application/json");
                   
            string url = String.Format(Constants.server_backend + "/lobbies/joinByCode/{0}/?user={1}",code, player.UnityID);
            Debug.Log(url);
                
            var response = await client.PostAsync(url, content);

           // response.Wait();

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                Debug.Log("Lobby response body: " + body);

                Lobby dataS = JsonConvert.DeserializeObject<Lobby>(body);
                Debug.Log("slots count: " + dataS.AvailableSlots);

                m_CurrentLobby = dataS;
                return dataS;
            }
            else
            {
                Debug.Log("No llego respuesta ");
                return null;
            }
    }

    public async Task UpdatePlayerDataAsync(Lobby lobby, Dictionary<string, string> data)
    {
         if (!InLobby())
                return;

            string playerId = AuthenticationService.Instance.PlayerId;
            Dictionary<string, PlayerDataObject> dataCurr = new Dictionary<string, PlayerDataObject>();
            foreach (var dataNew in data)
            {
                PlayerDataObject dataObj = new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member,
                    value: dataNew.Value);
                if (dataCurr.ContainsKey(dataNew.Key))
                    dataCurr[dataNew.Key] = dataObj;
                else
                    dataCurr.Add(dataNew.Key, dataObj);
            }

            PlayerDataObject dataObj0 = new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member,
            value: playerId);
            //dataCurr["player_id"] = playerId;
            
            dataCurr.Add("player_id", dataObj0);

            PlayerDataObject dataObj2 = new PlayerDataObject(visibility: PlayerDataObject.VisibilityOptions.Member,
            value: lobby.Id);

            dataCurr.Add("lobby_id", dataObj2);


            UpdatePlayerOptions updateOptions = new UpdatePlayerOptions
            {
                Data = dataCurr,
                AllocationId = null,
                ConnectionInfo = null
               
            };

             // string user_id = Context.PlayerData.UnityID;
                string url = String.Format("{0}/lobbies/updatePlayer", Constants.server_backend);
                Debug.Log(url);
                
                 var content = new StringContent( JsonConvert.SerializeObject(updateOptions), System.Text.Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync(url, content);

           // response.Wait();

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                Debug.Log("Lobby response body: " + body);

                Lobby dataS = JsonConvert.DeserializeObject<Lobby>(body);
               // Debug.Log("slots count: " + dataS.AvailableSlots);

                m_CurrentLobby = dataS;
                //return dataS;
            }
            else
            {
                Debug.Log("Error updating");
                //return null;
            }

            //m_CurrentLobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, updateOptions);
    }

    public async Task UpdateLobbyDataAsync(Lobby lobby, Dictionary<string, string> data)
    {
         if (!InLobby())
                return;

            Dictionary<string, DataObject> dataCurr = m_CurrentLobby.Data ?? new Dictionary<string, DataObject>();

            var shouldLock = false;
            foreach (var dataNew in data)
            {
                DataObject dataObj = new DataObject(DataObject.VisibilityOptions.Public, dataNew.Value); // Public so that when we request the list of lobbies, we can get info about them for filtering.
                if (dataCurr.ContainsKey(dataNew.Key))
                    dataCurr[dataNew.Key] = dataObj;
                else
                    dataCurr.Add(dataNew.Key, dataObj);

            }
             string playerId = AuthenticationService.Instance.PlayerId;
             DataObject dataObj0 = new DataObject(visibility: DataObject.VisibilityOptions.Member, value: playerId);
             dataCurr.Add("player_id", dataObj0);

             DataObject dataObj2 = new DataObject(visibility: DataObject.VisibilityOptions.Member,
             value: m_CurrentLobby.Id);
             dataCurr.Add("lobby_id", dataObj2);

            UpdateLobbyOptions updateOptions = new UpdateLobbyOptions { Data = dataCurr, IsLocked = shouldLock };

              string url = String.Format("{0}/lobbies/updateLobby", Constants.server_backend);
                Debug.Log(url);
                
                 var content = new StringContent( JsonConvert.SerializeObject(updateOptions), System.Text.Encoding.UTF8, "application/json");
            
            var response = await client.PostAsync(url, content);

           // response.Wait();

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                Debug.Log("Lobby response body: " + body);

                Lobby dataS = JsonConvert.DeserializeObject<Lobby>(body);
               // Debug.Log("slots count: " + dataS.AvailableSlots);

                m_CurrentLobby = dataS;
                //return dataS;
            }
            else
            {
                Debug.Log("Error updating");
                //return null;
            }

            //m_CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, updateOptions);
    }

     public async Task LeaveLobbyAsync(Lobby lobby)
    {
        if (!InLobby())
            return;
        string playerId = AuthenticationService.Instance.PlayerId;

        string url = String.Format("{0}/lobbies/leaveLobby", Constants.server_backend);
                Debug.Log(url);
                
            Dictionary<string, DataObject> dataCurr = new Dictionary<string, DataObject>();

             DataObject dataObj0 = new DataObject(visibility: DataObject.VisibilityOptions.Member, value: playerId);
             dataCurr.Add("player_id", dataObj0);

             DataObject dataObj2 = new DataObject(visibility: DataObject.VisibilityOptions.Member,
             value: lobby.Id);
             dataCurr.Add("lobby_id", dataObj2);

           var filters = new StringContent(JsonConvert.SerializeObject(dataCurr), System.Text.Encoding.UTF8, "application/json");

            var response = await client.PostAsync(url, filters);

            if (response.IsSuccessStatusCode)
            {
                string body = await response.Content.ReadAsStringAsync();
                Debug.Log("Lobby response body: " + body);
                 Dispose();
                
            }
            else
            {
                Debug.Log("Error updating");
                //return null;
            }


       
        
    }
    public async Task SubscribeEventsAsync(Lobby lobby, LobbyEventCallbacks events)
    {
       
    }

      public async Task<string> CreateTicketAsync(Lobby lobby)
    {
          var attributes = new Dictionary<string, object>
            {
                { "region", lobby.Data["Region"].Value },
                { "map", lobby.Data["Map"].Value }
            };

            List <Unity.Services.Matchmaker.Models.Player> players = new List<Unity.Services.Matchmaker.Models.Player>();

            foreach(var player in lobby.Players){
                var data = new Unity.Services.Matchmaker.Models.Player(player.Id);

                players.Add(data);
            }

            var options = new CreateTicketOptions("battleRoyale", attributes);

            var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);

            return ticketResponse.Id;
    }

     public void Dispose()
        {
            m_CurrentLobby = null;
            //m_LobbyEventCallbacks = new LobbyEventCallbacks();
        }
}
}