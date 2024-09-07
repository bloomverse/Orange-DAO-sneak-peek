using UnityEngine;

namespace TPSBR
{
	public class BloomiePickup : StaticPickup
	{
		// PRIVATE MEMBERS

		

		// StaticPickup INTERFACE

		protected override bool Consume(GameObject instigator, out string result)
		{
			bool status = true;
			result = string.Empty;
			
			//DataManager.instance.bloomP(agent.dBID);
			//Debug.Log("Consuming Bloomie " + agent.dBID);
			return status;

			 
		}
	}
}
