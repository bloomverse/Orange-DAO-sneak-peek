using UnityEngine;
using System;
using Fusion;

namespace TPSBR
{
	[Serializable]
	[CreateAssetMenu(fileName = "SkinSettings", menuName = "TPSBR/Skin Settings")]
	public class SkinSettings : ScriptableObject
	{
		// PUBLIC MEMBERS

		public SkinSetup[] Skins => _skins;

		// PRIVATE MEMBERS

		[SerializeField]
		private SkinSetup[] _skins;

		// PUBLIC METHODS

		public SkinSetup GetSkinSetup(string skinID)
		{
			if (skinID.HasValue() == false)
				return null;

			return _skins.Find(t => t.ID == skinID);
		}

		/*public AgentSetup GetSkinSetup(NetworkPrefabId prefabId)
		{
			if (prefabId.IsValid == false)
				return null;

			return _skins.Find(t => t.SkinPrefabId == prefabId);
		}*/

		public SkinSetup GetRandomSkinSetup()
		{
			//return _agents[UnityEngine.Random.Range(0, _agents.Length)];
			return _skins[0];
		}
	}

	[Serializable]
	public class SkinSetup
	{
		// PUBLIC MEMBERS

		public string               ID                => _id;
		public string               DisplayName       => _displayName;
		public string               Description       => _description;
		public Sprite               Icon              => _icon;

		//public GameObject           AgentPrefab       => _agentPrefab;
		//public GameObject           MenuAgentPrefab   => _menuAgentPrefab;
		public bool           		Available   	  => _available;

		
		// PRIVATE MEMBERS

		[SerializeField]
		private string _id;
		[SerializeField]
		private string _displayName;
		[SerializeField, TextArea(3, 6)]
		private string _description;
		[SerializeField]
		private Sprite _icon;
		

		[SerializeField]
		private bool _available;

		[NonSerialized]
		private NetworkPrefabId _skinPrefabId;
	}
}
