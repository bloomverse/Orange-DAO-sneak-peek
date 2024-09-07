using UnityEngine;
using Plugins.Outline;
using System;
using TPSBR.UI;
using ReadyPlayerMe;

namespace TPSBR
{
	public class PlayerPreview : CoreBehaviour
	{
		// PUBLIC MEMBERS

		public string AgentID => _agentID;

		// PRIVATE MEMBERS

		[SerializeField]
		private Transform _agentParent;

		private string _agentID;
		public GameObject _agentInstance;

		private OutlineBehaviour _outline;

		public Animator _animator;

		// PUBLIC METHODS

		public void ShowAgent(string agentID, bool force = false)
		{
			if (agentID == _agentID && force == false)
				return;

			ClearAgent();
			InstantiateAgent(agentID);
		}

		public void ShowOutline(bool value)
		{
			_outline.enabled = value;
		}

		public void HideAgent()
		{
			ClearAgent();
		}

		// MONOBEHAVIOUR

		protected void Awake()
		{
			_outline = GetComponentInChildren<OutlineBehaviour>(true);
			_outline.enabled = false;

			
		}

		// PRIVATE METHODS

		private void InstantiateAgent(string agentID)
		{
			if (agentID.HasValue() == false)
				return;


			var agentSetup = Global.Settings.Agent.GetAgentSetup(agentID);

			if (agentSetup == null)
				return;

			_agentInstance = Instantiate(agentSetup.MenuAgentPrefab, _agentParent);
			_agentID = agentID;

			if(PersistentStorage.GetString("Skin")!=null && _agentInstance.GetComponentInChildren<SkinAdaptorTextures>()!=null){
//				Debug.Log(PersistentStorage.GetString("Skin"));
				
				_agentInstance.GetComponentInChildren<SkinAdaptorTextures>().setRandom(PersistentStorage.GetString("Skin"));
			}
			 

			
			
			try {
				_animator = _agentInstance.GetComponentInChildren<Animator>();
			} catch(Exception e){
				Debug.Log("animator not available" + e);
			}

			    if(_agentInstance.GetComponentInChildren<Animator>()!=null){
                        
                        var pref =_agentInstance.GetComponentInChildren<Animator>();
                         System.Random random = new System.Random();
                         int randomNumber = random.Next(0, 4);
                       Debug.Log(randomNumber + " Random number animation");
                        pref.SetFloat("Blend",4);
                    }

		}

	

		private void ClearAgent()
		{
			_agentID = null;

			if (_agentInstance == null)
				return;

			_outline.enabled = false;

			Destroy(_agentInstance);
			_agentInstance = null;
		}
	}
}
