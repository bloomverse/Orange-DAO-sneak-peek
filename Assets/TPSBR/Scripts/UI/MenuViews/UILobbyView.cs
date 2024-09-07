using System;
using System.Collections;
using System.Collections.Generic;
using Fusion;
using TPSBR.UI;
using Unity.Services.Authentication;
using Unity.Services.Lobbies;
using Unity.Services.Lobbies.Models;
using Unity.Services.Matchmaker.Models;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;
using UnityEngine.Networking;
using Newtonsoft.Json;
using TMPro;
using MoreMountains.Tools;


namespace TPSBR
{
    public class UILobbyView : UICloseView
    {
        //PRIVATE
        [SerializeField] private GameManagerWebGL GameManager;
        [SerializeField] private UIPlayerList _playerList;
        [SerializeField] private UILobby _lobbyDetail;
        [SerializeField] private UIToggle _isReadyButton;
        [SerializeField] private UIButton _startButton;
        [SerializeField] private GameObject _startLoader;

        [SerializeField] private LobbyPlayers3D lobby3DControl;
        [SerializeField] private UIButton _onBackButton;

        [SerializeField] private TextMeshProUGUI _countdownText;


        [SerializeField] private GameObject _trophy;
        [SerializeField] private MMPlaylist playlistFeedback;


        private float timeRemaining = 180;
        private bool timerIsRunning = false;

        [SerializeField]
		private CinemachineVirtualCamera _camera;

        private bool is_starting = false;

        private List<PlayerData> _playerInfo = new List<PlayerData>(32);

        public class CertificateWhore : CertificateHandler
        {
            protected override bool ValidateCertificate(byte[] certificateData)
            {
                return true;
            }
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
     Debug.Log("Adding Listeners................ Onticketstatus");
            GameManager.LobbyJoined += OnLobbyJoined;
            GameManager.TicketStatus += OnTicketStatus;

            _onBackButton.onClick.AddListener(OnBackScreen);

            _isReadyButton.onValueChanged.AddListener(OnStatusChanged);
            _startButton.onClick.AddListener(StartGame);


        }

        protected override void OnDeinitialize()
        {
            Debug.Log("Removing Listeners................");
            GameManager.LobbyJoined -= OnLobbyJoined;
            GameManager.TicketStatus -= OnTicketStatus;
            _isReadyButton.onValueChanged.RemoveListener(OnStatusChanged);
            _startButton.onClick.RemoveListener(StartGame);

            base.OnDeinitialize();
        }

        private void OnBackScreen(){
			 GameManager.LeaveLobby(Context.PlayerData);
          
			 Switch<UIMultiplayerView>();
		}



        protected override bool OnBackAction()
        {
            Debug.Log("Lobby leave");
            GameManager.LeaveLobby(Context.PlayerData);
            base.OnBackAction();
            Close();
            Switch<UIMainMenuView>();

            return true;
        }

        protected override void OnOpen()
        {
            base.OnOpen();
            _camera.enabled = true;
            Debug.Log("Lobby show");
            
           // GetPlayerData();
            SetupLobby();
                StartCoroutine(GetLobbyData(GameManager.Lobby.Id));
                Context.PlayerData.SelectedSkin = PersistentStorage.GetString("Skin");
                
            //}

            if(Context.PlayerData.lobbyType=="Tournament"){
                _startButton.SetActive(false);
                _isReadyButton.SetActive(false);
                _trophy.SetActive(true);
                playlistFeedback.Play();
            }else{
                _countdownText.SetActive(false);
            }

        }
         void Update()
        {
            if (timerIsRunning && Context.PlayerData.lobbyType=="Tournament")
            {
                if (timeRemaining > 0)
                {
                    timeRemaining -= Time.deltaTime;
                    DisplayTime(timeRemaining);
                }
                else
                {
                   timeRemaining = 0;
                   timerIsRunning = false;
                   TimeUp();
                 
                }
            }

           
            
        }

        void TimeUp(){
            pickPlayer();
            Debug.Log(  " -TIMEUP- " + (GameManager.Lobby.Players[0].Id==Context.PlayerData.UnityID) + " -- " + (GameManager.Lobby.Data["Ticket"].Value==null));
            if(GameManager.Lobby.Players[0].Id==Context.PlayerData.UnityID && GameManager.Lobby.Data["Ticket"].Value=="null"){
                StartGame();
            }
        }


       void pickPlayer(){

            DateTime mostRecentPing = GameManager.Lobby.Players[0].LastUpdated;
            string  player = "";
            for (int i = 0; i < GameManager.Lobby.Players.Count; i++)
            {
                
                if(GameManager.Lobby.Players[i].LastUpdated>mostRecentPing){
                    mostRecentPing = GameManager.Lobby.Players[i].LastUpdated;
                    player = GameManager.Lobby.Players[i].Id;
                }
            }

            Debug.Log(mostRecentPing + " - " + player);
       }

       void DisplayTime(float timeToDisplay)
        {
            timeToDisplay += 1;
            float minutes = Mathf.FloorToInt(timeToDisplay / 60);  
            float seconds = Mathf.FloorToInt(timeToDisplay % 60);
            _countdownText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }

