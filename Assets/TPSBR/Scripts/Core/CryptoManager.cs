namespace TPSBR
{
    using UnityEngine;
    using UnityEngine.Networking;
    using System.Collections;
    using System.Runtime.InteropServices;
    using System;
    using static UnityEngine.UIElements.UxmlAttributeDescription;
    using System.Collections.Generic;
    using System.Linq;
    using Newtonsoft.Json;
    using ReadyPlayerMe;

    public class CryptoManager : ContextBehaviour
    {
        [DllImport("__Internal")]
        private static extern void ClickPhantom();
        [DllImport("__Internal")]
        private static extern void CheckIfPhantom();
         [DllImport("__Internal")]
        private static extern void CheckIfParticle();
        [DllImport("__Internal")]
        private static extern void ClickGoogle();
        [DllImport("__Internal")]
        private static extern void OnGoogleLogin();
        [DllImport("__Internal")]
        private static extern void OnGoogleLoginError();
        [DllImport("__Internal")]
        private static extern void bloomieTransaction(float amount, string tokenId, string tokenName);

         [DllImport("__Internal")]
        private static extern void solanaTransaction(float solPrice,float bloomiesAmount,string userWallet);

        [DllImport("__Internal")]
        private static extern void ClickParticle();

         [DllImport("__Internal")]
        private static extern void ClickTokenMarket();

        [DllImport("__Internal")]
        public static extern void OpenURL(string url);

        // public TextMeshProUGUI user_texto;
        // public GameObject loader;
        // public Button phantom_button;
        //public Button test_server_button;

        //public Button walletAccess;

        public static CryptoManager instance;


        // Start is called before the first frame update
        void Start()
        {
            instance = this;
            // PlayerPrefs.DeleteAll();
            //        login_button.onClick.AddListener(LoginClick);
            // phantom_button.onClick.AddListener(LoginPhantomClick);
            //test_server_button.onClick.AddListener(TestServer);
            // CheckIfPhantomIs();


            //        TestServer();


#if UNITY_WEBGL == true && UNITY_EDITOR == false
        //test_server_button.gameObject.SetActive(false);
#endif


        }

        public void startManager()
        {
           // Debug.Log(PersistentStorage.GetString("wallet"));

            if (PersistentStorage.GetString("_id").Length > 3)
            {
             //   Debug.Log("Starting coroutine crypto wallet retrive");
                //StartCoroutine(LoginWallet(PersistentStorage.GetString("wallet")));
                StartCoroutine(LoginUser(PersistentStorage.GetString("_id")));
                StartCoroutine(refreshUserData());
            }

            /**else if (PersistentStorage.GetString("googleId") != "")
            {
                StartCoroutine(LoginGoogle(PersistentStorage.GetString("googleId")));
            }*/
        }

        private bool refresh=true;
        IEnumerator refreshUserData(){
            while(refresh){
                getUserData(false);
                Debug.Log("Refreshing user data");
                yield return new WaitForSeconds(20);

            }
        }


        public void TestServer() // ASUMING WALLET LOGIN RESPONSE
        {
            //StartCoroutine(LoginWallet(Constants.server_test_wallet));
            OnPhantomCallback?.Invoke(Constants.server_test_wallet);
        }

        public void TestGoogle() // ASUMING WALLET LOGIN RESPONSE
        {
            StartCoroutine(LoginGoogle("marruendarol@gmail.com"));
        }

        public void GetNFTs(string wallet)  // PHANTOM WALLET RESPONSE
        {
            // StartCoroutine(GetNFTsFromSolana(wallet));
            // save wallet 
            StartCoroutine(LoginWallet(wallet));
        }

        public void sendTutorialReward(string wallet)
        {
            StartCoroutine(SendBloomies(wallet));
        }



        public void OnGoogleLogin(string email)
        {
            //Debug.Log(email + "email google response");
            StartCoroutine(LoginGoogle(email));
        }
        public void OnGoogleLoginError(string email)
        {
            //Debug.Log(email + "email google response error");
        }



        public void LoginPhantomClick()
        {
            #if UNITY_WEBGL == true && UNITY_EDITOR == false
                            ClickPhantom();
            #endif
        }
        public void LoginGoogleClick()
        {
            #if UNITY_WEBGL == true && UNITY_EDITOR == false
                            ClickGoogle();
            #endif
            }
        // Particle Network
        public void LoginParticleClick()
        {
            #if UNITY_WEBGL == true && UNITY_EDITOR == false
                            ClickParticle();

            #else
                        CryptoManager.instance.TestServer();
            #endif
        }

         public void LauncheTokenMarket()
        {
            Debug.Log("Launchig webgl Token Market");
            #if UNITY_WEBGL == true && UNITY_EDITOR == false
                    ClickTokenMarket();
            #else
                  
            #endif
        }

         public static Action<String> OnPhantomCallback;
        public void OnPhantom(string wallet ){ // Phantom wallet web return 
            OnPhantomCallback?.Invoke( wallet );
        }   


        public void ParticleConnection(string wallet)
        {
            //Debug.Log("Particle connection " + wallet);
            //StartCoroutine(LoginWallet(wallet));  !***************
        }



        public void IsPhantom()
        {
            //phantom_button.gameObject.SetActive(true);
        }

         public static Action<String> OnIsParticleResponse;
         public void IsParticle(string value)
        {
            
           Debug.Log("Is partyicle " + value);
           OnIsParticleResponse?.Invoke(value);
           //Context.PlayerData.userData.ParticleConnection = value.ToString();
        }


        public void CheckIfPhantomIs()
        {
        #if UNITY_WEBGL == true && UNITY_EDITOR == false
                        CheckIfPhantom();
        #endif
        }

        public void CheckIfParticleIs()
        {
        #if UNITY_WEBGL == true && UNITY_EDITOR == false
        Debug.Log("Starting webgl call");
                        CheckIfParticle();
        #endif
        }

        public static void AskbloomieTransaction(float amount, string tokenId, string tokenName)
        {

                    //Debug.Log("Asking bloomie transaction " + amount + " tokenId " + tokenId + " tokenName " + tokenName);
        #if UNITY_WEBGL == true && UNITY_EDITOR == false
                        //Debug.Log("__Invkoke unity" + amount + " - " + tokenId + " - " + tokenName);
                        bloomieTransaction(amount,tokenId,tokenName);
        #endif
        }

        // Tournament withdraw
        public static Action<String> OnWithdrawResponse;
        public void RequestWithdraw(string tournamentId){
            StartCoroutine(RequestWithdrawIE(tournamentId));
        }
        public IEnumerator RequestWithdrawIE(string tournamentId){
            WWWForm form = new WWWForm();
            form.AddField("tournamentId", tournamentId);
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/tournaments/"+ tournamentId +"/unsuscribe", form);
            www.SetRequestHeader("token", PersistentStorage.GetString("token"));
            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success){
                OnWithdrawResponse?.Invoke("error");
                Debug.Log(www.error);
            }
            else{
                OnWithdrawResponse?.Invoke("ok");
                www.Dispose();
                StartCoroutine(userData(true));
            }
        }


        public static Action<String> OnTokenResponse;
        public void OnTokenTransaction(string res)
        {
            OnTokenResponse?.Invoke(res);
        }

        // Pay Solana
        public static void AskSolanaTransaction(float solPrice,float bloomiesAmount,string walletID)
        {
       
            
          Debug.Log("__Invkoke unity" + solPrice + " - " + bloomiesAmount + " - " + walletID);
        #if UNITY_WEBGL == true && UNITY_EDITOR == false
                        
                        solanaTransaction(solPrice,bloomiesAmount,walletID);
        #endif
        }


        public static Action<String> OnSolanaResponse;
        public void OnSolanaTransaction(string res)
        {
            OnSolanaResponse?.Invoke(res);
        }

        //public delegate void NFTReceive();
        //public static event NFTReceive OnReceive;

        public void getNFTSPF(string wallet)
        {
            if (this != null)
            {
                StartCoroutine(GetNFTsFromSolana(wallet));
            }

        }

        public static Action<String> OnReceiveNFTS;

        public IEnumerator GetNFTsFromSolana(string wallet)
        {
            //Debug.Log("Getting items in solaana wallet." + wallet);
            string uri = string.Format(Constants.server_backend + "/crypto/nfts?wallet={0}", wallet);

            // Debug.Log(uri);

            var request = new UnityWebRequest(uri);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("accept", "application/json");

            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("Connection error: " + request.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + request.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                   // Debug.Log("Received: " + request.downloadHandler.text);

                    // NFT[] nfts = JsonUtility.FromJson<NFTS>(request.downloadHandler.text).nfts;

                    NFT[] nfts = Array.FindAll(JsonUtility.FromJson<NFTS>(request.downloadHandler.text).nfts, c => (c.collectionAddress == "bffoiWfiVYfT7b4NpXexhLmDLC6B6CZNHPtW6R1R46y"));
                    Array.Resize(ref nfts, 30);


                    OnReceiveNFTS?.Invoke(request.downloadHandler.text);

                    break;
            }
        }


        public static Action<String> OnBloomies;
        private IEnumerator GetBloomies(string wallet)
        {
           // Debug.Log("Getting bloomies in solaana wallet." + wallet);
            string uri = string.Format(Constants.server_backend + "/crypto/bloomies?wallet={0}", wallet);

            var request = new UnityWebRequest(uri);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("accept", "application/json");

            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("Connection error: " + request.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + request.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                   // Debug.Log("Received: " + request.downloadHandler.text);

                    PlayerData playerData = Global.PlayerService.PlayerData;
                    playerData.userData.cbloomies = float.Parse(request.downloadHandler.text);
                    //NFT[] nfts = JsonUtility.FromJson<NFTS>(request.downloadHandler.text);
                    OnBloomies?.Invoke(request.downloadHandler.text);


                    break;
            }
        }


        public void CurrenciesQ(string _id){
            StartCoroutine(GetCurrencies(_id));
        }
        // GET WALLET CURRENCIES
        public static Action<Currencies> OnCurrencies;
        public IEnumerator GetCurrencies(string referenceId)
        {
           // Debug.Log("Getting bloomies in solaana wallet." + wallet);
            string uri = string.Format(Constants.server_backend + "/clients/GScurrencies/" + referenceId);

            var request = new UnityWebRequest(uri);
            request.downloadHandler = (DownloadHandler)new DownloadHandlerBuffer();
            request.SetRequestHeader("accept", "application/json");

            yield return request.SendWebRequest();

            switch (request.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("Connection error: " + request.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + request.error);
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + request.error);
                    break;
                case UnityWebRequest.Result.Success:
                   // Debug.Log("Received: " + request.downloadHandler.text);

                    PlayerData playerData = Global.PlayerService.PlayerData;

                    Debug.Log(request.downloadHandler.text);
                    Debug.Log("currencies return");

                    //playerData.userData.cbloomies = float.Parse(request.downloadHandler.text);
                    //NFT[] nfts = JsonUtility.FromJson<NFTS>(request.downloadHandler.text);
                    //OnBloomies?.Invoke(request.downloadHandler.text);
                    Currencies currencies = JsonUtility.FromJson<Currencies>(request.downloadHandler.text);
                     float bloomieamount = GetCurrencyAmountByName(currencies, "Bloomie");
                    setBloomies(bloomieamount.ToString());
                    OnCurrencies?.Invoke(currencies);
                    break;
            }
        }

        public float GetCurrencyAmountByName(Currencies currencies, string currencyName)
        {
            if(currencies.data!=null){
                foreach (Currency currency in currencies.data)
            {
                if (currency.name == currencyName)
                {
                    return currency.amount;
                }
            }
                Debug.LogWarning($"Currency with name '{currencyName}' not found.");
            }
            
            return 0;  
        }

        public void setBloomies(string amount)
        {
            OnBloomies?.Invoke(amount);
        }



        public static Action<String> OnLoginCallback;

        private IEnumerator LoginUser(string _id)
        {
           
            WWWForm form = new WWWForm();
            form.AddField("_id", _id);
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/loginUser", form);
            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success){
                Debug.Log(www.error);
            }
            else{
                JSONWebToken token = JsonUtility.FromJson<JSONWebToken>(www.downloadHandler.text);
                PersistentStorage.SetString("token", token.token);  
                PersistentStorage.SetString("_id", _id); 
                OnLoginCallback?.Invoke(_id);
                www.Dispose();
                StartCoroutine(userData(true));
            }
        }

        private IEnumerator LoginWallet(string wallet)
        {
           // Debug.Log(wallet + " wallet to login");
            WWWForm form = new WWWForm();
            form.AddField("wallet", wallet);
            // form.AddField("googleId", PersistentStorage.GetString("googleId"));
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/loginWallet", form);
            //www.method = UnityWebRequest.kHttpVerbPOST;
            //www.SetRequestHeader("Content-Type", "application/json");
            //www.SetRequestHeader("accept", "application/json");
            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                JSONWebToken token = JsonUtility.FromJson<JSONWebToken>(www.downloadHandler.text);

                PersistentStorage.SetString("token", token.token);  // JSON WEB TOKEN
                PersistentStorage.SetString("wallet", wallet);

                //Debug.Log("Login Return " + www.downloadHandler.text );
                OnLoginCallback?.Invoke(wallet);
                //Context.PlayerData.WalletID = wallet;

                www.Dispose();
                StartCoroutine(userData(true));
            }
        }

        private IEnumerator LoginGoogle(string googleId)
        {

            WWWForm form = new WWWForm();
            form.AddField("googleId", googleId);
            form.AddField("wallet", PersistentStorage.GetString("wallet"));

            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/loginGoogle", form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                JSONWebToken token = JsonUtility.FromJson<JSONWebToken>(www.downloadHandler.text);
                PersistentStorage.SetString("token", token.token);  // JSON WEB TOKEN
                PersistentStorage.SetString("googleId", googleId);

                //Debug.Log("Login Return " + www.downloadHandler.text );
                OnLoginCallback?.Invoke(googleId);
                //Context.PlayerData.WalletID = wallet;

                www.Dispose();
                StartCoroutine(userData(true));
            }
        }

        // EMAIL CODES --------------------------------------------------
        public void requestCode(string email){
            StartCoroutine(requestCodeIE(email));
        }
        public static Action<string> OnCodeCallback;
        private IEnumerator requestCodeIE(string email)
        {
            WWWForm form = new WWWForm();
            form.AddField("email", email);
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/requestCode", form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success){
                Debug.Log(www.error);
            }
            else{
                JSONWebToken token = JsonUtility.FromJson<JSONWebToken>(www.downloadHandler.text);
                OnCodeCallback?.Invoke("code sent");
                www.Dispose();
            }
        }
        // EMAIL VALIDATE CODE
         public void validateCode(string email,string code){
            StartCoroutine(validateCodeIE(email,code));
        }
        public static Action<LoginValidationReturn> OnValidateCodeCallback;
        private IEnumerator validateCodeIE(string email,string code)
        {
            WWWForm form = new WWWForm();
            form.AddField("email", email);
            form.AddField("code", code);

            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/validateCode", form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success){
                Debug.Log(www.error);
                 OnValidateCodeCallback?.Invoke(null);
            }
            else{

                LoginValidationReturn token = JsonUtility.FromJson<LoginValidationReturn>(www.downloadHandler.text);
                Debug.Log(www.downloadHandler.text);
                if(token.status!="2"){
                     PersistentStorage.SetString("token", token.token);
                     
                    if(token.gameShiftWallet!="" ){
                        StartCoroutine(userData(true));
                        OnValidateCodeCallback?.Invoke(token);
                    }else{
                        OnValidateCodeCallback?.Invoke(token);
                    }
                    
                    
                      //
                }else{
                    OnValidateCodeCallback?.Invoke(token);
                }
                

                www.Dispose();
              
            }
        }



        public void getUserData(bool refreshCrypto)
        {
            StartCoroutine(userData(refreshCrypto));
        }

        public static Action<UserData> OnUserData;
        public static Action<String> OnNicknameData;
        public IEnumerator userData(bool refreshCrypto)
        {
            WWWForm form = new WWWForm();
            Debug.Log("Getting user data" + PersistentStorage.GetString("token"));
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/userData", form);
            www.SetRequestHeader("token", PersistentStorage.GetString("token"));
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success){
                Debug.Log(www.error);
            }
            else{
                UserData userData = JsonConvert.DeserializeObject<UserData>(www.downloadHandler.text);
                PlayerData playerData = Global.PlayerService.PlayerData;
                playerData.userData = userData;

                if (userData.nickname != null)
                {
                    playerData.Nickname = userData.nickname;
                }


                PersistentStorage.SetString("googleId", userData.googleId);
                PersistentStorage.SetString("wallet", userData.solanaWallet);
                PersistentStorage.SetString("_id", userData._id);

                //user_texto.SetText("Rorro");

                // if(playerData.userData.nickname!=""){
                //      Context.PlayerData.Nickname =  playerData.userData.nickname;
                //     OnNicknameData?.Invoke(playerData.userData.nickname);
                // }
              
                //StartCoroutine(GetBloomies(Constants.server_test_wallet));

                // Select Wallet
                string walletAvailable = "";

                if (userData.solanaWallet != ""){
                    walletAvailable = userData.solanaWallet;
                }
                if(userData.gameShiftWallet != ""){
                    walletAvailable = userData.gameShiftWallet;
                }
                if(userData.phantomWallet != ""){
                    walletAvailable = userData.phantomWallet;
                }
                
                 playerData.userData.availableWallet = walletAvailable;

                

                if (walletAvailable != "" && refreshCrypto)
                {
                    Debug.Log("wallet available" + walletAvailable);
                    StartCoroutine(GetCurrencies(userData._id));
                    StartCoroutine(GetNFTsFromSolana(walletAvailable));
                    //StartCoroutine(GetBloomies(walletAvailable));
                }

                OnUserData?.Invoke(userData);
                www.Dispose();

                //Open<UINFTSelectionView>();
                //StartCoroutine(GetNFTsFromSolana(Constants.server_test_wallet));
                
            }
        }

        public static Action OnWalletDetail;
        public void WalletClick()
        {

          //  Debug.Log("Opening wallet ui ");
            OnWalletDetail?.Invoke();

            //Open<UIWalletView>();
        }


        public void updateNickname()
        {
            StartCoroutine(userNicknameUpdate());
        }

        public static Action<String> OnUserDataUpdate;
        private IEnumerator userNicknameUpdate()
        {
           // Debug.Log("updating nickname on database");
            WWWForm form = new WWWForm();

            PlayerData playerData = Global.PlayerService.PlayerData;


            form.AddField("nickname", playerData.Nickname);
           // Debug.Log(playerData.Nickname + " nickanem");

            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/updateNickname", form);
            www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                OnUserDataUpdate?.Invoke("user data ready");
                www.Dispose();

            }
        }
        // Update Email
        public void updateEmail()
        {
            StartCoroutine(userEmailUpdate());
        }


        private IEnumerator userEmailUpdate()
        {
           // Debug.Log("updating email on database");
            WWWForm form = new WWWForm();

            PlayerData playerData = Global.PlayerService.PlayerData;


            form.AddField("email", playerData.Email);
           // Debug.Log(playerData.Email + " nickanem");

            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/updateEmail", form);
            www.SetRequestHeader("token", PersistentStorage.GetString("token"));

            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                OnUserDataUpdate?.Invoke("email data ready");
                www.Dispose();

            }
        }


        public void ReedemCode(string code, string userId)
        {
            StartCoroutine(userReedemCode(code,userId));
        }
        public static Action<string> OnReedemCode;
        private IEnumerator userReedemCode(string claimCode, string userId)
        {
           // Debug.Log("redeeming code" + claimCode + "+" + userId);
            WWWForm form = new WWWForm();

            var apiKey1Kin = "5fa29979e30a5e1d79a7bbb78e0b55f3319cdd0f3eb64fb0d3f181fbadbb24a1";

            form.AddField("claimCode", claimCode);
            form.AddField("collectibleId", "7ab86b7582de40be1bfb");
            form.AddField("details", "Claim from play.bloomverse.io");
            form.AddField("gameUserId", userId);

          //  Debug.Log(claimCode + " nickanem");

            UnityWebRequest www = UnityWebRequest.Post("https://api.1kin.io/v1/clients/claimCompleted", form);
            //www.method = UnityWebRequest.kHttpVerbPOST;
            www.SetRequestHeader("onekin-api-key", apiKey1Kin);
            www.SetRequestHeader("Content-Type", "application/x-www-form-urlencoded");
            

            www.certificateHandler = new CertificateWhore();
            yield return www.SendWebRequest();
           // Debug.Log(www.result);
            switch (www.result)
            {
                case UnityWebRequest.Result.ConnectionError:
                    Debug.LogError("Connection error: " + www.error);
                    break;
                case UnityWebRequest.Result.DataProcessingError:
                    Debug.LogError("Error: " + www.error);
                     OnReedemCode?.Invoke("Error");
                     // OnReedemCode?.Invoke("Completed");
                    break;
                case UnityWebRequest.Result.ProtocolError:
                    Debug.LogError("HTTP Error: " + www.error);
                     OnReedemCode?.Invoke("Error");
                     //OnReedemCode?.Invoke("Completed");
                    break;
                case UnityWebRequest.Result.Success:
                     OnReedemCode?.Invoke("Completed");
                    Debug.Log("Received: " + www.downloadHandler.text);
                    break;
            }

            /*if (www.result != UnityWebRequest.Result.Success)
            {
                OnReedemCode?.Invoke("Error");
                //OnReedemCode?.Invoke("Completed");
                Debug.Log(www.error);
            }
            else
            {
                OnReedemCode?.Invoke("Completed");
                www.Dispose();

            }*/
        }

        // SEND TOKEN
        public void SendToken(string walletId, string game_user_id)
        {
            StartCoroutine(sendTokenF(walletId,game_user_id));
        }
        public static Action<string> OnSendToken;
        private IEnumerator sendTokenF(string walletId,string game_user_id)
        {
           // Debug.Log("sending token to user");
            WWWForm form = new WWWForm();
            form.AddField("wallet", walletId);
            form.AddField("gameUserId", game_user_id);

            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/crypto/send_token", form);
             
            //www.SetRequestHeader("Content-Type", "application/json"); 3AYnUPVQzCoyKd9CYW7zcQTFGctgkptzp2KzF1GAVmBB
            www.certificateHandler = new CertificateWhore();
            

            yield return www.SendWebRequest();
            if (www.result != UnityWebRequest.Result.Success)
            {
                //OnReedemCode?.Invoke("Error");
                OnSendToken?.Invoke("Error");
           //     Debug.Log(www.error);
            }
            else
            {
                OnSendToken?.Invoke("Completed");
                www.Dispose();

            }
        }


         // CREATE GS Account
        public void createGSAccount(string referenceId,string email, string externalWalletAddress)
        {
            StartCoroutine(createGS(referenceId,email,externalWalletAddress));
        }
        public static Action<apiResponse> OnGScreation;
        private IEnumerator createGS(string referenceId,string email, string externalWalletAddress)
        {
            WWWForm form = new WWWForm();
            form.AddField("referenceId", referenceId);
            form.AddField("email", email);
            form.AddField("externalWalletAddress", externalWalletAddress);
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/createGMaccount", form);
            www.certificateHandler = new CertificateWhore();
            
            yield return www.SendWebRequest();
            apiResponse apiResponse = JsonUtility.FromJson<apiResponse>(www.downloadHandler.text);

            Debug.Log("Res GS creaction " + www.downloadHandler.text);
            if (www.result != UnityWebRequest.Result.Success)
            {
                OnGScreation?.Invoke(apiResponse);
            }
            else
            {
                
                if(apiResponse.statusCode==409){
                    Debug.Log(apiResponse.desc);
                    OnGScreation?.Invoke(apiResponse);
                    
                }else{
                    OnGScreation?.Invoke(apiResponse);
                    StartCoroutine(userData(true));
                }
                
                
            }
            www.Dispose();
        }

        // Discconnect Google

        public static Action<String> OnUserDisconnect;

        public void disconnectGoogle()
        {
            StartCoroutine(disconnectGoogleCR());
        }
        private IEnumerator disconnectGoogleCR()
        {
          //  Debug.Log("disconnecting google");
            WWWForm form = new WWWForm();
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/discconnectGoogle", form);
            www.SetRequestHeader("token", PersistentStorage.GetString("token"));
            yield return www.SendWebRequest();
            OnUserDisconnect?.Invoke("Google Disconnect");
            setBloomies("0");
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                www.Dispose();
            }
            else
            {
                www.Dispose();
            }
        }

        public void disconnectPhantom()
        {
            StartCoroutine(disconnectPhantomCR());
        }
        private IEnumerator disconnectPhantomCR()
        {
          //  Debug.Log("disconnecting phantom");
            WWWForm form = new WWWForm();
            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/clients/disconnectPhantom", form);
            www.SetRequestHeader("token", PersistentStorage.GetString("token"));
            yield return www.SendWebRequest();
            OnUserDisconnect?.Invoke("Phantom Disconnect");
            setBloomies("0");
            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
            }
            else
            {
                www.Dispose();
            }
        }






        public static Action<String> OnBloomiesSend;

        private IEnumerator SendBloomies(string wallet)
        {
            WWWForm form = new WWWForm();
            form.AddField("wallet", wallet);

            UnityWebRequest www = UnityWebRequest.Post(Constants.server_backend + "/crypto/send_bloomies", form);
            yield return www.SendWebRequest();

            if (www.result != UnityWebRequest.Result.Success)
            {
                Debug.Log(www.error);
                www.Dispose();
            }
            else
            {
               // Debug.Log("Form upload complete!");
                OnBloomiesSend?.Invoke("Receipt");
            }
        }




        public void PrintMessage(string message)
        {
            //Text show_text = campo_texto.GetComponent<Text>();

            //campo_texto.text += message;
            //campo_texto.text += '\n';


            // Debug.Log(message);
        }


    }
}
