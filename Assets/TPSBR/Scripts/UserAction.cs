using UnityEngine;
 using System.Runtime.InteropServices;
using System;
using UnityEngine.Events;

namespace TPSBR
{


	public class UserAction : StaticPickup
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private string urlLink = "https://www.happyvibes.life/";

		[DllImport("__Internal")]
        public static extern void OpenURL(string url);
        [DllImport("__Internal")]
        public static extern void OpenURLInSameTab(string url);

		[SerializeField] private UnityEvent referencedScript;

		[SerializeField] private EType ActionType = EType.URL;

		private Agent localAgent;

		// StaticPickup INTERFACE

		void Start(){
			PickupConsumed += consumedCustom;
		}

		void consumedCustom(StaticPickup item){
			
			Debug.Log("Consumed" + localAgent.HasInputAuthority);
			
            if(!localAgent.HasInputAuthority) return; 
            
            Refresh();

			Debug.Log(ActionType);
            

			if(ActionType== EType.URL){
				Application.OpenURL(urlLink);
			}

			if(ActionType == EType.Script){
				referencedScript.Invoke();
			}
			
			/*#if UNITY_WEBGL && !UNITY_EDITOR
				{
					
					OpenURL(urlLink);
					
				}
			#endif */
		}
		

		protected override bool Consume(GameObject instigator, out string result)
		{
			//Debug.Log(urlLink);

			localAgent = instigator.GetComponent<Agent>(); // agent;

			//Debug.Log(agent.HasInputAuthority+ " ageng ipu t consume");
			
			bool fuelAdded = true;//agent.Jetpack.AddFuel(_fuel);
			result = urlLink;
			
			
			
			return fuelAdded;

			 
		}
	}
}
