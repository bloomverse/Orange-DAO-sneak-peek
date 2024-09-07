
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Text.RegularExpressions;
using TPSBR.UI;
using System.Runtime.InteropServices;

namespace TPSBR
{
    

    public enum loginState{
        notsign = 1,
        expired = 2,
        signed  = 3,
        cryptoConnected = 4,
        signout = 5
    }

    public enum loginMode{
        bloom = 1,
        gameshift = 2,
        phantom = 3
    }

    public class LoginController : ContextBehaviour
    {

         [DllImport("__Internal")]
    	private static extern void CopyToClipboard(string text);
        
        public GameObject loader;
        public TextMeshProUGUI loaderDesc;

        [SerializeField] TextMeshProUGUI WelcomeText;

        [SerializeField] UIWalletView uIWalletView;

        public Button phantom_button;
        public Button google_button;

        public Button disconnectPhantom_button;
        public Button disconnectGoogle_button;

        public Button login_button;
        public Button requestCode_button;
        public Button validateCode_button;


        public TextMeshProUGUI nftCount;
        public TextMeshProUGUI bloomieCount;
        public TextMeshProUGUI solanaWallet;
        public TextMeshProUGUI googleId;
        public CryptoManager cryptoManager;

        
        [SerializeField] UIButton SignUpBT;
        [SerializeField] UIButton LogInBt;

        [SerializeField] GameObject statusConnected;

        public TMP_InputField email;
        public TMP_InputField code1Input;
        public TMP_InputField code2Input;
        public TMP_InputField code3Input;
        public TMP_InputField code4Input;
        public TMP_InputField code5Input;

        private string emailText;
        private string code1Text;
        private string code2Text;
        private string code3Text;
        private string code4Text;
        private string code5Text;

        [SerializeField] GameObject invalidEmailText;
        [SerializeField] GameObject invalidCodeText;
        
        [SerializeField] CanvasGroup PreScreen;
        [SerializeField] CanvasGroup EmailCanvas;
        [SerializeField] CanvasGroup CodeCanvas;
        [SerializeField] CanvasGroup GMlinkCanvas;
        [SerializeField] CanvasGroup InventoryCanvas;

        [SerializeField] UIButton whatisthis;
 
        [SerializeField] UIButton noPhantom;
        [SerializeField] UIButton yesPhantom;

        [SerializeField] UIButton BackBt;

        [SerializeField] UIButton logout;

        private bool testMode = false;

        private string ret_Id;
        private string ret_Email;
        private string ret_SolanaWallet;
        private string ret_GameShiftWallet;
        private string ret_PhantomWallet;

        [SerializeField] UIButton copyToClipBt;
        private string wallet;


        // Start is called before the first frame update
        void Start()
        {
            Debug.Log("Instance ID: " + this.GetInstanceID());
            //PlayerPrefs.DeleteAll();
            
            invalidCodeText.SetActive(false);
            invalidEmailText.SetActive(false);
                         
            phantom_button.SetActive(false);
            google_button.SetActive(false);
           // login_button.onClick.AddListener(login);

            requestCode_button.onClick.AddListener(requestCode);
            validateCode_button.onClick.AddListener(validateCode);

            whatisthis.onClick.AddListener(whatisthisClick);


            logout.onClick.AddListener(logoutF);

            disconnectGoogle_button.SetActive(false);
            disconnectPhantom_button.SetActive(false);

            SetBusy(false, "");
            phantom_button.onClick.AddListener(LoginPhantomClick);
            google_button.onClick.AddListener(GoogleClick);

            SignUpBT.onClick.AddListener(() => showEmailInput("signup"));
            LogInBt.onClick.AddListener(() => showEmailInput("login"));

            copyToClipBt.onClick.AddListener(copyToClipboard);

            BackBt.onClick.AddListener(backLogin);

            yesPhantom.onClick.AddListener(yesPhantomClick);
            noPhantom.onClick.AddListener(noPhantomClick);

            disconnectPhantom_button.onClick.AddListener(disconnectPhantom);
            disconnectGoogle_button.onClick.AddListener(disconnectGoogle);

            CryptoManager.OnCodeCallback += requestCodeCallBack;
            CryptoManager.OnValidateCodeCallback += validateCodeCallBack;

            CryptoManager.OnLoginCallback += loginCallback;
             CryptoManager.OnPhantomCallback += phantomCallback;
            CryptoManager.OnUserData += dataCallback;
            CryptoManager.OnBloomies += setBloomies;
            CryptoManager.OnReceiveNFTS += countBFFs;
            CryptoManager.OnCurrencies += setCurrencies;
            if(cryptoManager==null){
                 cryptoManager = GameObject.Find("CryptoManager").GetComponent<CryptoManager>();
            }

            EmailCanvas.alpha = 0;
            CodeCanvas.alpha = 0;
            InventoryCanvas.alpha = 0;
            PreScreen.alpha = 0;
            GMlinkCanvas.alpha = 0;

            PreScreen.SetActive(false);
            EmailCanvas.SetActive(false);
            CodeCanvas.SetActive(false);
            InventoryCanvas.SetActive(false);
            GMlinkCanvas.SetActive(false);

            if (PersistentStorage.GetString("token") != "")
            {
                Debug.Log("TOKEN PRESENT");
                SetBusy(true, "Loading user data");
                //StartCoroutine(refreshUserData());
                //cryptoManager.getUserData(true);
                statusConnected.SetActive(true);
            }
            else
            {

                //NO TOKEN 
                //EmailCanvas.SetActive(true);
                //EmailCanvas.alpha = 1;
                //phantom_button.SetActive(true);

                PreScreen.SetActive(true);
                PreScreen.alpha = 1;
                statusConnected.SetActive(false);
                Debug.Log("NO TOKEN PRESENT");
                //login_button.SetActive(true);
                //checkStatusButtons();
            }

        }

