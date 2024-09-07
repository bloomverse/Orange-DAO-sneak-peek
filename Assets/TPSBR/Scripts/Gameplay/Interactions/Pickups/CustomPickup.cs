using UnityEngine;
using TPSBR.UI;
using TMPro;
using System;
using Fusion;


namespace TPSBR
{
	public class CustomPickup : StaticPickup
	{
		//[Networked(OnChanged = nameof(SetActiveObj), OnChangedTargets = OnChangedTargets.All)]
		public  string internalId  { get; private set; }

		//[Networked(OnChanged = nameof(SetConsumed), OnChangedTargets = OnChangedTargets.All)]
		public  bool consumed  { get; private set; }

		[Networked]
		public string itemType  { get; private set; }

		[Networked]
		public string itemDesc  { get; private set; }

		[Networked]
		public string item3D  { get; private set; }

		[SerializeField] ParticleSystem comsumeEffect ;

		
		public Agent agentConsumed;

		[Networked]
		public string itemMaterial  { get; private set; }

		public GameObject prefab;
		public Material transparent;
		public ParticleSystem particles;

		public  TextMeshPro prizeDesc;

		public void StartObj(string _pickupinternalId,
		string _pickupitemType,
		string _pickupitem3D,
		string _pickupDesc,
		string _pickupitemMaterial){

				internalId = _pickupinternalId;
				itemType = _pickupitemType;
				item3D = _pickupitem3D;
				itemDesc = _pickupDesc;
				itemMaterial = _pickupitemMaterial;

			
		}

		public override void Spawned(){
			//Debug.Log("Executing RPC" + item3D + " " + itemDesc);
		}

		protected override bool Consume(GameObject instigator, out string result)
		{
			bool status = true;
			result = string.Empty;

			DataManager.claimedReturn += claimConfirm;
			//DataManager.instance.customPrize(agent.dBID,internalId,agent.nickname,itemDesc);
			//agentConsumed = agent;
			//Debug.Log("Consuming Custom " + agent.dBID  + "internal " + internalId);

			return status;

		}

		public static void SetActiveObj(CustomPickup changed){
				Debug.Log(changed.internalId + " Internal Id");
				var obj = changed.transform.gameObject.FindObject(changed.item3D);
					
					if(obj!=null){
						obj.SetActive(true);
					}

					changed.setTextPrize(changed.itemDesc);
				
		}

		public static void SetAgent(CustomPickup changed){


		}

		public static void SetConsumed(CustomPickup changed){
			//Debug.Log("input auht 1" + changed.Behaviour.Object.HasInputAuthority);
			if(changed.Object.HasInputAuthority){
				Debug.Log("input auht" + changed.Object.HasInputAuthority);
				UIGameplayMenu dialog = GameObject.Find("UIGameplayMenuView").GetComponent<UIGameplayMenu>();
                var desc = changed.itemDesc;
                dialog.Notification(desc,changed.internalId);
			
				// Saved 

			}
			
			changed.comsumeEffect.Play();
			//changed.Behaviour.SetActive(false);


			changed.prefab.transform.GetComponent<Renderer>().material = changed.transparent;
		}


		public void claimConfirm(string internalId){
			//PersistentStorage
			consumed = true;

			
			DataManager.claimedReturn -= claimConfirm;
		}
		

		public void setTextPrize(string desc){
			itemDesc = desc;
			prizeDesc.SetText(desc);
		}
	}
}
