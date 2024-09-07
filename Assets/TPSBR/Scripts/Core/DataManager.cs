using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine;
using Newtonsoft.Json;
using System;
using TPSBR.UI;

namespace TPSBR
{

    public class CertificateWhore : CertificateHandler
     {
         protected override bool ValidateCertificate(byte[] certificateData)
         {
             return true;
         }
     }

    public class 
    DataManager : MonoBehaviour
    {
        

        public static DataManager instance;


        void Awake(){
            instance = this;
//            Debug.Log("Data manager initalizaed");
        }
        //public static Action<String> OnMatchData ;
        private IEnumerator createMatch(string wallet)
        {
            WWWForm form = new WWWForm();
         //   Debug.Log("Getting user data");
            
            form.AddField("gameID", "gameID");  

            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend +  "/game/creatematch", form);
            //www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {//
                Debug.Log(www.downloadHandler.text+ " user data");
                www.Dispose();
            
            }
        }

        public void endIns(List<UserToBase> list){
            Debug.Log(" endIns coroutine " +  list.Count);
            StartCoroutine(endMatch(list));
        }
        //public static Action<String> OnUserData ;
        public IEnumerator endMatch(List<UserToBase> rewarded)
        {
            Debug.Log("Sending Match Data");
            Debug.Log(" endIns coroutine " +  rewarded.Count);
            WWWForm form = new WWWForm();

           

            //JsonConverter.Sel. 

            var jsondata = JsonConvert.SerializeObject(rewarded);
             //string jsondata =  JsonUtility.ToJson(rewarded, true);
            // Debug.Log(jsondata);

            form.AddField("data", jsondata);   
            UnityWebRequest www = UnityWebRequest.Put(Constants.server_backend +  "/game/endmatch", jsondata);
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.certificateHandler = new CertificateWhore();
            //www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text+ " user data");
                www.Dispose();
            
            }
        }

        // SAVE MATCH INFO ---------------------------------------
        public void endMatchTournament(TournamentPlayers list, string lobbyId){
           // Debug.Log(" endIns coroutine " +  list.Length);
            StartCoroutine(endMatchTournamentIE(list,lobbyId));
        }
        //public static Action<String> OnUserData ;
        public IEnumerator endMatchTournamentIE(TournamentPlayers players, string lobbyId)
        {
            Debug.Log("Sending Tournament Data " + Constants.server_backend +  "/tournaments/savetournament/" + lobbyId);
            WWWForm form = new WWWForm();
            var jsondata = JsonConvert.SerializeObject(players);
            //string tournamentId = "648bb27fe42be2d5fb9ef50a";
            //string matchId = "648bc29a283bbfbb4c05752e";
            form.AddField("players", jsondata);   
            UnityWebRequest www = UnityWebRequest.Put(Constants.server_backend +  "/tournaments/savetournament/" + lobbyId, jsondata);
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.certificateHandler = new CertificateWhore();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error + " Error saving tournament");
            }
            else
            {
                Debug.Log(www.downloadHandler.text+ " user data");
                www.Dispose();
            
            }
        }

        public void bloomP(string DBID){
            
            StartCoroutine(sendPrize(DBID));
        }
        public IEnumerator sendPrize(string DBID)
        {
            Debug.Log("Sending Prize");
            WWWForm form = new WWWForm();
            //JsonConverter.Sel. 

             var nplayer = new PrizeData();
			nplayer.DBID = DBID;
            var jsondata = JsonConvert.SerializeObject(nplayer);
             //string jsondata =  JsonUtility.ToJson(rewarded, true);
            // Debug.Log(jsondata);

            form.AddField("data", jsondata);   
            UnityWebRequest www = UnityWebRequest.Put(Constants.server_backend +  "/game/sendPrize", jsondata);
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.certificateHandler = new CertificateWhore();
            //www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text+ " user data");

                UIGameplayEvents eventUI = GameObject.Find("GameplayEvents").GetComponent<UIGameplayEvents>();
					eventUI.ShowEvent(new GameplayEventData
						{
							Name        = "Token Prize!",
							Description = www.downloadHandler.text,
							Color       = Color.green,
							Sound       = null,
						}, false, true);

                www.Dispose();
            
            }
        }

        //----------------------------------- CLAIM CHEST

        public void customPrize(string DBID, string internalId, string nickname,string description){
            
            StartCoroutine(claimCustomPrize(DBID,internalId,nickname,description));
        }
        public static Action<string> claimedReturn;
        public IEnumerator claimCustomPrize(string DBID, string internalId, string nickname,string description)
        {
            Debug.Log("Sending Custom Prize");
            WWWForm form = new WWWForm();
            //JsonConverter.Sel. 

            var nplayer = new CustomPrizeData();
			nplayer.DBID = DBID;
            nplayer.internalId = internalId;
            nplayer.nickname = nickname;

            var jsondata = JsonConvert.SerializeObject(nplayer);
             //string jsondata =  JsonUtility.ToJson(rewarded, true);
            // Debug.Log(jsondata);

            form.AddField("data", jsondata);   
            UnityWebRequest www = UnityWebRequest.Put(Constants.server_backend +  "/prizesc/claimPrize", jsondata);
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.certificateHandler = new CertificateWhore();
            //www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);

            }
            else
            {
              
                claimedReturn?.Invoke(internalId);
                Debug.Log(www.downloadHandler.text+ " user data");

                
          

             /*   UIGameplayEvents eventUI = GameObject.Find("GameplayEvents").GetComponent<UIGameplayEvents>();
					eventUI.ShowEvent(new GameplayEventData
						{
							Name        = "Chest Prize!",
							Description = www.downloadHandler.text,
							Color       = Color.green,
							Sound       = null,
						}, false, true);

                www.Dispose();*/
            
            }
        }


        //----------------------------------- CLAIM SPINWHEEL
        
        public void spinPrize(string DBID, string internalId, string nickname,string description){
            
            StartCoroutine(claimSpinPrize(DBID,internalId,nickname,description));
        }
        public static Action<string> spinClaimedReturn;
        public IEnumerator claimSpinPrize(string DBID, string internalId, string nickname,string description)
        {
            WWWForm form = new WWWForm();
            var nplayer = new CustomPrizeData();
			nplayer.DBID = DBID;
            nplayer.internalId = internalId;
            nplayer.nickname = nickname;
            var jsondata = JsonConvert.SerializeObject(nplayer);

            form.AddField("data", jsondata);   
            UnityWebRequest www = UnityWebRequest.Put(Constants.server_backend +  "/prizesSpin/claimPrize", jsondata);
            www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("Content-Type", "application/json");
            www.SetRequestHeader("Accept", "application/json");
            www.certificateHandler = new CertificateWhore();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                spinClaimedReturn?.Invoke(internalId);
                Debug.Log(www.downloadHandler.text+ " user data");
            }
        }


        //--------------------------------------------------
      /*  public void gptRequest(string prompt, ChatNewGui context){
            StartCoroutine(gptProc(prompt, context));
        }
        public static Action<String, ChatNewGui> OnGPTresponse ;
       
        public IEnumerator gptProc(string prompt, ChatNewGui context)
        {
            Debug.Log("Sending GPT request");
     
            WWWForm form = new WWWForm();
            form.AddField("prompt", prompt);   
           // byte[] postData = System.Text.Encoding.UTF8.GetBytes (form);
            //var jsondata = JsonConvert.SerializeObject(form);
            //string jsondata =  JsonUtility.ToJson(form, true);
            // Debug.Log(jsondata);

            //form.AddField("data", jsondata);   
           Debug.Log(Constants.server_backend +  "/gpt/generate" + " --- " + prompt);
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend +  "/gpt/generate", form);
            //www.method = UnityWebRequest.kHttpVerbPOST;
           // www.SetRequestHeader("Content-Type", "application/json");
            //www.SetRequestHeader("Accept", "application/json");
            www.certificateHandler = new CertificateWhore();
            //www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                //Debug.Log(www.downloadHandler.text+ " user gpt");
                var res = JsonUtility.FromJson<gptResponse>(www.downloadHandler.text).result;
                OnGPTresponse?.Invoke(res,context);
                www.Dispose();
            
            }
        }
        //--------------------------------------------------
*/

        public void gptRequestNPC(string prompt){
            StartCoroutine(gptNPC(prompt));
        }
         public static Action<String,String> OnGPTNPC ;
         public IEnumerator gptNPC(string prompt,string user="")
        {     
            WWWForm form = new WWWForm();
            form.AddField("prompt", prompt);   
            Debug.Log(Constants.server_backend +  "/gpt/generate" + " --- " + prompt);
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend +  "/gpt/generate", form);
            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var res = JsonUtility.FromJson<gptResponse>(www.downloadHandler.text).result;
                OnGPTNPC?.Invoke(res,user);
                www.Dispose();
            
            }
        }


         //-------------  TOURNAMENTS LIST ALL EVENTS -------------------------------------
        public void tlistAllRequest(string status,string user){
            StartCoroutine(tlAllProc(status,user));
        }
        public static Action<Tournaments> OnTLAllresponse ;
        public IEnumerator tlAllProc(string status,string user)
        {
            WWWForm form = new WWWForm();
            var filterF = ""; 
            if(status!=""){
                filterF += "status=" + status + "&"; 
            } 
           
            UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend +  "/tournaments/listall?" + filterF);
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                //var res = JsonConvert.DeserializeObject<Tournaments>(www.downloadHandler.text).tournaments;
                var res = JsonUtility.FromJson<Tournaments>("{\"tournaments\":" + www.downloadHandler.text +  "}");
                
                www.Dispose();
                OnTLAllresponse?.Invoke(res);  
            }
        }
        //--------------------------------------------------

        //-------------  TOURNAMENTS LIST -------------------------------------
        public void tlistRequest(string status,string user,bool suscribed){
            StartCoroutine(tlProc(status,user,suscribed));
        }
        public static Action<Tournaments> OnTLresponse ;
        public IEnumerator tlProc(string status,string user,bool suscribed)
        {
            Debug.Log("Sending Tournament List request - " + status + "- " + user + " " + suscribed + " - " );
            WWWForm form = new WWWForm();
            //form.AddField("prompt", prompt);  
            var filterF = ""; 
            if(status!=""){
                filterF += "status=" + status + "&"; 
            } 
              if(suscribed){
                filterF += "suscribed=" + suscribed + "&"; 
            }
            UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend +  "/tournaments/list?" + filterF);
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text);
                //var res = JsonConvert.DeserializeObject<Tournaments>(www.downloadHandler.text).tournaments;
                var res = JsonUtility.FromJson<Tournaments>("{\"tournaments\":" + www.downloadHandler.text +  "}");
                
                www.Dispose();
                OnTLresponse?.Invoke(res);  
            }
        }
        //--------------------------------------------------



        //-------------  TOURNAMENTS LIST UNSUSCRIBED -------------------------------------
        public void tlistUNRequest(string status,string user,bool suscribed, bool unsuscribed){
            StartCoroutine(tlUNProc(status,user,suscribed,unsuscribed));
        }
        public static Action<Tournaments> OnTLUNser_response ;
        public IEnumerator tlUNProc(string status,string user,bool suscribed, bool unsuscribed)
        {
            WWWForm form = new WWWForm();
            //form.AddField("prompt", prompt);  
            var filterF = ""; 
            if(status!=""){
                filterF += "status=" + status + "&"; 
            }
            if(user!=""){
                filterF += "user=" + user + "&"; 
            }
      
            if(suscribed){
                filterF += "suscribed=false&"; 
            }

            UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend +  "/tournaments/list?" + filterF);
