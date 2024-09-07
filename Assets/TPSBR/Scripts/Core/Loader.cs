namespace TPSBR
{
	using System;
	using UnityEngine;
	using UnityEngine.InputSystem;
	using UnityEngine.SceneManagement;
	using UnityEngine.AddressableAssets;
	using UnityEngine.ResourceManagement.AsyncOperations;
	 using UnityEngine.ResourceManagement.ResourceProviders;
	 using System.Collections;
	using Fusion;

	public sealed class Loader : MonoBehaviour
	{
		// PRIVATE MEMBERS

		[SerializeField]
		private string                  _batchModeScene;
		[SerializeField]
		private StandaloneConfiguration _batchModeConfiguration;

		// MonoBehaviour INTERFACE

		private void Awake()
		{
			if (Application.isBatchMode == true)
			{
				StartBatchGame();
			}
			else
			{
				//if(PlayerPrefs.GetString("TutorialComplete") != "true"){
				//	SceneManager.LoadScene("Tutorial");
				//}else{
					SceneManager.LoadScene(Global.Settings.MenuScene);
				//}
				
			}
		}

		// PRIVATE METHODS

		private void StartBatchGame()
		{
			Keyboard keyboard = InputSystem.AddDevice<Keyboard>();
			keyboard.MakeCurrent();

			Mouse mouse = InputSystem.AddDevice<Mouse>();
			mouse.MakeCurrent();

			PlayerData playerData = Global.PlayerService.PlayerData;
			playerData.AgentID  = Global.Settings.Agent.Agents.GetRandom().ID;
			playerData.Nickname = "Batch" + UnityEngine.Random.Range(1000, 10000);

			if (ApplicationSettings.IsQuickPlay == true)
			{
				SceneManager.LoadScene(Global.Settings.MenuScene);
				return;
			}

			if (ApplicationSettings.IsHost == true || ApplicationSettings.IsServer == true)
			{
				_batchModeConfiguration.ExtraPeers = 0;
			}

			string sessionName = ApplicationSettings.SessionName;
			if (ApplicationSettings.HasSessionName == true && sessionName == "random")
			{
				sessionName = Guid.NewGuid().ToString().ToLowerInvariant();
			}

			if (ApplicationSettings.IsHost         == true) { _batchModeConfiguration.GameMode      = GameMode.Host;                    }
			if (ApplicationSettings.IsServer       == true) { _batchModeConfiguration.GameMode      = GameMode.Server;                  }
			if (ApplicationSettings.IsClient       == true) { _batchModeConfiguration.GameMode      = GameMode.Client;                  }
			if (ApplicationSettings.IsDeathmatch   == true) { _batchModeConfiguration.GameplayType  = EGameplayType.Deathmatch;         }
			if (ApplicationSettings.IsElimination  == true) { _batchModeConfiguration.GameplayType  = EGameplayType.Elimination;        }
			if (ApplicationSettings.IsBattleRoyale == true) { _batchModeConfiguration.GameplayType  = EGameplayType.BattleRoyale;       }
			if (ApplicationSettings.IsLobby 		== true) { _batchModeConfiguration.GameplayType  = EGameplayType.Lobby;       		}
			if (ApplicationSettings.HasRegion      == true) { _batchModeConfiguration.Region        = ApplicationSettings.Region;       }
			if (ApplicationSettings.HasExtraPeers  == true) { _batchModeConfiguration.ExtraPeers    = ApplicationSettings.ExtraPeers;   }
			if (ApplicationSettings.HasServerName  == true) { _batchModeConfiguration.ServerName    = ApplicationSettings.ServerName;   }
			if (ApplicationSettings.HasMaxPlayers  == true) { _batchModeConfiguration.MaxPlayers    = ApplicationSettings.MaxPlayers;   }
			if (ApplicationSettings.HasSessionName == true) { _batchModeConfiguration.SessionName   = sessionName;                      }
			if (ApplicationSettings.HasCustomLobby == true) { _batchModeConfiguration.CustomLobby   = ApplicationSettings.CustomLobby;  }
			if (ApplicationSettings.HasIPAddress   == true) { _batchModeConfiguration.IPAddress     = ApplicationSettings.IPAddress;    }
			if (ApplicationSettings.HasPort        == true) { _batchModeConfiguration.Port          = (ushort)ApplicationSettings.Port; }
			if (ApplicationSettings.UseMultiplay   == true) { _batchModeConfiguration.Multiplay     = true;                             }
			if (ApplicationSettings.UseMatchmaking == true) { _batchModeConfiguration.Matchmaking   = true;                             }
			if (ApplicationSettings.UseBackfill    == true) { _batchModeConfiguration.Backfill      = true;                             }
			if (ApplicationSettings.UseSQP         == true) { _batchModeConfiguration.QueryProtocol = true;                             }

			StandaloneManager.ExternalConfiguration = _batchModeConfiguration;
			SceneManager.LoadScene(ApplicationSettings.HasCustomScene == true ? ApplicationSettings.CustomScene : _batchModeScene);

			/*string scene =ApplicationSettings.HasCustomScene == true ? ApplicationSettings.CustomScene : _batchModeScene;
			
			var scenePath = "Assets/TPSBR/Scenes/" + scene + ".unity";
			
			Debug.Log(scenePath + " Server Scene to load FV");

			AsyncOperationHandle handle = Addressables.LoadSceneAsync(scenePath);

			handle.Completed += (AsyncOperationHandle operation) =>
			{
				if (operation.Status == AsyncOperationStatus.Failed)
				{
					Debug.LogError($"Error loading scene: {scenePath} {operation.OperationException.Message}");
				}
			};

			StartCoroutine(UpdateProgressUI(handle));

			

			//SceneManager.LoadScene(scene);
		}
		private IEnumerator UpdateProgressUI(AsyncOperationHandle operation)
    {
        while (!operation.IsDone)
        {
            Debug.Log($"Loading: {Mathf.RoundToInt(operation.PercentComplete * 100)}%");
            yield return null;
        }
        
         Debug.Log("Load complete%");
    }*/
		}
	}
}
