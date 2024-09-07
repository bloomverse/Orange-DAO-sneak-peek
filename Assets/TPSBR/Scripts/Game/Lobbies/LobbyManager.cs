using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker;
using UnityEngine;

namespace TPSBR
{
    /// <summary>
    /// An abstraction layer between the direct calls into the Lobby API and the outcomes you actually want. E.g. you can request to get a readable list of
    /// current lobbies and not need to make the query call directly.
    /// </summary>
    ///

    public class LobbyManager : IDisposable
    {
        //ENUMS
        Lobby m_CurrentLobby;
        Task m_HeartBeatTask;

        //VARS
        //LobbyEventCallbacks m_LobbyEventCallbacks = new LobbyEventCallbacks();
        ServiceRateLimiter m_QueryCooldown = new ServiceRateLimiter(1, 1f);
        ServiceRateLimiter m_CreateCooldown = new ServiceRateLimiter(2, 6f);
        ServiceRateLimiter m_HeartBeatCooldown = new ServiceRateLimiter(5, 30);
        ServiceRateLimiter m_LeaveLobbyOrRemovePlayer = new ServiceRateLimiter(5, 1);
        ServiceRateLimiter m_JoinCooldown = new ServiceRateLimiter(2, 6f);
        ServiceRateLimiter m_UpdatePlayerCooldown = new ServiceRateLimiter(5, 5f);
        ServiceRateLimiter m_UpdateLobbyCooldown = new ServiceRateLimiter(5, 5f);

        //CONST
        const int k_maxLobbiesToShow = 16;

        //CALLBACK
        //LobbyEventCallbacks m_LobbyEventCallbacks = new LobbyEventCallbacks();

        public bool InLobby()
        {
            if (m_CurrentLobby == null)
            {
                Debug.LogWarning("LobbyManager not currently in a lobby. Did you CreateLobbyAsync or JoinLobbyAsync?");
                return false;
            }

            return true;
        }

        public virtual async Task<QueryResponse> RetrieveLobbyListAsync(EGameplayType gameplayType = EGameplayType.None)
        {
            var filters = LobbyGameplayTypeFilters(gameplayType);

            Debug.Log("Lobby filters: " + filters.ToString());

            if (m_QueryCooldown.TaskQueued)
                return null;
            await m_QueryCooldown.QueueUntilCooldown();

            QueryLobbiesOptions queryOptions = new QueryLobbiesOptions
            {
                Count = k_maxLobbiesToShow,
                Filters = filters
            };

            return await LobbyService.Instance.QueryLobbiesAsync(queryOptions);
        }

        public async Task<Lobby> CreateLobbyAsync(string name, Dictionary<string, string> data, bool isPrivate, int maxPlayers, PlayerData player)
        {

            Debug.Log("Creating lobby async" + player.AgentID);

            if (m_CreateCooldown.IsCoolingDown)
            {
                Debug.LogWarning("Create Lobby hit the rate limit.");
                return null;
            }

            await m_CreateCooldown.QueueUntilCooldown();

            try
            {
                Debug.Log("Lobby building...");
                //string uasId = AuthenticationService.Instance.PlayerId;

                Dictionary<string, DataObject> lobby_data = new Dictionary<string, DataObject>();
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

                m_CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers, options);
                //m_CurrentLobby = await LobbyService.Instance.CreateLobbyAsync(name, maxPlayers);
                //StartHeartBeat();

                return m_CurrentLobby;
            }
            catch (Exception ex)
            {
                Debug.LogError($"Lobby Create failed:\n{ex}");
                return null;
            }
        }

        public async Task<Lobby> JoinLobbyAsync(Lobby lobby, PlayerData player)
        {
            if (m_JoinCooldown.IsCoolingDown ||
                (lobby == null))
            {
                return null;
            }

            await m_JoinCooldown.QueueUntilCooldown();

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
            m_CurrentLobby = await LobbyService.Instance.JoinLobbyByIdAsync(lobby.Id, joinOptions);

            return m_CurrentLobby;
        }