//            Debug.Log("final request" + Constants.server_backend +  "/tournaments/list?" + filterF);
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateWhore();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
//                Debug.Log(www.downloadHandler.text);
                //var res = JsonConvert.DeserializeObject<Tournaments>(www.downloadHandler.text).tournaments;
                var res = JsonUtility.FromJson<Tournaments>("{\"tournaments\":" + www.downloadHandler.text +  "}");
                
                www.Dispose();

                    OnTLUNser_response?.Invoke(res);
            }
        }
        //--------------------------------------------------


        //-------------  TOURNAMENTS LIST UNSUSCRIBED -------------------------------------
        public void tlistSUSRequest(string status,string user,bool suscribed, bool unsuscribed){
            StartCoroutine(tlSUSProc(status,user,suscribed,unsuscribed));
        }
        public static Action<Tournaments> OnTLSUSser_response ;
        public IEnumerator tlSUSProc(string status,string user,bool suscribed, bool unsuscribed)
        {
            WWWForm form = new WWWForm();
            //form.AddField("prompt", prompt);  
            var filterF = ""; 
            if(status!=""){
                filterF += "status=" + status + "&"; 
            }
            if(user!=""){
                filterF += "user=" + user + "&"; 
            }
            if(suscribed){
                filterF += "suscribed=true&"; 
            }

//            Debug.Log(filterF+ " filger SUSCRIBED");
            UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend +  "/tournaments/list?" + filterF);
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateWhore();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
//                Debug.Log(www.downloadHandler.text);
                //var res = JsonConvert.DeserializeObject<Tournaments>(www.downloadHandler.text).tournaments;
                var res = JsonUtility.FromJson<Tournaments>("{\"tournaments\":" + www.downloadHandler.text +  "}");
                
                www.Dispose();

                    OnTLSUSser_response?.Invoke(res);
            }
        }
        //--------------------------------------------------


         //-------------  TOURNAMENS DETAIL -------------------------------------
        public void tlDetailRequest(string tournamentId){
            StartCoroutine(tlDetailProc(tournamentId));
        }
        public static Action<TournamentDetail> OnTLDresponse ;
        public IEnumerator tlDetailProc(string tournamentId)
        {
            Debug.Log("Sending Tournament List request" + Constants.server_backend +  "/tournaments/" + tournamentId);
            WWWForm form = new WWWForm();
            //form.AddField("prompt", prompt);   

            UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend +  "/tournaments/" + tournamentId);
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateWhore();
            //www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var res = JsonUtility.FromJson<TournamentDetail>(www.downloadHandler.text);
                Debug.Log(www.downloadHandler.text);
                OnTLDresponse?.Invoke(res);
                www.Dispose();
            }
        }
        //--------------------------------------------------



         //-------------  TOURNAMENS PLAYER SCOREBOARDS -------------------------------------
        public void tournamentPlayers(string tournamentId, int count){
            StartCoroutine(tpProc(tournamentId,count));
        }
        public static Action<TournamentPlayers> OnTPresponse ;
        public IEnumerator tpProc(string tournamentId,int count=1000)
        {
           
            WWWForm form = new WWWForm();
            //form.AddField("prompt", prompt);   

            UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend +  "/tournaments/" + tournamentId + "/players?count=" + count);
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateWhore();
            //www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var res = JsonUtility.FromJson<TournamentPlayers>(www.downloadHandler.text);
                Debug.Log(www.downloadHandler.text);
                OnTPresponse?.Invoke(res);
                www.Dispose();
            }
        }
        //--------------------------------------------------


           //-------------  TOURNAMENS PLAYER SCOREBOARDS -------------------------------------
        public void tournamentPlayerInfo(string tournamentId, string playerId){
            StartCoroutine(tpiProc(tournamentId,playerId));
        }
        public static Action<TournamentPlayer> OnTPIresponse ;
        public static Action<String> OnTPInotfound ;
        public IEnumerator tpiProc(string tournamentId,string playerId)
        {
           Debug.Log(tournamentId + " *** " + playerId);
            WWWForm form = new WWWForm();
            //form.AddField("prompt", prompt);   

            UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend +  "/tournaments/" + tournamentId + "/" + playerId);
            //www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateWhore();
            //www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
               // Debug.Log(www.downloadHandler.text);
               // Debug.Log(www.error);
                OnTPInotfound?.Invoke(www.downloadHandler.text);
            }
            else
            {
                var res = JsonUtility.FromJson<TournamentPlayer>(www.downloadHandler.text);
                Debug.Log(www.downloadHandler.text);
                OnTPIresponse?.Invoke(res);
                www.Dispose();
            }
        }
        //--------------------------------------------------



        //-------------  TOURNAMENS SUSCRIBE -------------------------------------
        public void tlSignup(string tournamentId){
            StartCoroutine(tlSignupProc(tournamentId));
        }
        public static Action<string> OnTLSignresponse ;
        public IEnumerator tlSignupProc(string tournamentId)
        {
            Debug.Log("Sending Tournament Signup request2" + Constants.server_backend +  "/tournaments/" + tournamentId  + "/suscribe/");
            WWWForm form = new WWWForm();
            //form.AddField("prompt", "prompt");   

            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend +  "/tournaments/" + tournamentId + "/suscribe/",form);
            //www.SetRequestHeader("Content-Type", "application/json");
           // www.certificateHandler = new CertificateWhore();
            www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.downloadHandler.text);
                Debug.Log(www.error+ "error ")  ;
            }
            else
            {
                var res = www.downloadHandler.text;
                Debug.Log(www.downloadHandler.text + " RES SIGNUP");
                OnTLSignresponse?.Invoke(res);
                www.Dispose();
            }
        }
        //--------------------------------------------------

        //-------------  TOURNAMENS SUSCRIBE -------------------------------------
        public void tlUnsuscribe(string tournamentId){
            StartCoroutine(tlUnsuscribeProc(tournamentId));
        }
        public static Action<string> OnTLUNSresponse ;
        public IEnumerator tlUnsuscribeProc(string tournamentId)
        {
            Debug.Log("Sending Tournament Signup request" + Constants.server_backend +  "/tournaments/" + tournamentId  + "/unsuscribe");
            WWWForm form = new WWWForm();
            //form.AddField("prompt", prompt);   

            UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend +  "/tournaments/" + tournamentId + "/unsuscribe");
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateWhore();
            www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var res = www.downloadHandler.text;
                Debug.Log(www.downloadHandler.text);
                OnTLUNSresponse?.Invoke(res);
                www.Dispose();
            }
        }
        //--------------------------------------------------


        // TOURNAMENT HEARTBEAT
        public void tournamentHearbeat(string userid){
            StartCoroutine(getHB(userid));
        }
         public static Action<TournamentMatchInfo> OnHeartBeatTournament ;
         public IEnumerator getHB(string userid)
        {     
            WWWForm form = new WWWForm();
            form.AddField("_id", userid);   
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend +  "/tournaments/heartbeat", form);
            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
