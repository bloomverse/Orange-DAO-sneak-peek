using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Fusion;
using TPSBR.UI;
using System.Collections.Generic;
using System;
using UnityEngine.Networking; 
using System.Collections;
using MoreMountains.Tools;
using MoreMountains.Feedbacks;

#pragma warning disable 4014



namespace TPSBR
{
    public class UITournamentDetail : UICloseView
	{
       
        [SerializeField]
        private TextMeshProUGUI _name;

         // public UITournamentDetailListHandler listHandler;

        private List<TournamentPlayer> _TournamentDetail = new List<TournamentPlayer>(32);
        private UITournamentListDetail _TournamentDetailList;
        private TournamentDetail  _tournamentDetail;


        [SerializeField]
        private MMF_Player signupTrigger;
      // CUSTOM FIELDS
        [SerializeField]
        private TextMeshProUGUI _tournamentTitle;
        [SerializeField]
        private TextMeshProUGUI _tournamentType;
        [SerializeField]
        private TextMeshProUGUI _tournamentCountdown;
        [SerializeField]
        private TextMeshProUGUI _tournamentDescription;
        [SerializeField]
        private TextMeshProUGUI _prizeH1;
        [SerializeField]
        private TextMeshProUGUI _prizeH2;
        [SerializeField]
        private TextMeshProUGUI _prizeH3;
        [SerializeField]
        private TextMeshProUGUI _prizeH4;

         [SerializeField]
        private TextMeshProUGUI _prize1;
        [SerializeField]
        private TextMeshProUGUI _prize2;
        [SerializeField]
        private TextMeshProUGUI _prize3;
        [SerializeField]
        private TextMeshProUGUI _prize4;


        [SerializeField]
        private UIButton _signupButton;

         [SerializeField]
        private UIButton _withdraw;


        [SerializeField]
        private TextMeshProUGUI _restrictions;

        [SerializeField]
        private TextMeshProUGUI _alreadySignedButton;


        [SerializeField]
        private TextMeshProUGUI _signupText;

        public DateTime startTime;

        public DateTime endTime;

        [SerializeField]
        private RawImage tImage;


        [SerializeField]
        private GameObject tableSection;


        private bool suscribedStatus = false;
        private bool ProcRunning;
        
    // [SerializeField] private UIButton _onBackButton;
        protected override void OnInitialize()
		{
			base.OnInitialize();
            _TournamentDetailList = GetComponentInChildren<UITournamentListDetail>();
        }
        protected override void OnOpen(){
            DataManager.OnTLDresponse += ListResD;  // Reponse Main List
            DataManager.OnTPresponse += playersRes; // Respone usare data 
            DataManager.OnTLSignresponse += signupRes;  // Response signup
            DataManager.OnTPIresponse += TPIuserRes;  // User is signed up
            DataManager.OnTPInotfound +=  OnTPInotfound; // User not signed up 

            _withdraw.SetActive(false);
            _signupButton.SetActive(false);
            _alreadySignedButton.SetActive(false);
            _signupButton.onClick.AddListener(signupF);
            tableSection.SetActive(false);
            tImage.SetActive(false);
            _TournamentDetailList.UpdateContent += SetListRes;        
            
//          _onBackButton.onClick.AddListener(OnBackScreen);

            // GET Tournament info and then userInfo
            ProcRunning = true;
            StartCoroutine(Proc());
            

        }

        IEnumerator Proc(){

            while(ProcRunning){
                mainList();
                Debug.Log("Refresing list");
                yield return new WaitForSeconds(30);
            }
            
        }

        public void mainList(){
            DataManager.instance.tlDetailRequest(Context.SelectedTournamentDetail._id);
        }

