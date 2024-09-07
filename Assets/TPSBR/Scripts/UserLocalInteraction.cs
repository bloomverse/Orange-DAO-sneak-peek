using UnityEngine;
 using System.Runtime.InteropServices;
using System;
using UnityEngine.Events;
using TPSBR.UI;

namespace TPSBR
{


	public class UserLocalInteraction : StaticPickup
	{
		// PRIVATE MEMBERS
		[DllImport("__Internal")]
        public static extern void OpenURL(string url);
        [DllImport("__Internal")]
        public static extern void OpenURLInSameTab(string url);

		[SerializeField] private UnityEvent referencedScript;

		

		[SerializeField] private EType ActionType = EType.URL;

		// StaticPickup INTERFACE
		private bool spinAv;
		private bool signedIn;

		void Start(){
			PickupConsumed += consumedCustom;
		}

		void consumedCustom(StaticPickup item){
			
			
			 Debug.Log("Consumed custom");
			if(ActionType == EType.Script && spinAv){
				referencedScript.Invoke();
			}
		}
		
		public void askSignin(bool state){
			if(!state){
				signedIn = false;
				//this.InteractionName = "Eyekon wheel";
				//this.InteractionDescription = "To claim rewards sign in.";
			}else{
				
				signedIn = true;
				//this.InteractionName = "Eyekon wheel";
				//this.InteractionDescription = "";
			}
			
		}	

		public void setState(bool state, string time){

//			Debug.Log(state + " - " + time);
			if(state){
				//this.InteractionName = "Eyekon wheel";
				//this.InteractionDescription = "test your luck!";
				spinAv = true;
				signedIn = true;
				
			}else{

				//this.InteractionName = "Eyekon wheel";
				//this.InteractionDescription = "Next Spin in... " + time.ToString();
				spinAv = false;
				signedIn = true;
			}
			
		}
		

		protected override bool Consume(GameObject instigator, out string result)
		{

			bool retVal = false;
			result= "";

			  Debug.Log(spinAv);
			  Debug.Log("SpinAv");

			
			if(spinAv){
				retVal = true;
				result = "Good Luck";
			}
			else{
				retVal =  false;
				result = "please wait for time refresh";
			}

			if(!signedIn){
				retVal = false;
				result = "Sign in to claim rewards";
				GameplayUI gameplayUI = GameObject.Find("GameplayUI").GetComponent<GameplayUI>();
			
            	gameplayUI.OpenUserSignIn();
			}

			 return retVal;
		}
	}
}
