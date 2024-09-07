using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TPSBR.UI;
using System.Threading.Tasks;
using System;
using TMPro;
using ReadyPlayerMe;

namespace TPSBR
{
    public struct SpawnDataC
	{
		public NetworkBehaviour Prefab;
		public Vector3          Position;
		public Quaternion       Rotation;

		public bool             IsConnector;
		public int              AreaID;
		public Material         Material;
		public float            Height;

	
	}

    public class LevelGcustom :  MonoBehaviour
    {

        [Header("Object Spawn")]
		[SerializeField]
		private ItemBox _itemBoxPrefab;

        [SerializeField]
		private ItemBoxCustom _CustomItemsitemBoxPrefab;

        [SerializeField]
        private Transform[] spawnItemsTransform;

        [SerializeField]
        private Transform[] customSpawnItemsTransform;

       // [SerializeField]
//private ChatNewGui Chat;

        public List<SpawnData> ObjectsToSpawn  => _objectsToSpawn;
        private List<SpawnData> _objectsToSpawn = new List<SpawnData>(1024);

        public List<SpawnData> ObjectsCustomToSpawn  => _objectsCustomToSpawn;
        private List<SpawnData> _objectsCustomToSpawn = new List<SpawnData>(1024);


        [SerializeField] bool customChests;
        [SerializeField] LeaderList customLeaderBoardHV;

        
       
        public NPC_chat[] npcs;

        // Start is called before the first frame update
        async public Task  Generate()
        {
            // Debug.Log("Entrando GENERATE--------------------------");
            if(customChests){
                getCustomChests();
            }
             spawnItems();
            // Chat.StartChat();
            checkURL();
          
            if(customLeaderBoardHV){
                customLeaderBoardHV.StartLeader();
            }
          
            //addSpawnPoints();
        }



        
     

        private void checkURL(){
            Dictionary<string, string>  d = URLParameters.GetSearchParameters();
			
			string value="";
			if(d.TryGetValue("partner",out value)){
                
                ActivityManager.instance.StartActivity();

			}
			
        }

        public static Action customChestsAction;
        async public Task getCustomChests(){
            DataManager.OnChestResponse += chestResponse;
            DataManager.instance.getChests();
        }
       

        void chestResponse(Chests chests){
            DataManager.OnChestResponse -= chestResponse;
            for (int i = 0; i < chests.chestArr.Length; i++){
                SpawnData spawndata = new SpawnData(_CustomItemsitemBoxPrefab,customSpawnItemsTransform[i].position, customSpawnItemsTransform[i].rotation);
                spawndata.InternalId = chests.chestArr[i].internalId;
                spawndata.ItemType = chests.chestArr[i].item_type;
                spawndata.Item3D = chests.chestArr[i].item_3d;
                spawndata.ItemDesc = chests.chestArr[i].description;
                spawndata.ItemMaterial = chests.chestArr[i].item_material;
                _objectsCustomToSpawn.Add(spawndata);
//                Debug.Log(chests.chestArr[i].internalId + " -9 " + chests.chestArr[i].item_material);
            }
            customChestsAction?.Invoke();

            
            
  
        }

        void spawnItems(){
//            Debug.Log("Entrando spawn objecto--------------------------" +  spawnItemsTransform.Length);
            for (int i = 0; i < spawnItemsTransform.Length; i++)
                {  
                    //Debug.Log("Agregando objecto--------------------------");
                    _objectsToSpawn.Add(new SpawnData(_itemBoxPrefab, spawnItemsTransform[i].position, spawnItemsTransform[i].rotation));
                }

        }


        void addSpawnPoints(){

        }

        // Update is called once per frame
        void Update()
        {
        
        }
    }
}