        private void ListResD(TournamentDetail  tournaments){

            _tournamentDetail = tournaments;


               string date = tournaments.rounds[0].date;
            //determinate which is the last date on array by closed bool parameter
            for (int i = 0; i < tournaments.rounds.Length; i++)
            {
                if(tournaments.rounds[i].closed == "false"){
                    break;
                }else {
                    date = tournaments.rounds[i].date;
                }
            }


             var mongoDate = DateTime.Parse(date);
            DateTime unityDate = DateTime.SpecifyKind(mongoDate, DateTimeKind.Utc);

            endTime = unityDate ;



            //Debug.Log(tournaments.start_date+ " date countdown" + tournaments.conditions);
           // var mongoDate = DateTime.Parse(tournaments.start_date);
           // DateTime unityDate = DateTime.SpecifyKind(mongoDate, DateTimeKind.Utc);

            startTime = unityDate ;

            _name.text = tournaments.name;
            _tournamentTitle.text =  "Region: " + tournaments.region;

            var totalSus =  tournaments.playerCount  ;
           

            _tournamentType.text = "Players: " +  totalSus + "/" +  tournaments.max_players;
           // _tournamentCountdown.text = tournaments.start_date;
            _tournamentDescription.text = tournaments.description;

            _prize1.text= "";
            _prize2.text= "";
            _prize3.text= "";
            _prize4.text= "";
            
            
            if(tournaments.prizes.Length>0){
                _prizeH1.SetActive(true);
                _prize1.text = tournaments.prizes[0].prize;
            }else{
                _prizeH1.SetActive(false);
            }
            if(tournaments.prizes.Length>1){
                _prizeH2.SetActive(true);
                _prize2.text = tournaments.prizes[1].prize;
            }else{
                _prizeH2.SetActive(false);
            }

            if(tournaments.prizes.Length>2){
                _prizeH3.SetActive(true);
                _prize3.text = tournaments.prizes[2].prize;
            }else{
                _prizeH3.SetActive(false);
            }

            if(tournaments.prizes.Length>3){
                _prizeH4.SetActive(true);
                _prize4.text = tournaments.prizes[3].prize;    
            }else{
                _prizeH4.SetActive(false);
            }

             if(_tournamentDetail.conditions!=null && _tournamentDetail.conditions.Length>0){
                _restrictions.SetActive(true);
                _restrictions.SetText("Token Gated with " + _tournamentDetail.conditions[0].amount + " " + _tournamentDetail.conditions[0].description );
             }



            // Check if :  its open / if you are already there 

            // Check if finished
            TimeSpan timeLeft = endTime - DateTime.Now;   
            if(timeLeft.Hours < 1 && timeLeft.Minutes<=3  && _tournamentDetail!=null ){
                    
                // Tournament Closed
                TournamentMode();
                DataManager.instance.tournamentPlayers(Context.SelectedTournamentDetail._id,10);

            }

             if(timeLeft.Hours >= 1   && _tournamentDetail!=null ){
                _withdraw.onClick.AddListener(RequestWithdraw);
                _withdraw.SetActive(true);
             }
            
            

            // Check if you are already in tournament
            // if your are show tournament mode



            // If not show toruamnent info
        
             if(Context.PlayerData.userData!=null){

                DataManager.instance.tournamentPlayerInfo(Context.SelectedTournamentDetail._id,Context.PlayerData.userData._id);
            } else{
                SignupMode();
            }

           

      
        }

        void TPIuserRes(TournamentPlayer TP){
                suscribedStatus = true; 

                if(TP!=null){
                    _alreadySignedButton.text = "Already signed up!";
                    _alreadySignedButton.SetActive(true);
                    _signupButton.SetActive(false); 
                    TournamentMode();
                    DataManager.instance.tournamentPlayers(Context.SelectedTournamentDetail._id,10);
                
                }
          
        }

        void OnTPInotfound(string str){
               suscribedStatus = false;
             _signupButton.SetActive(true); 
               SignupMode();
              // mainList();
        }

