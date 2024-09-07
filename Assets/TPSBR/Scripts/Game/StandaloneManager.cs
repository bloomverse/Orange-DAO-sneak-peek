using Fusion;
using Fusion.Photon.Realtime;
using UnityEngine;
using Fusion.Plugin;
using UnityEngine.SceneManagement;

namespace TPSBR
{
	using System;

	[Serializable]
	public sealed class StandaloneConfiguration
	{
		public EGameplayType GameplayType;
		public GameMode GameMode;
		public string ServerName;
		public int MaxPlayers;
		public int ExtraPeers;
		public string Region;
		public string SessionName;
		public string CustomLobby;
		public string IPAddress;
		public ushort Port;
		public bool Multiplay;
		public bool QueryProtocol;
		public bool Matchmaking;
		public bool Backfill;
		public bool Addressable;
	}

	public class StandaloneManager : MonoBehaviour
	{
		// PUBLIC MEMBERS

		public static StandaloneConfiguration ExternalConfiguration;

		// PRIVATE MEMBERS

		[SerializeField]
		private StandaloneConfiguration _defaultConfiguration;

		// MONOBEHAVIOUR

		protected void Awake()
		{
			if (Global.Networking.HasSession == true)
			{
				Destroy(gameObject);
			}
		}

		protected void Start()
		{
			StandaloneConfiguration configuration = ExternalConfiguration ?? _defaultConfiguration;

			var playerData = Global.PlayerService.PlayerData;
			var scenePath = SceneManager.GetActiveScene().path;

			if(configuration.Addressable){
				
			}else{
				scenePath = scenePath.Substring("Assets/".Length, scenePath.Length - "Assets/".Length - ".unity".Length);
			}
			

//			Debug.Log(scenePath + " Scene path SatandoAlone");

			PhotonAppSettings.Global.AppSettings.FixedRegion = configuration.Region;

			var request = new SessionRequest
			{
				UserID = playerData.UserID.HasValue() ? playerData.UserID : new Guid().ToString(),
				GameMode = configuration.GameMode,
				SessionName = configuration.SessionName.HasValue() ? configuration.SessionName : Guid.NewGuid().ToString(),
				DisplayName = configuration.ServerName,
				ScenePath = scenePath,
				//sceneType = configuration.Addressable ? SceneType.addressable : SceneType.embedded,
				GameplayType = configuration.GameplayType,
				ExtraPeers = configuration.ExtraPeers,
				MaxPlayers = configuration.MaxPlayers,
				CustomLobby = configuration.CustomLobby.HasValue() ? configuration.CustomLobby : "FusionBR." + Application.version,
				IPAddress = configuration.IPAddress,
				Port = configuration.Port,
			};



			if (configuration.Multiplay)
			{
				// A Multiplay allocation will trigger the game session creation
				Debug.Log("Launching multiplay from standalonemanager");
				Global.MultiplayManager.StartMultiplay(request, configuration);
			}
			else
			{
				Global.Networking.StartGame(request);
			}
		}
	}
}
