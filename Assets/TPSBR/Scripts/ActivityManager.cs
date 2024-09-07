using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using TPSBR.UI;
using DG.Tweening;
using ReadyPlayerMe;

namespace TPSBR
{
    public class ActivityManager : ContextBehaviour
    {

        [SerializeField] TextMeshProUGUI announcer;
        [SerializeField] ParticleSystem celebrationEffect;
        [SerializeField] ParticleSystem tokenEffect;
        [SerializeField] Animator animations;
        [SerializeField] Transform tokenObject;

        [SerializeField] Transform activityButton;

        private CryptoManager cryptoManager;

        public static ActivityManager instance;

        [SerializeField] AudioClip Token;
        [SerializeField] AudioClip FireWorks;

        [SerializeField] AudioSource audioSource;

        public  void Start(){
            instance = this;
            announcer.SetText("Interact with Chara to create an account and redeem code");
        }
        public void StartActivity()
        {
            //cryptoManager = GameObject.Find("CryptoManager").GetComponent<CryptoManager>();
            Step1();
        }
        
        // Open Wallet
        public void Step1(){
            announcer.SetText("Interact with Chara to create an account and redeem code");
        }

        // Deliver Battle Token
        public void Step2(){
            Debug.Log("Launch Wallet");

            announcer.SetText("Complete Sign in to claim");
            CryptoManager.OnUserData += ParticleReturn;
			CryptoManager.instance.LoginParticleClick();
        }

        private void ParticleReturn(UserData data)
		{
			// Send Token 

            Debug.Log(data.solanaWallet + " Solana Wallet");

                announcer.SetText("Enter your Redeem code to claim Tournament Token");

			CryptoManager.OnUserData -= ParticleReturn;
            announcer.SetText("Redeem your code");

            // aDD RETURN VALIDATION
           // if(data._id!=null){
                UIReedemCode.reedemCompleted += Step3;
                GameplayUI gameplayUI = GameObject.Find("GameplayUI").GetComponent<GameplayUI>();
                gameplayUI.ReedemUI();
            //}
            //Validate Claim


			//TODO SEND Token

		}



        public void Step3(){
           Debug.Log("Claim Completed");

            UIReedemCode.reedemCompleted -= Step3;
            activityButton.SetActive(false);

           celebrationEffect.SetActive(true);
           announcer.SetText("Congratulations your claim is complete! You'll now be redirected to the main menu, time to test your skills in the Bloomverse tutorial or play a multiplayer experience!");
           animations.CrossFade("dance",0.2f);
           audioSource.PlayOneShot(FireWorks);

            tokenObject.DOMove(new Vector3(-0.183f,.3f,1.7f),3).SetEase(Ease.OutQuad).OnComplete(()=>{
                tokenEffect.SetActive(true);
                audioSource.PlayOneShot(Token);
            });
           //Validate Claim deliver token to account 
            PlayerPrefs.SetString("TutorialComplete","true");
            PlayerPrefs.SetString("LastActivity","1kin");  // Always lowercase
            GlobalFunctions.ChangeURL("/");

            cryptoManager = GameObject.Find("CryptoManager").GetComponent<CryptoManager>();
            cryptoManager.SendToken(Context.PlayerData.userData.solanaWallet,Context.PlayerData.userData._id);
            StartCoroutine(EndSession());
        }



         //End Game
        IEnumerator EndSession(){

            yield return new WaitForSeconds(8);
            announcer.SetText("Sending to main screen in 5");
            yield return new WaitForSeconds(1);
            announcer.SetText("Sending to main screen in 4");
            yield return new WaitForSeconds(1);
            announcer.SetText("Sending to main screen in 3");
            yield return new WaitForSeconds(1);
            announcer.SetText("Sending to main screen in 2");
            yield return new WaitForSeconds(1);
            announcer.SetText("Sending to main screen in 1");
            yield return new WaitForSeconds(1);


            if (Context != null && Context.GameplayMode != null)
                {
                    Context.GameplayMode.StopGame();
                }
                else
                {
                    Global.Networking.StopGame();
                }
        }
        


        // Update is called once per frame
        void Update()
        {
        
        }

       
    }
}