        void Update()
        {
            TimeSpan timeLeft = endTime - DateTime.Now;   
            var pre = "Next Match in ";
            if(timeLeft.Days > 0){
                pre += "{0} days,";
            }

         

             if(timeLeft.Minutes > 0 && timeLeft.Hours >= 0 && timeLeft.Days >= 0){
                _tournamentCountdown.color = Color.green;
            _tournamentCountdown.text = string.Format(pre + " {1:00}:{2:00}:{3:00}", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
             }else{
                 _tournamentCountdown.text = "Tournament Closed";
                _tournamentCountdown.color = new Color(255,176,0,255);
                 _signupButton.SetActive(false);
                    _alreadySignedButton.SetActive(false);
             }
            

             //   TimeSpan timeLeft = startTime -DateTime.Now;   
              //  Debug.Log(GetPacificStandardTime(DateTime.Now));
           // _tournamentCountdown.text = string.Format("{0} days, {1:00}:{2:00}:{3:00}", timeLeft.Days, timeLeft.Hours, timeLeft.Minutes, timeLeft.Seconds);
          //  }
            
        }

        // Tournament List
        private void SetListRes(int index, UITournamentListedDetailItem content){
             content.SetData(_TournamentDetail[index]);

        }

        private void signupRes(string res){
            Debug.Log("regreso signup");

            _alreadySignedButton.SetActive(true);
            signupTrigger.PlayFeedbacks();
            DataManager.instance.tournamentPlayers(Context.SelectedTournamentDetail._id,10);
            TournamentMode();
            CryptoManager.instance.getUserData(true);
            
        }

        

        private TournamentConditions  ItemInfo;

        private void signupF(){
        
        string token = "";
        

       /* if(Context.PlayerData.userData.userLevel<0){
            var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Bloomverse Tournaments";
				infoDialog.Description.text = "You need to be level 5 to suscribe to tournaments.";
            return;
        }*/

        if(Context.PlayerData.userData==null){
            var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Bloomverse Tournaments";
				infoDialog.Description.text = "You need to be logged in for suscribing in a tournament.";
            return;
        }

        if(_tournamentDetail.conditions!=null && _tournamentDetail.conditions.Length>0){
              foreach (var item in _tournamentDetail.conditions)
                {
                    Debug.Log("-" + item.condition+ "-");
                    if(item.condition=="token"){
                        ItemInfo = item;
                        CryptoManager.OnIsParticleResponse += CheckParticleConnection;
		                CryptoManager.instance.CheckIfParticleIs(); 
                        break;                       
                    }
                }
        }else{
                Debug.Log("inscribiendo usuario en normal..." + Context.SelectedTournamentDetail._id);
                _signupButton.SetActive(false);
                tImage.SetActive(false);
                DataManager.instance.tlSignup(Context.SelectedTournamentDetail._id);
        }

        }

        public void CheckParticleConnection(string value){
            CryptoManager.OnIsParticleResponse -= CheckParticleConnection;
            Debug.Log(value + " Value is particle");
			if(value!="notConnected"){
                TournamentConditions item = ItemInfo;
                        Debug.Log("entro Pasando condiciones ++ Requesting token" + item.description + " " + item.condition);
                        //token = item.value;

                        RequestToken(item.amount,item.value,item.lives,item.description);
                        _signupButton.SetActive(false);
                        tImage.SetActive(false);
                        return;
            }else{

                var dialog = Open<UIYesNoDialogView>();
                    dialog.Title.text = "Connect Inventory";
                    dialog.YesButtonText.SetText("Connect");
                    dialog.Description.text = "Please connect your inventory and try again.";
                    dialog.HasClosed += (result) =>
                    {
                        if (result == true){
                            CryptoManager.instance.LoginParticleClick();
                        }
                        else{
                            Close();
                        }
                    };

		    }            
        }

         IEnumerator DownloadImage(string MediaUrl)
        {   
            Debug.Log(MediaUrl +  "media url");
            UnityWebRequest request = UnityWebRequestTexture.GetTexture(MediaUrl);
            yield return request.SendWebRequest();
            if(request.isNetworkError || request.isHttpError) 
                Debug.Log(request.error);
            else
                tImage.SetActive(true);
                tImage.texture = ((DownloadHandlerTexture) request.downloadHandler).texture;
        } 

        private void OnBackScreen(){
			Open<UIMainMenuView>();
		}

        protected override void OnClose()
		{
			//base.OnClose();
            DataManager.OnTLDresponse -= ListResD;  // Reponse Main List
            DataManager.OnTPresponse -= playersRes; // Respone usare data 
            DataManager.OnTLSignresponse -= signupRes;  // Response signup
            DataManager.OnTPIresponse -= TPIuserRes;  // User is signed up
            DataManager.OnTPInotfound -=  OnTPInotfound; // User not signed up 


            _signupButton.onClick.RemoveListener(signupF);
            
            _TournamentDetailList.UpdateContent -= SetListRes;        
            ProcRunning= false;
            _tournamentDetail = null;
            Open<UIMainMenuView>();
		}

        private void RequestToken(float amount,string tokenId,int lives,string description)
		{

            
			var dialog = Open<UIYesNoDialogView>();

			dialog.Title.text = "TOKEN GATED SIGN UP";
			dialog.Description.text = "This tournament requires " + amount + " " + description + " for signup completition , Tokens are not refundable, do you want to continue?.";

			dialog.HasClosed += (result) =>
			{
				if (result == true)
				{			
                    CryptoManager.OnTokenResponse += _tokenResponse;	
					CryptoManager.AskbloomieTransaction(amount,tokenId,description);
				}
			};
		}

        private void RequestWithdraw()
		{

            
			var dialog = Open<UIYesNoDialogView>();

			dialog.Title.text = "WITHDRAW FROM TOURNAMENT?";
			dialog.Description.text = "This action will withdraw your player from the tournament, if there is a signup token fee, it will be refund within 48 hours.";

			dialog.HasClosed += (result) =>
			{
				if (result == true)
				{			
                    CryptoManager.OnWithdrawResponse += _withdrawResponse;	
					CryptoManager.instance.RequestWithdraw(Context.SelectedTournamentDetail._id);
				}
			};
		}

          private void _withdrawResponse(string res){
            CryptoManager.OnWithdrawResponse -= _withdrawResponse;	
            if(res=="error"){
                var infoDialog = Open<UIInfoDialogView>();
				infoDialog.Title.text = "Bloomverse Services";
				infoDialog.Description.text = "There was an error trying to cancel your registration ";
            }else{
                _withdraw.SetActive(false);
                _signupButton.SetActive(false);
                var infoDialog = Open<UIInfoDialogView>();
                 _alreadySignedButton.SetActive(false);
				infoDialog.Title.text = "Bloomverse Services";
				infoDialog.Description.text = "You were successfully removed from this tournament ";
            }
        }



        private void _tokenResponse(string res){
            CryptoManager.OnTokenResponse -= _tokenResponse;	
            if(res=="PhantomError"){
                var infoDialog = Open<UIInfoDialogView>();

				infoDialog.Title.text = "Bloomverse Services";
				infoDialog.Description.text = "Error on Crypto Services, try again. ";
            }else{
                DataManager.instance.tlSignup(Context.SelectedTournamentDetail._id);
            }
        }

        private void SignupMode(){
            Debug.Log("Entering Signup mode");
            //tImage.SetActive(true);
            _withdraw.SetActive(false);
            //_signupButton.SetActive(true);
            tableSection.SetActive(false);
            //_alreadySignedButton.SetActive(false);
               if(_tournamentDetail.conditions!=null && _tournamentDetail.conditions.Length>0){
                _signupText.text = "Signup with " + _tournamentDetail.conditions[0].amount + " " + _tournamentDetail.conditions[0].description  ;
                }else{
                    _signupText.text = "Signup " ;
                }
                
                if(Context.PlayerData.userData==null){
                    
                    _signupButton.SetActive(true);
                }

                StartCoroutine(DownloadImage(_tournamentDetail.image));

        }

        private void TournamentMode(){
            Debug.Log("Entering tournament mode");
            tImage.SetActive(false);
            _signupButton.SetActive(false);
            tableSection.SetActive(true);
            
           // _alreadySignedButton.SetActive(true);
        }

       

        

       

     

        public DateTime GetPacificStandardTime(DateTime from) {
            //find TimeZoneInfo of PST
            TimeZoneInfo tzi = TimeZoneInfo.FindSystemTimeZoneById("Pacific Standard Time");
            //Convert time from Local to PST
            return TimeZoneInfo.ConvertTime(from, TimeZoneInfo.Local, tzi);
            }
 

      

        private void playersRes(TournamentPlayers tp){

            _TournamentDetail.Clear();
              if(tp.players!=null){

           	for (int i = 0; i < tp.players.Length; i++)
			{
                TournamentPlayer tournamentP = tp.players[i];

				_TournamentDetail.Add(tournamentP);
			}


           _TournamentDetailList.Refresh(tp.players.Length);

           }else{
               _TournamentDetailList.Refresh(0);
           }
        }

      
       
        
    }
}