//                Debug.Log(www.downloadHandler.text + " LISTA REGRESO");
                if(www.downloadHandler.text!="null"){
                    var res = JsonUtility.FromJson<TournamentMatchInfo>(www.downloadHandler.text);
                    OnHeartBeatTournament?.Invoke(res);
                }
                
                www.Dispose();
            
            }
            
        }


           //-------------  TOURNAMENS SUSCRIBE -------------------------------------
        public void solanaPrice(){
            StartCoroutine(solanaPriceProc());
        }
        public static Action<solanaData> OnSolanaPriceResponse ;
        public IEnumerator solanaPriceProc()
        {
          
            WWWForm form = new WWWForm();

            UnityWebRequest www = UnityWebRequest.Get("https://api.coingecko.com/api/v3/simple/price?ids=solana&vs_currencies=usd");
            www.SetRequestHeader("Content-Type", "application/json");
            www.certificateHandler = new CertificateWhore();

            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                var res = JsonUtility.FromJson<solanaData>(www.downloadHandler.text);
                Debug.Log(www.downloadHandler.text);

                OnSolanaPriceResponse?.Invoke(res);
                www.Dispose();
            }
        }



    // GET CHESTS 

        //-------------  CHEST PRIZES -------------------------------------
        public void getChests(){
            StartCoroutine(chestsProc());
        }
        public static Action<Chests> OnChestResponse ;
        public IEnumerator chestsProc()
        {
           WWWForm form = new WWWForm();
            form.AddField("serverId", "server1");   
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend +  "/prizesc/getPrizesForLobby", form);
            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text + " LISTA REGRESO");
                if(www.downloadHandler.text!="null"){
                    var res = JsonUtility.FromJson<Chests>(www.downloadHandler.text);
                    OnChestResponse?.Invoke(res);
                }
                www.Dispose();
            }
        }


          //-------------  CHEST PRIZES FOR SPIN -------------------------------------
        public void getSpin(){
            StartCoroutine(spinProc());
        }
        public static Action<SpinPrizes> OnSpinResponse ;
        public IEnumerator spinProc()
        {
           WWWForm form = new WWWForm();
            form.AddField("serverId", "server1");   
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend +  "/prizesSpin/getPrizesForLobby", form);
            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                Debug.Log(www.downloadHandler.text + " LISTA REGRESO");
                if(www.downloadHandler.text!="null"){
                    var res = JsonUtility.FromJson<SpinPrizes>(www.downloadHandler.text);
                    OnSpinResponse?.Invoke(res);
                }
                www.Dispose();
            }
        }


