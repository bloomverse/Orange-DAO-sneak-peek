using UnityEngine;
 using System.Runtime.InteropServices;

using UnityEngine.Events;

namespace TPSBR
{

	public enum EType
	{
			URL,
			Script
	}

	public class ActionPickup : StaticPickup
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

		// StaticPickup INTERFACE

		void Start(){
			PickupConsumed += consumedCustom;
		}

		void consumedCustom(StaticPickup item){
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
		
										// Agent
		protected override bool Consume(GameObject instigator, out string result)
		{
			//Debug.Log(urlLink);
			bool fuelAdded = true;//agent.Jetpack.AddFuel(_fuel);
			result = urlLink;
			
			
			
			return fuelAdded;

			 
		}
	}
}