         IEnumerator GetLobbyData(string lobby_id)
        {
            while (true)
            {
                string user_id = Context.PlayerData.UnityID;
                string url = String.Format(Constants.server_backend + "/lobbies/data/{0}/?user={1}", lobby_id, user_id);
                UnityWebRequest request = UnityWebRequest.Get(url);
                request.certificateHandler = new CertificateWhore();
                yield return request.SendWebRequest();

                if (request.result == UnityWebRequest.Result.Success)
                {
                    Lobby data = JsonConvert.DeserializeObject<Lobby>(request.downloadHandler.text);
                    GameManager.Lobby  = data;        
                    var State = GameManager.Lobby.Data["State"].Value;

                    //* Counter

                  //  Debug.Log(GameManagerWebGL.Instance.currentTournamentRound.date + " Current Date of round");

                    if(GameManagerWebGL.Instance.currentTournamentRound.date!=""){
                        var mongoDate = DateTime.Parse(GameManagerWebGL.Instance.currentTournamentRound.date ); 
                        DateTime unityDate = DateTime.SpecifyKind(mongoDate, DateTimeKind.Utc);
                        TimeSpan timeLeft = unityDate - DateTime.Now;   
                        timeRemaining = (int)timeLeft.TotalSeconds;
                    }else{
                        timeRemaining = 0;
                    }
                    if(State=="Lobby"){
                        if(GameManager.Lobby.Data.ContainsKey("Tournament") && GameManager.Lobby.Data["Tournament"].Value=="true"){
                            _startButton.SetActive(false);
                        }else{
                             _startButton.SetActive(CheckisOwner());
                        }
                    }

                    if(State=="Start"){  
                         _startButton.SetActive(false);
                         _startLoader.SetActive(true);   
                    }

                    if(State=="Launching"){
                        Debug.Log("Launching..." + GameManager.Lobby.Data["MatchId"].Value);
                        launchClient(GameManager.Lobby.Data["MatchId"].Value);
                    }
             
                        _lobbyDetail.SetData(data);
                        lobby3DControl.updateLobbyPlayers(data);
                        timerIsRunning = true;
                }
                else{
                    Debug.Log("Error Lobby quert" + request.result);
                }

                yield return new WaitForSeconds(3);
            }
        }

    

        protected override void OnClose()
        {
            base.OnClose();
            _camera.enabled = false;
        }

        protected override void OnTick()
        {
            base.OnTick();
            bool ready = CheckAllReady();
            _startButton.interactable = ready;

            if(ready){
                var gameObject = _startButton.GetComponent<SimpleAnimation>();
                gameObject.Play(0);
            }else{
                var gameObject = _startButton.GetComponent<SimpleAnimation>();
                gameObject.Stop();
            }
            
            
           
            //_dedicatedServerWarning.SetActive(_dedicatedServer.isOn);
        }

       

        private void OnLobbyJoined(object seder, Lobby lobby)
        {
            Open();
            Context.PlayerData.lobbyId = lobby.Id; //GameManager.Lobby.Id;
        }

        private void OnPlayerDataChanged(object seder, Lobby lobby)
        {
            //GetPlayerData();
        }

        private void OnLobbyDataChanged(object seder, Lobby lobby)
        {
            string state = GameManager.Lobby.Data["State"].Value;
            string ticket = GameManager.Lobby.Data["Ticket"].Value;

            Debug.Log("Lobby ticket: " + ticket);
            Debug.Log("Lobby state: " + state);

            if (Equals(state, "Start") && ticket != null)
            {
                //GameManager.
            }
            else
            {
                is_starting = false;
            }
        }

        private void OnTicketStatus(object seder, MultiplayAssignment assignment)
        {
            Debug.Log("Regreso ticket status" +assignment.MatchId );
            Context.Matchmaking.JoinSession("mm-" + assignment.MatchId);
            Close();
        }

        private void launchClient(string MatchId){
            Context.Matchmaking.JoinSession("mm-" + MatchId);
            Close();
        }

        private void OnStatusChanged(bool isSelected)
        {
//            Debug.Log("Changing status player");
            
           // GameManagerWebGL.Instance.JoinedLobbies(Context.PlayerData.UnityID);
            GameManager.SetPlayerStatus(Context.PlayerData, isSelected);
        }

        private void OnUpdatePlayerListContent(int index, UIPlayer content)
        {
            
            content.SetData(Context, _playerInfo[index], index + 1);
        }


        private bool CheckAllReady()
        {
            int players_ready = 0;

            for (int i = 0; i < GameManager.Lobby.Players.Count; i++)
            {
                if (GameManager.Lobby.Players[i].Data != null)
                {
                    if (GameManager.Lobby.Players[i].Data["Status"] != null)
                    {
                        if(String.Equals(GameManager.Lobby.Players[i].Data["Status"].Value, "Ready"))
                        {
                            players_ready += 1;
                        }
                    }
                }
            }

            return players_ready >= GameManager.Lobby.Players.Count && !is_starting && GameManager.Lobby.Players.Count > 1;
        }

        private bool CheckisOwner()
        {
            string owner_id = GameManager.Lobby.HostId; //.Players[0].Id;
            string player_id = AuthenticationService.Instance.PlayerId;

            //Debug.Log("Lobby owner id:" + owner_id);
            //Debug.Log("Lobby player id:" + player_id);

            if(String.Equals(owner_id, player_id))
            {
                Debug.Log("IS OWNER " + true );
                return true;
            }
            Debug.Log("IS OWNER " + false );
            return false;
        }

        private void StartGame()
        {
            is_starting = true;
//            _startButton.GetComponentInChildren<Text>().text = "Starting...";
           // var ticket = GameManager.Lobby.Data["Ticket"].Value;
           // GameManager.startTicketCheck(ticket) ;
            GameManager.StartGame(Context.PlayerData.UnityID);
        }

        private void SetupLobby()
        {
            _lobbyDetail.SetData(GameManager.Lobby);
            _isReadyButton.isOn = false;
        }
    }
}