        public async Task<Lobby> JoinPrivateLobbyAsync(string code, PlayerData player)
        {
            if (m_JoinCooldown.IsCoolingDown ||
                (code == null))
            {
                return null;
            }

            await m_JoinCooldown.QueueUntilCooldown();

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
            m_CurrentLobby = await LobbyService.Instance.JoinLobbyByCodeAsync(code, joinOptions);

            return m_CurrentLobby;
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

            if (m_UpdatePlayerCooldown.TaskQueued)
                return;
            await m_UpdatePlayerCooldown.QueueUntilCooldown();

            UpdatePlayerOptions updateOptions = new UpdatePlayerOptions
            {
                Data = dataCurr,
                AllocationId = null,
                ConnectionInfo = null
            };
            m_CurrentLobby = await LobbyService.Instance.UpdatePlayerAsync(lobby.Id, playerId, updateOptions);
        }

        public async Task UpdateLobbyDataAsync(Lobby lobby, Dictionary<string, string> data)
        {
            if (!InLobby())
                return;

            Dictionary<string, DataObject> dataCurr = m_CurrentLobby.Data ?? new Dictionary<string, DataObject>();

            var shouldLock = false;
            foreach (var dataNew in data)
            {
                // Special case: We want to be able to filter on our color data, so we need to supply an arbitrary index to retrieve later. Uses N# for numerics, instead of S# for strings.
                //DataObject.IndexOptions index = dataNew.Key == "LocalLobbyColor" ? DataObject.IndexOptions.N1 : 0;
                DataObject dataObj = new DataObject(DataObject.VisibilityOptions.Public, dataNew.Value); // Public so that when we request the list of lobbies, we can get info about them for filtering.
                if (dataCurr.ContainsKey(dataNew.Key))
                    dataCurr[dataNew.Key] = dataObj;
                else
                    dataCurr.Add(dataNew.Key, dataObj);

                //Special Use: Get the state of the Local lobby so we can lock it from appearing in queries if it's not in the "Lobby" LocalLobbyState
                //if (dataNew.Key == "LocalLobbyState")
                //{
                //    Enum.TryParse(dataNew.Value, out LobbyState lobbyState);
                //    shouldLock = lobbyState != LobbyState.Lobby;
                //}
            }

            //We can still update the latest data to send to the service, but we will not send multiple UpdateLobbySyncCalls
            if (m_UpdateLobbyCooldown.TaskQueued)
                return;
            await m_UpdateLobbyCooldown.QueueUntilCooldown();

            UpdateLobbyOptions updateOptions = new UpdateLobbyOptions { Data = dataCurr, IsLocked = shouldLock };
            m_CurrentLobby = await LobbyService.Instance.UpdateLobbyAsync(m_CurrentLobby.Id, updateOptions);
        }

        public async Task SubscribeEventsAsync(Lobby lobby, LobbyEventCallbacks events)
        {
            await LobbyService.Instance.SubscribeToLobbyEventsAsync(lobby.Id, events);
        }

        public async Task LeaveLobbyAsync(Lobby lobby)
        {
            await m_LeaveLobbyOrRemovePlayer.QueueUntilCooldown();
            if (!InLobby())
                return;
            string playerId = AuthenticationService.Instance.PlayerId;

            await LobbyService.Instance.RemovePlayerAsync(lobby.Id, playerId);
            Dispose();
        }

        public async Task<string> CreateTicketAsync(Lobby lobby)
        {
            var attributes = new Dictionary<string, object>
            {
                { "region", lobby.Data["Region"].Value },
                { "map", lobby.Data["Map"].Value },
                
            };

            if(lobby.Data.ContainsKey("Tournament") && lobby.Data["Tournament"].Value=="true"){
                Debug.Log("Adding tournament gametype.... to matchmaker");
                attributes.Add("gametype","tournament");
            }

            List <Unity.Services.Matchmaker.Models.Player> players = new List<Unity.Services.Matchmaker.Models.Player>();

            foreach(var player in lobby.Players){
                var data = new Unity.Services.Matchmaker.Models.Player(player.Id);

                players.Add(data);
            }

            var options = new CreateTicketOptions("battleRoyale", attributes);

            var ticketResponse = await MatchmakerService.Instance.CreateTicketAsync(players, options);
            Debug.Log("ticket res" + ticketResponse);
            return ticketResponse.Id;
        }