// ----------------- ACTIVITIES
    public static Action<Activities,string> OnActivitiesFetched;
    public static Action<ActivityData> OnActivityFetched;
    public static Action<ActivityData> OnActivityAdded;
    public static Action<ActivityData> OnActivityUpdated;

    public void AddActivityStart(ActivityData activityData){
        StartCoroutine(AddActivity(activityData));
    }
    public void UpdateActivityStart(string activityId, ActivityData updatedActivityData){
        StartCoroutine(UpdateActivity(activityId, updatedActivityData));
    }
    public void GeLastActivitiesForUser(string type,int count){
        StartCoroutine(GetAllActivitiesForUser(type,count));
    }
    
    public void GetActivityStart(string activityId){
        StartCoroutine(GetActivity(activityId));
    }

    private IEnumerator AddActivity(ActivityData activityData)
    {
        Debug.Log("Adding activity");
        string json = JsonUtility.ToJson(activityData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        UnityWebRequest www = new UnityWebRequest(Constants.server_backend + "/activities/addActivity", "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("token", PersistentStorage.GetString("token"));

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            ActivityData addedActivity = JsonUtility.FromJson<ActivityData>(www.downloadHandler.text);
            OnActivityAdded?.Invoke(addedActivity);
        }else{
            Debug.Log("Error adding activity");
            Debug.Log(www.error);
        }
    }

    private IEnumerator UpdateActivity(string activityId, ActivityData updatedActivityData)
    {
        string json = JsonUtility.ToJson(updatedActivityData);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        UnityWebRequest www = UnityWebRequest.Put(Constants.server_backend + "/activities/updateActivity/" + activityId, jsonToSend);
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("token", PersistentStorage.GetString("token"));

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            ActivityData updatedActivity = JsonUtility.FromJson<ActivityData>(www.downloadHandler.text);
            OnActivityUpdated?.Invoke(updatedActivity);
        }
    }

    private IEnumerator GetAllActivitiesForUser(string type,int count)
    {
       
        WWWForm form = new WWWForm();
        form.AddField("type", type);   
        form.AddField("count", count);   
        UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/activities/activities", form);
        www.certificateHandler = new CertificateWhore();
        www.SetRequestHeader("token", PersistentStorage.GetString("token"));
        
        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            Debug.Log(www.downloadHandler.text); 
            Activities activities = JsonUtility.FromJson<Activities>(www.downloadHandler.text);

            foreach (var activity in activities.activityData) {
                activity.ts = activity.GetDateTime().ToString(); // Or modify ts to store DateTime directly
            }

            OnActivitiesFetched?.Invoke(activities,type);
        }else{
            Debug.Log(www.error);
        }
    }

    private IEnumerator GetActivity(string activityId)
    {
        UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend + "/activities/activity/" + activityId);
        www.SetRequestHeader("token", PersistentStorage.GetString("token"));

        yield return www.SendWebRequest();

        if (www.result == UnityWebRequest.Result.Success)
        {
            ActivityData activity = JsonUtility.FromJson<ActivityData>(www.downloadHandler.text);
            OnActivityFetched?.Invoke(activity);
        }
    }

    //---------------------- SCORES ------------------------------
     public static Action<Activities,string> OnScoreRegistered;
    public static Action<topScores> OnTopScores;
   

    public void RegisterScoreStart(MinigScore minigScore){
        StartCoroutine(AddScore(minigScore));
    }
    public void topScoreStart(string gametype){
        StartCoroutine(topScores(gametype));
    }

    private IEnumerator AddScore(MinigScore minigScore)
    {
        Debug.Log("Adding score");
        string json = JsonUtility.ToJson(minigScore);
        byte[] jsonToSend = new System.Text.UTF8Encoding().GetBytes(json);
        UnityWebRequest www = new UnityWebRequest(Constants.server_backend + "/scores/registerGameScore", "POST");
        www.uploadHandler = (UploadHandler)new UploadHandlerRaw(jsonToSend);
        www.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
        www.SetRequestHeader("Content-Type", "application/json");
        www.SetRequestHeader("token", PersistentStorage.GetString("token"));
        yield return www.SendWebRequest();
        if (www.result == UnityWebRequest.Result.Success){
            ActivityData addedActivity = JsonUtility.FromJson<ActivityData>(www.downloadHandler.text);
            OnActivityAdded?.Invoke(addedActivity);
        }else{
            Debug.Log("Error adding activity");
            Debug.Log(www.error);
        }
    }

     private IEnumerator topScores(string gametype)
    {
            UnityWebRequest www = UnityWebRequest.Get(Constants.server_backend + "/scores/topScores/" + gametype);
            //www.SetRequestHeader("token", PersistentStorage.GetString("token"));
            yield return www.SendWebRequest();
            if (www.result == UnityWebRequest.Result.Success){
                topScores topScores = JsonUtility.FromJson<topScores>(www.downloadHandler.text);
                OnTopScores?.Invoke(topScores);
            }else{
                Debug.Log("Error adding activity");
                Debug.Log(www.error);
            }
        }

    }
       
    }

     [Serializable]
     public class topScores{
        public MinigScore[] scores;
     }

     [Serializable]
     public class MinigScore{
        public float score;
        public string typeofGame;
        public string username;
        public string ts;
        public float time;
     }

     [Serializable]
     public class Activities{
        public ActivityData[] activityData;
     }

     [Serializable]
    public class ActivityData {
        public string name;
        public string type;
        public string ts;
        public string description;
        public string extraData1;
        public string extraData2;

        public DateTime GetDateTime() {
        return DateTime.Parse(ts);
        }
    }

    


     [Serializable]
    public class solanaData {
        public solanaCurrency solana;
    }
    [Serializable]
    public class solanaCurrency {
        public float usd;
    }

    [Serializable]
    public class Chests {
        public ChestData[] chestArr;
    }

    [Serializable]
    public class ChestData {
        public string internalId;
        public string item_type;
        public string item_3d;
        public string item_material;
        public string description;
        public string status ="";
        public string username = "";
        public string claimDate= "";
    }

    // Prize Serializables
    [Serializable]
    public class SpinPrizes {
        public SpinData[] SpinArr;
    }

    [Serializable]
    public class SpinData {
        public string internalId;
        public string item_type;
        public string item_3d;
        public string item_material;
        public string name;
        public string description;
        public string status ="";
        public string username = "";
        public string claimDate= "";
    }
    

    [Serializable]
    public class gptRes {

    }


    


 /*string jsondata = JsonUtility.ToJson(rewarded);
            var request = new UnityWebRequest(Constants.server_backend +  "/game/endmatch", "POST");
        byte[] bodyRaw = Encoding.UTF8.GetBytes(jsondata);
        request.uploadHandler = (UploadHandler) new UploadHandlerRaw(bodyRaw);
        request.downloadHandler = (DownloadHandler) new DownloadHandlerBuffer();
        request.SetRequestHeader("Content-Type", "application/json");
        yield return request.SendWebRequest();
        Debug.Log("Status Code: " + request.responseCode);*/
    