        private void copyToClipboard(){
            GUIUtility.systemCopyBuffer = wallet;
					
            #if UNITY_WEBGL && !UNITY_EDITOR
            CopyToClipboard(wallet);
            #endif

          
        }

        public void OnOpen(){
                Debug.Log("openinig login controller");
                cryptoManager.getUserData(true);
               // CryptoManager.instance.CurrenciesQ(Global.PlayerService.PlayerData.userData._id);
           // CryptoManager.instance.getUserData(false);
        }

        public void setCurrencies(Currencies currencies){
            Debug.Log("REceiving currencies");
            float bloomieamount = GetCurrencyAmountByName(currencies, "Bloomie");
            setBloomies(bloomieamount.ToString());
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

        private void logoutF(){
          
            PlayerPrefs.DeleteKey("token");
            PlayerPrefs.DeleteKey("_id");
            PlayerPrefs.DeleteKey("wallet");
          
            InventoryCanvas.alpha=0;
            InventoryCanvas.SetActive(false);

            PreScreen.SetActive(true);
            PreScreen.alpha=1;

            UserData userEmpty = new UserData();
            Global.PlayerService.PlayerData.userData = userEmpty;
            CryptoManager.OnUserData?.Invoke(userEmpty);
            CryptoManager.OnUserDisconnect?.Invoke("");
            checkStatusButtons();
            uIWalletView.closeWallet();
            
        }

        private void showEmailInput(string mode){

            if(mode == "login"){
                WelcomeText.SetText("Enter your account email and receive a secure code to log in");
            }
            if(mode == "signup"){
                WelcomeText.SetText("Provide an email to receive a security code, this will create your Bloom account.");
            }

             PreScreen.alpha = 0;
             PreScreen.SetActive(false);
             EmailCanvas.SetActive(true);
             EmailCanvas.alpha = 1;
        }

        private void backLogin(){
             PreScreen.alpha = 1;
             PreScreen.SetActive(true);
             EmailCanvas.SetActive(false);
             EmailCanvas.alpha = 0;
             invalidEmailText.SetActive(false);
        }

        private void whatisthisClick(){
             try{
                GameplayUI gameplayUI = GameObject.Find("GameplayUI").GetComponent<GameplayUI>();
                gameplayUI.infoGameShift();
             }catch(SystemException ex){
                UIMainMenuView menuUI = GameObject.Find("UIMainMenuView").GetComponent<UIMainMenuView>();
                 menuUI.infoDialogGS();
             }
        }
        
        

        private void yesPhantomClick(){

            CryptoManager.OnGScreation += GSret;
             if (testMode == true){
                cryptoManager.TestServer();
                SetBusy(true, "Connecting test mode...");
            }
            else{
                cryptoManager.LoginPhantomClick();
                
            }
             GMlinkCanvas.alpha = 0;
             SetBusy(true, "Waiting for Phantom Wallet response...");
             GMlinkCanvas.SetActive(false);
        }

        private void noPhantomClick(){
            CryptoManager.OnGScreation += GSret;
            CryptoManager.instance.createGSAccount(ret_Id,ret_Email,"");
            GMlinkCanvas.alpha  = 0;
            GMlinkCanvas.SetActive(false);
            SetBusy(true, "Loading account...");
        }

        private void GSret(apiResponse res){
            
            CryptoManager.OnGScreation -= GSret;
            Debug.Log(res.statusCode);
            Debug.Log("res creation ");

            if(res.statusCode==409){
                UIMainMenuView menuUI = GameObject.Find("UIMainMenuView").GetComponent<UIMainMenuView>();
                menuUI.infoDialog(res.desc);
                GMlinkCanvas.alpha = 1;
                GMlinkCanvas.SetActive(true);
                SetBusy(false, "");
            } else{
                GMlinkCanvas.alpha = 0;
                GMlinkCanvas.SetActive(false);
                InventoryCanvas.alpha = 1;
                InventoryCanvas.SetActive(true);
                SetBusy(false, "");
            }
        }

        private void requestCode(){
            invalidEmailText.SetActive(false);
            SetBusy(true, "Requesting code...");
            emailText = email.text;
            requestCode_button.enabled = false;
            BackBt.SetActive(false);

            if(IsValidEmail(emailText)){
                CryptoManager.instance.requestCode(emailText);
            }else{
                invalidEmailText.SetActive(true);
                requestCode_button.enabled = true;
                SetBusy(false,"");
                BackBt.SetActive(true);
            }


        }

        public bool IsValidEmail(string email){
            if (string.IsNullOrWhiteSpace(email))
                return false;
            string pattern = @"^[^@\s]+@[^@\s]+\.[^@\s]+$";
            Regex regex = new Regex(pattern, RegexOptions.IgnoreCase);
            return regex.IsMatch(email);
        }

        private void requestCodeCallBack(string res){   
            try{
                    SetBusy(false, "");
        
                EmailCanvas.alpha = 0;
                EmailCanvas.SetActive(false);
          
                CodeCanvas.alpha = 1;
                CodeCanvas.SetActive(true);
                code1Input.Select();
            }catch(System.Exception e){

            }

            requestCode_button.enabled = true;
            code1Input.text = "";
            code2Input.text = "";
            code3Input.text = "";
            code4Input.text = "";
            code5Input.text = "";
            
        }
        private void validateCode(){
            invalidCodeText.SetActive(false);
            SetBusy(true, "Validating code...");

            code1Text = code1Input.text;
            code2Text = code2Input.text;
            code3Text = code3Input.text;
            code4Text = code4Input.text;
            code5Text = code5Input.text;

            string allCode = code1Text + code2Text + code3Text + code4Text + code5Text;
            CryptoManager.instance.validateCode(emailText,allCode);
        }

        private void validateCodeCallBack(LoginValidationReturn res){

            

            Debug.Log(res);
            Debug.Log("RES VALIDATE" + res.gameShiftCreated);
            string walletAvailable = "";
            if (res.solanaWallet != ""){
                    walletAvailable = res.solanaWallet;
                }
                if(res.phantomWallet != ""){
                    walletAvailable = res.phantomWallet;
                }
                if(res.gameShiftWallet != ""){
                    walletAvailable = res.gameShiftWallet;
                }

            if(res.token!=null && res.gameShiftCreated=="1"){

                ret_Id = res._id;
                ret_Email = res.email;
                ret_SolanaWallet = res.solanaWallet;
                ret_GameShiftWallet = res.gameShiftWallet;
                ret_PhantomWallet = res.phantomWallet;


                SetBusy(false, "");
                CodeCanvas.SetActive(false);
                InventoryCanvas.SetActive(true);
                InventoryCanvas.alpha = 1;
                GMlinkCanvas.SetActive(false);
                GMlinkCanvas.alpha = 1;
                statusConnected.SetActive(true);
                Debug.Log("valid code");
            }else {
                
                ret_Id = res._id;
                ret_Email = res.email;
                ret_SolanaWallet = res.solanaWallet;
                ret_GameShiftWallet = res.gameShiftWallet;
                CodeCanvas.alpha = 0;
                CodeCanvas.SetActive(false);
                GMlinkCanvas.SetActive(true);
                GMlinkCanvas.alpha = 1;
                SetBusy(false, "");
            }
            
            if(res.status=="2"){
                invalidCodeText.SetActive(true);
                SetBusy(false, "");
                
                Debug.Log("invalid code");
            }
            
        }

        private void login()
        {
            cryptoManager.LoginParticleClick();

        }

        private void dataCallback(UserData userData)
        {

            if(userData._id!=""){
                 try{
                SetBusy(false, "");

                InventoryCanvas.SetActive(true);

                InventoryCanvas.alpha = 1;
                phantom_button.SetActive(false);
                // if(userData.status!=-1){
                checkStatusButtons();

                }catch(System.Exception e){

                }
            }

           
                        //  }

        }


        public void disconnectGoogle()
        {
            SetBusy(true, "disconnecting google");
            //cryptoManager.disconnectGoogle();
            PersistentStorage.Delete("googleId", true);
            PlayerData playerData = Global.PlayerService.PlayerData;
            playerData.userData.googleId = "";
            checkStatusButtons();
            CryptoManager.instance.disconnectGoogle();
        }
        public void disconnectPhantom()
        {
            SetBusy(true, "disconnecting phantom wallet");
            //cryptoManager.disconnectPhantom();
            PersistentStorage.Delete("wallet", true);
            PlayerData playerData = Global.PlayerService.PlayerData;
            playerData.userData.solanaWallet = "";
            checkStatusButtons();
            CryptoManager.instance.disconnectPhantom();

        }


        public void LoginPhantomClick() // PHANTOM WALLET CLICK
        {

            if (testMode == true)
            {
                //cryptoManager.TestServer();
                SetBusy(true, "Connecting test mode...");
            }
            else
            {
                cryptoManager.LoginPhantomClick();
                SetBusy(true, "Waiting Phantom wallet response ...");
            }

        }

        private void phantomCallback(string wallet){
            CryptoManager.instance.createGSAccount(ret_Id,ret_Email,wallet);
            SetBusy(false, "");
        }
        

        private void loginCallback(string wallet)
        {
            // PHANTOM LOGIN CALLBACK
            // Create Gamshift acccount
        }

        private void onGSreturn(){}

        public void GoogleClick()
        {
            if (testMode == true){
                cryptoManager.TestGoogle();
                SetBusy(true, "Connecting google test mode...");
            }
            else{
                cryptoManager.LoginGoogleClick();
                SetBusy(true, "Waiting google login response ...");
            }
        }

        public string FormatWalletID(string walletID)
        {
            if (walletID.Length < 10)
            {
                return walletID; // Return the original if it's too short to format as desired.
            }

            string firstFive = walletID.Substring(0, 5);
            string lastFive = walletID.Substring(walletID.Length - 5, 5);

            return firstFive + "..." + lastFive;
        }

        private void checkStatusButtons()
        {

            PlayerData playerData = Global.PlayerService.PlayerData;
            //Solana Wallet
            Debug.Log(playerData.userData.availableWallet + " AVAILABLE");

            if (playerData.userData != null && playerData.userData.availableWallet != "")
            {
                wallet = playerData.userData.availableWallet;


                solanaWallet.SetText(FormatWalletID(playerData.userData.availableWallet));
                //cryptoManager.getNFTSPF(playerData.userData.availableWallet);
                // disconnectPhantom_button.SetActive(true);
                // phantom_button.SetActive(false);
            }
            else
            {
                // disconnectPhantom_button.SetActive(false);
                // phantom_button.SetActive(true);
                solanaWallet.SetText("--");
                //bloomieCount.SetText("0");
                //nftCount.SetText("0");
            }


            // Debug.Log(playerData.userData.googleId);
            //Google Account
            if (playerData.userData != null && playerData.userData.googleId != "") {
                googleId.SetText(playerData.userData.googleId);
                disconnectGoogle_button.SetActive(true);
                google_button.SetActive(false);
            }
            else{
                disconnectGoogle_button.SetActive(false);
                google_button.SetActive(true);
                googleId.SetText("--");
            }

            SetBusy(false, "");
        }
        private void setBloomies(string bloomies){
            bloomieCount.SetText(bloomies);
        }

        private NFT[] nfts = new NFT[0];
        private List<String> allowedTypes = new List<string>();
        private void countBFFs(string text)
        {
            Debug.Log("Receiving NFTS");

            nfts = Array.FindAll(JsonUtility.FromJson<NFTS>(text).nfts, c => (c.collectionAddress == "bffoiWfiVYfT7b4NpXexhLmDLC6B6CZNHPtW6R1R46y"));
            if (nfts.Length > 0)
            {
                for (int i = 0; i < nfts.Length; i++)
                {
                    if (nfts[i].traits.Length > 0)
                    {
                        for (int a = 0; a < nfts[i].traits.Length; a++)
                        {

                            if (nfts[i].traits[a].trait_type == "Rarity")
                            {
//                                Debug.Log(nfts[i].traits[a].value + " Rarity NFTS");
                                if (!allowedTypes.Contains(nfts[i].traits[a].value))
                                {
                                    allowedTypes.Add(nfts[i].traits[a].value.ToLower());
                                }

                            }
                        }
                    }
                }
            }


            nftCount.SetText(nfts.Length + "");
            //foreach( var x in allowedTypes) {
            //	Debug.Log( x.ToString() + " ------------------------------------LIST ITEM");
            //}



        }

        private void SetBusy(bool active, string desc)
        {
            if (loader != null)
            {
                loader.SetActive(active);
                loaderDesc.SetText(desc);
            }

        }






    }
}