        async Task SendHeartbeatPingAsync()
        {
            if (!InLobby())
                return;
            if (m_HeartBeatCooldown.IsCoolingDown)
                return;
            await m_HeartBeatCooldown.QueueUntilCooldown();

            await LobbyService.Instance.SendHeartbeatPingAsync(m_CurrentLobby.Id);
        }

        void StartHeartBeat()
        {
#pragma warning disable 4014
            m_HeartBeatTask = HeartBeatLoop();
#pragma warning restore 4014
        }

        async Task HeartBeatLoop()
        {
            while (m_CurrentLobby != null)
            {
                await SendHeartbeatPingAsync();
                await Task.Delay(8000);
            }
        }

        List<QueryFilter> LobbyGameplayTypeFilters(EGameplayType gameplayType)
        {
            List<QueryFilter> filters = new List<QueryFilter>();
            if (gameplayType == EGameplayType.Elimination)
                filters.Add(new QueryFilter(QueryFilter.FieldOptions.N1, ((int)EGameplayType.Elimination).ToString(),
                    QueryFilter.OpOptions.EQ));
            //else if (gameplayType == LobbyColor.Green)
            //    filters.Add(new QueryFilter(QueryFilter.FieldOptions.N1, ((int)LobbyColor.Green).ToString(),
            //        QueryFilter.OpOptions.EQ));
            //else if (gameplayType == LobbyColor.Blue)
            //    filters.Add(new QueryFilter(QueryFilter.FieldOptions.N1, ((int)LobbyColor.Blue).ToString(),
            //        QueryFilter.OpOptions.EQ));
            return filters;
        }

        public void Dispose()
        {
            m_CurrentLobby = null;
            //m_LobbyEventCallbacks = new LobbyEventCallbacks();
        }
    }

    //Manages the Amount of times you can hit a service call.
    //Adds a buffer to account for ping times.
    //Will Queue the latest overflow task for when the cooldown ends.
    //Created to mimic the way rate limits are implemented Here:  https://docs.unity.com/lobby/rate-limits.html
    public class ServiceRateLimiter
    {
        public Action<bool> onCooldownChange;
        public readonly int coolDownMS;
        public bool TaskQueued { get; private set; } = false;

        readonly int m_ServiceCallTimes;
        bool m_CoolingDown = false;
        int m_TaskCounter;

        //(If you're still getting rate limit errors, try increasing the pingBuffer)
        public ServiceRateLimiter(int callTimes, float coolDown, int pingBuffer = 120)
        {
            m_ServiceCallTimes = callTimes;
            m_TaskCounter = m_ServiceCallTimes;
            coolDownMS =
                Mathf.CeilToInt(coolDown * 1000) +
                pingBuffer;
        }

        public async Task QueueUntilCooldown()
        {
            if (!m_CoolingDown)
            {
#pragma warning disable 4014
                ParallelCooldownAsync();
#pragma warning restore 4014
            }

            m_TaskCounter--;

            if (m_TaskCounter > 0)
            {
                return;
            }

            if (!TaskQueued)
                TaskQueued = true;
            else
                return;

            while (m_CoolingDown)
            {
                await Task.Delay(10);
            }
        }

        async Task ParallelCooldownAsync()
        {
            IsCoolingDown = true;
            await Task.Delay(coolDownMS);
            IsCoolingDown = false;
            TaskQueued = false;
            m_TaskCounter = m_ServiceCallTimes;
        }

        public bool IsCoolingDown
        {
            get => m_CoolingDown;
            private set
            {
                if (m_CoolingDown != value)
                {
                    m_CoolingDown = value;
                    onCooldownChange?.Invoke(m_CoolingDown);
                }
            }
        }
    }
}
